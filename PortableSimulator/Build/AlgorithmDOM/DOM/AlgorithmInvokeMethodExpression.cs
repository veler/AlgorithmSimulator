namespace PortableSimulator.Build.AlgorithmDOM.DOM
{
    
    class AlgorithmInvokeMethodExpression : AlgorithmExpression
    {
        #region Properties

        public AlgorithmClassReferenceExpression Class { get; set; }

        public string MethodName { get; set; }

        public object[] Arguments { get; set; }

        #endregion

        #region Consturctors

        public AlgorithmInvokeMethodExpression(AlgorithmClassReferenceExpression classInfo, string methodName, params object[] arguments)
        {
            this.Class = classInfo;
            this.MethodName = methodName;
            this.Arguments = arguments;
        }

        #endregion
    }
}
