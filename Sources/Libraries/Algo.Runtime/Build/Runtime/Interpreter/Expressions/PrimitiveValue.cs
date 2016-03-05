using Algo.Runtime.Build.AlgorithmDOM.DOM;
using Algo.Runtime.Build.Runtime.Interpreter.Interpreter;

namespace Algo.Runtime.Build.Runtime.Interpreter.Expressions
{
    sealed internal class PrimitiveValue : InterpretExpression<AlgorithmPrimitiveExpression>
    {
        #region Constructors

        internal PrimitiveValue(bool memTrace, BlockInterpreter parentInterpreter, AlgorithmPrimitiveExpression expression)
            : base(memTrace, parentInterpreter, expression)
        {
        }

        #endregion

        #region Methods

        internal override object Execute()
        {
            ParentInterpreter.Log(this, "Primitive value : {0}", Expression.Value == null ? "{null}" : $"'{Expression.Value}' (type:{Expression.Value.GetType().FullName})");

            return Expression.Value;
        }

        #endregion
    }
}
