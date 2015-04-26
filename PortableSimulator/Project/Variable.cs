namespace PortableSimulator.Project
{
    public class Variable
    {
        #region Properties

        public string Name { get; private set; }

        public string Description { get; private set; }

        public bool IsArray { get; private set; }

        #endregion

        #region Constructors

        public Variable(string name, string description, bool isArray)
        {
            this.Name = name;
            this.Description = description;
            this.IsArray = isArray;
        }

        #endregion
    }
}
