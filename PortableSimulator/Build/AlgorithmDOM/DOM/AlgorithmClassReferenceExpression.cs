namespace PortableSimulator.Build.AlgorithmDOM.DOM
{
    class AlgorithmClassReferenceExpression : AlgorithmExpression
    {
        #region Properties

        public string Namespace { get; set; }

        public string ClassName { get; set; }

        #endregion

        #region Constructors

        public AlgorithmClassReferenceExpression(string namespacePath, string className)
        {
            this.Namespace = namespacePath;
            this.ClassName = className;
        }

        #endregion

        #region Methods

        public override string ToString()
        {
            if (string.IsNullOrWhiteSpace(this.Namespace))
                return this.ClassName;
            return string.Format("{0}.{1}", this.Namespace, this.ClassName);
        }

        #endregion
    }
}
