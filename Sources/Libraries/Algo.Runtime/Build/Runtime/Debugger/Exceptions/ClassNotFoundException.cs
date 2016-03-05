using System;
using Newtonsoft.Json;

namespace Algo.Runtime.Build.Runtime.Debugger.Exceptions
{
    public class ClassNotFoundException : MemberAccessException
    {
        [JsonProperty]
        public string ClassName { get; private set; }

        public ClassNotFoundException(string className, string message)
            : base(message)
        {
            ClassName = className;
        }
    }
}
