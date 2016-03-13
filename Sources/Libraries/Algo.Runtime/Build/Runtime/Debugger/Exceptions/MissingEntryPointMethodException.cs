using System;
using Newtonsoft.Json;

namespace Algo.Runtime.Build.Runtime.Debugger.Exceptions
{
    /// <summary>
    /// Represents an excpetion thrown when no entry point has been found in a program
    /// </summary>
    public class MissingEntryPointMethodException : Exception
    {
        /// <summary>
        /// Gets or sets the path to the program entry point
        /// </summary>
        [JsonProperty]
        public string EntryPointPath { get; private set; }

        /// <summary>
        /// Initialize a new instance of <see cref="MissingEntryPointMethodException"/>
        /// </summary>
        /// <param name="entryPointPath">The path to the entry point</param>
        public MissingEntryPointMethodException(string entryPointPath)
            : base("The program entry point method is missing or has not been found.")
        {
            EntryPointPath = entryPointPath;
        }
    }
}
