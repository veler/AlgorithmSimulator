using System;
using Newtonsoft.Json;

namespace Algo.Runtime.Build.Runtime.Debugger.Exceptions
{
    /// <summary>
    /// Represents an exception thrown when we try to access to an index from a non indexable value
    /// </summary>
    public class IndexerException : MemberAccessException
    {
        /// <summary>
        /// Gets or sets the property or variable name
        /// </summary>
        [JsonProperty]
        public string VariableName { get; private set; }

        /// <summary>
        /// Initialize a new instance of <see cref="IndexerException"/>
        /// </summary>
        /// <param name="variableName">The property or variable name</param>
        public IndexerException(string variableName)
            : base($"The property or variable '{variableName}' does not exist or is not accessible.")
        {
            VariableName = variableName;
        }
    }
}
