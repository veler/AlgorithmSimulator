using System;
using Newtonsoft.Json;

namespace Algo.Runtime.Build.Runtime.Debugger.Exceptions
{
    public class VariableNotFoundException : MemberAccessException
    {
        [JsonProperty]
        public string VariableName { get; private set; }

        public VariableNotFoundException(string variableName)
            : base($"The variable '{variableName}' does not exist or is not accessible.")
        {
            VariableName = variableName;
        }
    }
}
