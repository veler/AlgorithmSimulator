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
    /// <summary>
    /// Provide the interpreter for a invocation
    /// </summary>
    internal sealed class InvokeMethod : InterpretExpression
    {
        #region Fields

        private object _result;

        #endregion

        #region Constructors

        /// <summary>
        /// Initialize a new instance of <see cref="InvokeMethod"/>
        /// </summary>
        /// <param name="debugMode">Defines if the debug mode is enabled</param>
        /// <param name="parentInterpreter">The parent block interpreter</param>
        /// <param name="expression">The algorithm expression</param>
        internal InvokeMethod(bool debugMode, BlockInterpreter parentInterpreter, AlgorithmExpression expression)
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
            if (Expression._targetObject == null)
            {
                ParentInterpreter.ChangeState(this, new AlgorithmInterpreterStateEventArgs(new Error(new NullReferenceException("Unable to invoke a method when the TargetObject of an AlgorithmInvokeMethodExpression is null."), Expression), ParentInterpreter.GetDebugInfo()));
                return null;
            }

            if (DebugMode)
            {
                ParentInterpreter.Log(this, $"Calling method '{Expression._targetObject}.{Expression._methodName}'");
            }

            var referenceClass = ParentInterpreter.RunExpression(Expression._targetObject) as ClassInterpreter;
            var callerMethod = ParentInterpreter.ParentMethodInterpreter;

            if (ParentInterpreter.FailedOrStop)
            {
                return null;
            }

            if (referenceClass == null)
            {
                ParentInterpreter.ChangeState(this, new AlgorithmInterpreterStateEventArgs(new Error(new ClassNotFoundException("{Unknow}", "It looks like the reference class does not exists."), Expression), ParentInterpreter.GetDebugInfo()));
                return null;
            }

            if (!referenceClass.IsInstance)
            {
                ParentInterpreter.ChangeState(this, new AlgorithmInterpreterStateEventArgs(new Error(new NoInstanceReferenceException("Unable to invoke a method of a not instancied class."), Expression), ParentInterpreter.GetDebugInfo()));
                return null;
            }

            var argumentValues = GetArgumentValues(Expression, ParentInterpreter);

            if (!ParentInterpreter.FailedOrStop)
            {
                var callStackService = ParentInterpreter.ParentProgramInterpreter.DebugInfo.CallStackService;
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
                    _result = referenceClass.InvokeMethod(ParentInterpreter, Expression, argumentValues, callerMethod, callStackService);
                }

                return _result;
            }
            return null;
        }
        
        /// <summary>
        /// Invoke a method in a new thread to avoid a stack overflow exception
        /// </summary>
        /// <param name="referenceClass">The class reference</param>
        /// <param name="argumentValues">The arguments values</param>
        /// <param name="callerMethod">The method from which we do the call</param>
        /// <param name="callStackService">The user call stack service</param>
        /// <returns></returns>
        private async Task CallMethodNewThread(ClassInterpreter referenceClass, Collection<object> argumentValues, MethodInterpreter callerMethod, CallStackService callStackService)
        {
            await ThreadPool.RunAsync(delegate { _result = referenceClass.InvokeMethod(ParentInterpreter, Expression, argumentValues, callerMethod, callStackService); });
        }

        /// <summary>
        /// Returns the arguments values
        /// </summary>
        /// <returns>Returns a collection of object which represents the argument value</returns>
        internal static Collection<object> GetArgumentValues(AlgorithmExpression expression, BlockInterpreter parentInterpreter)
        {
            var argumentValues = new Collection<object>();

            foreach (var arg in expression._argumentsExpression)
            {
                if (!parentInterpreter.FailedOrStop)
                {
                    argumentValues.Add(parentInterpreter.RunExpression(arg));
                }
            }

            return argumentValues;
        }

        #endregion
    }
}
