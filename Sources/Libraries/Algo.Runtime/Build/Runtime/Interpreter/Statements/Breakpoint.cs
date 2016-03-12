using Algo.Runtime.Build.AlgorithmDOM.DOM;
using Algo.Runtime.Build.Runtime.Interpreter.Interpreter;

namespace Algo.Runtime.Build.Runtime.Interpreter.Statements
{
    internal sealed class Breakpoint : InterpretStatement
    {
        #region Constructors

        internal Breakpoint(bool memTrace, BlockInterpreter parentInterpreter)
            : base(memTrace, parentInterpreter, null)
        {
        }

        #endregion

        #region Methods

        internal override void Execute()
        {
            if (MemTrace)
            {
                ParentInterpreter.Log(this, "Breakpoint intercepted");
                ParentInterpreter.ChangeState(this, new SimulatorStateEventArgs(SimulatorState.PauseBreakpoint, ParentInterpreter.GetDebugInfo(false)));
            }
        }

        #endregion
    }
}
