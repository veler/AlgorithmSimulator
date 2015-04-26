namespace PortableSimulator.Build.AlgorithmDOM.DOM
{
    public class AlgorithmProgram : AlgorithmObject
    {
        #region Properties

        public AlgorithmVariableDeclarationCollection Variables { get; set; }

        public AlgorithmClassDeclarationCollection Classes { get; set; }

        #endregion

        #region Constructors

        public AlgorithmProgram()
        {
            this.Variables = new AlgorithmVariableDeclarationCollection();
            this.Classes = new AlgorithmClassDeclarationCollection();
        }

        #endregion
    }
}
