namespace PortableSimulator.Build.AlgorithmDOM.DOM
{
    public class AlgorithmClassDeclaration : AlgorithmClassMember
    {
        #region Properties

        public AlgorithmClassMemberCollection Members { get; set; }

        #endregion

        #region Constructors

        public AlgorithmClassDeclaration(string name)
            : base(name)
        {
            this.Members = new AlgorithmClassMemberCollection();
        }

        #endregion
    }
}
