namespace PortableSimulator.Build.Simulator.Debugger
{
    using System;

    public class SimulatorError
    {
        #region Properties

        public Exception Exception { get; private set; }

        public string ErrorDescription { get; set; }

        public string SolutionDescription { get; set; }

        #endregion

        #region Constructors

        public SimulatorError(Exception exception)
        {
            this.Exception = exception;
        }

        #endregion
    }
}
