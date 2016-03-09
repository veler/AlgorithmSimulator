using System;
using Algo.Runtime.Build.AlgorithmDOM.DOM;
using Algo.Runtime.Build.Runtime.Debugger;
using Algo.Runtime.Build.Runtime.Interpreter.Interpreter;

namespace Algo.Runtime.Build.Runtime.Interpreter.Statements
{
    internal sealed class Condition : InterpretStatement
    {
        #region Properties

        internal bool ReturnOccured { get; set; }

        #endregion

        #region Constructors

        public Condition(bool memTrace, BlockInterpreter parentInterpreter, AlgorithmStatement statement)
            : base(memTrace, parentInterpreter, statement)
        {
        }

        #endregion

        #region Methods

        internal override void Execute()
        {
            var conditionResult = RunCondition(ParentInterpreter, Statement._condition);

            if (ParentInterpreter.FailedOrStop || conditionResult == null)
            {
                return;
            }

            RunResultStatements(conditionResult.Value);
        }

        internal static bool? RunCondition(BlockInterpreter parentInterpreter, AlgorithmExpression condition)
        {
            if (condition == null)
            {
                parentInterpreter.ChangeState(parentInterpreter, new SimulatorStateEventArgs(new Error(new NullReferenceException("A conditional expression is missing."), parentInterpreter.GetDebugInfo())));
                return null;
            }

            var conditionResult = parentInterpreter.RunExpression(condition);

            if (parentInterpreter.FailedOrStop)
            {
                return null;
            }

            var boolResult = conditionResult as bool?;
            if (boolResult != null)
            {
                return boolResult.Value;
            }

            var intResult = conditionResult as int?;
            if (intResult != null)
            {
                switch (intResult.Value)
                {
                    case 1:
                        return true;
                    case 0:
                        return false;
                    default:
                        parentInterpreter.ChangeState(parentInterpreter, new SimulatorStateEventArgs(new Error(new InvalidCastException("Unable to cast this number to a boolean."), parentInterpreter.GetDebugInfo())));
                        return null;
                }
            }

            parentInterpreter.ChangeState(parentInterpreter, new SimulatorStateEventArgs(new Error(new InvalidCastException("Unable to perform a condition statement without a boolean value as conditional expression result."), parentInterpreter.GetDebugInfo())));
            return null;
        }

        private void RunResultStatements(bool conditionResult)
        {
            AlgorithmStatementCollection statements;

            if (conditionResult)
            {
                statements = Statement._trueStatements;
            }
            else
            {
                statements = Statement._falseStatements;
            }

            if (statements == null || statements.Count == 0)
            {
                return;
            }

            var block = new BlockInterpreter(statements, MemTrace);
            block.OnGetParentInterpreter += new Func<BlockInterpreter>(() => ParentInterpreter);
            block.StateChanged += ParentInterpreter.ChangeState;
            block.Initialize();
            ReturnOccured = block.Run();
            block.StateChanged -= ParentInterpreter.ChangeState;
            block.Dispose();
        }

        #endregion

    }
}
