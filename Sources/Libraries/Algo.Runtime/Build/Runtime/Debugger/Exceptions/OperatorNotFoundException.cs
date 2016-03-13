using System;
using Newtonsoft.Json;

namespace Algo.Runtime.Build.Runtime.Debugger.Exceptions
{
    /// <summary>
    /// Represents an exception thrown when the specified operator of a binary operation has not been found.
    /// </summary>
    public class OperatorNotFoundException : MemberAccessException
    {
        /// <summary>
        /// Gets or sets the binary operator
        /// </summary>
        [JsonProperty]
        public string Operator { get; private set; }

        /// <summary>
        /// Initialize a new instance of <see cref="OperatorNotFoundException"/>
        /// </summary>
        /// <param name="operatorName">The binary operator</param>
        /// <param name="message">The error message</param>
        public OperatorNotFoundException(string operatorName, string message)
            : base(message)
        {
            Operator = operatorName;
        }
    }
}
