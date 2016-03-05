using Newtonsoft.Json;

namespace Algo.Runtime.Build.Runtime.Debugger
{
    sealed public class DebugInfo
    {
        #region Properties

        /// <summary>
        /// Gets or sets the call stack of the algorithm
        /// </summary>   
        [JsonProperty]
        public CallStack.CallStack CallStack { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initialize a new instance of <see cref="DebugInfo"/>
        /// </summary>
        /// <param name="callstack">The call stack of the algorithm</param>
        internal DebugInfo(CallStack.CallStack callstack)
        {
            CallStack = callstack;
        }

        #endregion
    }
}
