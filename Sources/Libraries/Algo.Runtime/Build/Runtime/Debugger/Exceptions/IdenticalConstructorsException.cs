using System;
using Newtonsoft.Json;

namespace Algo.Runtime.Build.Runtime.Debugger.Exceptions
{
    /// <summary>
    /// Represents an exception when two identical constructors has been found
    /// </summary>
    public class IdenticalConstructorsException : TypeLoadException
    {
        /// <summary>
        /// Gets or sets the class name
        /// </summary>
        [JsonProperty]
        public string ClassName { get; private set; }

        /// <summary>
        /// Initialize de new instance of <see cref="IdenticalConstructorsException"/>
        /// </summary>
        /// <param name="className">The class name</param>
        /// <param name="message">The exception message</param>
        public IdenticalConstructorsException(string className, string message)
            : base(message)
        {
            ClassName = className;
        }
    }
}
