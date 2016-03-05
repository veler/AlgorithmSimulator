using System;
using Newtonsoft.Json;

namespace Algo.Runtime.Build.Runtime.Debugger.Exceptions
{
    public class PropertyNotFoundException : MemberAccessException
    {
        [JsonProperty]
        public string PropertyName { get; private set; }

        public PropertyNotFoundException(string propertyName)
            : base($"The property '{propertyName}' does not exist or is not accessible.")
        {
            PropertyName = propertyName;
        }
    }
}
