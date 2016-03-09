using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Algo.Runtime.Build.Runtime.Debugger.CallStack
{
    public sealed class CallStackService
    {
        #region Properties

        [JsonIgnore]
        internal Dictionary<Guid, short> StackTraceCallCount { get; set; }

        [JsonIgnore]
        internal short CallCount { get; set; }

        [JsonProperty]
        public List<CallStack> CallStacks { get; set; }

        #endregion

        #region Constructors

        public CallStackService()
        {
            StackTraceCallCount = new Dictionary<Guid, short>();
            CallCount = 0;
            CallStacks = new List<CallStack>();
        }

        #endregion

        #region Methods



        #endregion
    }
}
