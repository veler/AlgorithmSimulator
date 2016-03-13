using System;
using Algo.Runtime.Build.AlgorithmDOM.DOM;
using Algo.Runtime.Build.Runtime.Debugger;
using Algo.Runtime.Build.Runtime.Interpreter.Interpreter;

namespace Algo.Runtime.Build.Runtime.Interpreter.Statements
{
    /// <summary>
    /// Provide the interpreter for a condition
    /// </summary>
    internal sealed class Condition : InterpretStatement
    {
        #region Properties

        /// <summary>
        /// Gets or sets if a "return" occured in the execution block of the condition
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
        public Condition(bool debugMode, BlockInterpreter parentInterpreter, AlgorithmStatement statement)
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
            var conditionResult = RunCondition(ParentInterpreter, Statement._condition);

            if (ParentInterpreter.FailedOrStop || conditionResult == null)
            {
                return;
            }

            RunResultStatements(conditionResult.Value);
        }

        /// <summary>
        /// Execute the condition
        /// </summary>
        /// <param name="parentInterpreter">The parent block interpreter</param>
        /// <param name="condition">The condition expression</param>
        /// <returns>Return true, false, or null in case of error</returns>
        internal static bool? RunCondition(BlockInterpreter parentInterpreter, AlgorithmExpression condition)
        {
            if (condition == null)
            {
                parentInterpreter.ChangeState(parentInterpreter, new AlgorithmInterpreterStateEventArgs(new Error(new NullReferenceException("A conditional expression is missing.")), parentInterpreter.GetDebugInfo()));
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
                        parentInterpreter.ChangeState(parentInterpreter, new AlgorithmInterpreterStateEventArgs(new Error(new InvalidCastException("Unable to cast this number to a boolean."), condition), parentInterpreter.GetDebugInfo()));
                        return null;
                }
            }

            parentInterpreter.ChangeState(parentInterpreter, new AlgorithmInterpreterStateEventArgs(new Error(new InvalidCastException("Unable to perform a condition statement without a boolean value as conditional expression result."), condition), parentInterpreter.GetDebugInfo()));
            return null;
        }

        /// <summary>
        /// Execute the statements depending on the result of the condition
        /// </summary>
        /// <param name="conditionResult">The condition result</param>
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

            var block = new BlockInterpreter(statements, DebugMode, ParentInterpreter.ParentProgramInterpreter, ParentInterpreter.ParentMethodInterpreter, ParentInterpreter.ParentBlockInterpreter, ParentInterpreter.ParentClassInterpreter);
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
