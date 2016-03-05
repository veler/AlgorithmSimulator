namespace Algo.Runtime.Build.AlgorithmDOM.DOM
{
    /// <summary>
    /// Basic class that represents a snippet expression in an algorithm
    /// </summary>
    public class AlgorithmSnippetExpression : AlgorithmExpression
    {
        #region Properties
        
        /// <summary>
        /// Gets or sets the snippet code
        /// </summary>
        public string Code { get; set; }

        #endregion

        #region Constructors   

        /// <summary>
        /// Initialize a new instance of <see cref="AlgorithmSnippetExpression"/>
        /// </summary>
        public AlgorithmSnippetExpression()
        {
        }

        /// <summary>
        /// Initialize a new instance of <see cref="AlgorithmSnippetExpression"/>
        /// </summary>
        /// <param name="code">The snippet code</param>
        public AlgorithmSnippetExpression(string code)
        {
            Code = code;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets a string representation of the reference
        /// </summary>
        /// <returns>String that reprensents the reference</returns>
        public override string ToString()
        {
            return $"'{Code}'";
        }

        #endregion
    }
}
