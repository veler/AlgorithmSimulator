namespace Algo.Runtime.Build.Parser.SyntaxTree
{
    public enum SyntaxTreeTokenType
    {
        Unknow,

        BeginProgram,
        BeginClass,
        BeginMethod,
        BeginBlock,

        EndBlock,
        EndMethod,
        EndClass,
        EndProgram,

        VariableDeclaration,
        PropertyDeclaration,

        PrimitiveValue
    }
}
