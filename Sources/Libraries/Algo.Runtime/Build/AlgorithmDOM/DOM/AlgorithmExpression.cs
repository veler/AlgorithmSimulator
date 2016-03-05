namespace Algo.Runtime.Build.AlgorithmDOM.DOM
{
    /// <summary>
    /// Basic class that represents an expression in an algorithm
    /// </summary>
    public abstract class AlgorithmExpression : AlgorithmObject
    {
        #region Methods

        /// <summary>
        /// Gets a string representation of the reference
        /// </summary>
        /// <returns>String that reprensents the reference</returns>
        public abstract override string ToString();

        #endregion
    }
}
