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
    sealed internal class Instanciate : InterpretExpression<AlgorithmInstanciateExpression>
    {
        #region Constructors

        internal Instanciate(bool memTrace, BlockInterpreter parentInterpreter, AlgorithmInstanciateExpression expression)
            : base(memTrace, parentInterpreter, expression)
        {
        }

        #endregion

        #region Methods

        internal override object Execute()
        {
            Collection<object> argumentValues;
            var createType = Expression.CreateType;
            var reference = ParentInterpreter.RunExpression(createType);

            if (ParentInterpreter.Failed)
            {
                return null;
            }

            ParentInterpreter.Log(this, $"Creating a new instance of '{createType}'");

            var classInterpreter = reference as ClassInterpreter;
            if (classInterpreter != null)
            {
                var program = ParentInterpreter.GetFirstNextParentInterpreter<ProgramInterpreter>();
                var classInstance = (classInterpreter).CreateNewInstance();
                
                classInstance.StateChanged += ParentInterpreter.ChangeState;
                classInstance.OnGetParentInterpreter += new Func<ProgramInterpreter>(() => program);
                classInstance.OnDone += new Action<ClassInterpreter>((cl) =>
                {
                    cl.StateChanged -= ParentInterpreter.ChangeState;
                });
                classInstance.Initialize();

                argumentValues = GetArgumentValues();

                if (ParentInterpreter.Failed)
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

                if (ParentInterpreter.Failed)
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
                        ParentInterpreter.ChangeState(this, new SimulatorStateEventArgs(new Error(new BadArgumentException("{Unknow}", ex.Message), ParentInterpreter.GetDebugInfo())));
                    }
                    else if (ex is TargetParameterCountException)
                    {
                        ParentInterpreter.ChangeState(this, new SimulatorStateEventArgs(new Error(new MethodNotFoundException("ctor", $"There is no constructor with {argumentValues.Count} argument(s) in the class '{Expression.CreateType}'."), ParentInterpreter.GetDebugInfo())));
                    }
                    else
                    {
                        ParentInterpreter.ChangeState(this, new SimulatorStateEventArgs(new Error(ex, ParentInterpreter.GetDebugInfo())));
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
