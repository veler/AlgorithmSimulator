using Algo.Runtime.Build.Runtime.Debugger.CallStack;
using Newtonsoft.Json;

namespace Algo.Runtime.Build.Runtime.Debugger
{
    /// <summary>
    /// Represents the debug information
    /// </summary>
    public sealed class DebugInfo
    {
        #region Properties

        /// <summary>
        /// Gets or sets the call stack service
        /// </summary>   
        [JsonProperty]
        public CallStackService CallStackService { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initialize a new instance of <see cref="DebugInfo"/>
        /// </summary>
        internal DebugInfo()
        {
            CallStackService = new CallStackService();
        }

        #endregion
    }
}
