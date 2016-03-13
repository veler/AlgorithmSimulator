using Algo.Runtime.Build.AlgorithmDOM.DOM;
using Algo.Runtime.Build.Runtime.Debugger;
using Algo.Runtime.Build.Runtime.Debugger.Exceptions;
using Algo.Runtime.Build.Runtime.Interpreter.Interpreter;
using Algo.Runtime.Build.Runtime.Memory;

namespace Algo.Runtime.Build.Runtime.Interpreter.Expressions
{
    /// <summary>
    /// Provide the interpreter for a reference to a variable
    /// </summary>
    internal sealed class VariableReference : InterpretExpression, IAssignable
    {
        #region Constructors

        /// <summary>
        /// Initialize a new instance of <see cref="VariableReference"/>
        /// </summary>
        /// <param name="debugMode">Defines if the debug mode is enabled</param>
        /// <param name="parentInterpreter">The parent block interpreter</param>
        /// <param name="expression">The algorithm expression</param>
        internal VariableReference(bool debugMode, BlockInterpreter parentInterpreter, AlgorithmExpression expression)
            : base(debugMode, parentInterpreter, expression)
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Run the interpretation
        /// </summary>
        /// <returns>Returns the result of the interpretation</returns>
        internal override object Execute()
        {
            var variable = GetAssignableObject() as Variable;

            if (ParentInterpreter.FailedOrStop || variable == null)
            {
                return null;
            }

            return variable.Value;
        }

        /// <summary>
        /// Returns the corresponding assignable object
        /// </summary>
        /// <returns>the assignable object</returns>
        public object GetAssignableObject()
        {
            var variable = ParentInterpreter.FindVariable(Expression._name.ToString());

            if (variable == null)
            {
                ParentInterpreter.ChangeState(this, new AlgorithmInterpreterStateEventArgs(new Error(new VariableNotFoundException(Expression._name.ToString()), Expression), ParentInterpreter.GetDebugInfo()));
                return null;
            }

            if (DebugMode)
            {
                ParentInterpreter.Log(this, "Value of the variable '{0}' is {1}", variable.Name, variable.Value == null ? "{null}" : $"'{variable.Value}' (type:{variable.Value.GetType().FullName})");
            }
            return variable;
        }

        #endregion
    }
}
