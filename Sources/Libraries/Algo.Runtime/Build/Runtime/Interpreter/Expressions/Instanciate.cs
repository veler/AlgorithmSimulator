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
    /// <summary>
    /// Provide the interpreter for a instanciation
    /// </summary>
    internal sealed class Instanciate : InterpretExpression
    {
        #region Constructors

        /// <summary>
        /// Initialize a new instance of <see cref="Instanciate"/>
        /// </summary>
        /// <param name="debugMode">Defines if the debug mode is enabled</param>
        /// <param name="parentInterpreter">The parent block interpreter</param>
        /// <param name="expression">The algorithm expression</param>
        internal Instanciate(bool debugMode, BlockInterpreter parentInterpreter, AlgorithmExpression expression)
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
            Collection<object> argumentValues;
            var createType = Expression._createType;
            var reference = ParentInterpreter.RunExpression(createType);

            if (ParentInterpreter.FailedOrStop)
            {
                return null;
            }

            if (DebugMode)
            {
                ParentInterpreter.Log(this, $"Creating a new instance of '{createType}'");
            }

            var classInterpreter = reference as ClassInterpreter;
            if (classInterpreter != null)
            {
                var program = ParentInterpreter.ParentProgramInterpreter;
                var classInstance = (classInterpreter).CreateNewInstance();

                classInstance.StateChanged += ParentInterpreter.ChangeState;
                classInstance.OnGetParentInterpreter += new Func<ProgramInterpreter>(() => program);
                classInstance.OnDone += new Action<ClassInterpreter>((cl) =>
                {
                    cl.StateChanged -= ParentInterpreter.ChangeState;
                });
                classInstance.Initialize();

                argumentValues = InvokeMethod.GetArgumentValues(Expression, ParentInterpreter);

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
                argumentValues = InvokeMethod.GetArgumentValues(Expression, ParentInterpreter);

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
                        ParentInterpreter.ChangeState(this, new AlgorithmInterpreterStateEventArgs(new Error(new BadArgumentException("{Unknow}", ex.Message), Expression), ParentInterpreter.GetDebugInfo()));
                    }
                    else if (ex is TargetParameterCountException)
                    {
                        ParentInterpreter.ChangeState(this, new AlgorithmInterpreterStateEventArgs(new Error(new MethodNotFoundException("ctor", $"There is no constructor with {argumentValues.Count} argument(s) in the class '{Expression._createType}'."), Expression), ParentInterpreter.GetDebugInfo()));
                    }
                    else
                    {
                        ParentInterpreter.ChangeState(this, new AlgorithmInterpreterStateEventArgs(new Error(ex, Expression), ParentInterpreter.GetDebugInfo()));
                    }
                    return null;
                }
                return classInstance;
            }
            return null;
        }

        #endregion
    }
}
