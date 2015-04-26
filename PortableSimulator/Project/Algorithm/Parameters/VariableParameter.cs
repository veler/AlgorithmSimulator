namespace PortableSimulator.Project.Algorithm.Parameters
{
    
    class VariableParameter : Parameter
    {
        #region Properties

        public string Name { get; set; }

        public bool IsArray { get; set; }

        #endregion

        #region Constructors

        public VariableParameter(Variable variable)
        {
            this.Name = variable.Name;
            this.IsArray = variable.IsArray;
        }

        #endregion
    }
}
