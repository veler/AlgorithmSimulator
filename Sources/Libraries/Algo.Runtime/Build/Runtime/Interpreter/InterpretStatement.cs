using Algo.Runtime.Build.AlgorithmDOM.DOM;
using Algo.Runtime.Build.Runtime.Debugger;
using Algo.Runtime.Build.Runtime.Interpreter.Interpreter;
using Newtonsoft.Json;

namespace Algo.Runtime.Build.Runtime.Interpreter
{
    internal abstract class InterpretStatement<T> : MemoryTraceObject where T : AlgorithmStatement
    {
        #region Properties

        [JsonProperty]
        protected T Statement { get; private set; }

        [JsonProperty]
        protected BlockInterpreter ParentInterpreter { get; private set; }

        #endregion

        #region Constructors

        internal InterpretStatement(bool memTrace, BlockInterpreter parentInterpreter, T statement)
            : base(memTrace)
        {
            ParentInterpreter = parentInterpreter;
            Statement = statement;
        }

        #endregion

        #region Methods

        internal abstract void Execute();

        #endregion
    }
}