using System;
using Newtonsoft.Json;

namespace Algo.Runtime.Build.Runtime.Debugger.Exceptions
{
    /// <summary>
    /// Represents an exception thrown when a method is not found
    /// </summary>
    public class MethodNotFoundException : MemberAccessException
    {
        /// <summary>
        /// Gets or sets the name of the method
        /// </summary>
        [JsonProperty]
        public string MethodName { get; private set; }

        /// <summary>
        /// Initialize a new instance of <see cref="MethodNotFoundException"/>
        /// </summary>
        /// <param name="methodName">The method name</param>
        /// <param name="message">The error message</param>
        public MethodNotFoundException(string methodName, string message)
            : base(message)
        {
            MethodName = methodName;
        }
    }
}
