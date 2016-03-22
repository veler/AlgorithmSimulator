using Algo.Runtime.ComponentModel;
using Newtonsoft.Json;

namespace Algo.Runtime.Build.AlgorithmDOM.DOM
{
    /// <summary>
    /// Represents a binary conditional expression
    /// </summary>
    public class AlgorithmBinaryOperatorExpression : AlgorithmExpression
    {
        #region Properties

        /// <summary>
        /// Gets a <see cref="AlgorithmDomType"/> used to identify the object without reflection
        /// </summary>
        internal override AlgorithmDomType DomType => AlgorithmDomType.BinaryOperatorExpression;

        /// <summary>
        /// Gets or sets the left expression
        /// </summary>
        [JsonProperty]
        public AlgorithmExpression LeftExpression { get { return _leftExpression; } set { _leftExpression = value; } }

        /// <summary>
        /// Gets or sets the binary operator
        /// </summary>
        [JsonProperty]
        public AlgorithmBinaryOperatorType Operator { get { return _operator; } set { _operator = value; } }

        /// <summary>
        /// Gets or sets the right expression
        /// </summary>
        [JsonProperty]
        public AlgorithmExpression RightExpression { get { return _rightExpression; } set { _rightExpression = value; } }

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
