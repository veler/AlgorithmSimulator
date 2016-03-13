using System;

namespace Algo.Runtime.Build.Runtime.Debugger.Exceptions
{
    /// <summary>
    /// Represents an exception thrown when we try to assign a non-assignable object, like a method.
    /// </summary>
    public class NotAssignableException : InvalidOperationException
    {
        /// <summary>
        /// Initialize a new instance of <see cref="NotAssignableException"/>
        /// </summary>
        /// <param name="message">The error message</param>
        public NotAssignableException(string message)
            : base(message)
        {
        }
    }
}
