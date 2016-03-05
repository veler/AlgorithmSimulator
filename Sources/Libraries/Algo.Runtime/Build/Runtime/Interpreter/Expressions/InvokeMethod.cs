using System;
using System.Collections.ObjectModel;
using Algo.Runtime.Build.AlgorithmDOM.DOM;
using Algo.Runtime.Build.Runtime.Debugger;
using Algo.Runtime.Build.Runtime.Debugger.Exceptions;
using Algo.Runtime.Build.Runtime.Interpreter.Interpreter;

namespace Algo.Runtime.Build.Runtime.Interpreter.Expressions
{
    sealed internal class InvokeMethod : InterpretExpression<AlgorithmInvokeMethodExpression>
    {
        #region Constructors

        internal InvokeMethod(bool memTrace, BlockInterpreter parentInterpreter, AlgorithmInvokeMethodExpression expression)
            : base(memTrace, parentInterpreter, expression)
        {
        }

        #endregion

        #region Methods

        internal override object Execute()
        {
            if (Expression.TargetObect == null)
            {
                ParentInterpreter.ChangeState(this, new SimulatorStateEventArgs(new Error(new NullReferenceException("Unable to invoke a method when the TargetObject of an AlgorithmInvokeMethodExpression is null."), ParentInterpreter.GetDebugInfo())));
                return null;
            }

            ParentInterpreter.Log(this, $"Calling method '{Expression.TargetObect}.{Expression.MethodName}'");

            var referenceClass = ParentInterpreter.RunExpression(Expression.TargetObect) as ClassInterpreter;

            if (ParentInterpreter.Failed)
            {
                return null;
            }

            if (referenceClass == null)
            {
                ParentInterpreter.ChangeState(this, new SimulatorStateEventArgs(new Error(new ClassNotFoundException("{Unknow}", "It looks like the reference class does not exists."), ParentInterpreter.GetDebugInfo())));
                return null;
            }

            if (!referenceClass.IsInstance)
            {
                ParentInterpreter.ChangeState(this, new SimulatorStateEventArgs(new Error(new NoInstanceReferenceException("Unable to invoke a method of a not instancied class."), ParentInterpreter.GetDebugInfo())));
                return null;
            }

            var argumentValues = GetArgumentValues();

            if (!ParentInterpreter.Failed)
            {
                // TODO: Detect infinite loop and stackoverflow
                ParentInterpreter.UpdateCallStack();
                return referenceClass.CallMethod(ParentInterpreter, Expression, argumentValues);
            }
            return null;
        }

        private Collection<object> GetArgumentValues()
        {
            var argumentValues = new Collection<object>();

            foreach (var arg in Expression.Arguments)
            {
                if (!ParentInterpreter.Failed)
                {
                    argumentValues.Add(ParentInterpreter.RunExpression(arg));
                }
            }

            return argumentValues;
        }

        #endregion
    }
}
