using System;

namespace Algo.Runtime.Build.Runtime.Debugger.Exceptions
{
    /// <summary>
    /// Represents an exception thrown when we try to invoke a non-static method from a not-instanciated class.
    /// </summary>
    public class NoInstanceReferenceException : NullReferenceException
    {
        /// <summary>
        /// Initialize a new instance of <see cref="NoInstanceReferenceException"/>
        /// </summary>
        /// <param name="message">The error message</param>
        public NoInstanceReferenceException(string message)
            : base(message)
        {
        }
    }
}
