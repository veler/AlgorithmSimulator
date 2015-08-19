namespace PortableSimulator.Actions
{
    using System.Collections.Generic;
    using System.Diagnostics;

    using PortableSimulator.Build.AlgorithmDOM.DOM;
    using PortableSimulator.Build.Simulator.Debugger;
    using PortableSimulator.Project.Algorithm;
    using PortableSimulator.Project.Algorithm.Parameters;

    public class AssignAction : Action
    {
        #region Properties

        public override string Name
        {
            get
            {
                return "Assign a variable";
            }
        }

        #endregion

        #region Constructors

        public AssignAction(IActionTools tools)
            : base(tools)
        {
        }

        #endregion

        #region Methods

        public override void Edit()
        {
            this.Parameters.Clear();
            this.Parameters.Add("Variable", new VariableParameter(this.Tools.GetCurrentProjectVariables()[0])); // the variable "Path"
            this.Parameters.Add("Value", new PrimitiveParameter("file.txt"));

            Debug.WriteLine("The action \"{0}\" has been edited", this.Name);
        }

        public override IEnumerable<AlgorithmStatement> GetCode()
        {
            yield return new AlgorithmAssignStatement(new AlgorithmVariableReferenceExpression(((VariableParameter)this.Parameters["Variable"]).Name),
                                                      new AlgorithmPrimitiveExpression(((PrimitiveParameter)this.Parameters["Value"]).Value));
        }

        public override SimulatorError DescribeSimulationError(SimulatorError error)
        {
            return error;
        }

        #endregion
    }
}
