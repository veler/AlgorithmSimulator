namespace PortableSimulator.Build.AlgorithmDOM.DOM
{
    
    class AlgorithmClassMethod : AlgorithmClassMember
    {
        #region Properties

        public AlgorithmStatementCollection Statements { get; set; }

        #endregion

        #region Constructors

        public AlgorithmClassMethod(string name)
            : base(name)
        {
            this.Statements = new AlgorithmStatementCollection();
        }

        #endregion
    }
}
