using System;
using System.Reflection;
using Algo.Runtime.Build.AlgorithmDOM;
using Algo.Runtime.Build.AlgorithmDOM.DOM;
using Algo.Runtime.Build.Runtime.Debugger;
using Algo.Runtime.Build.Runtime.Debugger.Exceptions;
using Algo.Runtime.Build.Runtime.Interpreter.Expressions;
using Algo.Runtime.Build.Runtime.Interpreter.Interpreter;
using Algo.Runtime.Build.Runtime.Memory;
using Algo.Runtime.ComponentModel.Types;

namespace Algo.Runtime.Build.Runtime.Interpreter.Statements
{
    sealed internal class Assign : InterpretStatement<AlgorithmAssignStatement>
    {
        #region Constructors

        internal Assign(bool memTrace, BlockInterpreter parentInterpreter, AlgorithmAssignStatement statement)
            : base(memTrace, parentInterpreter, statement)
        {
        }

        #endregion

        #region Methods

        internal override void Execute()
        {
            object targetObject = null;
            object leftValue = null;
            object rightValue;
            PropertyInfo propertyInfo;
            Variable propertyVariable;
            var leftExpression = Statement.LeftExpression;
            var rightExpression = Statement.RightExpression;

            ParentInterpreter.Log(this, $"Assign '{leftExpression}' to '{rightExpression}'");

            if (!(leftExpression is IAlgorithmAssignable))
            {
                ParentInterpreter.ChangeState(this, new SimulatorStateEventArgs(new Error(new NotAssignableException($"The left expression is not assignable."), ParentInterpreter.GetDebugInfo())));
                return;
            }

            TypeSwitch.Switch(
                leftExpression,
                TypeSwitch.Case<AlgorithmPropertyReferenceExpression>(expr =>
                {
                    var interpreter = new PropertyReference(MemTrace, ParentInterpreter, expr);
                    leftValue = interpreter.GetAssignableObject();
                    targetObject = interpreter.TargetObject;
                }),
                TypeSwitch.Case<AlgorithmVariableReferenceExpression>(expr =>
                {
                    leftValue = new VariableReference(MemTrace, ParentInterpreter, expr).GetAssignableObject();
                }),
                TypeSwitch.Default(() =>
                {
                    ParentInterpreter.ChangeState(this, new SimulatorStateEventArgs(new Error(new InvalidCastException($"Unable to find an interpreter  for this expression : '{leftExpression.GetType().FullName}'"), ParentInterpreter.GetDebugInfo())));
                }));

            if (ParentInterpreter.Failed)
            {
                return;
            }

            rightValue = ParentInterpreter.RunExpression(rightExpression);

            if (ParentInterpreter.Failed)
            {
                return;
            }

            propertyInfo = leftValue as PropertyInfo;
            if (propertyInfo != null)
            {
                if (!propertyInfo.CanWrite)
                {
                    ParentInterpreter.ChangeState(this, new SimulatorStateEventArgs(new Error(new NotAssignableException($"This core property is not assignable."), ParentInterpreter.GetDebugInfo())));
                    return;
                }
                propertyInfo.SetValue(targetObject, rightValue);
            }

            propertyVariable = leftValue as Variable;
            if (propertyVariable != null)
            {
                propertyVariable.Value = rightValue;
            }

            ParentInterpreter.Log(this, "'{0}' is now equal to {1}", leftExpression.ToString(), rightValue == null ? "{null}" : $"'{rightValue}' (type:{rightValue.GetType().FullName})");
        }

        #endregion
    }
}
