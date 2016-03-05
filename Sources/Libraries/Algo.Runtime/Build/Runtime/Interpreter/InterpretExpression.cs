using Algo.Runtime.Build.AlgorithmDOM.DOM;
using Algo.Runtime.Build.Runtime.Debugger;
using Algo.Runtime.Build.Runtime.Interpreter.Interpreter;
using Newtonsoft.Json;

namespace Algo.Runtime.Build.Runtime.Interpreter
{
    internal abstract class InterpretExpression<T> : MemoryTraceObject where T : AlgorithmExpression
    {
        #region Properties

        [JsonProperty]
        protected T Expression { get; private set; }

        [JsonProperty]
        protected BlockInterpreter ParentInterpreter { get; private set; }

        #endregion

        #region Constructors

        internal InterpretExpression(bool memTrace, BlockInterpreter parentInterpreter, T expression)
            : base(memTrace)
        {
            ParentInterpreter = parentInterpreter;
            Expression = expression;
        }

        #endregion

        #region Methods

        internal abstract object Execute();

        #endregion
    }
}
