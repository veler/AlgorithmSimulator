namespace PortableSimulator.Build.AlgorithmDOM.DOM
{
    using System;

    class AlgorithmInvokeCoreMethodExpression : AlgorithmInvokeMethodExpression
    {
        #region Properties

        public Type[] ArgumentsTypes { get; set; }

        #endregion

        public AlgorithmInvokeCoreMethodExpression(AlgorithmClassReferenceExpression classInfo, string methodName, Type[] argumentTypes, params object[] arguments)
            : base(classInfo, methodName, arguments)
        {
            this.ArgumentsTypes = argumentTypes;
        }
    }
}
