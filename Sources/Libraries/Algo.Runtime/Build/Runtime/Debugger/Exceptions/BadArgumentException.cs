using System;

namespace Algo.Runtime.Build.Runtime.Debugger.Exceptions
{
    public class BadArgumentException : ArgumentException
    {
        public BadArgumentException(string parameterName, string message)
            : base(message, parameterName)
        {
        }
    }
}
