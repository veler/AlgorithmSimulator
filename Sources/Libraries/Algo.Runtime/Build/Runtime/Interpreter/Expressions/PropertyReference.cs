using System;
using System.Reflection;
using Algo.Runtime.Build.AlgorithmDOM.DOM;
using Algo.Runtime.Build.Runtime.Debugger;
using Algo.Runtime.Build.Runtime.Debugger.Exceptions;
using Algo.Runtime.Build.Runtime.Interpreter.Interpreter;
using Algo.Runtime.Build.Runtime.Memory;

namespace Algo.Runtime.Build.Runtime.Interpreter.Expressions
{
    /// <summary>
    /// Provide the interpreter for a invocation
    /// </summary>
    internal sealed class PropertyReference : InterpretExpression, IAssignable
    {
        #region Properties

        /// <summary>
        /// Gets or sets the targeted object
        /// </summary>
        internal object TargetObject { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initialize a new instance of <see cref="PropertyReference"/>
        /// </summary>
        /// <param name="debugMode">Defines if the debug mode is enabled</param>
        /// <param name="parentInterpreter">The parent block interpreter</param>
        /// <param name="expression">The algorithm expression</param>
        internal PropertyReference(bool debugMode, BlockInterpreter parentInterpreter, AlgorithmExpression expression)
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
            object value = null;
            PropertyInfo propertyInfo;
            Variable propertyVariable;
            var property = GetAssignableObject();

            if (ParentInterpreter.FailedOrStop)
            {
                return null;
            }

            propertyInfo = property as PropertyInfo;
            if (propertyInfo != null)
            {
                value = propertyInfo.GetValue(TargetObject);
            }

            propertyVariable = property as Variable;
            if (propertyVariable != null)
            {
                value = propertyVariable.Value;
            }

            return value;
        }

        /// <summary>
        /// Returns the corresponding assignable object
        /// </summary>
        /// <returns>the assignable object</returns>
        public object GetAssignableObject()
        {
            if (Expression._targetObject == null)
            {
                ParentInterpreter.ChangeState(this, new AlgorithmInterpreterStateEventArgs(new Error(new NullReferenceException("Unable to access to a property when the TargetObject of an AlgorithmPropertyReferenceExpression is null."), Expression), ParentInterpreter.GetDebugInfo()));
                return null;
            }

            object property;
            object value;
            PropertyInfo propertyInfo;
            ClassInterpreter classTargetObject;
            Variable propertyVariable;

            if (DebugMode)
            {
                ParentInterpreter.Log(this, $"Getting the property '{Expression}'");
            }

            TargetObject = ParentInterpreter.RunExpression(Expression._targetObject);

            if (ParentInterpreter.FailedOrStop)
            {
                return null;
            }

            if (TargetObject == null)
            {
                ParentInterpreter.ChangeState(this, new AlgorithmInterpreterStateEventArgs(new Error(new ClassNotFoundException("{Unknow}", "It looks like the reference object does not exists."), Expression), ParentInterpreter.GetDebugInfo()));
                return null;
            }

            classTargetObject = TargetObject as ClassInterpreter;
            if (classTargetObject != null)
            {
                propertyVariable = classTargetObject.FindVariableInTheCurrentInterpreter(Expression._propertyName.ToString());

                if (propertyVariable == null)
                {
                    ParentInterpreter.ChangeState(this, new AlgorithmInterpreterStateEventArgs(new Error(new PropertyNotFoundException(Expression._propertyName.ToString()), Expression), ParentInterpreter.GetDebugInfo()));
                    return null;
                }

                property = propertyVariable;
                value = propertyVariable.Value;
            }
            else
            {
                propertyInfo = TargetObject.GetType().GetProperty(Expression._propertyName.ToString());

                if (propertyInfo == null)
                {
                    ParentInterpreter.ChangeState(this, new AlgorithmInterpreterStateEventArgs(new Error(new PropertyNotFoundException(Expression._propertyName.ToString()), Expression), ParentInterpreter.GetDebugInfo()));
                    return null;
                }

                property = propertyInfo;
                value = propertyInfo.GetValue(TargetObject);
            }

            if (DebugMode)
            {
                ParentInterpreter.Log(this, "Value of the property '{0}' is {1}", Expression._propertyName.ToString(), value == null ? "{null}" : $"'{value}' (type:{value.GetType().FullName})");
            }
            return property;
        }

        #endregion
    }
}
