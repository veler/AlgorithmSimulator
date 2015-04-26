namespace PortableSimulator.Build.AlgorithmDOM.DOM
{
    
    class AlgorithmPrimitiveExpression : AlgorithmExpression
    {
        #region Properties

        public object Value { get; set; }

        #endregion

        #region Consturctors

        public AlgorithmPrimitiveExpression(object value)
        {
            this.Value = value;
        }

        #endregion
    }
}
