using System;
using Algo.Runtime.Build.AlgorithmDOM.DOM;
using Algo.Runtime.Build.Runtime.Interpreter.Interpreter;

namespace Algo.Runtime.Build.Runtime.Interpreter.Statements
{
    sealed internal class Iteration : InterpretStatement<AlgorithmIterationStatement>
    {
        #region Properties

        internal bool ReturnOccured { get; set; }

        #endregion

        #region Constructors

        public Iteration(bool memTrace, BlockInterpreter parentInterpreter, AlgorithmIterationStatement statement)
            : base(memTrace, parentInterpreter, statement)
        {
        }

        #endregion

        #region Methods

        internal override void Execute()
        {
            var conditionResult = false;

            if (Statement.InitializationStatement != null)
            {
                ParentInterpreter.RunStatement(Statement.InitializationStatement);
                if (ParentInterpreter.Failed)
                {
                    return;
                }
            }

            _IterationLoop:

            if (!Statement.ConditionAfterBody)
            {
                conditionResult = RunCondition();
                if (!conditionResult)
                {
                    return;
                }
            }

            var block = new BlockInterpreter(Statement.Statements, MemTrace);
            block.OnGetParentInterpreter += new Func<BlockInterpreter>(() => ParentInterpreter);
            block.StateChanged += ParentInterpreter.ChangeState;
            block.Initialize();
            block.UpdateCallStack();
            ReturnOccured = block.Run();
            block.StateChanged -= ParentInterpreter.ChangeState;
            block.Dispose();

            if (ReturnOccured || ParentInterpreter.Failed)
            {
                return;
            }
            
            ParentInterpreter.RunStatement(Statement.IncrementStatement);
            if (ParentInterpreter.Failed)
            {
                return;
            }

            if (Statement.ConditionAfterBody)
            {
                conditionResult = RunCondition();
            }

            if (conditionResult)
            {
                goto _IterationLoop;
            }
        }

        private bool RunCondition()
        {
            var conditionResult = Condition.RunCondition(ParentInterpreter, Statement.Condition);

            if (ParentInterpreter.Failed || conditionResult == null)
            {
                return false;
            }

            return conditionResult.Value;
        }

        #endregion

    }
}
