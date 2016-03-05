namespace Algo.Runtime.Build.AlgorithmDOM.DOM
{
    /// <summary>
    /// Represents a conditional statement in an algorithm, typically represented as an if statement
    /// </summary>
    public class AlgorithmConditionStatement : AlgorithmStatement
    {
        #region Properties

        /// <summary>
        /// Gets or sets the expression to evaluate true or false
        /// </summary>
        public AlgorithmExpression Condition { get; set; }

        /// <summary>
        /// Gets or sets a collection of statements to run when the condition is true
        /// </summary>
        public AlgorithmStatementCollection TrueStatements { get; set; }

        /// <summary>
        /// Gets or sets a collection of statements to run when the condition is false
        /// </summary>
        public AlgorithmStatementCollection FalseStatements { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initialize a new instance of <see cref="AlgorithmConditionStatement"/>
        /// </summary>
        public AlgorithmConditionStatement()
        {  
        }

        /// <summary>
        /// Initialize a new instance of <see cref="AlgorithmConditionStatement"/>
        /// </summary>
        /// <param name="condition">The expression to evaluate true or false</param>
        /// <param name="trueStatement">The collection of statements to run when the condition is true</param>
        /// <param name="falseStatement">The collection of statements to run when the condition is false</param>
        public AlgorithmConditionStatement(AlgorithmExpression condition, AlgorithmStatementCollection trueStatement, AlgorithmStatementCollection falseStatement)
        {
            Condition = condition;
            TrueStatements = trueStatement == null ? new AlgorithmStatementCollection() : trueStatement;
            FalseStatements = falseStatement == null ? new AlgorithmStatementCollection() : falseStatement;
        }

        #endregion
    }
}
