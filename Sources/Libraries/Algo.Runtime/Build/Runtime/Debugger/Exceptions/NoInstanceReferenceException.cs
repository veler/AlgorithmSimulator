using System;

namespace Algo.Runtime.Build.Runtime.Debugger.Exceptions
{
    public class NoInstanceReferenceException : NullReferenceException
    {
        public NoInstanceReferenceException(string message)
            : base(message)
        {
        }
    }
}
