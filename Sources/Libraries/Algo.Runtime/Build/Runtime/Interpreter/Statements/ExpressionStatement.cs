using Algo.Runtime.Build.AlgorithmDOM.DOM;
using Algo.Runtime.Build.Runtime.Interpreter.Interpreter;

namespace Algo.Runtime.Build.Runtime.Interpreter.Statements
{
    sealed internal class ExpressionStatement : InterpretStatement<AlgorithmExpressionStatement>
    {
        #region Constructors

        public ExpressionStatement(bool memTrace, BlockInterpreter parentInterpreter, AlgorithmExpressionStatement statement)
            : base(memTrace, parentInterpreter, statement)
        {
        }

        #endregion

        #region Methods

        internal override void Execute()
        {
            ParentInterpreter.RunExpression(Statement.Expression);
        }

        #endregion

    }
}
