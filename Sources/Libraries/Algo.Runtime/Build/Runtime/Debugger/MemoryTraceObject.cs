using Newtonsoft.Json;

namespace Algo.Runtime.Build.Runtime.Debugger
{
    /// <summary>
    /// Represents an object with a parameter that will define whether a memory trace will be kept or not
    /// </summary>
    public abstract class MemoryTraceObject
    {
        #region Properties

        /// <summary>
        /// Gets or sets a value that defines whether a memory trace will be kept or not
        /// </summary>    
        [JsonProperty]
        protected bool DebugMode { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initialize a new instance of <see cref="MemoryTraceObject"/>
        /// </summary>
        /// <param name="debugMode">Defines whether a memory trace will be kept or not</param>
        internal MemoryTraceObject(bool debugMode)
        {
            DebugMode = debugMode;
        }

        #endregion
    }
}
