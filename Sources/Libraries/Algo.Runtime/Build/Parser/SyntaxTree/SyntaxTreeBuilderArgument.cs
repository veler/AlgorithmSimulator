using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Algo.Runtime.Build.Parser.Lexer;

namespace Algo.Runtime.Build.Parser.SyntaxTree
{
    internal class SyntaxTreeBuilderArgument
    {
        #region Properties

        internal string DocumentName { get; private set; }

        internal int LinePosition { get; private set; }

        internal int LineNumber { get; private set; }

        internal TokenEvaluatorResult EvaluatorResult { get; private set; }

        #endregion

        #region Constructors

        public SyntaxTreeBuilderArgument(string documentName, int lineNumber, int linePosition, TokenEvaluatorResult evaluatorResult)
        {
            DocumentName = documentName;
            LineNumber = lineNumber;
            LinePosition = linePosition;
            EvaluatorResult = evaluatorResult;
        }

        #endregion
    }
}
