using System.Text.RegularExpressions;

namespace Algo.Runtime.Build.Parser.Lexer
{
    internal class TokenDefinition
    {
        #region Properties

        internal TokenType Token { get; private set; }

        internal TokenEvaluator Evaluator { get; private set; }

        internal Regex Regex { get; private set; }

        /// <summary>
        /// Gets or sets the priority order for regex match. A higher number means it overrides the previous match.
        /// </summary>
        internal ushort Priority { get; private set; }

        internal string Pattern
        {
            set
            {
                Regex = new Regex(value);
            }
        }

        #endregion

        #region Constructors

        internal TokenDefinition(TokenType token, string pattern, ushort priority)
            : this(token, pattern, priority, null)
        {
        }

        internal TokenDefinition(TokenType token, string pattern, ushort priority, TokenEvaluator evaluator)
        {
            Token = token;
            Pattern = $"^({pattern})";
            Priority = priority;
            Evaluator = evaluator;
        }

        #endregion

        #region Methods

        internal int Match(string text)
        {
            var m = Regex.Match(text);
            return m.Success ? m.Length : 0;
        }

        internal string[] Split(string text)
        {
            return Regex.Split(text);
        }

        public override string ToString()
        {
            return Regex.ToString();
        }

        #endregion
    }
}
