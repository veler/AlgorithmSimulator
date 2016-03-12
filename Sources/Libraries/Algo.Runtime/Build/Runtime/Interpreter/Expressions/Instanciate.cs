using System;
using Algo.Runtime.Build.AlgorithmDOM.DOM;
using Algo.Runtime.Build.Runtime.Debugger;
using Algo.Runtime.Build.Runtime.Debugger.Exceptions;
using Algo.Runtime.Build.Runtime.Interpreter.Interpreter;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

namespace Algo.Runtime.Build.Runtime.Interpreter.Expressions
{
    internal sealed class Instanciate : InterpretExpression
    {
        #region Constructors

        internal Instanciate(bool memTrace, BlockInterpreter parentInterpreter, AlgorithmExpression expression)
            : base(memTrace, parentInterpreter, expression)
        {
        }

        #endregion

        #region Methods

        internal override object Execute()
        {
            Collection<object> argumentValues;
            var createType = Expression._createType;
            var reference = ParentInterpreter.RunExpression(createType);

            if (ParentInterpreter.FailedOrStop)
            {
                return null;
            }

            if (MemTrace)
            {
                ParentInterpreter.Log(this, $"Creating a new instance of '{createType}'");
            }

            var classInterpreter = reference as ClassInterpreter;
            if (classInterpreter != null)
            {
                var program = (ProgramInterpreter)ParentInterpreter.GetFirstNextParentInterpreter(InterpreterType.ProgramInterpreter);
                var classInstance = (classInterpreter).CreateNewInstance();

                classInstance.StateChanged += ParentInterpreter.ChangeState;
                classInstance.OnGetParentInterpreter += new Func<ProgramInterpreter>(() => program);
                classInstance.OnDone += new Action<ClassInterpreter>((cl) =>
                {
                    cl.StateChanged -= ParentInterpreter.ChangeState;
                });
                classInstance.Initialize();

                argumentValues = GetArgumentValues();

                if (ParentInterpreter.FailedOrStop)
                {
                    return null;
                }

                classInstance.CreateNewInstanceCallConstructors(argumentValues);
                return classInstance;
            }

            var type = reference as Type;
            if (type != null)
            {
                object classInstance = null;
                argumentValues = GetArgumentValues();

                if (ParentInterpreter.FailedOrStop)
                {
                    return null;
                }

                try
                {
                    classInstance = Activator.CreateInstance(type, argumentValues.ToArray());
                }
                catch (Exception ex)
                {
                    if (ex is ArgumentException)
                    {
                        ParentInterpreter.ChangeState(this, new SimulatorStateEventArgs(new Error(new BadArgumentException("{Unknow}", ex.Message)), ParentInterpreter.GetDebugInfo()));
                    }
                    else if (ex is TargetParameterCountException)
                    {
                        ParentInterpreter.ChangeState(this, new SimulatorStateEventArgs(new Error(new MethodNotFoundException("ctor", $"There is no constructor with {argumentValues.Count} argument(s) in the class '{Expression._createType}'.")), ParentInterpreter.GetDebugInfo()));
                    }
                    else
                    {
                        ParentInterpreter.ChangeState(this, new SimulatorStateEventArgs(new Error(ex), ParentInterpreter.GetDebugInfo()));
                    }
                    return null;
                }
                return classInstance;
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

        #endregion
    }
}
