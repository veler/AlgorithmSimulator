using System;
using Newtonsoft.Json;

namespace Algo.Runtime.Build.Runtime.Debugger.Exceptions
{
    public class MissingEntryPointMethodException : Exception
    {
        [JsonProperty]
        public string EntryPointPath { get; private set; }

        public MissingEntryPointMethodException(string entryPointPath)
            : base("The program entry point method is missing or has not been found.")
        {
            EntryPointPath = entryPointPath;
        }
    }
}
