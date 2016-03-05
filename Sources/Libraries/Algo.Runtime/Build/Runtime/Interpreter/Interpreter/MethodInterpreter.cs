using Algo.Runtime.Build.AlgorithmDOM.DOM;
using Algo.Runtime.Build.Runtime.Debugger;
using Algo.Runtime.Build.Runtime.Debugger.Exceptions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Algo.Runtime.Build.Runtime.Memory;

namespace Algo.Runtime.Build.Runtime.Interpreter.Interpreter
{
    sealed internal class MethodInterpreter : Interpret
    {
        #region Fields

        private object _returnedValue;

        #endregion

        #region Properties

        internal AlgorithmClassMethodDeclaration MethodDeclaration { get; set; }

        internal bool Done { get; set; }

        internal object ReturnedValue
        {
            get
            {
                return _returnedValue;
            }
            set
            {
                if (value is AlgorithmExpression || value is AlgorithmStatement)
                {
                    ChangeState(this, new SimulatorStateEventArgs(new Error(new Exception("A method's returned value must not be an AlgorithmObject"), GetDebugInfo())));
                    return;
                }
                _returnedValue = value;
            }
        }

        #endregion

        #region Handlers

        internal Action<MethodInterpreter> OnDone;

        #endregion

        #region Constructors

        internal MethodInterpreter(AlgorithmClassMethodDeclaration methodDecl, bool memTrace)
            : base(memTrace)
        {
            MethodDeclaration = methodDecl;
        }

        #endregion

        #region Methods

        internal override void Initialize()
        {
            Variables = new Collection<Variable>();
        }

        internal void Run(bool awaitCall, IReadOnlyList<object> argumentValues)
        {
            if (MethodDeclaration.Arguments.Count != argumentValues.Count)
            {
                ChangeState(this, new SimulatorStateEventArgs(new Error(new MethodNotFoundException(MethodDeclaration.Name.ToString(), $"There is a method '{MethodDeclaration.Name}' in the class '{GetFirstNextParentInterpreter<ClassInterpreter>().ClassDeclaration.Name}', but it does not have {argumentValues.Count} argument(s)."), GetDebugInfo())));
                return;
            }

            if (MethodDeclaration.IsAsync)
            {
                if (awaitCall)
                {
                    RunAsync(argumentValues).Wait();
                }
                else
                {
                    RunAsync(argumentValues);
                }
            }
            else
            {
                RunSync(argumentValues);
            }
        }

        private Task RunAsync(IReadOnlyList<object> argumentValues)
        {
            return Task.Run(() => RunSync(argumentValues));
        }

        private void RunSync(IReadOnlyList<object> argumentValues)
        {
            var block = new BlockInterpreter(MethodDeclaration.Statements, MemTrace);
            block.OnGetParentInterpreter += new Func<MethodInterpreter>(() => this);
            block.StateChanged += ChangeState;
            block.Initialize();

            for (var i = 0; i < MethodDeclaration.Arguments.Count; i++)
            {
                var argDecl = MethodDeclaration.Arguments[i];
                var argValue = argumentValues[i];

                if (!(argValue is string) && argValue is IEnumerable && !argDecl.IsArray)
                {
                    ChangeState(this, new SimulatorStateEventArgs(new Error(new BadArgumentException(argDecl.Name.ToString(), $"The argument's value '{argDecl.Name}' must not be an array of values."), GetParentInterpreter().GetDebugInfo())));
                    return;
                }
                if ((!(argValue is IEnumerable) || argValue is string) && argDecl.IsArray)
                {
                    ChangeState(this, new SimulatorStateEventArgs(new Error(new BadArgumentException(argDecl.Name.ToString(), $"The argument's value '{argDecl.Name}' must be an array of values."), GetParentInterpreter().GetDebugInfo())));
                    return;
                }

                block.AddVariable(argDecl, argValue, true);
            }

            if (Failed)
            {
                return;
            }

            block.UpdateCallStack();
            block.Run();
            block.StateChanged -= ChangeState;
            block.Dispose();

            Done = true;
            OnDone(this);
        }

        public override void Dispose()
        {
            Task.Run(() =>
            {
                if (Variables != null)
                {
                    foreach (var variable in Variables)
                    {
                        var value = variable.Value as IDisposable;
                        if (value != null)
                        {
                            value.Dispose();
                        }
                    }
                    Variables.Clear();
                }
                Variables = null;

                MethodDeclaration = null;
            });
        }

        #endregion
    }
}
