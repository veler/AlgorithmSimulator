using System;

namespace Algo.Runtime.Build.Runtime.Debugger.Exceptions
{
    public class NotAssignableException : InvalidOperationException
    {
        public NotAssignableException(string message)
            : base(message)
        {
        }
    }
}
