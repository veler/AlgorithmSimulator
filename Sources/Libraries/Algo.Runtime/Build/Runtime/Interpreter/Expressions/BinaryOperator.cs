using System;
using System.Reflection;
using Algo.Runtime.Build.AlgorithmDOM.DOM;
using Algo.Runtime.Build.Runtime.Debugger;
using Algo.Runtime.Build.Runtime.Debugger.Exceptions;
using Algo.Runtime.Build.Runtime.Interpreter.Interpreter;
using Algo.Runtime.ComponentModel.OperatorHelper;

namespace Algo.Runtime.Build.Runtime.Interpreter.Expressions
{
    internal sealed class BinaryOperator : InterpretExpression
    {
        #region Constructors

        internal BinaryOperator(bool memTrace, BlockInterpreter parentInterpreter, AlgorithmExpression expression)
            : base(memTrace, parentInterpreter, expression)
        {
        }

        #endregion

        #region Methods

        internal override object Execute()
        {
            object left;
            object right;
            MethodInfo operatorMethod;

            left = ParentInterpreter.RunExpression(Expression._leftExpression);

            if (ParentInterpreter.FailedOrStop)
            {
                return null;
            }

            right = ParentInterpreter.RunExpression(Expression._rightExpression);

            if (ParentInterpreter.FailedOrStop)
            {
                return null;
            }

            if (MemTrace)
            {
                ParentInterpreter.Log(this, $"Doing an operation '{Expression._operator}'");
            }

            if (Expression._operator == AlgorithmBinaryOperatorType.Equals)
            {
                if (left == null)
                {
                    if (right == null)
                    {
                        return true; // null == null
                    }
                    return right.Equals(null);
                }
                return left.Equals(right);
            }

            operatorMethod = OperatorHelperCache.GetOperator(Expression._operator, left.GetType(), right.GetType());
            if (operatorMethod == null)
            {
                ParentInterpreter.ChangeState(this, new SimulatorStateEventArgs(new Error(new OperatorNotFoundException(Expression._operator.ToString(), $"Operator '{Expression._operator}' cannot be applied to operands of type '{left.GetType().FullName}' and '{right.GetType().FullName}'"), ParentInterpreter.GetDebugInfo())));
                return null;
            }

            if (MemTrace)
            {
                try
                {
                    return operatorMethod.Invoke(null, new[] { left, right });
                }
                catch (Exception ex)
                {
                    ParentInterpreter.ChangeState(this, new SimulatorStateEventArgs(new Error(new OperatorNotFoundException(Expression._operator.ToString(), ex.InnerException?.Message), ParentInterpreter.GetDebugInfo())));
                    return null;
                }
            }
            return operatorMethod.Invoke(null, new[] { left, right });
        }

        #endregion
    }
}
