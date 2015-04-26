namespace PortableSimulator.Build.AlgorithmDOM.DOM
{
    
    class AlgorithmVariableReferenceExpression : AlgorithmExpression
    {
        #region Properties

        public string Name { get; set; }

        #endregion

        #region Constuctors

        public AlgorithmVariableReferenceExpression(string name)
        {
            this.Name = name;
        }

        public AlgorithmVariableReferenceExpression(AlgorithmVariableDeclaration variable)
        {
            this.Name = variable.Name;
        }

        #endregion
    }
}
