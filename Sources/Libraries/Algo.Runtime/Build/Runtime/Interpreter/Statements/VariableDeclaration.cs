using Algo.Runtime.Build.AlgorithmDOM;
using Algo.Runtime.Build.AlgorithmDOM.DOM;
using Algo.Runtime.Build.Runtime.Interpreter.Interpreter;

namespace Algo.Runtime.Build.Runtime.Interpreter.Statements
{
    /// <summary>
    /// Provide the interpreter for a variable declaration
    /// </summary>
    internal sealed class VariableDeclaration : InterpretStatement
    {
        #region Constructors

        /// <summary>
        /// Initialize a new instance of <see cref="VariableDeclaration"/>
        /// </summary>
        /// <param name="debugMode">Defines if the debug mode is enabled</param>
        /// <param name="parentInterpreter">The parent block interpreter</param>
        /// <param name="statement">The algorithm statement</param>
        public VariableDeclaration(bool debugMode, BlockInterpreter parentInterpreter, AlgorithmStatement statement)
            : base(debugMode, parentInterpreter, statement)
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Run the interpretation
        /// </summary>
        internal override void Execute()
        {
            object defaultValue = null;
            if (Statement._defaultValue != null)
            {
                defaultValue = ParentInterpreter.RunExpression(Statement._defaultValue);
            }

            if (ParentInterpreter.FailedOrStop)
            {
                return;
            }

            ParentInterpreter.AddVariable((IAlgorithmVariable)Statement, defaultValue);
        }

        #endregion

    }
}
