namespace Algo.Runtime.Build.AlgorithmDOM.DOM
{
    /// <summary>
    /// Represents a statement that consists of a single expression
    /// </summary>
    public class AlgorithmExpressionStatement : AlgorithmStatement
    {
        #region Properties

        /// <summary>
        /// The single expression of the statement
        /// </summary>
        public AlgorithmExpression Expression { get; set; }

        #endregion

        #region Consturctors

        /// <summary>
        /// Initialize a new instance of <see cref="AlgorithmExpressionStatement"/>
        /// </summary>
        public AlgorithmExpressionStatement()
        {
        }

        /// <summary>
        /// Initialize a new instance of <see cref="AlgorithmExpressionStatement"/>
        /// </summary>
        /// <param name="expression">The single expression of the statement</param>
        public AlgorithmExpressionStatement(AlgorithmExpression expression)
        {
            Expression = expression;
        }

        #endregion
    }
}
