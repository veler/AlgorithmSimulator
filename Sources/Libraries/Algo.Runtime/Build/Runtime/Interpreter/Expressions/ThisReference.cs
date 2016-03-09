using Algo.Runtime.Build.AlgorithmDOM.DOM;
using Algo.Runtime.Build.Runtime.Debugger;
using Algo.Runtime.Build.Runtime.Debugger.Exceptions;
using Algo.Runtime.Build.Runtime.Interpreter.Interpreter;

namespace Algo.Runtime.Build.Runtime.Interpreter.Expressions
{
    internal sealed class ThisReference : InterpretExpression
    {
        #region Constructors

        internal ThisReference(bool memTrace, BlockInterpreter parentInterpreter, AlgorithmExpression expression)
            : base(memTrace, parentInterpreter, expression)
        {
        }

        #endregion

        #region Methods

        internal override object Execute()
        {
            var parent = ParentInterpreter.GetFirstNextParentInterpreter(InterpreterType.ClassInterpreter);

            if (parent == null)
            {
                ParentInterpreter.ChangeState(this, new SimulatorStateEventArgs(new Error(new ClassNotFoundException("{Unknow}", "It looks like the parent class does not exists."), ParentInterpreter.GetDebugInfo())));
                return null;
            }

            var parentClass = (ClassInterpreter)parent;

            if (!parentClass.IsInstance)
            {
                ParentInterpreter.ChangeState(this, new SimulatorStateEventArgs(new Error(new NoInstanceReferenceException("Unable to get the instance of the parent class of a static method."), ParentInterpreter.GetDebugInfo())));
                return null;
            }

            if (MemTrace)
            {
                ParentInterpreter.Log(this, $"Reference to the current instance : {parentClass.ClassDeclaration.Name}");
            }

            return parentClass;
        }

        #endregion
    }
}
