using System;
using Algo.Runtime.Build.AlgorithmDOM.DOM;
using Algo.Runtime.Build.Runtime.Interpreter.Interpreter;

namespace Algo.Runtime.Build.Runtime.Interpreter.Statements
{
    internal sealed class Iteration : InterpretStatement
    {
        #region Properties

        internal bool ReturnOccured { get; set; }

        #endregion

        #region Constructors

        public Iteration(bool memTrace, BlockInterpreter parentInterpreter, AlgorithmStatement statement)
            : base(memTrace, parentInterpreter, statement)
        {
        }

        #endregion

        #region Methods

        internal override void Execute()
        {
            var conditionResult = false;

            if (Statement._initializationStatement != null)
            {
                ParentInterpreter.RunStatement(Statement._initializationStatement);
                if (ParentInterpreter.FailedOrStop)
                {
                    return;
                }
            }

            _IterationLoop:

            if (!Statement._conditionAfterBody)
            {
                conditionResult = RunCondition();
                if (!conditionResult)
                {
                    return;
                }
            }

            var block = new BlockInterpreter(Statement._statements, MemTrace);
            block.OnGetParentInterpreter += new Func<BlockInterpreter>(() => ParentInterpreter);
            block.StateChanged += ParentInterpreter.ChangeState;
            block.Initialize();
            ReturnOccured = block.Run();
            block.StateChanged -= ParentInterpreter.ChangeState;
            block.Dispose();

            if (ReturnOccured || ParentInterpreter.FailedOrStop)
            {
                return;
            }

            if (Statement._incrementStatement != null)
            {
                ParentInterpreter.RunStatement(Statement._incrementStatement);
            }
            if (ParentInterpreter.FailedOrStop)
            {
                return;
            }

            if (Statement._conditionAfterBody)
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
            var conditionResult = Condition.RunCondition(ParentInterpreter, Statement._condition);

            if (ParentInterpreter.FailedOrStop || conditionResult == null)
            {
                return false;
            }

            return conditionResult.Value;
        }

        #endregion

    }
}
