using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Algo.Runtime.Build.Parser.Exceptions;

namespace Algo.Runtime.Build.Parser.Lexer
{
    internal sealed class Lexer : IDisposable
    {
        #region Fields

        private IReadOnlyList<TokenDefinition> _tokenDefinitions;
        private string _documentName;
        private string _previousText;

        #endregion

        #region Properties

        internal string RemainingText { get; private set; }

        internal string CurrentTokenContents { get; private set; }

        internal string[] CurrentSplittedTokenContents { get; private set; }

        internal TokenDefinition CurrentTokenDefinition { get; private set; }

        internal int CurrentLinePosition { get; private set; }

        internal int CurrentLineNumber { get; private set; }

        #endregion

        #region Constructors

        public Lexer(IReadOnlyList<TokenDefinition> tokenDefinitions)
        {
            if (tokenDefinitions == null)
            {
                throw new ArgumentNullException(nameof(tokenDefinitions));
            }
            _tokenDefinitions = tokenDefinitions;
        }

        #endregion

        #region Methods

        internal void Initialize(string documentName, string text)
        {
            _documentName = documentName;
            _previousText = string.Empty;
            CurrentLinePosition = 0;
            CurrentLineNumber = 1;
            CurrentTokenDefinition = null;
            CurrentTokenContents = string.Empty;
            CurrentSplittedTokenContents = null;
            RemainingText = text;
            Update(0);
        }

        internal bool Next()
        {
            if (string.IsNullOrEmpty(RemainingText))
            {
                _previousText = string.Empty;
                CurrentLinePosition = 0;
                CurrentLineNumber = 1;
                CurrentTokenDefinition = null;
                CurrentTokenContents = string.Empty;
                CurrentSplittedTokenContents = null;
                return false;
            }

            TokenDefinition token;
            TokenDefinition previousToken = null;
            TokenDefinition resultToken = null;
            int matched;
            var syntaxErrorDetected = true;
            var resultMatched = 0;
            var i = 0;

            while (i < _tokenDefinitions.Count)
            {
                token = _tokenDefinitions[i];
                matched = token.Match(RemainingText);
                if (matched > 0 && (previousToken == null || token.Priority > previousToken.Priority))
                {
                    resultMatched = matched;
                    resultToken = token;
                    previousToken = token;
                    syntaxErrorDetected = false;
                }
                i++;
            }

            if (syntaxErrorDetected)
            {
                throw new SyntaxErrorException(_documentName, CurrentLineNumber, CurrentLinePosition);
            }
            
            CurrentTokenDefinition = resultToken;
            CurrentTokenContents = RemainingText.Substring(0, resultMatched);
            CurrentSplittedTokenContents = resultToken.Split(CurrentTokenContents);

            Update(resultMatched);

            return true;
        }

        private void Update(int newPosition)
        {
            _previousText += RemainingText.Substring(0, newPosition);
            RemainingText = RemainingText.Substring(newPosition);

            CurrentLineNumber = _previousText.Count(c => c == '\n') + 1;
            CurrentLinePosition = _previousText.Split('\n').Last().Length;

            // do
            // {
            //     _lineRemaining = _reader.ReadLine();
            //     ++CurrentLineNumber;
            //     CurrentLinePosition = 0;
            // } while (_lineRemaining != null && _lineRemaining.Length == 0);
            //
            // if (!string.IsNullOrWhiteSpace(_lineRemaining))
            // {
            //     _lineRemaining = _lineRemaining.Trim();
            // }
        }

        public void Dispose()
        {
            _previousText = string.Empty;
            CurrentLinePosition = 0;
            CurrentLineNumber = 1;
            CurrentTokenDefinition = null;
            CurrentTokenContents = string.Empty;
            CurrentSplittedTokenContents = null;
            RemainingText = string.Empty;
            RemainingText = string.Empty;
            _tokenDefinitions = null;
        }

        #endregion
    }
}
