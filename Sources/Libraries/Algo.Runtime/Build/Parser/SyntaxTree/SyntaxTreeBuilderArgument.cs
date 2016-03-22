using Algo.Runtime.Build.Parser.Lexer;

namespace Algo.Runtime.Build.Parser.SyntaxTree
{
    internal class SyntaxTreeBuilderArgument : EvaluatorArgument
    {
        #region Properties

        internal TokenEvaluatorResult EvaluatorResult { get; private set; }

        #endregion

        #region Constructors

        public SyntaxTreeBuilderArgument(string documentName, int lineNumber, int linePosition, TokenEvaluatorResult evaluatorResult)
            : base(documentName, lineNumber, linePosition)
        {
            EvaluatorResult = evaluatorResult;
        }

        #endregion
    }
}
