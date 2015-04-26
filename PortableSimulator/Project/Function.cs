namespace PortableSimulator.Project
{
    using PortableSimulator.Project.Algorithm;

    public class Function
    {
        #region Properties

        public string Name { get; set; }

        public ActionCollection Algorithm { get; set; }

        #endregion

        #region Constructors

        public Function(string name)
        {
            this.Algorithm = new ActionCollection();
            this.Name = name;
        }

        #endregion
    }
}
