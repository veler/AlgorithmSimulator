using Algo.Runtime.Build.AlgorithmDOM.DOM;
using Algo.Runtime.Build.Runtime.Interpreter.Interpreter;

namespace Algo.Runtime.Build.Runtime.Interpreter.Expressions
{
    /// <summary>
    /// Provide the interpreter for a invocation
    /// </summary>
    internal sealed class PrimitiveValue : InterpretExpression
    {
        #region Constructors

        /// <summary>
        /// Initialize a new instance of <see cref="PrimitiveValue"/>
        /// </summary>
        /// <param name="debugMode">Defines if the debug mode is enabled</param>
        /// <param name="parentInterpreter">The parent block interpreter</param>
        /// <param name="expression">The algorithm expression</param>
        internal PrimitiveValue(bool debugMode, BlockInterpreter parentInterpreter, AlgorithmExpression expression)
            : base(debugMode, parentInterpreter, expression)
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Run the interpretation
        /// </summary>
        /// <returns>Returns the result of the interpretation</returns>
        internal override object Execute()
        {
            if (DebugMode)
            {
                ParentInterpreter.Log(this, "Primitive value : {0}", Expression._value == null ? "{null}" : $"'{Expression._value}' (type:{Expression._value.GetType().FullName})");
            }
            return Expression._value;
        }

        #endregion
    }
}
