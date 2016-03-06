namespace Algo.Runtime.Build.AlgorithmDOM.DOM
{
    /// <summary>
    /// Basic class that represents a snippet statement in an algorithm
    /// </summary>
    public class AlgorithmSnippetStatement : AlgorithmStatement
    {
        #region Properties

        internal override AlgorithmDomType DomType => AlgorithmDomType.SnippetStatement;

        /// <summary>
        /// Gets or sets the snippet code
        /// </summary>
        public string Code { get { return _code; } set { _code = value; } }

        #endregion

        #region Constructors   

        /// <summary>
        /// Initialize a new instance of <see cref="AlgorithmSnippetStatement"/>
        /// </summary>
        public AlgorithmSnippetStatement()
        {
        }

        /// <summary>
        /// Initialize a new instance of <see cref="AlgorithmSnippetStatement"/>
        /// </summary>
        /// <param name="code">The snippet code</param>
        public AlgorithmSnippetStatement(string code)
        {
            Code = code;
        }

        #endregion
    }
}
