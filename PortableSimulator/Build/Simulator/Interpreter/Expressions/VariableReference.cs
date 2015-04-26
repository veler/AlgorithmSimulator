namespace PortableSimulator.Build.Simulator.Interpreter.Expressions
{
    using System.Linq;

    using PortableSimulator.Build.AlgorithmDOM.DOM;

    class VariableReference : InterpretExpression<AlgorithmVariableReferenceExpression>
    {
        #region Constructors

        public VariableReference(AlgorithmVariableReferenceExpression expression)
            : base(expression)
        {
        }

        #endregion

        #region Methods

        public override object Execute()
        {
            return StaticVariables.CurrentSimulator.Program.Variables.Single(v => v.Name == this.Expression.Name).Value;
        }

        #endregion
    }
}
