using Algo.Runtime.Build.AlgorithmDOM.DOM;
using Algo.Runtime.Build.Runtime.Debugger;
using Algo.Runtime.Build.Runtime.Debugger.Exceptions;
using Algo.Runtime.Build.Runtime.Interpreter.Interpreter;

namespace Algo.Runtime.Build.Runtime.Interpreter.Expressions
{
    /// <summary>
    /// Provide the interpreter for a reference to the current class
    /// </summary>
    internal sealed class ThisReference : InterpretExpression
    {
        #region Constructors

        /// <summary>
        /// Initialize a new instance of <see cref="ThisReference"/>
        /// </summary>
        /// <param name="debugMode">Defines if the debug mode is enabled</param>
        /// <param name="parentInterpreter">The parent block interpreter</param>
        /// <param name="expression">The algorithm expression</param>
        internal ThisReference(bool debugMode, BlockInterpreter parentInterpreter, AlgorithmExpression expression)
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
            var parentClass = ParentInterpreter.ParentClassInterpreter;

            if (parentClass == null)
            {
                ParentInterpreter.ChangeState(this, new AlgorithmInterpreterStateEventArgs(new Error(new ClassNotFoundException("{Unknow}", "It looks like the parent class does not exists."), Expression), ParentInterpreter.GetDebugInfo()));
                return null;
            }

            if (!parentClass.IsInstance)
            {
                ParentInterpreter.ChangeState(this, new AlgorithmInterpreterStateEventArgs(new Error(new NoInstanceReferenceException("Unable to get the instance of the parent class of a static method."), Expression), ParentInterpreter.GetDebugInfo()));
                return null;
            }

            if (DebugMode)
            {
                ParentInterpreter.Log(this, $"Reference to the current instance : {parentClass.ClassDeclaration.Name}");
            }

            return parentClass;
        }

        #endregion
    }
}
