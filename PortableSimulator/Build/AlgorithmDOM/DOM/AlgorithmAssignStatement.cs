namespace PortableSimulator.Build.AlgorithmDOM.DOM
{
    
    class AlgorithmAssignStatement : AlgorithmStatement
    {
        #region Properties

        public AlgorithmExpression LeftExpression { get; set; }

        public AlgorithmExpression RightExpression { get; set; }

        #endregion

        #region Consturctors

        public AlgorithmAssignStatement()
        {
        }

        public AlgorithmAssignStatement(AlgorithmExpression leftExpression, AlgorithmExpression rightExpression)
        {
            this.LeftExpression = leftExpression;
            this.RightExpression = rightExpression;
        }

        #endregion
    }
}
