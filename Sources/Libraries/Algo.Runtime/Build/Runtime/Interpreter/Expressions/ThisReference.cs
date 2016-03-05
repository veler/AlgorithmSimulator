using Algo.Runtime.Build.AlgorithmDOM.DOM;
using Algo.Runtime.Build.Runtime.Debugger;
using Algo.Runtime.Build.Runtime.Debugger.Exceptions;
using Algo.Runtime.Build.Runtime.Interpreter.Interpreter;

namespace Algo.Runtime.Build.Runtime.Interpreter.Expressions
{
    sealed internal class ThisReference : InterpretExpression<AlgorithmThisReferenceExpression>
    {
        #region Constructors

        internal ThisReference(bool memTrace, BlockInterpreter parentInterpreter, AlgorithmThisReferenceExpression expression)
            : base(memTrace, parentInterpreter, expression)
        {
        }

        #endregion

        #region Methods

        internal override object Execute()
        {
            var parentClass = ParentInterpreter.GetFirstNextParentInterpreter<ClassInterpreter>();

            if (parentClass == null)
            {
                ParentInterpreter.ChangeState(this, new SimulatorStateEventArgs(new Error(new ClassNotFoundException("{Unknow}", "It looks like the parent class does not exists."), ParentInterpreter.GetDebugInfo())));
                return null;
            }

            if (!parentClass.IsInstance)
            {
                ParentInterpreter.ChangeState(this, new SimulatorStateEventArgs(new Error(new NoInstanceReferenceException("Unable to get the instance of the parent class of a static method."), ParentInterpreter.GetDebugInfo())));
                return null;
            }

            ParentInterpreter.Log(this, $"Reference to the current instance : {parentClass.ClassDeclaration.Name}");

            return parentClass;
        }

        #endregion
    }
}
