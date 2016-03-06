﻿using System;
using System.Reflection;
using Algo.Runtime.Build.AlgorithmDOM;
using Algo.Runtime.Build.AlgorithmDOM.DOM;
using Algo.Runtime.Build.Runtime.Debugger;
using Algo.Runtime.Build.Runtime.Debugger.Exceptions;
using Algo.Runtime.Build.Runtime.Interpreter.Expressions;
using Algo.Runtime.Build.Runtime.Interpreter.Interpreter;
using Algo.Runtime.Build.Runtime.Memory;

namespace Algo.Runtime.Build.Runtime.Interpreter.Statements
{
    internal sealed class Assign : InterpretStatement
    {
        #region Constructors

        internal Assign(bool memTrace, BlockInterpreter parentInterpreter, AlgorithmStatement statement)
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
            var leftExpression = Statement._leftExpression;
            var rightExpression = Statement._rightExpression;

            ParentInterpreter.Log(this, $"Assign '{leftExpression}' to '{rightExpression}'");

            if (!(leftExpression is IAlgorithmAssignable))
            {
                ParentInterpreter.ChangeState(this, new SimulatorStateEventArgs(new Error(new NotAssignableException($"The left expression is not assignable."), ParentInterpreter.GetDebugInfo())));
                return;
            }

            switch (leftExpression.DomType)
            {
                case AlgorithmDomType.PropertyReferenceExpression:
                    var interpreter = new PropertyReference(MemTrace, ParentInterpreter, leftExpression);
                    leftValue = interpreter.GetAssignableObject();
                    targetObject = interpreter.TargetObject;
                    break;

                case AlgorithmDomType.VariableReferenceExpression:
                    leftValue = new VariableReference(MemTrace, ParentInterpreter, leftExpression).GetAssignableObject();
                    break;

                default:
                    ParentInterpreter.ChangeState(this, new SimulatorStateEventArgs(new Error(new InvalidCastException($"Unable to find an interpreter  for this expression : '{leftExpression.GetType().FullName}'"), ParentInterpreter.GetDebugInfo())));
                    break;
            }

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
