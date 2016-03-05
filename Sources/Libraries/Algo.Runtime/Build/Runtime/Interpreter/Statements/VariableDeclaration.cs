using Algo.Runtime.Build.AlgorithmDOM.DOM;
using Algo.Runtime.Build.Runtime.Interpreter.Interpreter;

namespace Algo.Runtime.Build.Runtime.Interpreter.Statements
{
    sealed internal class VariableDeclaration : InterpretStatement<AlgorithmVariableDeclaration>
    {
        #region Constructors

        public VariableDeclaration(bool memTrace, BlockInterpreter parentInterpreter, AlgorithmVariableDeclaration statement)
            : base(memTrace, parentInterpreter, statement)
        {
        }

        #endregion

        #region Methods

        internal override void Execute()
        {
            object defaultValue = null;
            if (Statement.DefaultValue != null)
            {
                defaultValue = ParentInterpreter.RunExpression(Statement.DefaultValue);
            }

            if (ParentInterpreter.Failed)
            {
                return;
            }

            ParentInterpreter.AddVariable(Statement, defaultValue);
        }

        #endregion

    }
}
