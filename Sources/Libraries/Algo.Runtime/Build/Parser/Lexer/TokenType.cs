using Algo.Runtime.ComponentModel;

namespace Algo.Runtime.Build.Parser.Lexer
{
    public enum TokenType
    {
        Unknow,
        [Description("term")]
        Term,
        [Description("left paren or bracket")]
        LeftParen,
        [Description("right paren or bracket")]
        RightParen,
        [Description("argument separator (usually a comma)")]
        ArgumentSeparator,
        [Description("operator")]
        Operator,
        [Description("statement separator")]
        StatementSeparator
    }
}
