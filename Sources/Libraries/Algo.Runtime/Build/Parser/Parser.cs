using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Algo.Runtime.Build.AlgorithmDOM.DOM;
using Algo.Runtime.Build.Parser.Exceptions;
using Algo.Runtime.Build.Parser.Lexer;
using Algo.Runtime.Build.Parser.SyntaxTree;
using Algo.Runtime.ComponentModel;

namespace Algo.Runtime.Build.Parser
{
    public sealed class Parser<T> : IDisposable where T : LanguageParser
    {
        #region Fields

        private readonly Lexer.Lexer _lexer;
        private SyntaxTreeBuilder _syntaxTreeBuilder;

        #endregion

        #region Properties

        public AlgorithmProgram AlgorithmProgram { get; private set; }

        public SyntaxErrorException Error { get; private set; }

        #endregion

        #region Events

        public event EventHandler<EventArgs> ParsingCompleted;

        #endregion

        #region Constructors

        public Parser()
        {
            var languageParser = LanguageParserService.GetService().GetLanguageParser<T>();
            languageParser.FixPatterns();
            _lexer = new Lexer.Lexer(languageParser.TokenDefinitions.Values.ToList());
        }

        #endregion

        #region Methods

        public Task ParseAsync(CodeDocument code)
        {
            return ParseAsync(code, null);
        }

        public Task ParseAsync(CodeDocument code, IReadOnlyList<object> windows)
        {
            return ParseAsync(new List<CodeDocument> { code }, windows);
        }

        public Task ParseAsync(IReadOnlyList<CodeDocument> codes, IReadOnlyList<object> windows)
        {
            return Task.Run(() =>
            {
                AlgorithmProgram = null;
                Error = null;
                _syntaxTreeBuilder = new SyntaxTreeBuilder();

                try
                {
                    foreach (var code in codes)
                    {
                        _lexer.Initialize(code.DocumentName, code.Code);

                        var nextExpectedToken = TokenType.Unknow;
                        var previousLineNumber = 1;
                        var previousLinePosition = 0;

                        while (_lexer.Next())
                        {
                            if (nextExpectedToken != TokenType.Unknow && _lexer.CurrentTokenDefinition.Token != nextExpectedToken)
                            {
                                throw new SyntaxErrorException(code.DocumentName, _lexer.CurrentLineNumber, _lexer.CurrentLinePosition, $"A {nextExpectedToken.GetDescription()} is expected.");
                            }

                            nextExpectedToken = TokenType.Unknow;
                            if (_lexer.CurrentTokenDefinition.Evaluator != null)
                            {
                                var evaluatorResult = _lexer.CurrentTokenDefinition.Evaluator(_lexer.CurrentTokenContents, _lexer.CurrentSplittedTokenContents);
                                if (evaluatorResult != null)
                                {
                                    nextExpectedToken = evaluatorResult.NextExpectedToken;
                                    _syntaxTreeBuilder.BuildSyntaxTree(new SyntaxTreeBuilderArgument(code.DocumentName, previousLineNumber, previousLinePosition, evaluatorResult));
                                }
                            }
                            previousLineNumber = _lexer.CurrentLineNumber;
                            previousLinePosition = _lexer.CurrentLinePosition;
                        }
                    }
                    AlgorithmProgram = _syntaxTreeBuilder.AlgorithmProgram;
                }
                catch (SyntaxErrorException syntaxErrorException)
                {
                    AlgorithmProgram = null;
                    Error = syntaxErrorException;
                }

                _syntaxTreeBuilder.Dispose();
                _syntaxTreeBuilder = null;

                if (ParsingCompleted != null)
                {
                    ParsingCompleted(this, new EventArgs());
                }
            });
        }

        public void Dispose()
        {
            _lexer.Dispose();
            AlgorithmProgram = null;
        }

        #endregion
    }
}
