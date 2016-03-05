using System;
using Newtonsoft.Json;

namespace Algo.Runtime.Build.Runtime.Debugger.Exceptions
{
    public class IdenticalConstructorsException : TypeLoadException
    {
        [JsonProperty]
        public string ClassName { get; private set; }

        public IdenticalConstructorsException(string className, string message)
            : base(message)
        {
            ClassName = className;
        }
    }
}
