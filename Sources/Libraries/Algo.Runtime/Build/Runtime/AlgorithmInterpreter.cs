using Algo.Runtime.Build.AlgorithmDOM.DOM;
using Algo.Runtime.Build.Runtime.Debugger;
using Algo.Runtime.Build.Runtime.Interpreter.Interpreter;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Algo.Runtime.Build.Runtime.Debugger.Exceptions;

namespace Algo.Runtime.Build.Runtime
{
    /// <summary>
    /// Provide a set of feature to interpreter an <see cref="AlgorithmProgram"/>
    /// </summary>
    public class AlgorithmInterpreter : IDisposable
    {
        #region Fields

        private readonly List<AlgorithmInterpreterStateEventArgs> _stateChangedHistory;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the state historic
        /// </summary>
        public ReadOnlyCollection<AlgorithmInterpreterStateEventArgs> StateChangeHistory => _stateChangedHistory.AsReadOnly();

        /// <summary>
        /// Gets or sets the current interpreter state
        /// </summary>
        public AlgorithmInterpreterState State { get; private set; }

        /// <summary>
        /// Gets or sets the current interpreter error
        /// </summary>
        public Error Error { get; private set; }

        /// <summary>
        /// Gets or sets the current interpreter debug information
        /// </summary>
        public DebugInfo DebugInfo { get; private set; }

        /// <summary>
        /// Gets or sets if the current interpreter is in debug mode
        /// </summary>
        public bool InDebugMode { get; private set; }

        /// <summary>
        /// Gets or sets the current <see cref="AlgorithmProgram"/>
        /// </summary>
        private AlgorithmProgram Program { get; set; }

        /// <summary>
        /// Gets or sets the current <see cref="ProgramInterpreter"/>
        /// </summary>
        private ProgramInterpreter ProgramInterpreter { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// Raised just before a state change
        /// </summary>
        public event AlgorithmInterpreterStateEventHandler PreviewStateChanged;

        /// <summary>
        /// Raised when the interpretrer state has changed
        /// </summary>
        public event AlgorithmInterpreterStateEventHandler StateChanged;

        #endregion

        #region Constructors

        /// <summary>
        /// Initialize a new instance of <see cref="AlgorithmInterpreter"/>
        /// </summary>
        /// <param name="program">the program to interpret</param>
        public AlgorithmInterpreter(AlgorithmProgram program)
        {
            if (program == null)
            {
                throw new ArgumentNullException(nameof(program));
            }

            _stateChangedHistory = new List<AlgorithmInterpreterStateEventArgs>();
            ChangeState(this, new AlgorithmInterpreterStateEventArgs(AlgorithmInterpreterState.Ready));
            Program = program;
        }

        #endregion

        #region Methods

        #region States  

        /// <summary>
        /// Start the interpreter
        /// </summary>
        /// <param name="debugMode">Defines if the debug mode must be enabled or not</param>
        /// <returns>Returns an awaitable task that can wait the end of the program execution</returns>
        public Task StartAsync(bool debugMode)
        {
            if (State != AlgorithmInterpreterState.Ready && State != AlgorithmInterpreterState.Stopped && State != AlgorithmInterpreterState.StoppedWithError)
            {
                throw new StateException(State);
            }

            return RunAsync(debugMode);
        }

        /// <summary>
        /// Resume a paused program
        /// </summary>
        public void Resume()
        {
            if (State != AlgorithmInterpreterState.Pause && State != AlgorithmInterpreterState.PauseBreakpoint)
            {
                throw new StateException(State);
            }

            DebugInfo = null;
            ProgramInterpreter.Resume();
        }

        /// <summary>
        /// Put the program in pause
        /// </summary>
        public void Pause()
        {
            if (State != AlgorithmInterpreterState.Running)
            {
                throw new StateException(State);
            }

            ProgramInterpreter.Pause();
        }

        /// <summary>
        /// Stop the execution of the current interpreter
        /// </summary>
        public void Stop()
        {
            if (State != AlgorithmInterpreterState.Stopped && State != AlgorithmInterpreterState.StoppedWithError)
            {
                DebugInfo = null;
                ProgramInterpreter.Stop();
            }
        }

        /// <summary>
        /// Step into the current statement
        /// </summary>
        public void StepInto()
        {
            if (State != AlgorithmInterpreterState.Pause && State != AlgorithmInterpreterState.PauseBreakpoint)
            {
                throw new StateException(State);
            }
            ProgramInterpreter.StepInto();
        }

        /// <summary>
        /// Step over the current statement without goind inside the method, if it's a method invocation
        /// </summary>
        public void StepOver()
        {
            if (State != AlgorithmInterpreterState.Pause && State != AlgorithmInterpreterState.PauseBreakpoint)
            {
                throw new StateException(State);
            }
            ProgramInterpreter.StepOver();
        }

        /// <summary>
        /// Step out the current method and pause in the parent block
        /// </summary>
        public void StepOut()
        {
            if (State != AlgorithmInterpreterState.Pause && State != AlgorithmInterpreterState.PauseBreakpoint)
            {
                throw new StateException(State);
            }
            ProgramInterpreter.StepOut();
        }

        /// <summary>
        /// Stop the execution of the current interpreter
        /// </summary>
        internal void Stop(bool calledByChangeState, bool withError = false)
        {
            if (!calledByChangeState)
            {
                if (State != AlgorithmInterpreterState.Stopped && State != AlgorithmInterpreterState.StoppedWithError)
                {
                    ChangeState(this, new AlgorithmInterpreterStateEventArgs(AlgorithmInterpreterState.Stopped));
                }
            }
            else
            {
                State = AlgorithmInterpreterState.Stopped;
            }

            if (withError)
            {
                State = AlgorithmInterpreterState.StoppedWithError;
            }
        }

        /// <summary>
        /// Put the program in pause
        /// </summary>
        internal void Pause(bool calledByChangeState)
        {
            Break(calledByChangeState);
        }

        /// <summary>
        /// Put the program in pause after a breakpoint or an error
        /// </summary>
        private void Break(bool calledByChangeState, AlgorithmInterpreterStateEventArgs algorithmInterpreterState = null)
        {
            if (State == AlgorithmInterpreterState.Running)
            {
                if (algorithmInterpreterState != null)
                {
                    Error = algorithmInterpreterState.Error;
                    DebugInfo = algorithmInterpreterState.DebugInfo;

                    switch (algorithmInterpreterState.State)
                    {
                        case AlgorithmInterpreterState.StoppedWithError:
                            Stop(calledByChangeState, true);
                            break;

                        case AlgorithmInterpreterState.PauseBreakpoint:
                            ProgramInterpreter.Breakpoint();
                            State = AlgorithmInterpreterState.PauseBreakpoint;
                            break;

                        case AlgorithmInterpreterState.PauseWithError:
                            if (!calledByChangeState)
                            {
                                ChangeState(this, new AlgorithmInterpreterStateEventArgs(AlgorithmInterpreterState.Pause));
                            }
                            else
                            {
                                State = AlgorithmInterpreterState.Pause;
                            }
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
                else
                {
                    if (!calledByChangeState)
                    {
                        ChangeState(this, new AlgorithmInterpreterStateEventArgs(AlgorithmInterpreterState.Pause));
                    }
                    else
                    {
                        State = AlgorithmInterpreterState.Pause;
                    }
                }
            }
        }

        #endregion

        #region Interpret

        /// <summary>
        /// Start the interpreter
        /// </summary>
        /// <param name="debugMode">Defines if the debug mode must be enabled or not</param>
        /// <returns>Returns an awaitable task that can wait the end of the program execution</returns>
        private Task RunAsync(bool debugMode)
        {
            InDebugMode = debugMode;
            return Task.Run(() =>
            {
                RuntimeHelpers.EnsureSufficientExecutionStack();
                GCSettings.LatencyMode = GCLatencyMode.LowLatency;

                ProgramInterpreter = new ProgramInterpreter(Program, debugMode);

                ProgramInterpreter.StateChanged += ChangeState;
                ProgramInterpreter.Start();

                Stop(false);

                ProgramInterpreter.StateChanged -= ChangeState;
                ProgramInterpreter.Dispose();

                GCSettings.LatencyMode = GCLatencyMode.Interactive;
                GC.Collect();
                GC.WaitForPendingFinalizers();
            });
        }

        #endregion

        /// <summary>
        /// Dispose the resources
        /// </summary>
        public void Dispose()
        {
            Stop();
            if (ProgramInterpreter != null)
            {
                ProgramInterpreter.Dispose();
            }
            if (_stateChangedHistory != null)
            {
                _stateChangedHistory.Clear();
            }
            if (DebugInfo != null)
            {
                DebugInfo.CallStackService.StackTraceCallCount.Clear();
                DebugInfo.CallStackService.CallStacks.Clear();
                DebugInfo.CallStackService.CallCount = 0;
                DebugInfo = null;
            }
            Error = null;
            Program = null;
            ProgramInterpreter = null;
        }

        #endregion

        #region Handled Methods

        /// <summary>
        /// Change the state of the algorithm interpreter
        /// </summary>
        /// <param name="source">The source from where we changed the state (an interpreter usually)</param>
        /// <param name="e">The new state</param>
        private void ChangeState(object source, AlgorithmInterpreterStateEventArgs e)
        {
#if !DEBUG
            Task.Run(() =>
            {
#endif
                if (PreviewStateChanged != null)
                {
                    PreviewStateChanged(source, e);
                }

                switch (e.State)
                {
                    case AlgorithmInterpreterState.StoppedWithError:
                    case AlgorithmInterpreterState.PauseWithError:
                    case AlgorithmInterpreterState.PauseBreakpoint:
                        Break(true, e);
                        break;

                    case AlgorithmInterpreterState.Stopped:
                        Stop(true);
                        break;

                    case AlgorithmInterpreterState.Pause:
                        Pause(true);
                        break;

                    case AlgorithmInterpreterState.Preparing:
                        State = AlgorithmInterpreterState.Preparing;
                        break;

                    case AlgorithmInterpreterState.Ready:
                        State = AlgorithmInterpreterState.Ready;
                        break;

                    case AlgorithmInterpreterState.Running:
                        State = AlgorithmInterpreterState.Running;
                        break;

                    case AlgorithmInterpreterState.Log:
                        if (InDebugMode)
                        {
                            //Debug.WriteLine(e.LogMessage);
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                AddStateChangeHistory(e);

                if (StateChanged != null)
                {
                    StateChanged(source, e);
                }
#if !DEBUG
            });
#endif
        }

        /// <summary>
        /// Add a new state to the historic
        /// </summary>
        /// <param name="algorithmInterpreterStateEventArgs">the new interpreter state</param>
        private void AddStateChangeHistory(AlgorithmInterpreterStateEventArgs algorithmInterpreterStateEventArgs)
        {
            if (_stateChangedHistory.Count > 0)
            {
                var lastAlgorithmInterpreterStateEventArgs = _stateChangedHistory[_stateChangedHistory.Count - 1];
                if (lastAlgorithmInterpreterStateEventArgs != null && lastAlgorithmInterpreterStateEventArgs.State == AlgorithmInterpreterState.Stopped)
                {
                    return;
                }
            }

            _stateChangedHistory.Add(algorithmInterpreterStateEventArgs);
        }

        #endregion
    }
}
