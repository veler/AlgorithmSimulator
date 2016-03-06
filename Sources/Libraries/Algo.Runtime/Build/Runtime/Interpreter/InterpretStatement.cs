using Algo.Runtime.Build.AlgorithmDOM.DOM;
using Algo.Runtime.Build.Runtime.Debugger;
using Algo.Runtime.Build.Runtime.Interpreter.Interpreter;
using Newtonsoft.Json;

namespace Algo.Runtime.Build.Runtime.Interpreter
{
    internal abstract class InterpretStatement : MemoryTraceObject
    {
        #region Properties

        [JsonProperty]
        protected AlgorithmStatement Statement { get; private set; }

        [JsonProperty]
        protected BlockInterpreter ParentInterpreter { get; private set; }

        #endregion

        #region Constructors

        internal InterpretStatement(bool memTrace, BlockInterpreter parentInterpreter, AlgorithmStatement statement)
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