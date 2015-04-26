namespace PortableSimulator.Project.Algorithm
{
    using System.Collections.Generic;

    using PortableSimulator.Build.AlgorithmDOM.DOM;
    using PortableSimulator.Build.Simulator.Debugger;
    using PortableSimulator.Project.Algorithm.Parameters;

    public abstract class Action
    {
        #region Properties

        public abstract string Name { get; }

        public ParameterCollection Parameters { get; set; }

        public ActionCollection Children { get; set; }

        protected IActionTools Tools { get; private set; }

        #endregion

        #region Constructors

        protected Action(IActionTools tools)
        {
            this.Parameters = new ParameterCollection();
            this.Children = new ActionCollection();
            this.Tools = tools;
        }

        #endregion

        #region Methods

        public abstract void Edit();

        public abstract IEnumerable<AlgorithmStatement> GetCode();

        public abstract SimulatorError DescribeSimulationError(SimulatorError error);

        #endregion
    }
}
