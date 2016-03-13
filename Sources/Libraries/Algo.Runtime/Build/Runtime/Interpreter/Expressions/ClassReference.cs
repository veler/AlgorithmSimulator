using System;
using Algo.Runtime.Build.AlgorithmDOM.DOM;
using Algo.Runtime.Build.Runtime.Debugger;
using Algo.Runtime.Build.Runtime.Debugger.Exceptions;
using Algo.Runtime.Build.Runtime.Interpreter.Interpreter;

namespace Algo.Runtime.Build.Runtime.Interpreter.Expressions
{
    /// <summary>
    /// Provide the interpreter for a class reference
    /// </summary>
    internal sealed class ClassReference : InterpretExpression
    {
        #region Constructors

        /// <summary>
        /// Initialize a new instance of <see cref="ClassReference"/>
        /// </summary>
        /// <param name="debugMode">Defines if the debug mode is enabled</param>
        /// <param name="parentInterpreter">The parent block interpreter</param>
        /// <param name="expression">The algorithm expression</param>
        internal ClassReference(bool debugMode, BlockInterpreter parentInterpreter, AlgorithmExpression expression)
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
            object type;
            var fullName = Expression.ToString();

            if (DebugMode)
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

            ParentInterpreter.ChangeState(this, new AlgorithmInterpreterStateEventArgs(new Error(new ClassNotFoundException(fullName, $"Unable to find the class '{fullName}' because it does not exist or it is not accessible."), Expression), ParentInterpreter.GetDebugInfo()));
            return null;
        }

        /// <summary>
        /// Returns a <see cref="Type"/> from a full name
        /// </summary>
        /// <param name="fullName">The full name</param>
        /// <returns>The type</returns>
        private Type GetCoreClassReference(string fullName)
        {
            return Type.GetType(fullName, false, true);
        }

        /// <summary>
        /// Returns a <see cref="ClassInterpreter"/> from a class name
        /// </summary>
        /// <param name="className">The class name</param>
        /// <returns>Returns the class interpreter</returns>
        private ClassInterpreter GetProjectClassReference(string className)
        {
            ClassInterpreter classReference = null;
            var program = ParentInterpreter.ParentProgramInterpreter;
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
