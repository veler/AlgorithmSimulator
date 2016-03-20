using System.Reflection;
using Algo.Runtime.Build.AlgorithmDOM.DOM;
using Algo.Runtime.Build.Runtime.Debugger;
using Algo.Runtime.Build.Runtime.Debugger.Exceptions;
using Algo.Runtime.Build.Runtime.Interpreter.Interpreter;
using Algo.Runtime.ComponentModel.OperatorHelper;

namespace Algo.Runtime.Build.Runtime.Interpreter.Expressions
{
    /// <summary>
    /// Provide the interpreter for a binany operation
    /// </summary>
    internal sealed class BinaryOperator : InterpretExpression
    {
        #region Constructors

        /// <summary>
        /// Initialize a new instance of <see cref="BinaryOperator"/>
        /// </summary>
        /// <param name="debugMode">Defines if the debug mode is enabled</param>
        /// <param name="parentInterpreter">The parent block interpreter</param>
        /// <param name="expression">The algorithm expression</param>
        internal BinaryOperator(bool debugMode, BlockInterpreter parentInterpreter, AlgorithmExpression expression)
            : base(debugMode, parentInterpreter, expression)
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Run the interpretation
        /// </summary>
        /// <returns>Returns the result of the interpretation</returns>
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

            if (DebugMode)
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
                ParentInterpreter.ChangeState(this, new AlgorithmInterpreterStateEventArgs(new Error(new OperatorNotFoundException(Expression._operator.ToString(), $"Operator '{Expression._operator}' cannot be applied to operands of type '{left.GetType().FullName}' and '{right.GetType().FullName}'"), Expression), ParentInterpreter.GetDebugInfo()));
                return null;
            }
            return operatorMethod.Invoke(null, new[] { left, right });
        }

        #endregion
    }
}
