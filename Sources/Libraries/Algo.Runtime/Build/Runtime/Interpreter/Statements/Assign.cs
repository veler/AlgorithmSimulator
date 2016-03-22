using System;
using System.Collections;
using System.Reflection;
using Algo.Runtime.Build.AlgorithmDOM;
using Algo.Runtime.Build.AlgorithmDOM.DOM;
using Algo.Runtime.Build.Runtime.Debugger;
using Algo.Runtime.Build.Runtime.Debugger.Exceptions;
using Algo.Runtime.Build.Runtime.Interpreter.Expressions;
using Algo.Runtime.Build.Runtime.Interpreter.Interpreter;
using Algo.Runtime.Build.Runtime.Memory;
// ReSharper disable UseIsOperator.1
// ReSharper disable UseMethodIsInstanceOfType

namespace Algo.Runtime.Build.Runtime.Interpreter.Statements
{
    /// <summary>
    /// Provide the interpreter for an assignation
    /// </summary>
    internal sealed class Assign : InterpretStatement
    {
        #region Constructors

        /// <summary>
        /// Initialize a new instance of <see cref="Assign"/>
        /// </summary>
        /// <param name="debugMode">Defines if the debug mode is enabled</param>
        /// <param name="parentInterpreter">The parent block interpreter</param>
        /// <param name="statement">The algorithm statement</param>
        internal Assign(bool debugMode, BlockInterpreter parentInterpreter, AlgorithmStatement statement)
            : base(debugMode, parentInterpreter, statement)
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Run the interpretation
        /// </summary>
        internal override void Execute()
        {
            int indexValue = -1;
            object targetObject = null;
            object leftValue = null;
            object rightValue;
            PropertyInfo propertyInfo;
            Variable propertyVariable;
            IList propertyVariableList;
            var leftExpression = Statement._leftExpression;
            var rightExpression = Statement._rightExpression;

            if (DebugMode)
            {
                ParentInterpreter.Log(this, $"Assign '{leftExpression}' to '{rightExpression}'");

                if (!typeof(IAlgorithmAssignable).IsAssignableFrom(leftExpression.GetType()))
                {
                    ParentInterpreter.ChangeState(this, new AlgorithmInterpreterStateEventArgs(new Error(new NotAssignableException($"The left expression is not assignable."), Statement), ParentInterpreter.GetDebugInfo()));
                    return;
                }
            }

            switch (leftExpression.DomType)
            {
                case AlgorithmDomType.PropertyReferenceExpression:
                    var propertyReferenceInterpreter = new PropertyReference(DebugMode, ParentInterpreter, leftExpression);
                    leftValue = propertyReferenceInterpreter.GetAssignableObject();
                    targetObject = propertyReferenceInterpreter.TargetObject;
                    break;

                case AlgorithmDomType.VariableReferenceExpression:
                    leftValue = new VariableReference(DebugMode, ParentInterpreter, leftExpression).GetAssignableObject();
                    break;

                case AlgorithmDomType.ArrayIndexerExpression:
                    var arrayIndexerInterpreter = new ArrayIndexerExpression(DebugMode, ParentInterpreter, leftExpression);
                    leftValue = arrayIndexerInterpreter.GetAssignableObject();
                    indexValue = arrayIndexerInterpreter.IndexValue;
                    break;

                default:
                    ParentInterpreter.ChangeState(this, new AlgorithmInterpreterStateEventArgs(new Error(new InvalidCastException($"Unable to find an interpreter for this expression : '{leftExpression.GetType().FullName}'"), Statement), ParentInterpreter.GetDebugInfo()));
                    break;
            }

            if (ParentInterpreter.FailedOrStop)
            {
                return;
            }

            rightValue = ParentInterpreter.RunExpression(rightExpression);

            if (ParentInterpreter.FailedOrStop)
            {
                return;
            }

            propertyInfo = leftValue as PropertyInfo;
            if (propertyInfo != null)
            {
                if (!propertyInfo.CanWrite)
                {
                    ParentInterpreter.ChangeState(this, new AlgorithmInterpreterStateEventArgs(new Error(new NotAssignableException($"This core property is not assignable."), Statement), ParentInterpreter.GetDebugInfo()));
                    return;
                }
                propertyInfo.SetValue(targetObject, rightValue);
            }

            propertyVariable = leftValue as Variable;
            if (propertyVariable != null)
            {
                if (propertyVariable.IsArray && !(typeof(Array).IsAssignableFrom(rightValue.GetType()) || typeof(IList).IsAssignableFrom(rightValue.GetType())))
                {
                    ParentInterpreter.ChangeState(this, new AlgorithmInterpreterStateEventArgs(new Error(new NotAssignableException($"The left expression wait for an array, but the right value is not an array."), Statement), ParentInterpreter.GetDebugInfo()));
                    return;
                }
                if (!propertyVariable.IsArray && (typeof(Array).IsAssignableFrom(rightValue.GetType()) || typeof(IList).IsAssignableFrom(rightValue.GetType())))
                {
                    ParentInterpreter.ChangeState(this, new AlgorithmInterpreterStateEventArgs(new Error(new NotAssignableException($"The left expression does not support array value, but the right value is  an array."), Statement), ParentInterpreter.GetDebugInfo()));
                    return;
                }
                propertyVariable.Value = rightValue;
            }

            propertyVariableList = leftValue as IList;
            if (propertyVariableList != null)
            {
                if (indexValue < 0 || indexValue >= propertyVariableList.Count)
                {
                    ParentInterpreter.ChangeState(this, new AlgorithmInterpreterStateEventArgs(new Error(new IndexOutOfRangeException($"Unable to get the item number '{indexValue}' because the limit of the array is '{propertyVariableList.Count - 1}'."), Statement), ParentInterpreter.GetDebugInfo()));
                    return;
                }
                propertyVariableList[indexValue] = rightValue;
            }

            if (DebugMode)
            {
                ParentInterpreter.Log(this, "'{0}' is now equal to {1}", leftExpression.ToString(), rightValue == null ? "{null}" : $"'{rightValue}' (type:{rightValue.GetType().FullName})");
            }
        }

        #endregion
    }
}
