namespace PortableSimulator.Build.AlgorithmDOM.DOM
{
    public class AlgorithmVariableDeclaration : AlgorithmClassMember
    {
        #region Properties

        public bool IsArray { get; set; }

        public object Value { get; set; }

        #endregion

        #region Constuctors

        public AlgorithmVariableDeclaration(string name, bool isArray = false)
            : base(name)
        {
            this.IsArray = isArray;
        }

        #endregion
    }
}
