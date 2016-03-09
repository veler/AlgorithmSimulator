using Algo.Runtime.Build.AlgorithmDOM.DOM;
using Algo.Runtime.Build.Runtime.Interpreter.Interpreter;

namespace Algo.Runtime.Build.Runtime.Interpreter.Expressions
{
    internal sealed class PrimitiveValue : InterpretExpression
    {
        #region Constructors

        internal PrimitiveValue(bool memTrace, BlockInterpreter parentInterpreter, AlgorithmExpression expression)
            : base(memTrace, parentInterpreter, expression)
        {
        }

        #endregion

        #region Methods

        internal override object Execute()
        {
            if (MemTrace)
            {
                ParentInterpreter.Log(this, "Primitive value : {0}", Expression._value == null ? "{null}" : $"'{Expression._value}' (type:{Expression._value.GetType().FullName})");
            }
            return Expression._value;
        }

        #endregion
    }
}
