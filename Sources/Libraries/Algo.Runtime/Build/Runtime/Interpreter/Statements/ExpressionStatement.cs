﻿using Algo.Runtime.Build.AlgorithmDOM.DOM;
using Algo.Runtime.Build.Runtime.Interpreter.Interpreter;

namespace Algo.Runtime.Build.Runtime.Interpreter.Statements
{
    internal sealed class ExpressionStatement : InterpretStatement
    {
        #region Constructors

        public ExpressionStatement(bool memTrace, BlockInterpreter parentInterpreter, AlgorithmStatement statement)
            : base(memTrace, parentInterpreter, statement)
        {
        }

        #endregion

        #region Methods

        internal override void Execute()
        {
            ParentInterpreter.RunExpression(Statement._expression);
        }

        #endregion

    }
}
