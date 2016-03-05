using System;

namespace Algo.Runtime.Build.Runtime.Debugger.Exceptions
{
    public class MethodNotAwaitableException : MethodNotFoundException
    {
        public MethodNotAwaitableException(string methodName)
            : base(methodName, $"The method '{methodName}' is not awaitable because this method does not has the property IsAsync on true.")
        {
        }
    }
}
