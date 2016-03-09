using Algo.Runtime.Build.AlgorithmDOM.DOM;
using Algo.Runtime.Build.Runtime.Debugger;
using Algo.Runtime.Build.Runtime.Interpreter.Interpreter;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Algo.Runtime.Build.Runtime
{
    public class Simulator
    {
        #region Fields

        private readonly List<SimulatorStateEventArgs> _stateChangedHistory;

        #endregion

        #region Properties

        public ReadOnlyCollection<SimulatorStateEventArgs> StateChangeHistory => _stateChangedHistory.AsReadOnly();

        public SimulatorState State { get; private set; }

        public Error Error { get; private set; }

        private AlgorithmProgram Program { get; set; }

        private ProgramInterpreter ProgramInterpreter { get; set; }

        #endregion

        #region Events

        public event SimulatorStateEventHandler PreviewStateChanged;

        public event SimulatorStateEventHandler StateChanged;

        #endregion

        #region Constructors

        public Simulator(AlgorithmProgram program)
        {
            if (program == null)
            {
                throw new ArgumentNullException(nameof(program));
            }

            _stateChangedHistory = new List<SimulatorStateEventArgs>();
            ChangeState(this, new SimulatorStateEventArgs(SimulatorState.Ready));
            Program = program;
        }

        #endregion

        #region Methods

        #region States  

        public Task StartAsync(bool debugMode)
        {
            if (State == SimulatorState.Ready || State == SimulatorState.Pause || State == SimulatorState.Stopped || State == SimulatorState.StoppedWithError)
            {
                return RunAsync(debugMode);
            }
            return null;
        }

        public void Resume()
        {

        }

        public void Pause()
        {
            Pause(false);
        }

        internal void Pause(bool calledByChangeState)
        {
            Break(calledByChangeState);
        }

        private void Break(bool calledByChangeState, SimulatorStateEventArgs simulatorState = null)
        {
            if (State == SimulatorState.Running)
            {
                if (simulatorState != null)
                {
                    Error = simulatorState.Error;

                    switch (simulatorState.State)
                    {
                        case SimulatorState.StoppedWithError:
                            Stop(calledByChangeState, true);
                            break;

                        case SimulatorState.PauseWithError:
                            if (!calledByChangeState)
                            {
                                ChangeState(this, new SimulatorStateEventArgs(SimulatorState.Pause));
                            }
                            else
                            {
                                State = SimulatorState.Pause;
                            }
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
                // TODO: else if (breakpoint != null)
                else
                {
                    if (!calledByChangeState)
                    {
                        ChangeState(this, new SimulatorStateEventArgs(SimulatorState.Pause));
                    }
                    else
                    {
                        State = SimulatorState.Pause;
                    }
                }
            }
        }

        public void Stop()
        {
            ProgramInterpreter.Stop();
        }

        internal void Stop(bool calledByChangeState, bool withError = false)
        {
            if (State == SimulatorState.Pause || State == SimulatorState.Running)
            {
                if (!calledByChangeState)
                {
                    ChangeState(this, new SimulatorStateEventArgs(SimulatorState.Stopped));
                }
                else
                {
                    State = SimulatorState.Stopped;
                }

                if (withError)
                {
                    State = SimulatorState.StoppedWithError;
                }
            }
        }

        #endregion

        #region Interpret

        private Task RunAsync(bool debugMode)
        {
            return Task.Run(() =>
            {
                ProgramInterpreter = new ProgramInterpreter(Program, debugMode);

                ProgramInterpreter.StateChanged += ChangeState;
                ProgramInterpreter.Start();

                Stop(false);

                ProgramInterpreter.StateChanged -= ChangeState;
                ProgramInterpreter.Dispose();
            });
        }

        #endregion

        #endregion

        #region Handled Methods

        private void ChangeState(object source, SimulatorStateEventArgs e)
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
                case SimulatorState.StoppedWithError:
                case SimulatorState.PauseWithError:
                    Break(true, e);
                    break;

                case SimulatorState.PauseBreakpoint:
                    // TODO: Break(breakpoint);
                    State = SimulatorState.PauseBreakpoint;
                    break;

                case SimulatorState.Stopped:
                    Stop(true);
                    break;

                case SimulatorState.Pause:
                    Pause(true);
                    break;

                case SimulatorState.Preparing:
                    State = SimulatorState.Preparing;
                    break;

                case SimulatorState.Ready:
                    State = SimulatorState.Ready;
                    break;

                case SimulatorState.Running:
                    State = SimulatorState.Running;
                    break;

                case SimulatorState.Log:
                    Debug.WriteLine(e.LogMessage);
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

        private void AddStateChangeHistory(SimulatorStateEventArgs simulatorStateEventArgs)
        {
#if DEBUG
            if (_stateChangedHistory.Count > 0)
            {
                var lastSimulatorStateEventArgs = _stateChangedHistory[_stateChangedHistory.Count - 1];
                if (lastSimulatorStateEventArgs != null && lastSimulatorStateEventArgs.State == SimulatorState.Stopped)
                {
                    return;
                }
            }

            _stateChangedHistory.Add(simulatorStateEventArgs);
#endif
        }

        #endregion
    }
}
