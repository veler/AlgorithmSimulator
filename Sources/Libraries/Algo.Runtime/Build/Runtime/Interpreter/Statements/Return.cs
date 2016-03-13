using Algo.Runtime.Build.AlgorithmDOM.DOM;
using Algo.Runtime.Build.Runtime.Debugger;
using Algo.Runtime.Build.Runtime.Debugger.Exceptions;
using Algo.Runtime.Build.Runtime.Interpreter.Interpreter;

namespace Algo.Runtime.Build.Runtime.Interpreter.Statements
{
    /// <summary>
    /// Provide the interpreter for a return
    /// </summary>
    internal sealed class Return : InterpretStatement
    {
        #region Constructors

        /// <summary>
        /// Initialize a new instance of <see cref="Return"/>
        /// </summary>
        /// <param name="debugMode">Defines if the debug mode is enabled</param>
        /// <param name="parentInterpreter">The parent block interpreter</param>
        /// <param name="statement">The algorithm statement</param>
        public Return(bool debugMode, BlockInterpreter parentInterpreter, AlgorithmStatement statement)
            : base(debugMode, parentInterpreter, statement)
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Run the interpretation
        /// </summary>
        internal override void Execute()
        {
            var interpreter = ParentInterpreter.ParentMethodInterpreter;

            if (interpreter == null)
            {
                ParentInterpreter.ChangeState(this, new AlgorithmInterpreterStateEventArgs(new Error(new MethodNotFoundException("{Unknow}", "It looks like the caller/parent's method does not exists."), Statement), ParentInterpreter.GetDebugInfo()));
                return;
            }

            var method = (MethodInterpreter)interpreter;
            method.ReturnedValue = ParentInterpreter.RunExpression(Statement._expression);

            if (DebugMode && !ParentInterpreter.FailedOrStop)
            {
                ParentInterpreter.Log(this, "({0}) Return : {1}", method.MethodDeclaration._name, method.ReturnedValue == null ? "{null}" : $"'{method.ReturnedValue}' (type:{method.ReturnedValue.GetType().FullName})");
            }
        }

        #endregion

    }
}
