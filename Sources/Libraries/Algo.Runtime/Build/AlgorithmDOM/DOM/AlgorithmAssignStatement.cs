namespace Algo.Runtime.Build.AlgorithmDOM.DOM
{
    /// <summary>
    /// Represents an assignment in an algorithm
    /// </summary>
    public class AlgorithmAssignStatement : AlgorithmStatement
    {
        #region Properties

        /// <summary>
        /// The expression on the left of the assign symbol in the algorithm
        /// </summary>
        public AlgorithmExpression LeftExpression { get; set; }

        /// <summary>
        /// The expression on the right of the assign symbol in the algorithm
        /// </summary>
        public AlgorithmExpression RightExpression { get; set; }

        #endregion

        #region Consturctors

        /// <summary>
        /// Initialize a new instance of <see cref="AlgorithmAssignStatement"/>
        /// </summary>
        public AlgorithmAssignStatement()
        {
        }

        /// <summary>  
        /// Initialize a new instance of <see cref="AlgorithmAssignStatement"/>
        /// </summary>
        /// <param name="leftExpression">The expression on the left of the assign symbol in the algorithm</param>
        /// <param name="rightExpression">The expression on the right of the assign symbol in the algorithm</param>
        public AlgorithmAssignStatement(AlgorithmExpression leftExpression, AlgorithmExpression rightExpression)
        {
            LeftExpression = leftExpression;
            RightExpression = rightExpression;
        }

        #endregion
    }
}
