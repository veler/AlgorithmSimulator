using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algo.Runtime.Build.AlgorithmDOM.DOM
{
    internal enum AlgorithmDomType
    {
        ExpressionStatement,
        AssignStatement,
        BinaryOperatorExpression,
        ClassConstructorDeclaration,
        ClassDeclaration,
        ClassMethodDeclaration,
        ClassPropertyDeclaration,
        ClassReferenceExpression,
        ConditionStatement,
        EntryPointMethod,
        Identifier,
        InstanciateExpression,
        InvokeCoreMethodExpression,
        InvokeMethodExpression,
        IterationStatement,
        ParameterDeclaration,
        PrimitiveExpression,
        Program,
        PropertyReferenceExpression,
        ReturnStatement,
        SnippetExpression,
        SnippetStatement,
        ThisReferenceExpression,
        VariableDeclaration,
        VariableReferenceExpression
    }
}
