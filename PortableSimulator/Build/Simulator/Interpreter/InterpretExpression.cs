namespace PortableSimulator.Build.Simulator.Interpreter
{
    using PortableSimulator.Build.AlgorithmDOM.DOM;

    abstract class InterpretExpression<T> where T : AlgorithmExpression
    {
        #region Properties

        protected T Expression { get; private set; }

        #endregion

        #region Constructors

        protected InterpretExpression(T expression)
        {
            this.Expression = expression;
        }

        #endregion

        #region Methods

        public abstract object Execute();

        #endregion
    }
}
