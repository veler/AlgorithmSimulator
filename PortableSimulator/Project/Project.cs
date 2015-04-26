namespace PortableSimulator.Project
{
    using System.Collections.ObjectModel;

    public class Project
    {
        #region Properties

        public Collection<Document> Documents { get; set; }

        public Collection<Variable> Variables { get; set; }

        #endregion

        #region Constructors

        public Project()
        {
            this.Documents = new Collection<Document>();
            this.Variables = new Collection<Variable>();
        }

        #endregion
    }
}
