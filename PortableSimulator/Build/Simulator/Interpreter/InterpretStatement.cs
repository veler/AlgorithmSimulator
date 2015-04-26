namespace PortableSimulator.Build.Simulator.Interpreter
{
    using PortableSimulator.Build.AlgorithmDOM.DOM;

    abstract class InterpretStatement<T> where T : AlgorithmStatement
    {
        #region Properties

        protected T Statement { get; private set; }

        #endregion

        #region Constructors

        protected InterpretStatement(T statement)
        {
            this.Statement = statement;
        }

        #endregion

        #region Methods

        public abstract void Execute();

        #endregion
    }
}
