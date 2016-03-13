using System;

namespace Algo.Runtime.Build.Runtime.Debugger.Exceptions
{
    /// <summary>
    /// Represents an exception thrown when the user invoke too many methods
    /// </summary>
    public class StackOverflowException : Exception
    {
        /// <summary>
        /// Initialize a new instance of <see cref="StackOverflowException"/>
        /// </summary>
        /// <param name="message">The error message</param>
        public StackOverflowException(string message)
            : base(message)
        {
        }
    }
}
