using System;
using Newtonsoft.Json;

namespace Algo.Runtime.Build.Runtime.Debugger.Exceptions
{
    /// <summary>
    /// Represents an exception thrown when the current state of the algorithm interpreter is not correct
    /// </summary>
    public class StateException : Exception
    {
        /// <summary>
        /// Gets or sets the state
        /// </summary>
        [JsonProperty]
        public AlgorithmInterpreterState Sate { get; private set; }

        /// <summary>
        /// Initialize a new instance of <see cref="StateException"/>
        /// </summary>
        /// <param name="state">The state</param>
        public StateException(AlgorithmInterpreterState state)
            : base("The current state of the algorithm interpreter is not correct for this operation.")
        {
            Sate = state;
        }
    }
}
