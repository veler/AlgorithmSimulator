using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Algo.Runtime.Build.Runtime.Debugger.CallStack
{
    public sealed class CallStack
    {
        #region Properties

        [JsonIgnore]
        internal Guid TaceId { get; private set; }

        [JsonProperty]
        public Stack<Call> Stack { get; private set; }

        #endregion

        #region Constructors

        public CallStack(Guid traceId)
        {
            TaceId = traceId;
            Stack = new Stack<Call>();
        }

        #endregion
    }
}
