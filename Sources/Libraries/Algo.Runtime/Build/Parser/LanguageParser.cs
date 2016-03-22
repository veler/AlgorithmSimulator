using System.Collections.Generic;
using Algo.Runtime.Build.Parser.Lexer;

namespace Algo.Runtime.Build.Parser
{
    public abstract class LanguageParser
    {
        #region Fields

        private readonly Dictionary<string, TokenDefinition> _tokenDefinitions;

        private bool _patternFixed;

        #endregion

        #region Properties

        public abstract string LanguageName { get; }

        internal IReadOnlyDictionary<string, TokenDefinition> TokenDefinitions => _tokenDefinitions;

        #endregion

        #region Constructors

        protected LanguageParser()
        {
            _tokenDefinitions = new Dictionary<string, TokenDefinition>();
        }

        #endregion

        #region Methods

        public abstract void Reset();

        internal void FixPatterns()
        {
            if (_patternFixed)
            {
                return;
            }

            foreach (var tokenDefinition in _tokenDefinitions)
            {
                foreach (var definition in _tokenDefinitions)
                {
                    var pattern = definition.Value.ToString();
                    if (pattern.Contains(tokenDefinition.Key))
                    {
                        var concatPattern = tokenDefinition.Value.ToString();
                        concatPattern = concatPattern.Substring(2, concatPattern.Length - 3); // remove the "^(" and ")" at the beginning and the end.
                        definition.Value.Pattern = pattern.Replace(tokenDefinition.Key, concatPattern);
                    }
                }
            }

            _patternFixed = true;
        }

        protected void AddTerm(string name, string pattern, ushort priority, TokenEvaluator evaluator)
        {
            AddTokenDefinition(name, new TokenDefinition(TokenType.Term, pattern, priority, evaluator));
        }

        protected void AddOperator(string name, string pattern, ushort priority, TokenEvaluator evaluator)
        {
            AddTokenDefinition(name, new TokenDefinition(TokenType.Operator, pattern, priority, evaluator));
        }

        protected void AddLeftParen(string name, string pattern, ushort priority)
        {
            AddTokenDefinition(name, new TokenDefinition(TokenType.LeftParen, pattern, priority));
        }

        protected void AddRightParen(string name, string pattern, ushort priority)
        {
            AddTokenDefinition(name, new TokenDefinition(TokenType.LeftParen, pattern, priority));
        }

        protected void AddArgumentSeparator(string name, string pattern, ushort priority)
        {
            AddTokenDefinition(name, new TokenDefinition(TokenType.ArgumentSeparator, pattern, priority));
        }

        protected void AddStatementSeparator(string name, string pattern, ushort priority)
        {
            AddTokenDefinition(name, new TokenDefinition(TokenType.StatementSeparator, pattern, priority));
        }

        private void AddTokenDefinition(string name, TokenDefinition tokenDefinition)
        {
            _tokenDefinitions.Add(name, tokenDefinition);
        }

        #endregion
    }
}
