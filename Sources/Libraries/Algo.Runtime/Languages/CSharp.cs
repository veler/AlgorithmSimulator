using Algo.Runtime.Build.Parser;
using Algo.Runtime.Build.Parser.Lexer;

namespace Algo.Runtime.Languages
{
    public sealed class CSharp : LanguageParser
    {
        #region Properties

        public override string LanguageName => "C#";

        #endregion

        #region Constructors

        public CSharp()
        {
            var anySpacePattern = @"(\s?\n?\r?)+";

            AddTerm("<identifier>", @"[_a-zA-Z][_a-zA-Z0-9]+", 0, EvaluateAdditionOperator);
            AddTerm("<string>", @"""([^""\\]|\\['""\\0abfnrtv]|\\x[a-fA-F0-9][a-fA-F0-9]{0,3})*""", 20, EvaluateAdditionOperator);
            AddTerm("<character>", @"'([^'\\]|\\['""\\0abfnrtv]|\\x[a-fA-F0-9][a-fA-F0-9]{0,3})'", 20, EvaluateAdditionOperator);

            AddLeftParen("<leftParen>", @"\(", 1);
            AddRightParen("<rightParen>", @"\)", 1);
            AddLeftParen("<leftBracket>", @"\[", 1);
            AddRightParen("<leftBracket>", @"\]", 1);
            AddArgumentSeparator("<comma>", ",", 1);

            AddStatementSeparator("<leftBrace>", string.Format(@"^({0}{{0})", anySpacePattern), 2);
            AddStatementSeparator("<rightBrace>", string.Format(@"^({0}}{0})", anySpacePattern), 2);
            AddStatementSeparator("<semicolon>", string.Format(@"^({0};{0})", anySpacePattern), 2);

            AddOperator("<+>", @"\+", 0, EvaluateAdditionOperator);

            AddTerm("<program>", @"PROGRAM\s+<identifier>", 10, EvaluateStartBlock);
            AddTerm("<class>", @"CLASS\s+<identifier>", 10, EvaluateStartBlock);
            AddTerm("<method>", @"METHOD\s+<identifier>", 10, EvaluateStartBlock);

            AddTerm("<endMethod>", @"END METHOD", 10, EvaluateEndBlock);
            AddTerm("<endClass>", @"END CLASS", 10, EvaluateEndBlock);
            AddTerm("<endProgram>", @"END PROGRAM", 10, EvaluateEndBlock);
        }

        #endregion

        #region Methods

        public override void Reset()
        {
        }

        private TokenEvaluatorResult EvaluateStartBlock(string text, string[] splittedText, EvaluatorArgument evaluatorArgument)
        {
            return null;
        }

        private TokenEvaluatorResult EvaluateEndBlock(string text, string[] splittedText, EvaluatorArgument evaluatorArgument)
        {
            return null;
        }

        private TokenEvaluatorResult EvaluateAdditionOperator(string text, string[] splittedText, EvaluatorArgument evaluatorArgument)
        {
            return null;
        }

        #endregion
    }
}
