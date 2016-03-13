using System.Collections.ObjectModel;
using Newtonsoft.Json;

namespace Algo.Runtime.Build.Runtime.Memory
{
    /// <summary>
    /// Represents a variable at runtime
    /// </summary>
    public sealed class Variable : MemoryObject
    {
        #region Properties

        /// <summary>
        /// Gets or sets the name of the variable 
        /// </summary>     
        [JsonProperty]
        public string Name { get; private set; }

        /// <summary>
        /// Gets or sets whether the variable is of type <see cref="object"/> or <see cref="Collection{T}"/> of <see cref="object"/>
        /// </summary>    
        [JsonProperty]
        public bool IsArray { get; private set; }

        #endregion

        #region Constuctors

        /// <summary>
        /// Initialize a new instance of <see cref="Variable"/>
        /// </summary>
        internal Variable()
        {
        }

        /// <summary>
        /// Initialize a new instance of <see cref="Variable"/>
        /// </summary>
        /// <param name="name">The name of the variable</param>
        /// <param name="debugMode">Defines whether the size of the value should be determinated each time the value change</param>
        /// <param name="defaultValue">Sets a value</param>
        /// <param name="isArray">Define whether the variable is of type <see cref="object"/> or <see cref="Collection{T}"/> of <see cref="object"/></param>
        public Variable(string name, bool debugMode, object defaultValue = null, bool isArray = false)
            : base(debugMode, defaultValue)
        {
            Name = name;
            IsArray = isArray;
        }

        #endregion
    }
}
