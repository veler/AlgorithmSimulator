namespace PortableSimulator.Actions
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;

    using PortableSimulator.Build.AlgorithmDOM.DOM;
    using PortableSimulator.Build.Simulator.Debugger;
    using PortableSimulator.Project.Algorithm;
    using PortableSimulator.Project.Algorithm.Parameters;

    public class ReadFileAction : Action
    {
        #region Properties

        public override string Name
        {
            get
            {
                return "Read a file";
            }
        }

        #endregion

        #region Constructors

        public ReadFileAction(IActionTools tools)
            : base(tools)
        {
        }

        #endregion

        #region Methods

        public override void Edit()
        {
            this.Parameters.Clear();
            this.Parameters.Add("ResultVariable", new VariableParameter(this.Tools.GetCurrentProjectVariables()[1])); // the variable "FileContent"
            this.Parameters.Add("PathToTheFile", new VariableParameter(this.Tools.GetCurrentProjectVariables()[0])); // the variable "Path"

            Debug.WriteLine("The action \"{0}\" has been edited", this.Name);
        }

        public override IEnumerable<AlgorithmStatement> GetCode()
        {
            var pathToTheFileReference = new AlgorithmVariableReferenceExpression(((VariableParameter) this.Parameters["PathToTheFile"]).Name);
            yield return new AlgorithmAssignStatement(new AlgorithmVariableReferenceExpression(((VariableParameter)this.Parameters["ResultVariable"]).Name),
                                                      new AlgorithmInvokeCoreMethodExpression(
                                                          new AlgorithmClassReferenceExpression("System.IO", "File"),
                                                          "ReadAllText",
                                                          new[] { typeof(string) },
                                                          pathToTheFileReference));
        }

        public override SimulatorError DescribeSimulationError(SimulatorError error)
        {
            if (error.Exception is FileNotFoundException)
            {
                error.ErrorDescription = string.Format("The file '{0}' does not exists...", ((FileNotFoundException)error.Exception).FileName);
                error.SolutionDescription = "Try to use the action 'Check if a file exists' before.";
            }
            return error;
        }

        #endregion
    }
}
