namespace Algo.Runtime.Build.AlgorithmDOM.DOM
{
    /// <summary>
    /// Represents a reference to the current instance of a class in an algorithm
    /// </summary>
    public class AlgorithmThisReferenceExpression : AlgorithmReferenceExpression
    {
        #region Methods
        
        /// <summary>
        /// Gets a string representation of the reference
        /// </summary>
        /// <returns>String that reprensents the reference</returns>
        public override string ToString()
        {
            return "This";
        }

        #endregion
    }
}
