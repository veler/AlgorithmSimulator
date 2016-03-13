using Algo.Runtime.Build.AlgorithmDOM.DOM;
using Algo.Runtime.Build.Runtime.Debugger;
using Algo.Runtime.Build.Runtime.Interpreter.Interpreter;
using Newtonsoft.Json;

namespace Algo.Runtime.Build.Runtime.Interpreter
{
    /// <summary>
    /// Represents a basic interpreter for an algorithm statement
    /// </summary>
    internal abstract class InterpretStatement : MemoryTraceObject
    {
        #region Properties

        /// <summary>
        /// Gets or sets the algorithm statement
        /// </summary>
        [JsonProperty]
        protected AlgorithmStatement Statement { get; private set; }

        /// <summary>
        /// Gets or sets the parent <see cref="BlockInterpreter"/>
        /// </summary>
        [JsonProperty]
        protected BlockInterpreter ParentInterpreter { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initialize a new instance of <see cref="InterpretStatement"/>
        /// </summary>
        /// <param name="debugMode">Defines if the debug mode is enabled</param>
        /// <param name="parentInterpreter">the parent <see cref="BlockInterpreter"/></param>
        /// <param name="statement">the algorithm statement</param>
        internal InterpretStatement(bool debugMode, BlockInterpreter parentInterpreter, AlgorithmStatement statement)
            : base(debugMode)
        {
            ParentInterpreter = parentInterpreter;
            Statement = statement;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Run the interpretation
        /// </summary>
        internal abstract void Execute();

        #endregion
    }
}