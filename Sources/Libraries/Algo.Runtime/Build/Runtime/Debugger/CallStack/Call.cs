using System.Collections.ObjectModel;
using Algo.Runtime.Build.Runtime.Memory;
using Newtonsoft.Json;

namespace Algo.Runtime.Build.Runtime.Debugger.CallStack
{
    public class Call
    {
        #region Properties

        /// <summary>
        /// Gets or sets the list of accessible variables
        /// </summary>   
        [JsonProperty]
        public ReadOnlyCollection<Variable> Variables { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initialize a new instance of <see cref="Call"/>
        /// </summary>
        /// <param name="variables">The list of accessible variables</param>
        internal Call(ReadOnlyCollection<Variable> variables)
        {
            Variables = variables;
        }

        #endregion
    }
}
