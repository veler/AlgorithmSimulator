using System;
using Newtonsoft.Json;

namespace Algo.Runtime.Build.Runtime.Debugger.Exceptions
{
    /// <summary>
    /// Represents an exception thrown when we try to access to a property which has not been found in a class
    /// </summary>
    public class PropertyNotFoundException : MemberAccessException
    {
        /// <summary>
        /// Gets or sets the property name
        /// </summary>
        [JsonProperty]
        public string PropertyName { get; private set; }

        /// <summary>
        /// Initialize a new instance of <see cref="PropertyNotFoundException"/>
        /// </summary>
        /// <param name="propertyName">The property name</param>
        public PropertyNotFoundException(string propertyName)
            : base($"The property '{propertyName}' does not exist or is not accessible.")
        {
            PropertyName = propertyName;
        }
    }
}
