namespace Algo.Runtime.Build.Runtime.Interpreter
{
    /// <summary>
    /// Represents an assignable expression
    /// </summary>
    interface IAssignable
    {
        /// <summary>
        /// Returns an assignable object
        /// </summary>
        /// <returns></returns>
        object GetAssignableObject();
    }
}
