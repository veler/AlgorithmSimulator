namespace PortableSimulator.Build.Simulator
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    using PortableSimulator.Build.AlgorithmDOM.DOM;
    using PortableSimulator.Build.Simulator.Debugger;
    using PortableSimulator.Build.Simulator.Interpreter.Statements;
    using PortableSimulator.ComponentModel;

    using Action = PortableSimulator.Project.Algorithm.Action;

    public class Simulator
    {
        #region Properties

        public SimulatorState State { get; set; }

        public AlgorithmProgram Program { get; private set; }

        #endregion

        #region Constructors

        public Simulator()
        {
            this.State = SimulatorState.Ready;
        }

        #endregion

        #region Methods

        public void Start()
        {
            this.Initialization();

            this.State = SimulatorState.Running;
            Debug.WriteLine("\nSimulator started\n");

            this.Simulate();

            this.Stop();
        }

        public void Break(Action action, SimulatorError error = null)
        {
            this.State = SimulatorState.Pause;

            if (error == null)
            {
                Debug.WriteLine("In pause on the action '{0}'", action.Name);
                return;
            }

            error = action.DescribeSimulationError(error);
            Debug.WriteLine("\n\n\n\n\n");
            Debug.WriteLine("In pause on the action '{0}'. There is an error :", action.Name);
            Debug.WriteLine("Error : {0}", error.ErrorDescription);
            Debug.WriteLine("Suggested solution : {0}", error.SolutionDescription);
        }

        public void Stop()
        {
            this.State = SimulatorState.Stopped;
            Debug.WriteLine("\nSimulator stopped\n");
        }

        private void Initialization()
        {
            this.State = SimulatorState.Preparing;

            this.Program = new AlgorithmProgram();

            foreach (var variable in StaticVariables.CurrentProject.Variables)
            {
                this.Program.Variables.Add(new AlgorithmVariableDeclaration(variable.Name, variable.IsArray));
            }

            foreach (var document in StaticVariables.CurrentProject.Documents)
            {
                var classDeclaration = new AlgorithmClassDeclaration(document.Name);
                foreach (var function in document.Functions)
                {
                    classDeclaration.Members.Add(new AlgorithmClassMethod(function.Name));
                }
                this.Program.Classes.Add(classDeclaration);
            }

            this.State = SimulatorState.Ready;
        }

        private void Simulate()
        {
            var algorithm = StaticVariables.CurrentProject.Documents[0].Functions[0].Algorithm;
            var method = (AlgorithmClassMethod)this.Program.Classes[0].Members[0];
            method.Statements.Clear();

            foreach (var action in algorithm)
            {
                this.DisplayStatistics(action);
                var statements = action.GetCode().ToList();

                var error = this.InterpretAndExecuteCode(statements);
                if (error != null)
                {
                    this.Break(action, error);
                    return;
                }

                method.Statements.AddRange(statements);
            }

            this.DisplayStatistics(null);
        }

        private SimulatorError InterpretAndExecuteCode(IEnumerable<AlgorithmStatement> statements)
        {
            try
            {
                foreach (var statement in statements)
                {
                    TypeSwitch.Do(
                        statement,
                        TypeSwitch.Case<AlgorithmAssignStatement>(s => new Assign(s).Execute()), // AlgoAssignStatement
                        TypeSwitch.Default(() => { }));
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    return new SimulatorError(ex.InnerException);
                return new SimulatorError(ex);
            }
            return null;
        }

        private void DisplayStatistics(Action currentAction)
        {
            Debug.WriteLine("\n****************************");
            Debug.WriteLine("Current action : {0}", currentAction == null ? "{None}" : currentAction.Name);
            Debug.WriteLine("---------VARIABLES----------");
            foreach (var variable in this.Program.Variables)
            {
                Debug.WriteLine("{0} : {1}", variable.Name, variable.Value == null ? "{Null}" : variable.Value.ToString());
            }
            Debug.WriteLine("****************************");
        }

        #endregion
    }
}
