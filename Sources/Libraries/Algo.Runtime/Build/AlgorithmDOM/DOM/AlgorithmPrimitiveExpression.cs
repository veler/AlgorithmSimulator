namespace Algo.Runtime.Build.AlgorithmDOM.DOM
{
    /// <summary>
    /// Represents a primitive value (like True, 10, 3.14) in an algorithm
    /// </summary>
    public class AlgorithmPrimitiveExpression : AlgorithmExpression
    {
        #region Properties

        /// <summary>
        /// Gets or sets the primitive value
        /// </summary>
        public object Value { get { return _value; } set { _value = value; } }

        #endregion

        #region Consturctors

        internal override AlgorithmDomType DomType => AlgorithmDomType.PrimitiveExpression;

        /// <summary>
        /// Initialize a new instance of <see cref="AlgorithmPrimitiveExpression"/>
        /// </summary>
        public AlgorithmPrimitiveExpression()
        {
        }

        /// <summary>
        /// Initialize a new instance of <see cref="AlgorithmPrimitiveExpression"/>
        /// </summary>
        /// <param name="value">The primitive value</param>
        public AlgorithmPrimitiveExpression(object value)
        {
            Value = value;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets a string representation of the reference
        /// </summary>
        /// <returns>String that reprensents the reference</returns>
        public override string ToString()
        {
            return Value == null ? "{null}" : $"'{Value}' (type:{Value.GetType().FullName})";
        }

        #endregion
    }
}
