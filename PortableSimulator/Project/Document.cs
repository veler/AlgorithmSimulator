namespace PortableSimulator.Project
{
    using System.Collections.ObjectModel;

    public class Document
    {
        #region Properties

        public string Name { get; set; }

        public Collection<Function> Functions { get; set; }

        #endregion

        #region Constructors

        public Document(string name)
        {
            this.Functions = new Collection<Function>();
            this.Name = name;
        }

        #endregion
    }
}
