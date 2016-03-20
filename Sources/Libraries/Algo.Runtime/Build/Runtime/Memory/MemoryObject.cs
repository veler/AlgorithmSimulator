using System;
using System.Collections;
using System.Reflection;
using System.Text;
using Algo.Runtime.Build.Runtime.Debugger;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Algo.Runtime.Build.Runtime.Memory
{
    /// <summary>
    /// Provide a representation of an object in the memory and the size of this object.
    /// </summary>
    public abstract class MemoryObject : MemoryTraceObject
    {
        #region Properties
        
        /// <summary>
        /// Gets or sets the value in the memory
        /// </summary> 
        public object Value { get; set; }

        #endregion

        #region Constructors

        /// <summary>   
        /// Initialize a new instance of <see cref="MemoryObject"/>
        /// </summary>
        internal MemoryObject()
            : this(false)
        {
        }

        /// <summary>
        /// Initialize a new instance of <see cref="MemoryObject"/>
        /// </summary>
        /// <param name="debugMode">Defines whether the size of the value should be determinated each time the value change</param>
        /// <param name="value">Sets a value</param>
        internal MemoryObject(bool debugMode, object value = null)
            : base(debugMode)
        {
            Value = value;
        }

        #endregion
    }
}
