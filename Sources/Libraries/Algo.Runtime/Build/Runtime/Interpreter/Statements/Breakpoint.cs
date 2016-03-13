using Algo.Runtime.Build.Runtime.Interpreter.Interpreter;

namespace Algo.Runtime.Build.Runtime.Interpreter.Statements
{
    /// <summary>
    /// Provide the interpreter for a breakpoint in debug mode
    /// </summary>
    internal sealed class Breakpoint : InterpretStatement
    {
        #region Constructors

        /// <summary>
        /// Initialize a new instance of <see cref="Breakpoint"/>
        /// </summary>
        /// <param name="debugMode">Defines if the debug mode is enabled</param>
        /// <param name="parentInterpreter">The parent block interpreter</param>
        internal Breakpoint(bool debugMode, BlockInterpreter parentInterpreter)
            : base(debugMode, parentInterpreter, null)
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Run the interpretation
        /// </summary>
        internal override void Execute()
        {
            if (DebugMode)
            {
                ParentInterpreter.Log(this, "Breakpoint intercepted");
                ParentInterpreter.ChangeState(this, new AlgorithmInterpreterStateEventArgs(AlgorithmInterpreterState.PauseBreakpoint, ParentInterpreter.GetDebugInfo(false)));
            }
        }

        #endregion
    }
}
