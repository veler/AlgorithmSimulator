namespace Algo.Runtime.Build.Runtime.Debugger.Exceptions
{
    /// <summary>
    /// Represents an exception thrown when we try to await a non-asynchronous method
    /// </summary>
    public class MethodNotAwaitableException : MethodNotFoundException
    {
        /// <summary>
        /// Initialize a new instance of <see cref="MethodNotAwaitableException"/>
        /// </summary>
        /// <param name="methodName">The name of the method</param>
        public MethodNotAwaitableException(string methodName)
            : base(methodName, $"The method '{methodName}' is not awaitable because this method does not has the property IsAsync on true.")
        {
        }
    }
}
