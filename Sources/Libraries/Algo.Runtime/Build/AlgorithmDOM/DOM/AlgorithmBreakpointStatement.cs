namespace Algo.Runtime.Build.AlgorithmDOM.DOM
{
    /// <summary>
    /// Represents a breakpoint statement in an algorithm 
    /// </summary>
    public class AlgorithmBreakpointStatement : AlgorithmExpressionStatement
    {
        #region Properties

        /// <summary>
        /// Gets a <see cref="AlgorithmDomType"/> used to identify the object without reflection
        /// </summary>
        internal override AlgorithmDomType DomType => AlgorithmDomType.BreakpointStatement;

        #endregion

        #region Consturctors

        /// <summary>
        /// Initialize a new instance of <see cref="AlgorithmBreakpointStatement"/>
        /// </summary>
        public AlgorithmBreakpointStatement()
        {
        }

        #endregion
    }
}
