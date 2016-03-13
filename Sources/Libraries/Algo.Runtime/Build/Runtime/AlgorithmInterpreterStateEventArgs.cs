using Algo.Runtime.Build.Runtime.Debugger;
using System;
using Newtonsoft.Json;

namespace Algo.Runtime.Build.Runtime
{
    /// <summary>
    /// Provide an <see cref="EventHandler"/> for a algorithm interpreter state change
    /// </summary>
    /// <param name="sender">The source from where the state has changed</param>
    /// <param name="e">The arguments that describe the change</param>
    public delegate void AlgorithmInterpreterStateEventHandler(object sender, AlgorithmInterpreterStateEventArgs e);

    /// <summary>
    /// Represents an event data for a algorithm interpreter state update
    /// </summary>
    public class AlgorithmInterpreterStateEventArgs : EventArgs
    {
        #region Properties

        /// <summary>
        /// Gets or sets the <see cref="AlgorithmInterpreterState"/>
        /// </summary>
        [JsonProperty]
        public AlgorithmInterpreterState State { get; private set; }

        /// <summary>
        /// Gets or sets an <see cref="Error"/>
        /// </summary>
        [JsonProperty]
        public Error Error { get; private set; }

        /// <summary>
        /// Gets or sets the debug information
        /// </summary>   
        [JsonProperty]
        public DebugInfo DebugInfo { get; private set; }

        [JsonProperty]
        public string LogMessage { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initialize a new instance of <see cref="AlgorithmInterpreterStateEventArgs"/>
        /// </summary>
        /// <param name="state">The <see cref="AlgorithmInterpreterState"/></param>
        public AlgorithmInterpreterStateEventArgs(AlgorithmInterpreterState state)
        {
            if (state == AlgorithmInterpreterState.StoppedWithError)
            {
                throw new ArgumentOutOfRangeException(nameof(state), "You might want to stop the algorithm interpreter by throwing an error. Please do not use this constructor and use AlgorithmInterpreterStateEventArgs(Error error).");
            }
            State = state;
        }

        /// <summary>
        /// Initialize a new instance of <see cref="AlgorithmInterpreterStateEventArgs"/>
        /// </summary>
        /// <param name="state">The <see cref="AlgorithmInterpreterState"/></param>
        /// <param name="debugInfo">The debug information</param>
        public AlgorithmInterpreterStateEventArgs(AlgorithmInterpreterState state, DebugInfo debugInfo)
            : this(state)
        {
            DebugInfo = debugInfo;
        }

        /// <summary>
        /// Initialize a new instance of <see cref="AlgorithmInterpreterStateEventArgs"/>
        /// </summary>
        /// <param name="error">An <see cref="Error"/></param>
        /// <param name="debugInfo">The debug information</param>
        public AlgorithmInterpreterStateEventArgs(Error error, DebugInfo debugInfo)
        {
            State = AlgorithmInterpreterState.StoppedWithError;
            Error = error;
            DebugInfo = debugInfo;
        }

        /// <summary>
        /// Initialize a new instance of <see cref="AlgorithmInterpreterStateEventArgs"/>
        /// </summary>
        /// <param name="log">A log message</param>
        public AlgorithmInterpreterStateEventArgs(string log)
        {
            State = AlgorithmInterpreterState.Log;
            LogMessage = log;
        }

        #endregion
    }
}
