using System;
using Newtonsoft.Json;

namespace Algo.Runtime.Build.Runtime.Debugger.Exceptions
{
    /// <summary>
    /// Represents an exception thrown when we try to access to a variable which is not accessible or does not exist
    /// </summary>
    public class VariableNotFoundException : MemberAccessException
    {
        /// <summary>
        /// Gets or sets the variable name
        /// </summary>
        [JsonProperty]
        public string VariableName { get; private set; }

        /// <summary>
        /// Initialize a new instance of <see cref="VariableNotFoundException"/>
        /// </summary>
        /// <param name="variableName">The variable name</param>
        public VariableNotFoundException(string variableName)
            : base($"The variable '{variableName}' does not exist or is not accessible.")
        {
            VariableName = variableName;
        }
    }
}
