using System;
using Newtonsoft.Json;

namespace Algo.Runtime.Build.Runtime.Debugger.Exceptions
{
    public class VariableAlreadyExistsException : Exception
    {
        [JsonProperty]
        public string VariableName { get; private set; }

        public VariableAlreadyExistsException(string variableName)
            : base($"The variable '{variableName}' already exists in the program, class, method or block of the algorithm and cannot be declared again.")
        {
            VariableName = variableName;
        }
    }
}
