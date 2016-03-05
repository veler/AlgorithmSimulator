﻿using Algo.Runtime.Build.AlgorithmDOM.DOM;
using Algo.Runtime.Build.Runtime.Debugger;
using Algo.Runtime.Build.Runtime.Debugger.Exceptions;
using Algo.Runtime.Build.Runtime.Interpreter.Interpreter;

namespace Algo.Runtime.Build.Runtime.Interpreter.Statements
{
    sealed internal class Return : InterpretStatement<AlgorithmReturnStatement>
    {
        #region Constructors

        public Return(bool memTrace, BlockInterpreter parentInterpreter, AlgorithmReturnStatement statement)
            : base(memTrace, parentInterpreter, statement)
        {
        }

        #endregion

        #region Methods

        internal override void Execute()
        {
            var method = ParentInterpreter.GetFirstNextParentInterpreter<MethodInterpreter>();

            if (method == null)
            {
                ParentInterpreter.ChangeState(this, new SimulatorStateEventArgs(new Error(new MethodNotFoundException("{Unknow}", "It looks like the caller/parent's method does not exists."), ParentInterpreter.GetDebugInfo())));
                return;
            }

            method.ReturnedValue = ParentInterpreter.RunExpression(Statement.Expression);

            if (!ParentInterpreter.Failed)
            {
                ParentInterpreter.Log(this, "({0}) Return : {1}", method.MethodDeclaration.Name, method.ReturnedValue == null ? "{null}" : $"'{method.ReturnedValue}' (type:{method.ReturnedValue.GetType().FullName})");
            }
        }

        #endregion

    }
}
