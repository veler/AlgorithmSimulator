using System;
using Algo.Runtime.Build.AlgorithmDOM.DOM;
using Algo.Runtime.Build.Parser;
using Algo.Runtime.Build.Parser.Lexer;
using Algo.Runtime.Build.Parser.SyntaxTree;

namespace Algo.Runtime.Languages
{
    public sealed class AlgoBASIC : LanguageParser
    {
        #region Properties

        public override string LanguageName => "AlgoBASIC";

        #endregion

        #region Constructors

        public AlgoBASIC()
        {
            var anySpacePattern = @"(\s?)+";

            AddTerm("<identifier>", @"([*<>\?\-+/A-Za-z->!]+)", 3, EvaluateAdditionOperator);
            AddTerm("<string>", @"""([^""\\]|\\['""\\0abfnrtv]|\\x[a-fA-F0-9][a-fA-F0-9]{0,3})*""", 3, EvaluateAdditionOperator);
            AddTerm("<character>", @"'([^'\\]|\\['""\\0abfnrtv]|\\x[a-fA-F0-9][a-fA-F0-9]{0,3})'", 3, EvaluateAdditionOperator);

            AddLeftParen("<leftParen>", @"\(", 1);
            AddRightParen("<rightParen>", @"\)", 1);
            AddLeftParen("<leftBracket>", @"\[", 1);
            AddRightParen("<rightBracket>", @"\]", 1);
            AddArgumentSeparator("<comma>", ",", 1);

            AddStatementSeparator("<newLine>", string.Format(@"{0}\n{0}", anySpacePattern), 0);

            AddOperator("<+>", @"\+", 2, EvaluateAdditionOperator);

            AddTerm("<program>", @"(PROGRAM)\s+<identifier>", 4, EvaluateStartBlock);
            AddTerm("<class>", @"(CLASS)\s+<identifier>", 4, EvaluateStartBlock);
            AddTerm("<method>", @"(METHOD)\s+<identifier>", 4, EvaluateStartBlock);

            AddTerm("<endMethod>", @"END\s+(METHOD)", 4, EvaluateEndBlock);
            AddTerm("<endClass>", @"END\s+(CLASS)", 4, EvaluateEndBlock);
            AddTerm("<endProgram>", @"END\s+(PROGRAM)", 4, EvaluateEndBlock);
        }

        #endregion

        #region Methods

        private TokenEvaluatorResult EvaluateStartBlock(string text, string[] splittedText)
        {
            AlgorithmObject algorithmObject = null;
            SyntaxTreeTokenType currentToken = SyntaxTreeTokenType.Unknow;
            var keyword = splittedText[2];
            var identifier = splittedText[3];

            switch (keyword)
            {
                case "METHOD":
                    currentToken = SyntaxTreeTokenType.BeginMethod;
                    //  algorithmObject = new AlgorithmClassMethodDeclaration(identifier);
                    break;
                case "CLASS":
                    currentToken = SyntaxTreeTokenType.BeginClass;
                    algorithmObject = new AlgorithmClassDeclaration(identifier);
                    break;
                case "PROGRAM":
                    currentToken = SyntaxTreeTokenType.BeginProgram;
                    algorithmObject = new AlgorithmProgram(identifier);
                    break;
            }
            return new TokenEvaluatorResult(currentToken, TokenType.StatementSeparator, algorithmObject);
        }

        private TokenEvaluatorResult EvaluateEndBlock(string text, string[] splittedText)
        {
            SyntaxTreeTokenType currentToken = SyntaxTreeTokenType.Unknow;
            var keyword = splittedText[2];

            switch (keyword)
            {
                case "METHOD":
                    currentToken = SyntaxTreeTokenType.EndMethod;
                    break;
                case "CLASS":
                    currentToken = SyntaxTreeTokenType.EndClass;
                    break;
                case "PROGRAM":
                    currentToken = SyntaxTreeTokenType.EndProgram;
                    break;
            }
            return new TokenEvaluatorResult(currentToken, TokenType.StatementSeparator);
        }

        private TokenEvaluatorResult EvaluateAdditionOperator(string text, string[] splittedText)
        {
            return null;
        }

        #endregion
    }
}
