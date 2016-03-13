namespace Algo.Runtime.Build.Runtime.Interpreter
{
    /// <summary>
    /// Defines identifiers used to identify the interpreter without reflection
    /// </summary>
    internal enum InterpreterType
    {
        /// <summary>
        /// <see cref="ProgramInterpreter"/>
        /// </summary>
        ProgramInterpreter,
        /// <summary>
        /// <see cref="ClassInterpreter"/>
        /// </summary>
        ClassInterpreter,
        /// <summary>
        /// <see cref="MethodInterpreter"/>
        /// </summary>
        MethodInterpreter,
        /// <summary>
        /// <see cref="BlockInterpreter"/>
        /// </summary>
        BlockInterpreter
    }
}
