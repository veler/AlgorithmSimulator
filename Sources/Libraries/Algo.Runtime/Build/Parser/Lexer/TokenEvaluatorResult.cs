using Algo.Runtime.Build.AlgorithmDOM.DOM;
using Algo.Runtime.Build.Parser.SyntaxTree;

namespace Algo.Runtime.Build.Parser.Lexer
{
    public class TokenEvaluatorResult
    {
        #region Properties

        public SyntaxTreeTokenType CurrentSyntaxTreeTokenType { get; private set; }

        public TokenType NextExpectedToken { get; private set; }

        public AlgorithmObject AlgorithmObject { get; private set; }

        #endregion

        #region Constructors

        public TokenEvaluatorResult(SyntaxTreeTokenType currentSyntaxTreeTokenType, AlgorithmObject algorithmObject)
            : this(currentSyntaxTreeTokenType, TokenType.Unknow, algorithmObject)
        {
        }

        public TokenEvaluatorResult(SyntaxTreeTokenType currentSyntaxTreeTokenType, TokenType nextExpectedToken)
            : this(currentSyntaxTreeTokenType, nextExpectedToken, null)
        {
        }

        public TokenEvaluatorResult(SyntaxTreeTokenType currentSyntaxTreeTokenType, TokenType nextExpectedToken, AlgorithmObject algorithmObject)
        {
            CurrentSyntaxTreeTokenType = currentSyntaxTreeTokenType;
            NextExpectedToken = nextExpectedToken;
            AlgorithmObject = algorithmObject;
        }

        #endregion
    }
}
