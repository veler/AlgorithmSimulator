using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.System.Threading;
using Algo.Runtime.Build.AlgorithmDOM.DOM;
using Algo.Runtime.Build.Runtime.Debugger;
using Algo.Runtime.Build.Runtime.Debugger.Exceptions;
using Algo.Runtime.Build.Runtime.Interpreter.Interpreter;

namespace Algo.Runtime.Build.Runtime.Interpreter.Expressions
{
    internal sealed class InvokeMethod : InterpretExpression
    {
        #region Fields

        private static short _callCount = 0;

        private object _result;

        #endregion

        #region Constructors

        internal InvokeMethod(bool memTrace, BlockInterpreter parentInterpreter, AlgorithmExpression expression)
            : base(memTrace, parentInterpreter, expression)
        {
        }

        #endregion

        #region Methods

        internal override object Execute()
        {
            if (Expression._targetObject == null)
            {
                ParentInterpreter.ChangeState(this, new SimulatorStateEventArgs(new Error(new NullReferenceException("Unable to invoke a method when the TargetObject of an AlgorithmInvokeMethodExpression is null."), ParentInterpreter.GetDebugInfo())));
                return null;
            }

            ParentInterpreter.Log(this, $"Calling method '{Expression._targetObject}.{Expression._methodName}'");

            var referenceClass = ParentInterpreter.RunExpression(Expression._targetObject) as ClassInterpreter;

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
                _result = null;
                ParentInterpreter.UpdateCallStack();

                if (_callCount > 400)
                {
                    // Make a new thread avoid the stack overflow.
                    _callCount = 0;
                    CallMethodNewThread(referenceClass, argumentValues).Wait();
                }
                else
                {
                    _callCount++;
                    _result = referenceClass.CallMethod(ParentInterpreter, Expression, argumentValues);
                }

                return _result;
            }
            return null;
        }

        private Collection<object> GetArgumentValues()
        {
            var argumentValues = new Collection<object>();

            foreach (var arg in Expression._argumentsExpression)
            {
                if (!ParentInterpreter.Failed)
                {
                    argumentValues.Add(ParentInterpreter.RunExpression(arg));
                }
            }

            return argumentValues;
        }

        private async Task CallMethodNewThread(ClassInterpreter referenceClass, Collection<object> argumentValues)
        {
            await ThreadPool.RunAsync(delegate { _result = referenceClass.CallMethod(ParentInterpreter, Expression, argumentValues); });
        }

        #endregion
    }
}
