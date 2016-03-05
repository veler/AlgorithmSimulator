using Algo.Runtime.Build.Runtime.Debugger;
using System;

namespace Algo.Runtime.Build.Runtime
{
    /// <summary>
    /// Provide an <see cref="EventHandler"/> for a simulator state change
    /// </summary>
    /// <param name="source">The source from where the state has changed</param>
    /// <param name="e">The arguments that describe the change</param>
    public delegate void SimulatorStateEventHandler(object source, SimulatorStateEventArgs e);

    /// <summary>
    /// Represents an event data for a simulator state update
    /// </summary>
    public class SimulatorStateEventArgs : EventArgs
    {
        #region Properties

        /// <summary>
        /// Gets or sets the <see cref="SimulatorState"/>
        /// </summary>
        public SimulatorState State { get; private set; }

        /// <summary>
        /// Gets or sets an <see cref="Error"/>
        /// </summary>
        public Error Error { get; private set; }

        public string LogMessage { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initialize a new instance of <see cref="SimulatorStateEventArgs"/>
        /// </summary>
        /// <param name="state">The <see cref="SimulatorState"/></param>
        public SimulatorStateEventArgs(SimulatorState state)
        {
            if (state == SimulatorState.StoppedWithError)
            {
                throw new ArgumentOutOfRangeException(nameof(state), "You might want to stop the simulator by throwing an error. Please do not use this constructor and use SimulatorStateEventArgs(Error error).");
            }
            State = state;
        }

        /// <summary>
        /// Initialize a new instance of <see cref="SimulatorStateEventArgs"/>
        /// </summary>
        /// <param name="error">An <see cref="Error"/></param>
        public SimulatorStateEventArgs(Error error)
        {
            State = SimulatorState.StoppedWithError;
            Error = error;
        }

        /// <summary>
        /// Initialize a new instance of <see cref="SimulatorStateEventArgs"/>
        /// </summary>
        /// <param name="log">A log message</param>
        public SimulatorStateEventArgs(string log)
        {
            State = SimulatorState.Log;
            LogMessage = log;
        }

        #endregion
    }
}
