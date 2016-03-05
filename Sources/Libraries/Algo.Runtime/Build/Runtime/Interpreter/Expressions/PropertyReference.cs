using System;
using System.Reflection;
using Algo.Runtime.Build.AlgorithmDOM.DOM;
using Algo.Runtime.Build.Runtime.Debugger;
using Algo.Runtime.Build.Runtime.Debugger.Exceptions;
using Algo.Runtime.Build.Runtime.Interpreter.Interpreter;
using Algo.Runtime.Build.Runtime.Memory;

namespace Algo.Runtime.Build.Runtime.Interpreter.Expressions
{
    sealed internal class PropertyReference : InterpretExpression<AlgorithmPropertyReferenceExpression>, IAssignable
    {
        #region Properties

        internal object TargetObject { get; set; }

        #endregion

        #region Constructors

        internal PropertyReference(bool memTrace, BlockInterpreter parentInterpreter, AlgorithmPropertyReferenceExpression expression)
            : base(memTrace, parentInterpreter, expression)
        {
        }

        #endregion

        #region Methods

        internal override object Execute()
        {
            object value = null;
            PropertyInfo propertyInfo;
            Variable propertyVariable;
            var property = GetAssignableObject();

            if (ParentInterpreter.Failed)
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

        public object GetAssignableObject()
        {
            if (Expression.TargetObect == null)
            {
                ParentInterpreter.ChangeState(this, new SimulatorStateEventArgs(new Error(new NullReferenceException("Unable to access to a property when the TargetObject of an AlgorithmPropertyReferenceExpression is null."), ParentInterpreter.GetDebugInfo())));
                return null;
            }

            object property;
            object value;
            PropertyInfo propertyInfo;
            ClassInterpreter classTargetObject;
            Variable propertyVariable;

            ParentInterpreter.Log(this, $"Getting the property '{Expression}'");

            TargetObject = ParentInterpreter.RunExpression(Expression.TargetObect);

            if (ParentInterpreter.Failed)
            {
                return null;
            }

            if (TargetObject == null)
            {
                ParentInterpreter.ChangeState(this, new SimulatorStateEventArgs(new Error(new ClassNotFoundException("{Unknow}", "It looks like the reference object does not exists."), ParentInterpreter.GetDebugInfo())));
                return null;
            }

            classTargetObject = TargetObject as ClassInterpreter;
            if (classTargetObject != null)
            {
                propertyVariable = classTargetObject.FindVariableInTheCurrentInterpreter(Expression.PropertyName.ToString());

                if (propertyVariable == null)
                {
                    ParentInterpreter.ChangeState(this, new SimulatorStateEventArgs(new Error(new PropertyNotFoundException(Expression.PropertyName.ToString()), ParentInterpreter.GetDebugInfo())));
                    return null;
                }

                property = propertyVariable;
                value = propertyVariable.Value;
            }
            else
            {
                propertyInfo = TargetObject.GetType().GetProperty(Expression.PropertyName.ToString());

                if (propertyInfo == null)
                {
                    ParentInterpreter.ChangeState(this, new SimulatorStateEventArgs(new Error(new PropertyNotFoundException(Expression.PropertyName.ToString()), ParentInterpreter.GetDebugInfo())));
                    return null;
                }

                property = propertyInfo;
                value = propertyInfo.GetValue(TargetObject);
            }

            ParentInterpreter.Log(this, "Value of the property '{0}' is {1}", Expression.PropertyName.ToString(), value == null ? "{null}" : $"'{value}' (type:{value.GetType().FullName})");

            return property;
        }

        #endregion
    }
}
