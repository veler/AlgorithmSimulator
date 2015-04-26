namespace PortableSimulator.Build.Simulator.Interpreter.Statements
{
    using System.Linq;

    using PortableSimulator.Build.AlgorithmDOM.DOM;
    using PortableSimulator.Build.Simulator.Interpreter.Expressions;

    class Assign : InterpretStatement<AlgorithmAssignStatement>
    {
        #region Constructors

        public Assign(AlgorithmAssignStatement statement)
            : base(statement)
        {
        }

        #endregion

        #region Methods

        public override void Execute()
        {
            if (this.Statement.LeftExpression is AlgorithmVariableReferenceExpression)
            {
                if (this.Statement.RightExpression is AlgorithmPrimitiveExpression)
                    this.AssignPrimitiveValueToVariable();
                else if (this.Statement.RightExpression is AlgorithmInvokeCoreMethodExpression)
                    this.AssignCoreMethodeResultToVariable();
            }
        }

        private void AssignPrimitiveValueToVariable()
        {
            var variableRefExp = (AlgorithmVariableReferenceExpression)this.Statement.LeftExpression;
            var primitiveExp = (AlgorithmPrimitiveExpression)this.Statement.RightExpression;

            StaticVariables.CurrentSimulator.Program.Variables.Single(v => v.Name == variableRefExp.Name).Value = primitiveExp.Value;
        }

        private void AssignCoreMethodeResultToVariable()
        {
            var variableRefExp = (AlgorithmVariableReferenceExpression)this.Statement.LeftExpression;
            var invokeCoreMethodExp = (AlgorithmInvokeCoreMethodExpression)this.Statement.RightExpression;

            StaticVariables.CurrentSimulator.Program.Variables.Single(v => v.Name == variableRefExp.Name).Value = new InvokeCoreMethod(invokeCoreMethodExp).Execute();
        }

        #endregion
    }
}
