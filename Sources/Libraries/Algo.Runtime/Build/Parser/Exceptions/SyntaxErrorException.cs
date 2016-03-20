using System;
using Algo.Runtime.Build.Parser.SyntaxTree;

namespace Algo.Runtime.Build.Parser.Exceptions
{
    public class SyntaxErrorException : Exception
    {
        #region Properties

        public string DocumentName { get; set; }

        public int LineNumber { get; set; }

        public int Position { get; set; }

        #endregion

        #region Constructors

        internal SyntaxErrorException(SyntaxTreeBuilderArgument syntaxTreeBuilderArgument, string message)
            : this(syntaxTreeBuilderArgument.DocumentName, syntaxTreeBuilderArgument.LineNumber, syntaxTreeBuilderArgument.LinePosition, message)
        {
        }

        internal SyntaxErrorException(string documentName, int lineNumber, int position)
            : this(documentName, lineNumber, position, "no message")
        {
        }

        internal SyntaxErrorException(string documentName, int lineNumber, int position, string message)
            : base($"Syntax error in '{documentName}', line {lineNumber} : {position} - {message}")
        {
            DocumentName = documentName;
            LineNumber = lineNumber;
            Position = position;
        }

        #endregion
    }
}
