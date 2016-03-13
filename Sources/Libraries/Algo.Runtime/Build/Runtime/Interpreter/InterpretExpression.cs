using Algo.Runtime.Build.AlgorithmDOM.DOM;
using Algo.Runtime.Build.Runtime.Debugger;
using Algo.Runtime.Build.Runtime.Interpreter.Interpreter;
using Newtonsoft.Json;

namespace Algo.Runtime.Build.Runtime.Interpreter
{
    /// <summary>
    /// Represents a basic interpreter for an algorithm expression
    /// </summary>
    internal abstract class InterpretExpression : MemoryTraceObject
    {
        #region Properties

        /// <summary>
        /// Gets or sets the algorithm expression
        /// </summary>
        [JsonProperty]
        protected AlgorithmExpression Expression { get; private set; }

        /// <summary>
        /// Gets or sets the parent <see cref="BlockInterpreter"/>
        /// </summary>
        [JsonProperty]
        protected BlockInterpreter ParentInterpreter { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initialize a new instance of <see cref="InterpretExpression"/>
        /// </summary>
        /// <param name="debugMode">Defines if the debug mode is enabled</param>
        /// <param name="parentInterpreter">the parent <see cref="BlockInterpreter"/></param>
        /// <param name="expression">the algorithm expression</param>
        internal InterpretExpression(bool debugMode, BlockInterpreter parentInterpreter, AlgorithmExpression expression)
            : base(debugMode)
        {
            ParentInterpreter = parentInterpreter;
            Expression = expression;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Run the interpretation
        /// </summary>
        /// <returns>Returns the result of the interpretation</returns>
        internal abstract object Execute();

        #endregion
    }
}
