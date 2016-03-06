using System;
using Algo.Runtime.Build.AlgorithmDOM.DOM;
using Algo.Runtime.Build.Runtime.Debugger;
using Algo.Runtime.Build.Runtime.Debugger.Exceptions;
using Algo.Runtime.Build.Runtime.Interpreter.Interpreter;
using Algo.Runtime.Build.Runtime.Memory;

namespace Algo.Runtime.Build.Runtime.Interpreter.Expressions
{
    internal sealed class VariableReference : InterpretExpression, IAssignable
    {
        #region Constructors

        internal VariableReference(bool memTrace, BlockInterpreter parentInterpreter, AlgorithmExpression expression)
            : base(memTrace, parentInterpreter, expression)
        {
        }

        #endregion

        #region Methods

        internal override object Execute()
        {
            var variable = GetAssignableObject() as Variable;

            if (ParentInterpreter.Failed || variable == null)
            {
                return null;
            }

            return variable.Value;
        }

        public object GetAssignableObject()
        {
            var variable = ParentInterpreter.FindVariable(Expression._name.ToString());

            if (variable == null)
            {
                ParentInterpreter.ChangeState(this, new SimulatorStateEventArgs(new Error(new VariableNotFoundException(Expression._name.ToString()), ParentInterpreter.GetDebugInfo())));
                return null;
            }

            ParentInterpreter.Log(this, "Value of the variable '{0}' is {1}", variable.Name, variable.Value == null ? "{null}" : $"'{variable.Value}' (type:{variable.Value.GetType().FullName})");

            return variable;
        }

        #endregion
    }
}
