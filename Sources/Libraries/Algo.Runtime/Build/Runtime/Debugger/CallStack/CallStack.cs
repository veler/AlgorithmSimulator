using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Algo.Runtime.Build.Runtime.Debugger.CallStack
{
    /// <summary>
    /// Represents a call stack for a thread
    /// </summary>
    public sealed class CallStack
    {
        #region Properties

        /// <summary>
        /// Gets or sets the unique ID of the call stack for a thread.
        /// </summary>
        [JsonIgnore]
        internal Guid TaceId { get; private set; }

        /// <summary>
        /// Gets or sets the list of call
        /// </summary>
        [JsonProperty]
        public Stack<Call> Stack { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initialize a new instance of <see cref="CallStack"/>
        /// </summary>
        /// <param name="traceId">the unique ID of the call stack for a thread</param>
        public CallStack(Guid traceId)
        {
            TaceId = traceId;
            Stack = new Stack<Call>();
        }

        #endregion
    }
}
