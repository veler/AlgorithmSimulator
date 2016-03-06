using Algo.Runtime.Build.AlgorithmDOM;
using Algo.Runtime.Build.AlgorithmDOM.DOM;
using Algo.Runtime.Build.Runtime.Interpreter.Interpreter;

namespace Algo.Runtime.Build.Runtime.Interpreter.Statements
{
    internal sealed class VariableDeclaration : InterpretStatement
    {
        #region Constructors

        public VariableDeclaration(bool memTrace, BlockInterpreter parentInterpreter, AlgorithmStatement statement)
            : base(memTrace, parentInterpreter, statement)
        {
        }

        #endregion

        #region Methods

        internal override void Execute()
        {
            object defaultValue = null;
            if (Statement._defaultValue != null)
            {
                defaultValue = ParentInterpreter.RunExpression(Statement._defaultValue);
            }

            if (ParentInterpreter.Failed)
            {
                return;
            }

            ParentInterpreter.AddVariable((IAlgorithmVariable)Statement, defaultValue);
        }

        #endregion

    }
}
