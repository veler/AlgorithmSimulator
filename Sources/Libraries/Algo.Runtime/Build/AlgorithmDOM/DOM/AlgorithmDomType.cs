namespace Algo.Runtime.Build.AlgorithmDOM.DOM
{
    /// <summary>
    /// Defines identifiers used to identify the object without reflection
    /// </summary>
    internal enum AlgorithmDomType
    {
        /// <summary>
        /// <see cref="AlgorithmExpressionStatement"/>
        /// </summary>
        ExpressionStatement = 0,
        /// <summary>
        /// <see cref="AlgorithmAssignStatement"/>
        /// </summary>
        AssignStatement = 1,
        /// <summary>
        /// <see cref="AlgorithmBinaryOperatorExpression"/>
        /// </summary>
        BinaryOperatorExpression = 2,
        /// <summary>
        /// <see cref="AlgorithmClassConstructorDeclaration"/>
        /// </summary>
        ClassConstructorDeclaration = 3,
        /// <summary>
        /// <see cref="AlgorithmClassDeclaration"/>
        /// </summary>
        ClassDeclaration = 4,
        /// <summary>
        /// <see cref="AlgorithmClassMethodDeclaration"/>
        /// </summary>
        ClassMethodDeclaration = 5,
        /// <summary>
        /// <see cref="AlgorithmClassPropertyDeclaration"/>
        /// </summary>
        ClassPropertyDeclaration = 6,
        /// <summary>
        /// <see cref="AlgorithmClassReferenceExpression"/>
        /// </summary>
        ClassReferenceExpression = 7,
        /// <summary>
        /// <see cref="AlgorithmConditionStatement"/>
        /// </summary>
        ConditionStatement = 8,
        /// <summary>
        /// <see cref="AlgorithmEntryPointMethod"/>
        /// </summary>
        EntryPointMethod = 9,
        /// <summary>
        /// <see cref="AlgorithmIdentifier"/>
        /// </summary>
        Identifier = 10,
        /// <summary>
        /// <see cref="AlgorithmInstanciateExpression"/>
        /// </summary>
        InstanciateExpression = 11,
        /// <summary>
        /// <see cref="AlgorithmInvokeCoreMethodExpression"/>
        /// </summary>
        InvokeCoreMethodExpression = 12,
        /// <summary>
        /// <see cref="AlgorithmInvokeMethodExpression"/>
        /// </summary>
        InvokeMethodExpression = 13,
        /// <summary>
        /// <see cref="AlgorithmIterationStatement"/>
        /// </summary>
        IterationStatement = 14,
        /// <summary>
        /// <see cref="AlgorithmParameterDeclaration"/>
        /// </summary>
        ParameterDeclaration = 15,
        /// <summary>
        /// <see cref="AlgorithmPrimitiveExpression"/>
        /// </summary>
        PrimitiveExpression = 16,
        /// <summary>
        /// <see cref="AlgorithmProgram"/>
        /// </summary>
        Program = 17,
        /// <summary>
        /// <see cref="AlgorithmPropertyReferenceExpression"/>
        /// </summary>
        PropertyReferenceExpression = 18,
        /// <summary>
        /// <see cref="AlgorithmReturnStatement"/>
        /// </summary>
        ReturnStatement = 19,
        /// <summary>
        /// <see cref="AlgorithmSnippetExpression"/>
        /// </summary>
        SnippetExpression = 20,
        /// <summary>
        /// <see cref="AlgorithmSnippetStatement"/>
        /// </summary>
        SnippetStatement = 21,
        /// <summary>
        /// <see cref="AlgorithmThisReferenceExpression"/>
        /// </summary>
        ThisReferenceExpression = 22,
        /// <summary>
        /// <see cref="AlgorithmVariableDeclaration"/>
        /// </summary>
        VariableDeclaration = 23,
        /// <summary>
        /// <see cref="AlgorithmVariableReferenceExpression"/>
        /// </summary>
        VariableReferenceExpression = 24,
        /// <summary>
        /// <see cref="AlgorithmArrayIndexerExpression"/>
        /// </summary>
        ArrayIndexerExpression = 25,
        /// <summary>
        /// <see cref="AlgorithmBreakpointStatement"/>
        /// </summary>
        BreakpointStatement = 26
    }
}
