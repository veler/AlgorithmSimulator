using Algo.Runtime.Build.AlgorithmDOM.DOM;
using Algo.Runtime.Build.Runtime.Memory;
using System.Collections.ObjectModel;
using Algo.Runtime.Build.Runtime.Debugger;
using Algo.Runtime.Build.Runtime.Debugger.Exceptions;
using System;
using System.Threading;
using System.Threading.Tasks;
using Algo.Runtime.ComponentModel.OperatorHelper;
using Newtonsoft.Json;

namespace Algo.Runtime.Build.Runtime.Interpreter.Interpreter
{
    /// <summary>
    /// Provide a sets of method to interpret a program in an algorithm
    /// </summary>
    internal sealed class ProgramInterpreter : Interpret
    {
        #region Properties

        /// <summary>
        /// Gets a <see cref="InterpreterType"/> used to identify the object without reflection
        /// </summary>
        [JsonIgnore]
        internal override InterpreterType InterpreterType => InterpreterType.ProgramInterpreter;

        /// <summary>
        /// Gets or sets the program declaration
        /// </summary>
        [JsonProperty]
        internal AlgorithmProgram ProgramDeclaration { get; set; }

        /// <summary>
        /// Gets or sets the list of classes in the program
        /// </summary>
        [JsonProperty]
        internal Collection<ClassInterpreter> Classes { get; set; }

        /// <summary>
        /// Gets or sets the current program execution state
        /// </summary>
        [JsonProperty]
        internal AlgorithmInterpreterState State { get; private set; }

        /// <summary>
        /// Gets or sets the current debug information
        /// </summary>
        [JsonProperty]
        internal DebugInfo DebugInfo { get; private set; }

        /// <summary>
        /// Gets or sets the instance of the class which have the entry point
        /// </summary>
        [JsonProperty]
        private ClassInterpreter EntryPointInstance { get; set; }

        /// <summary>
        /// Gets or sets a waiter used by the debugger to put the program in pause
        /// </summary>
        internal AutoResetEvent Waiter { get; private set; }

        /// <summary>
        /// Gets or sets a secondary waiter used by the debugger to put the program in pause when we do a Step Into, Step Over or Step Out
        /// </summary>
        internal AutoResetEvent StepIntoOverOutWaiter { get; private set; }

        /// <summary>
        /// Gets or sets the we actually do a Step Over
        /// </summary>
        internal bool StepOverSignal { get; set; }

        /// <summary>
        /// Gets or sets the we should not consider the current Step Over (usually because of a breakpoint in a method)
        /// </summary>
        internal bool CancelStepOverSignal { get; set; }

        /// <summary>
        /// Gets or sets the we actually do a Step Out
        /// </summary>
        internal bool StepOutSignal { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initialize a new instance of <see cref="ProgramInterpreter"/>
        /// </summary>
        /// <param name="programDeclaration">the program declaration</param>
        /// <param name="debugMode">Defines if the debug mode is enabled</param>
        internal ProgramInterpreter(AlgorithmProgram programDeclaration, bool debugMode)
            : base(debugMode)
        {
            ProgramDeclaration = programDeclaration;

            // Just for better performances after, we call it a first time
            OperatorHelperCache.Initialize();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Initialize, after the constructor, the other properties
        /// </summary>
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
                    Classes.Add(new ClassInterpreter(cl, DebugMode));
                }
            }
        }

        /// <summary>
        /// Start the program by finding the entry point and calling it
        /// </summary>
        internal void Start()
        {
            if (State != AlgorithmInterpreterState.Ready && State != AlgorithmInterpreterState.Stopped && State != AlgorithmInterpreterState.StoppedWithError)
            {
                throw new InvalidOperationException("Unable to start a algorithm interpreter which is not stopped.");
            }

            ChangeState(this, new AlgorithmInterpreterStateEventArgs(AlgorithmInterpreterState.Preparing));

            Initialize();

            var entryPointMethod = ProgramDeclaration.GetEntryPointMethod();
            var i = 0;
            ClassInterpreter entryPointClass = null;

            if (entryPointMethod == null)
            {
                ChangeState(this, new AlgorithmInterpreterStateEventArgs(new Error(new MissingEntryPointMethodException(ProgramDeclaration.EntryPointPath)), GetDebugInfo()));
                return;
            }

            ChangeState(this, new AlgorithmInterpreterStateEventArgs(AlgorithmInterpreterState.Running));

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
                ChangeState(this, new AlgorithmInterpreterStateEventArgs(new Error(new MissingEntryPointMethodException(ProgramDeclaration.EntryPointPath)), GetDebugInfo()));
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

        /// <summary>
        /// Stop the program
        /// </summary>
        internal void Stop()
        {
            ChangeState(this, new AlgorithmInterpreterStateEventArgs(AlgorithmInterpreterState.Stopped));
            Task.Delay(TimeSpan.FromSeconds(1)).Wait();
            FreeWaiter();
        }

        /// <summary>
        /// Ask the program to pause
        /// </summary>
        internal void Pause()
        {
            Waiter = new AutoResetEvent(false);
            Waiter.Reset();
            Task.Delay(TimeSpan.FromMilliseconds(200)).Wait();
            ChangeState(this, new AlgorithmInterpreterStateEventArgs(AlgorithmInterpreterState.Pause));
#if DEBUG
            Task.Delay(TimeSpan.FromSeconds(1)).Wait();
#endif
        }

        /// <summary>
        /// Put the program in pause after a breakpoint
        /// </summary>
        internal void Breakpoint()
        {
            Waiter = new AutoResetEvent(false);
            Waiter.Reset();
#if DEBUG
            Task.Delay(TimeSpan.FromSeconds(1)).Wait();
#endif
        }

        /// <summary>
        /// Resume the paused program
        /// </summary>
        internal void Resume()
        {
            CancelStepOverSignal = true;
            ChangeState(this, new AlgorithmInterpreterStateEventArgs(AlgorithmInterpreterState.Running));
            Task.Delay(TimeSpan.FromSeconds(1)).Wait();
            FreeWaiter();
            FreeStepIntoOverOutWaiter();
        }

        /// <summary>
        /// Step into the current statement
        /// </summary>
        internal void StepInto()
        {
            Resume();
            ResetStepIntoOverOutWaiter();
#if DEBUG
            Task.Delay(TimeSpan.FromSeconds(1)).Wait();
#endif
        }

        /// <summary>
        /// Step over the current statement without goind inside the method, if it's a method invocation
        /// </summary>
        internal void StepOver()
        {
            StepOverSignal = true;
            Resume();
            CancelStepOverSignal = false;
            ResetStepIntoOverOutWaiter();
#if DEBUG
            Task.Delay(TimeSpan.FromSeconds(1)).Wait();
#endif
        }

        /// <summary>
        /// Step out the current method and pause in the parent block
        /// </summary>
        internal void StepOut()
        {
            StepOutSignal = true;
            Resume();
            ResetStepIntoOverOutWaiter();
#if DEBUG
            Task.Delay(TimeSpan.FromSeconds(1)).Wait();
#endif
        }

        /// <summary>
        /// Change the state of the current program
        /// </summary>
        /// <param name="source">The source from where we changed the state (an interpreter usually)</param>
        /// <param name="e">The new state</param>
        internal override void ChangeState(object source, AlgorithmInterpreterStateEventArgs e)
        {
            base.ChangeState(source, e);

            if (e.State != AlgorithmInterpreterState.Log)
            {
                State = e.State;
            }
        }

        /// <summary>
        /// Set the waiter
        /// </summary>
        private void FreeWaiter()
        {
            if (Waiter != null)
            {
                Waiter.Set();
                Waiter.Dispose();
                Waiter = null;
            }
        }

        /// <summary>
        /// Set the secondary waiter
        /// </summary>
        internal void FreeStepIntoOverOutWaiter()
        {
            if (StepIntoOverOutWaiter != null)
            {
                StepIntoOverOutWaiter.Set();
                StepIntoOverOutWaiter.Dispose();
                StepIntoOverOutWaiter = null;
            }
        }

        /// <summary>
        /// Reset the secondary waiter
        /// </summary>
        internal void ResetStepIntoOverOutWaiter()
        {
            StepIntoOverOutWaiter = new AutoResetEvent(false);
            StepIntoOverOutWaiter.Reset();
        }

        /// <summary>
        /// Dispose the resources
        /// </summary>
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

                FreeWaiter();
                FreeStepIntoOverOutWaiter();
            });
        }

        #endregion
    }
}
