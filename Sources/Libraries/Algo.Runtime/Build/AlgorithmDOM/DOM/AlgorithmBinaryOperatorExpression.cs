using Algo.Runtime.ComponentModel;

namespace Algo.Runtime.Build.AlgorithmDOM.DOM
{
    /// <summary>
    /// Represents a binary conditional expression
    /// </summary>
    public class AlgorithmBinaryOperatorExpression : AlgorithmExpression
    {
        #region Properties

        /// <summary>
        /// Gets or sets the left expression
        /// </summary>
        public AlgorithmExpression LeftExpression { get; set; } 

        /// <summary>
        /// Gets or sets the binary operator
        /// </summary>
        public AlgorithmBinaryOperatorType Operator { get; set; }

        /// <summary>
        /// Gets or sets the right expression
        /// </summary>
        public AlgorithmExpression RightExpression { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initialize a new instance of <see cref="AlgorithmBinaryOperatorExpression"/>
        /// </summary>
        public AlgorithmBinaryOperatorExpression()
        {

        }

        /// <summary>
        /// Initialize a new instance of <see cref="AlgorithmBinaryOperatorExpression"/>
        /// </summary>
        /// <param name="leftExpression">The left expression</param>
        /// <param name="conditionalOperator">The binary operator</param>
        /// <param name="rightExpression">The right expression</param>
        public AlgorithmBinaryOperatorExpression(AlgorithmExpression leftExpression, AlgorithmBinaryOperatorType conditionalOperator, AlgorithmExpression rightExpression)
        {
            LeftExpression = leftExpression;
            Operator = conditionalOperator;
            RightExpression = rightExpression;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets a string representation of the reference
        /// </summary>
        /// <returns>String that reprensents the reference</returns>
        public override string ToString()
        {
            return $"{LeftExpression} {Operator.GetDescription()} {RightExpression}";
        }

        #endregion
    }
}
