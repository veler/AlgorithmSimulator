using System;
using Newtonsoft.Json;

namespace Algo.Runtime.Build.Runtime.Debugger.Exceptions
{
    public class MethodNotFoundException : MemberAccessException
    {
        [JsonProperty]
        public string MethodName { get; private set; }

        public MethodNotFoundException(string methodName, string message)
            : base(message)
        {
            MethodName = methodName;
        }
    }
}
