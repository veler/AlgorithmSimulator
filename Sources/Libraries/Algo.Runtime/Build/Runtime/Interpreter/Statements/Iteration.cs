using System;
using Algo.Runtime.Build.AlgorithmDOM.DOM;
using Algo.Runtime.Build.Runtime.Interpreter.Interpreter;

namespace Algo.Runtime.Build.Runtime.Interpreter.Statements
{
    /// <summary>
    /// Provide the interpreter for an iteration
    /// </summary>
    internal sealed class Iteration : InterpretStatement
    {
        #region Properties

        /// <summary>
        /// Gets or sets if a "return" occured in the execution block of the iteration
        /// </summary>
        internal bool ReturnOccured { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initialize a new instance of <see cref="Condition"/>
        /// </summary>
        /// <param name="debugMode">Defines if the debug mode is enabled</param>
        /// <param name="parentInterpreter">The parent block interpreter</param>
        /// <param name="statement">The algorithm statement</param>
        public Iteration(bool debugMode, BlockInterpreter parentInterpreter, AlgorithmStatement statement)
            : base(debugMode, parentInterpreter, statement)
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Run the interpretation
        /// </summary>
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

            var block = new BlockInterpreter(Statement._statements, DebugMode, ParentInterpreter.ParentProgramInterpreter, ParentInterpreter.ParentMethodInterpreter, ParentInterpreter.ParentBlockInterpreter, ParentInterpreter.ParentClassInterpreter);
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

        /// <summary>
        /// Execute the condition
        /// </summary>
        /// <returns>Return true or false, even in case of error</returns>
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
