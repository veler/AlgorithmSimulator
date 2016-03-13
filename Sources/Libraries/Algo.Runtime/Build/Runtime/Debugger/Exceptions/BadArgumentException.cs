using System;

namespace Algo.Runtime.Build.Runtime.Debugger.Exceptions
{
    /// <summary>
    /// Represents an exception when a argument is defined or set with a bad value.
    /// </summary>
    public class BadArgumentException : ArgumentException
    {
        /// <summary>
        /// Initialize de new instance of <see cref="BadArgumentException"/>
        /// </summary>
        /// <param name="parameterName">The argument name</param>
        /// <param name="message">The exception message</param>
        public BadArgumentException(string parameterName, string message)
            : base(message, parameterName)
        {
        }
    }
}
