using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.System.Threading;
using Algo.Runtime.Build.AlgorithmDOM.DOM;
using Algo.Runtime.Build.Runtime.Debugger;
using Algo.Runtime.Build.Runtime.Debugger.CallStack;
using Algo.Runtime.Build.Runtime.Debugger.Exceptions;
using Algo.Runtime.Build.Runtime.Interpreter.Interpreter;
using Algo.Runtime.Build.Runtime.Utils;

namespace Algo.Runtime.Build.Runtime.Interpreter.Expressions
{
    internal sealed class InvokeMethod : InterpretExpression
    {
        #region Fields

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

            if (MemTrace)
            {
                ParentInterpreter.Log(this, $"Calling method '{Expression._targetObject}.{Expression._methodName}'");
            }

            var referenceClass = ParentInterpreter.RunExpression(Expression._targetObject) as ClassInterpreter;
            var callerMethod = (MethodInterpreter)ParentInterpreter.GetFirstNextParentInterpreter(InterpreterType.MethodInterpreter);

            if (ParentInterpreter.FailedOrStop)
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

            if (!ParentInterpreter.FailedOrStop)
            {
                var callStackService = ((ProgramInterpreter)ParentInterpreter.GetFirstNextParentInterpreter(InterpreterType.ProgramInterpreter)).DebugInfo.CallStackService;
                _result = null;

                if (callStackService.CallCount > Consts.InvokeMethodCountBeforeNewThread)
                {
                    // Make a new thread avoid the stack overflow.
                    callStackService.CallCount = 0;
                    CallMethodNewThread(referenceClass, argumentValues, callerMethod, callStackService).Wait();
                }
                else
                {
                    callStackService.CallCount++;
                    _result = referenceClass.CallMethod(ParentInterpreter, Expression, argumentValues, callerMethod, callStackService);
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
                if (!ParentInterpreter.FailedOrStop)
                {
                    argumentValues.Add(ParentInterpreter.RunExpression(arg));
                }
            }

            return argumentValues;
        }

        private async Task CallMethodNewThread(ClassInterpreter referenceClass, Collection<object> argumentValues, MethodInterpreter callerMethod, CallStackService callStackService)
        {
            await ThreadPool.RunAsync(delegate { _result = referenceClass.CallMethod(ParentInterpreter, Expression, argumentValues, callerMethod, callStackService); });
        }

        #endregion
    }
}
