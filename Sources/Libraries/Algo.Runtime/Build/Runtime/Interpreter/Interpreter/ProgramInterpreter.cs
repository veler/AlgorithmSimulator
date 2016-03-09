using Algo.Runtime.Build.AlgorithmDOM.DOM;
using Algo.Runtime.Build.Runtime.Memory;
using System.Collections.ObjectModel;
using Algo.Runtime.Build.Runtime.Debugger;
using Algo.Runtime.Build.Runtime.Debugger.Exceptions;
using System;
using System.Threading.Tasks;
using Algo.Runtime.Build.Runtime.Debugger.CallStack;
using Algo.Runtime.Build.Runtime.Interpreter.Expressions;
using Algo.Runtime.ComponentModel.OperatorHelper;
using Newtonsoft.Json;

namespace Algo.Runtime.Build.Runtime.Interpreter.Interpreter
{
    internal sealed class ProgramInterpreter : Interpret
    {
        #region Properties

        [JsonIgnore]
        internal override InterpreterType InterpreterType => InterpreterType.ProgramInterpreter;

        [JsonProperty]
        internal AlgorithmProgram ProgramDeclaration { get; set; }

        [JsonProperty]
        internal Collection<ClassInterpreter> Classes { get; set; }

        [JsonProperty]
        internal SimulatorState State { get; private set; }

        [JsonProperty]
        internal DebugInfo DebugInfo { get; private set; }

        [JsonProperty]
        private ClassInterpreter EntryPointInstance { get; set; }

        #endregion

        #region Constructors

        internal ProgramInterpreter(AlgorithmProgram programDecl, bool memTrace)
            : base(memTrace)
        {
            ProgramDeclaration = programDecl;

            // Just for better performances after, we call it a first time
            OperatorHelperCache.Initialize();
        }

        #endregion

        #region Methods

        internal override void Initialize()
        {
            Variables = new Collection<Variable>();
            Classes = new Collection<ClassInterpreter>();
            DebugInfo = new DebugInfo();

            foreach (var variable in ProgramDeclaration.Variables)
            {
                if (!FailedOrStop)
                {
                    AddVariable(variable);
                }
            }

            foreach (var cl in ProgramDeclaration.Classes)
            {
                if (!FailedOrStop)
                {
                    Classes.Add(new ClassInterpreter(cl, MemTrace));
                }
            }
        }

        internal void Start()
        {
            ChangeState(this, new SimulatorStateEventArgs(SimulatorState.Preparing));

            Initialize();

            var entryPointMethod = ProgramDeclaration.GetEntryPointMethod();
            var i = 0;
            ClassInterpreter entryPointClass = null;

            if (entryPointMethod == null)
            {
                ChangeState(this, new SimulatorStateEventArgs(new Error(new MissingEntryPointMethodException(ProgramDeclaration.EntryPointPath), GetDebugInfo())));
                return;
            }

            ChangeState(this, new SimulatorStateEventArgs(SimulatorState.Running));

            while (i < Classes.Count && entryPointClass == null)
            {
                if (Classes[i].ClassDeclaration.Name.ToString() == ProgramDeclaration.EntryPointPath)
                {
                    entryPointClass = Classes[i];
                }

                i++;
            }

            if (entryPointClass == null)
            {
                ChangeState(this, new SimulatorStateEventArgs(new Error(new MissingEntryPointMethodException(ProgramDeclaration.EntryPointPath), GetDebugInfo())));
                return;
            }

            // TODO: try to use the Instanciate & InvokeMethod interpreter's functions
            EntryPointInstance = entryPointClass.CreateNewInstance();
            EntryPointInstance.StateChanged += ChangeState;
            EntryPointInstance.OnGetParentInterpreter += new Func<ProgramInterpreter>(() => this);
            EntryPointInstance.OnDone += new Action<ClassInterpreter>((cl) =>
            {
                cl.StateChanged -= ChangeState;
            });
            EntryPointInstance.Initialize();
            EntryPointInstance.CreateNewInstanceCallConstructors(null);

            EntryPointInstance.EntryPoint.StateChanged += ChangeState;
            EntryPointInstance.EntryPoint.OnGetParentInterpreter += new Func<ClassInterpreter>(() => EntryPointInstance);
            EntryPointInstance.EntryPoint.OnDone += new Action<MethodInterpreter>((met) =>
            {
                met.Dispose();
                met.StateChanged -= ChangeState;
            });
            EntryPointInstance.EntryPoint.Initialize();
            EntryPointInstance.EntryPoint.Run(false, new Collection<object>(), Guid.Empty);

            EntryPointInstance.StateChanged -= ChangeState;
        }

        internal void Stop()
        {
            ChangeState(this, new SimulatorStateEventArgs(SimulatorState.Stopped));
            Task.Delay(TimeSpan.FromSeconds(1)).Wait();
        }

        internal override void ChangeState(object source, SimulatorStateEventArgs e)
        {
            base.ChangeState(source, e);

            if (e.State != SimulatorState.Log)
            {
                State = e.State;
            }
        }

        public override void Dispose()
        {
            Task.Run(() =>
            {
                ProgramDeclaration = null;
                if (EntryPointInstance != null)
                {
                    EntryPointInstance = null;
                }

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

                if (Classes != null)
                {
                    Classes.Clear();
                }
                Classes = null;
            });
        }

        #endregion
    }
}
