using System;
using Algo.Runtime.Build.AlgorithmDOM.DOM;
using Algo.Runtime.Build.Runtime.Debugger;
using Algo.Runtime.Build.Runtime.Debugger.Exceptions;
using Algo.Runtime.Build.Runtime.Interpreter.Interpreter;

namespace Algo.Runtime.Build.Runtime.Interpreter.Expressions
{
    internal sealed class ClassReference : InterpretExpression
    {
        #region Constructors

        internal ClassReference(bool memTrace, BlockInterpreter parentInterpreter, AlgorithmExpression expression)
            : base(memTrace, parentInterpreter, expression)
        {
        }

        #endregion

        #region Methods

        internal override object Execute()
        {
            object type;
            var fullName = Expression.ToString();

            if (MemTrace)
            {
                ParentInterpreter.Log(this, $"Reference to the class : {fullName}");
            }

            if (Expression._type != null)
            {
                return Expression._type;
            }

            if (string.IsNullOrWhiteSpace(Expression._namespace))
            {
                type = GetProjectClassReference(fullName);
            }
            else
            {
                type = GetCoreClassReference(fullName);
            }

            if (type != null)
            {
                return type;
            }

            ParentInterpreter.ChangeState(this, new SimulatorStateEventArgs(new Error(new ClassNotFoundException(fullName, $"Unable to find the class '{fullName}' because it does not exist or it is not accessible.")), ParentInterpreter.GetDebugInfo()));
            return null;
        }

        private Type GetCoreClassReference(string fullName)
        {
            return Type.GetType(fullName, false, true);
        }

        private ClassInterpreter GetProjectClassReference(string className)
        {
            ClassInterpreter classReference = null;
            var program = (ProgramInterpreter)ParentInterpreter.GetFirstNextParentInterpreter(InterpreterType.ProgramInterpreter);
            var i = 0;

            while (i < program.Classes.Count && classReference == null)
            {
                if (program.Classes[i].ClassDeclaration.Name.ToString() == className)
                {
                    classReference = program.Classes[i];
                }

                i++;
            }

            return classReference;
        }

        #endregion
    }
}
