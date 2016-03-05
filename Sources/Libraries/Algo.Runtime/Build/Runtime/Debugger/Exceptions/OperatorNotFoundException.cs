using System;
using Newtonsoft.Json;

namespace Algo.Runtime.Build.Runtime.Debugger.Exceptions
{
    public class OperatorNotFoundException : MemberAccessException
    {
        [JsonProperty]
        public string Operator { get; private set; }

        public OperatorNotFoundException(string operatorName, string message)
            : base(message)
        {
            Operator = operatorName;
        }
    }
}
