namespace PortableSimulator.Build.AlgorithmDOM.DOM
{
    public abstract class AlgorithmClassMember : AlgorithmObject
    {
        #region Properties

        public string Name { get; set; }

        #endregion

        #region Constructors

        public AlgorithmClassMember(string name)
        {
            this.Name = name;
        }

        #endregion
    }
}
