using System;
using System.Collections;
using System.Reflection;
using Algo.Runtime.Build.AlgorithmDOM.DOM;
using Algo.Runtime.Build.Runtime.Debugger;
using Algo.Runtime.Build.Runtime.Debugger.Exceptions;
using Algo.Runtime.Build.Runtime.Interpreter.Interpreter;

namespace Algo.Runtime.Build.Runtime.Interpreter.Expressions
{
    /// <summary>
    /// Provide the interpreter for a reference to an index of an array.
    /// </summary>
    internal sealed class ArrayIndexerExpression : InterpretExpression, IAssignable
    {
        #region Properties

        /// <summary>
        /// Gets or sets the index value
        /// </summary>
        internal int IndexValue { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initialize a new instance of <see cref="ArrayIndexerExpression"/>
        /// </summary>
        /// <param name="debugMode">Defines if the debug mode is enabled</param>
        /// <param name="parentInterpreter">The parent block interpreter</param>
        /// <param name="expression">The algorithm expression</param>
        internal ArrayIndexerExpression(bool debugMode, BlockInterpreter parentInterpreter, AlgorithmExpression expression)
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
            IndexValue = -1;

            var indexableValue = (IList)GetAssignableObject();

            if (ParentInterpreter.FailedOrStop || indexableValue == null)
            {
                return null;
            }
            
            if (IndexValue < 0 || IndexValue >= indexableValue.Count)
            {
                ParentInterpreter.ChangeState(this, new AlgorithmInterpreterStateEventArgs(new Error(new IndexOutOfRangeException($"Unable to get the item number '{IndexValue}' because the limit of the array is '{indexableValue.Count - 1}'."), Expression), ParentInterpreter.GetDebugInfo()));
                return null;
            }

            return indexableValue[IndexValue];
        }

        /// <summary>
        /// Returns the corresponding assignable object
        /// </summary>
        /// <returns>the assignable object</returns>
        public object GetAssignableObject()
        {
            if (Expression._targetObject == null)
            {
                ParentInterpreter.ChangeState(this, new AlgorithmInterpreterStateEventArgs(new Error(new NullReferenceException("Unable to access to a property or variable when the TargetObject of an AlgorithmArrayIndexerExpression is null."), Expression), ParentInterpreter.GetDebugInfo()));
                return null;
            }

            if (DebugMode)
            {
                ParentInterpreter.Log(this, $"Getting the value '{Expression}'");
            }

            var indexableValue = ParentInterpreter.RunExpression(Expression._targetObject);

            if (ParentInterpreter.FailedOrStop)
            {
                return null;
            }

            if (indexableValue == null)
            {
                ParentInterpreter.ChangeState(this, new AlgorithmInterpreterStateEventArgs(new Error(new NoInstanceReferenceException("Unable to get a value because the array is null."), Expression), ParentInterpreter.GetDebugInfo()));
                return null;
            }
            // ReSharper disable once UseIsOperator.1
            // ReSharper disable once UseMethodIsInstanceOfType
            if (!typeof(IList).IsAssignableFrom(indexableValue.GetType()))
            {
                ParentInterpreter.ChangeState(this, new AlgorithmInterpreterStateEventArgs(new Error(new IndexerException(Expression._propertyName.ToString()), Expression), ParentInterpreter.GetDebugInfo()));
                return null;
            }

            if (ParentInterpreter.FailedOrStop)
            {
                return null;
            }

            var indiceValue = ParentInterpreter.RunExpression(Expression._indice);

            if (ParentInterpreter.FailedOrStop)
            {
                return null;
            }

            var indexValue = indiceValue as int?;
            if (indexValue != null)
            {
                IndexValue = indexValue.Value;
                return indexableValue;
            }

            ParentInterpreter.ChangeState(this, new AlgorithmInterpreterStateEventArgs(new Error(new InvalidCastException("Unable to cast this value to a number."), Expression), ParentInterpreter.GetDebugInfo()));
            return null;
        }

        #endregion
    }
}
