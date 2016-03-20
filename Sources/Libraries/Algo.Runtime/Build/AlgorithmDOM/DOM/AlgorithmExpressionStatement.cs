using Newtonsoft.Json;

namespace Algo.Runtime.Build.AlgorithmDOM.DOM
{
    /// <summary>
    /// Represents a statement that consists of a single expression
    /// </summary>
    public class AlgorithmExpressionStatement : AlgorithmStatement
    {
        #region Properties

        /// <summary>
        /// Gets a <see cref="AlgorithmDomType"/> used to identify the object without reflection
        /// </summary>
        internal override AlgorithmDomType DomType => AlgorithmDomType.ExpressionStatement;

        /// <summary>
        /// The single expression of the statement
        /// </summary>
        [JsonProperty]
        public AlgorithmExpression Expression { get { return _expression; } set { _expression = value; } }

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
