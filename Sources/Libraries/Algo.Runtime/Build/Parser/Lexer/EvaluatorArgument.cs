using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algo.Runtime.Build.Parser.Lexer
{
    public class EvaluatorArgument
    {
        #region Properties

        internal string DocumentName { get; private set; }

        internal int LinePosition { get; private set; }

        internal int LineNumber { get; private set; }

        #endregion

        #region Constructors

        public EvaluatorArgument(string documentName, int lineNumber, int linePosition)
        {
            DocumentName = documentName;
            LineNumber = lineNumber;
            LinePosition = linePosition;
        }

        #endregion
    }
}
