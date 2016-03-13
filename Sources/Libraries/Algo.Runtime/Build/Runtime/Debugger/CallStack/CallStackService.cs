using System;
using System.Collections.Generic;
using Algo.Runtime.Build.Runtime.Interpreter.Expressions;
using Newtonsoft.Json;

namespace Algo.Runtime.Build.Runtime.Debugger.CallStack
{
    /// <summary>
    /// Provide a set or properties for call stack
    /// </summary>
    public sealed class CallStackService
    {
        #region Properties

        /// <summary>
        /// Gets or sets the number of calls made by a user in a thread.
        /// </summary>
        [JsonIgnore]
        internal Dictionary<Guid, short> StackTraceCallCount { get; set; }

        /// <summary>
        /// Gets or sets the number of calls to <see cref="InvokeMethod"/>. If the number is too high the interpreter will continue in a new thread.
        /// </summary>
        [JsonIgnore]
        internal short CallCount { get; set; }

        /// <summary>
        /// Gets or sets the user call stacks
        /// </summary>
        [JsonProperty]
        public List<CallStack> CallStacks { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initialize a new instance of <see cref="CallStackService"/>
        /// </summary>
        public CallStackService()
        {
            StackTraceCallCount = new Dictionary<Guid, short>();
            CallCount = 0;
            CallStacks = new List<CallStack>();
        }

        #endregion
    }
}
