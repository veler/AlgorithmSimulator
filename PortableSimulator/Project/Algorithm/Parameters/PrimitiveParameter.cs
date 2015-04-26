namespace PortableSimulator.Project.Algorithm.Parameters
{
    
    class PrimitiveParameter : Parameter
    {
        #region Properties

        public object Value { get; set; }

        #endregion

        #region Constructors

        public PrimitiveParameter(object value)
        {
            this.Value = value;
        }

        #endregion
    }
}
