using System;

namespace Algo.Runtime.Build.Runtime.Debugger.Exceptions
{
    public class StackOverflowException : Exception
    {
        public StackOverflowException(string message)
            : base(message)
        {
        }
    }
}
