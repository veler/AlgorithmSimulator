using System;
using Newtonsoft.Json;

namespace Algo.Runtime.Build.Runtime.Debugger.Exceptions
{
    /// <summary>
    /// Represents an exception thrown when a declared variable already exists in the program, class, method or block
    /// </summary>
    public class VariableAlreadyExistsException : Exception
    {
        /// <summary>
        /// Gets or sets the variable name
        /// </summary>
        [JsonProperty]
        public string VariableName { get; private set; }

        /// <summary>
        /// Initialize a new instance of <see cref="VariableAlreadyExistsException"/>
        /// </summary>
        /// <param name="variableName">The variable name</param>
        public VariableAlreadyExistsException(string variableName)
            : base($"The variable '{variableName}' already exists in the program, class, method or block of the algorithm and cannot be declared again.")
        {
            VariableName = variableName;
        }
    }
}
