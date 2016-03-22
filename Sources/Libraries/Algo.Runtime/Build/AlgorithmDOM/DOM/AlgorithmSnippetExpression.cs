using Newtonsoft.Json;

namespace Algo.Runtime.Build.AlgorithmDOM.DOM
{
    /// <summary>
    /// Basic class that represents a snippet expression in an algorithm
    /// </summary>
    public class AlgorithmSnippetExpression : AlgorithmExpression
    {
        #region Properties

        /// <summary>
        /// Gets a <see cref="AlgorithmDomType"/> used to identify the object without reflection
        /// </summary>
        internal override AlgorithmDomType DomType => AlgorithmDomType.SnippetExpression;

        /// <summary>
        /// Gets or sets the snippet code
        /// </summary>
        [JsonProperty]
        public string Code { get { return _code; } set { _code = value; } }

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
        /// Gets a string representation of the expression
        /// </summary>
        /// <returns>String that reprensents the expression</returns>
        public override string ToString()
        {
            return $"'{Code}'";
        }

        #endregion
    }
}
