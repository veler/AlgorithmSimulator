namespace Algo.Runtime.Build.AlgorithmDOM.DOM
{
    /// <summary>
    /// Represents a return statement in an algorithm 
    /// </summary>
    public class AlgorithmReturnStatement : AlgorithmExpressionStatement
    {
        #region Properties

        /// <summary>
        /// Gets a <see cref="AlgorithmDomType"/> used to identify the object without reflection
        /// </summary>
        internal override AlgorithmDomType DomType => AlgorithmDomType.ReturnStatement;

        #endregion

        #region Consturctors

        /// <summary>
        /// Initialize a new instance of <see cref="AlgorithmReturnStatement"/>
        /// </summary>
        public AlgorithmReturnStatement()
        {
        }

        /// <summary>
        /// Initialize a new instance of <see cref="AlgorithmReturnStatement"/>
        /// </summary>
        /// <param name="expression">The expression to return</param>
        public AlgorithmReturnStatement(AlgorithmExpression expression)
            :base(expression)
        {                                
        }

        #endregion
    }
}
