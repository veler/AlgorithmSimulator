using Algo.Runtime.Build.Runtime.Interpreter.Expressions;

namespace Algo.Runtime.Build.Runtime.Utils
{
    /// <summary>
    /// Provides a set of constant
    /// </summary>
    internal class Consts
    {
        /// <summary>
        /// The maximum user call stack
        /// </summary>
        internal const int CallStackSize = 10000; // 10000

        /// <summary>
        /// The maximum number of call to <see cref="InvokeMethod"/> before creating a new thread to avoid stack overflow exception
        /// </summary>
        internal const int InvokeMethodCountBeforeNewThread = 275; // 275
    }
}
