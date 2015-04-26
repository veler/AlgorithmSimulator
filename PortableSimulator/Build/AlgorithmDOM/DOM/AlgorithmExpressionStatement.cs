namespace PortableSimulator.Build.AlgorithmDOM.DOM
{
    
    class AlgorithmExpressionStatement : AlgorithmStatement
    {
        #region Properties

        public AlgorithmExpression Expression { get; set; }

        #endregion

        #region Consturctors

        public AlgorithmExpressionStatement(AlgorithmExpression expression)
        {
            this.Expression = expression;
        }

        #endregion
    }
}
