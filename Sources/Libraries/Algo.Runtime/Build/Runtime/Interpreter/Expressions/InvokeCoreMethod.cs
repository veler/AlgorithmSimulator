using System;
using System.Collections.ObjectModel;
using System.Linq;
using Algo.Runtime.Build.AlgorithmDOM.DOM;
using Algo.Runtime.Build.Runtime.Debugger;
using Algo.Runtime.Build.Runtime.Debugger.Exceptions;
using Algo.Runtime.Build.Runtime.Interpreter.Interpreter;
using System.Reflection;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;

namespace Algo.Runtime.Build.Runtime.Interpreter.Expressions
{
    /// <summary>
    /// Provide the interpreter for a invocation of a core method
    /// </summary>
    internal sealed class InvokeCoreMethod : InterpretExpression
    {
        #region Fields

        private readonly CoreDispatcher _dispatcher;

        #endregion

        #region Constructors

        /// <summary>
        /// Initialize a new instance of <see cref="InvokeCoreMethod"/>
        /// </summary>
        /// <param name="debugMode">Defines if the debug mode is enabled</param>
        /// <param name="parentInterpreter">The parent block interpreter</param>
        /// <param name="expression">The algorithm expression</param>
        internal InvokeCoreMethod(bool debugMode, BlockInterpreter parentInterpreter, AlgorithmExpression expression)
            : base(debugMode, parentInterpreter, expression)
        {
            _dispatcher = CoreApplication.MainView.CoreWindow.Dispatcher;
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
                ParentInterpreter.ChangeState(this, new AlgorithmInterpreterStateEventArgs(new Error(new NullReferenceException("Unable to invoke a core method when the TargetObject of an AlgorithmInvokeCoreMethodExpression is null."), Expression), ParentInterpreter.GetDebugInfo()));
                return null;
            }

            object referenceClass;
            object returnedValue;
            Type type;
            Task task;

            if (DebugMode)
            {
                ParentInterpreter.Log(this, $"Calling core method '{Expression._targetObject}.{Expression._methodName}'");
            }

            referenceClass = ParentInterpreter.RunExpression(Expression._targetObject);

            if (ParentInterpreter.FailedOrStop)
            {
                return null;
            }

            if (referenceClass == null)
            {
                ParentInterpreter.ChangeState(this, new AlgorithmInterpreterStateEventArgs(new Error(new ClassNotFoundException("{Unknow}", "It looks like the reference object does not exists."), Expression), ParentInterpreter.GetDebugInfo()));
                return null;
            }

            if (referenceClass is ClassInterpreter)
            {
                ParentInterpreter.ChangeState(this, new AlgorithmInterpreterStateEventArgs(new Error(new ClassNotFoundException("{Unknow}", "Unable to call a core method from a class made with AlgorithmDOM."), Expression), ParentInterpreter.GetDebugInfo()));
                return null;
            }

            type = referenceClass as Type;
            if (type != null)
            {
                returnedValue = InvokeMethod(type, null);
            }
            else
            {
                returnedValue = InvokeMethod(referenceClass.GetType(), referenceClass);
            }

            if (ParentInterpreter.FailedOrStop)
            {
                return null;
            }

            if (Expression._await)
            {
                task = returnedValue as Task;
                if (task != null)
                {
                    task.Wait();
                    var resultPropertyInfo = task.GetType().GetProperty("Result");
                    if (resultPropertyInfo != null && resultPropertyInfo.PropertyType.Name != "VoidTaskResult")
                    {
                        return resultPropertyInfo.GetValue(task);
                    }
                    return null;
                }
                ParentInterpreter.ChangeState(this, new AlgorithmInterpreterStateEventArgs(new Error(new MethodNotAwaitableException($"{Expression._targetObject}.{Expression._methodName}"), Expression), ParentInterpreter.GetDebugInfo()));
                return null;
            }

            return returnedValue;
        }

        /// <summary>
        /// Invoke a code method
        /// </summary>
        /// <param name="type">The type which contains the method</param>
        /// <param name="obj">The instance</param>
        /// <returns>Returns the result of the invoke</returns>
        private object InvokeMethod(Type type, object obj)
        {
            if (Expression._argumentsTypes == null)
            {
                Expression._argumentsTypes = new Type[0];
            }

            object result;
            Collection<object> arguments;
            var method = type.GetRuntimeMethod(Expression._methodName.ToString(), Expression._argumentsTypes);

            if (method == null)
            {
                ParentInterpreter.ChangeState(this, new AlgorithmInterpreterStateEventArgs(new Error(new MethodNotFoundException(Expression._methodName.ToString(), $"The method '{Expression._methodName}' does not exists in the current class or is not accessible."), Expression), ParentInterpreter.GetDebugInfo()));
                return null;
            }

            if (obj == null && !method.IsStatic)
            {
                ParentInterpreter.ChangeState(this, new AlgorithmInterpreterStateEventArgs(new Error(new NoInstanceReferenceException($"Unable to invoke the non-static core method '{method.Name}' without instanciate the class."), Expression), ParentInterpreter.GetDebugInfo()));
                return null;
            }

            arguments = Expressions.InvokeMethod.GetArgumentValues(Expression, ParentInterpreter);

            if (ParentInterpreter.FailedOrStop)
            {
                return null;
            }

            result = null;
            _dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                try
                {
                    result = method.Invoke(obj, arguments.ToArray());
                }
                catch (Exception ex)
                {
                    if (ex is ArgumentException)
                    {
                        ParentInterpreter.ChangeState(this, new AlgorithmInterpreterStateEventArgs(new Error(new BadArgumentException("{Unknow}", ex.Message), Expression), ParentInterpreter.GetDebugInfo()));
                    }
                    else if (ex is TargetParameterCountException)
                    {
                        ParentInterpreter.ChangeState(this, new AlgorithmInterpreterStateEventArgs(new Error(new MethodNotFoundException(Expression._methodName.ToString(), $"There is a method '{Expression._methodName}' in the class '{Expression._targetObject}', but it does not have {arguments.Count} argument(s)."), Expression), ParentInterpreter.GetDebugInfo()));
                    }
                    else
                    {
                        ParentInterpreter.ChangeState(this, new AlgorithmInterpreterStateEventArgs(new Error(ex, Expression), ParentInterpreter.GetDebugInfo()));
                    }
                }
            }).AsTask().Wait();

            return result;
        }
        
        #endregion
    }
}
