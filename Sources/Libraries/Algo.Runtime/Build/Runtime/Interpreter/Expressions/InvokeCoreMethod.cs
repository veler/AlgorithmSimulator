using System;
using System.Collections.ObjectModel;
using System.Linq;
using Algo.Runtime.Build.AlgorithmDOM.DOM;
using Algo.Runtime.Build.Runtime.Debugger;
using Algo.Runtime.Build.Runtime.Debugger.Exceptions;
using Algo.Runtime.Build.Runtime.Interpreter.Interpreter;
using System.Reflection;
using System.Threading.Tasks;

namespace Algo.Runtime.Build.Runtime.Interpreter.Expressions
{
    sealed internal class InvokeCoreMethod : InterpretExpression<AlgorithmInvokeCoreMethodExpression>
    {
        #region Constructors

        internal InvokeCoreMethod(bool memTrace, BlockInterpreter parentInterpreter, AlgorithmInvokeCoreMethodExpression expression)
            : base(memTrace, parentInterpreter, expression)
        {
        }

        #endregion

        #region Methods

        internal override object Execute()
        {
            if (Expression.TargetObect == null)
            {
                ParentInterpreter.ChangeState(this, new SimulatorStateEventArgs(new Error(new NullReferenceException("Unable to invoke a core method when the TargetObject of an AlgorithmInvokeCoreMethodExpression is null."), ParentInterpreter.GetDebugInfo())));
                return null;
            }

            object referenceClass;
            object returnedValue;
            Type type;
            Task<object> taskResult;
            Task task;

            ParentInterpreter.Log(this, $"Calling core method '{Expression.TargetObect}.{Expression.MethodName}'");

            referenceClass = ParentInterpreter.RunExpression(Expression.TargetObect);

            if (ParentInterpreter.Failed)
            {
                return null;
            }

            if (referenceClass == null)
            {
                ParentInterpreter.ChangeState(this, new SimulatorStateEventArgs(new Error(new ClassNotFoundException("{Unknow}", "It looks like the reference object does not exists."), ParentInterpreter.GetDebugInfo())));
                return null;
            }

            if (referenceClass is ClassInterpreter)
            {
                ParentInterpreter.ChangeState(this, new SimulatorStateEventArgs(new Error(new ClassNotFoundException("{Unknow}", "Unable to call a core method from a class made with AlgorithmDOM."), ParentInterpreter.GetDebugInfo())));
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

            if (ParentInterpreter.Failed)
            {
                return null;
            }

            if (Expression.Await)
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
                ParentInterpreter.ChangeState(this, new SimulatorStateEventArgs(new Error(new MethodNotAwaitableException($"{Expression.TargetObect}.{Expression.MethodName}"), ParentInterpreter.GetDebugInfo())));
                return null;
            }

            return returnedValue;
        }

        private object InvokeMethod(Type type, object obj)
        {
            if (Expression.ArgumentsTypes == null)
            {
                Expression.ArgumentsTypes = new Type[0];
            }

            object result;
            Collection<object> arguments;
            var method = type.GetRuntimeMethod(Expression.MethodName.ToString(), Expression.ArgumentsTypes);

            if (method == null)
            {
                ParentInterpreter.ChangeState(this, new SimulatorStateEventArgs(new Error(new MethodNotFoundException(Expression.MethodName.ToString(), $"The method '{Expression.MethodName}' does not exists in the current class or is not accessible."), ParentInterpreter.GetDebugInfo())));
                return null;
            }

            if (obj == null && !method.IsStatic)
            {
                ParentInterpreter.ChangeState(this, new SimulatorStateEventArgs(new Error(new NoInstanceReferenceException($"Unable to invoke the non-static core method '{method.Name}' without instanciate the class."), ParentInterpreter.GetDebugInfo())));
                return null;
            }

            arguments = GetArgumentValues();

            if (ParentInterpreter.Failed)
            {
                return null;
            }

            try
            {
                result = method.Invoke(obj, arguments.ToArray());
            }
            catch (Exception ex)
            {
                if (ex is ArgumentException)
                {
                    ParentInterpreter.ChangeState(this, new SimulatorStateEventArgs(new Error(new BadArgumentException("{Unknow}", ex.Message), ParentInterpreter.GetDebugInfo())));
                }
                else if (ex is TargetParameterCountException)
                {
                    ParentInterpreter.ChangeState(this, new SimulatorStateEventArgs(new Error(new MethodNotFoundException(Expression.MethodName.ToString(), $"There is a method '{Expression.MethodName}' in the class '{Expression.TargetObect}', but it does not have {arguments.Count} argument(s)."), ParentInterpreter.GetDebugInfo())));
                }
                else
                {
                    ParentInterpreter.ChangeState(this, new SimulatorStateEventArgs(new Error(ex, ParentInterpreter.GetDebugInfo())));
                }
                return null;
            }

            return result;
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
