namespace Algo.Runtime.Build.AlgorithmDOM.DOM
{
    /// <summary>
    /// Represents a return statement in an algorithm 
    /// </summary>
    public class AlgorithmReturnStatement : AlgorithmExpressionStatement
    {
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
