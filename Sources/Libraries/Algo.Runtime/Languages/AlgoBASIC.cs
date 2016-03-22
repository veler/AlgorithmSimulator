using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using Algo.Runtime.Build.AlgorithmDOM.DOM;
using Algo.Runtime.Build.Parser;
using Algo.Runtime.Build.Parser.Exceptions;
using Algo.Runtime.Build.Parser.Lexer;
using Algo.Runtime.Build.Parser.SyntaxTree;

namespace Algo.Runtime.Languages
{
    public sealed class AlgoBASIC : LanguageParser
    {
        #region Enumeration

        public enum Culture
        {
            English,
            French
        }

        #endregion

        #region Consts

        private const string IdentifierArrayPattern = @"([_a-zA-Z][_a-zA-Z0-9]*(\[\])?)";

        #endregion

        #region Fields

        private bool _inMethod;
        private bool _inClass;
        private bool _inProgram;

        private string _currentClassName;

        #endregion

        #region Properties

        public override string LanguageName => "AlgoBASIC";

        private string ClassConstructorName { get; set; }

        #endregion

        #region Constructors

        public AlgoBASIC()
            : this(Culture.English)
        {
        }

        public AlgoBASIC(Culture culture)
        {
            AddTerm("<identifier>", @"([_a-zA-Z][_a-zA-Z0-9]+)", 3, null);
            AddTerm("<identifierArray>", IdentifierArrayPattern, 3, null);
            AddTerm("<string>", @"""([^""\\]|\\['""\\0abfnrtv]|\\x[a-fA-F0-9][a-fA-F0-9]{0,3})*""", 3, null);
            AddTerm("<character>", @"'([^'\\]|\\['""\\0abfnrtv]|\\x[a-fA-F0-9][a-fA-F0-9]{0,3})'", 3, null);

            AddLeftParen("<leftParen>", @"\(", 1);
            AddRightParen("<rightParen>", @"\)", 1);
            AddLeftParen("<leftBracket>", @"\[", 1);
            AddRightParen("<rightBracket>", @"\]", 1);
            AddArgumentSeparator("<comma>", @",", 1);

            AddStatementSeparator("<newLine>", @"\s*\n\s*", 0);
            AddStatementSeparator("<comment>", @"\s*#.*", 0);

            AddOperator("<+>", @"\+", 2, EvaluateAdditionOperator);

            AddTerm("<variableDeclaration>", @"VARIABLE\s+<identifierArray>", 4, EvaluateVariableDeclaration); // TODO : variable initialization => VARIABLE myVar[] = ["item1", "item2"]

            if (culture == Culture.English)
            {
                AddTerm("<program>", @"(PROGRAM)\s+<identifier>", 4, EvaluateStartBlock);
                AddTerm("<class>", @"(MODEL)\s+<identifier>", 4, EvaluateStartBlock);
                AddTerm("<method>", @"(ASYNC )?(FUNCTION)\s+<identifier>\s*\((.*)\)", 4, EvaluateStartBlock);

                AddTerm("<endMethod>", @"END\s+(FUNCTION)", 4, EvaluateEndBlock);
                AddTerm("<endClass>", @"END\s+(MODEL)", 4, EvaluateEndBlock);
                AddTerm("<endProgram>", @"END\s+(PROGRAM)", 4, EvaluateEndBlock);

                ClassConstructorName = "Initialize";
            }
            else if (culture == Culture.French)
            {
                AddTerm("<program>", @"(PROGRAMME)\s+<identifier>", 4, EvaluateStartBlock);
                AddTerm("<class>", @"(MODELE)\s+<identifier>", 4, EvaluateStartBlock);
                AddTerm("<method>", @"(FONCTION)( ASYNC)?\s+<identifier>\s*\(\s*<identifierArray>*\s*\)", 4, EvaluateStartBlock);

                AddTerm("<endMethod>", @"FIN\s+(FONCTION)", 4, EvaluateEndBlock);
                AddTerm("<endClass>", @"FIN\s+(MODELE)", 4, EvaluateEndBlock);
                AddTerm("<endProgram>", @"FIN\s+(PROGRAMME)", 4, EvaluateEndBlock);

                ClassConstructorName = "Initialiser";
            }
        }

        #endregion

        #region Methods

        public override void Reset()
        {
            _inMethod = false;
            _inClass = false;
            _inProgram = false;
            _currentClassName = string.Empty;
        }

        private TokenEvaluatorResult EvaluateStartBlock(string text, string[] splittedText, EvaluatorArgument evaluatorArgument)
        {
            AlgorithmObject algorithmObject;
            SyntaxTreeTokenType currentToken;
            var keyword = splittedText[2];

            switch (keyword)
            {
                case "MODEL":
                case "MODELE":
                    _inClass = true;
                    _currentClassName = splittedText[3];
                    currentToken = SyntaxTreeTokenType.BeginClass;
                    algorithmObject = new AlgorithmClassDeclaration(_currentClassName);
                    break;
                case "PROGRAM":
                case "PROGRAMME":
                    _inProgram = true;
                    currentToken = SyntaxTreeTokenType.BeginProgram;
                    algorithmObject = new AlgorithmProgram(splittedText[3]);
                    break;
                default:
                    if (text.StartsWith("ASYNC FUNCTION ") || text.StartsWith("FONCTION ASYNC ") || text.StartsWith("FUNCTION ") || text.StartsWith("FONCTION "))
                    {
                        return EvaluateFunctionDeclaration(text, splittedText, evaluatorArgument);
                    }
                    throw new SyntaxErrorException(evaluatorArgument, "Cannot resolve this symbol.");
            }
            return new TokenEvaluatorResult(currentToken, TokenType.StatementSeparator, algorithmObject);
        }

        private TokenEvaluatorResult EvaluateEndBlock(string text, string[] splittedText, EvaluatorArgument evaluatorArgument)
        {
            var currentToken = SyntaxTreeTokenType.Unknow;
            var keyword = splittedText[2];

            switch (keyword)
            {
                case "FUNCTION":
                case "FONCTION":
                    _inMethod = false;
                    currentToken = SyntaxTreeTokenType.EndMethod;
                    break;
                case "MODEL":
                case "MODELE":
                    _inClass = false;
                    _currentClassName = string.Empty;
                    currentToken = SyntaxTreeTokenType.EndClass;
                    break;
                case "PROGRAM":
                case "PROGRAMME":
                    _inProgram = false;
                    currentToken = SyntaxTreeTokenType.EndProgram;
                    break;
            }
            return new TokenEvaluatorResult(currentToken, TokenType.StatementSeparator);
        }

        private TokenEvaluatorResult EvaluateAdditionOperator(string text, string[] splittedText, EvaluatorArgument evaluatorArgument)
        {
            return null;
        }

        private TokenEvaluatorResult EvaluateVariableDeclaration(string text, string[] splittedText, EvaluatorArgument evaluatorArgument)
        {
            var variableName = splittedText[2];
            var isArray = variableName.EndsWith("[]");

            variableName = variableName.Replace("[]", string.Empty);

            // TODO : block
            if (!_inMethod && _inClass)
            {
                var propertyDeclaration = new AlgorithmClassPropertyDeclaration(variableName, isArray);
                if (isArray)
                {
                    propertyDeclaration.DefaultValue = new AlgorithmPrimitiveExpression(new List<object>());
                }
                return new TokenEvaluatorResult(SyntaxTreeTokenType.PropertyDeclaration, TokenType.StatementSeparator, propertyDeclaration);
            }
            if (_inMethod || _inProgram)
            {
                var variableDeclaration = new AlgorithmVariableDeclaration(variableName, isArray);
                if (isArray)
                {
                    variableDeclaration.DefaultValue = new AlgorithmPrimitiveExpression(new List<object>());
                }
                return new TokenEvaluatorResult(SyntaxTreeTokenType.VariableDeclaration, TokenType.StatementSeparator, variableDeclaration);
            }
            throw new ValidationException();
        }

        private TokenEvaluatorResult EvaluateFunctionDeclaration(string text, string[] splittedText, EvaluatorArgument evaluatorArgument)
        {
            var identifierArrayPattern = string.Format("^{0}$", IdentifierArrayPattern);
            var isAsync = false;
            var identifier = splittedText[3];
            var arguments = splittedText[4].Split(',');
            var i = 0;

            if (text.StartsWith("ASYNC FUNCTION ") || text.StartsWith("FONCTION ASYNC "))
            {
                isAsync = true;
                identifier = splittedText[4];
                arguments = splittedText[5].Split(',');
            }

            arguments = arguments.Where(arg => !string.IsNullOrWhiteSpace(arg)).ToArray();

            var parameters = new AlgorithmParameterDeclaration[arguments.Length];

            while (i < arguments.Length)
            {
                var argument = arguments[i].Trim(' ');

                if (argument.Contains(" "))
                {
                    throw new SyntaxErrorException(evaluatorArgument, "Cannot resolve this symbol.");
                }
                if (!Regex.IsMatch(argument, identifierArrayPattern))
                {
                    throw new SyntaxErrorException(evaluatorArgument, "Cannot resolve this symbol.");
                }

                var isArray = argument.EndsWith("[]");
                argument = argument.Replace("[]", string.Empty);

                parameters[i] = new AlgorithmParameterDeclaration(argument, isArray);

                i++;
            }

            _inMethod = true;

            if (identifier.ToLower() == "main")
            {
                if (isAsync)
                {
                    throw new SyntaxErrorException(evaluatorArgument, "The entry point method cannot be asynchronous.");
                }
                if (parameters.Length > 0)
                {
                    throw new SyntaxErrorException(evaluatorArgument, "The entry point method cannot take any argument.");
                }

                return new TokenEvaluatorResult(SyntaxTreeTokenType.BeginMethod, TokenType.StatementSeparator, new AlgorithmEntryPointMethod());
            }

            if (identifier == ClassConstructorName)
            {
                if (isAsync)
                {
                    throw new SyntaxErrorException(evaluatorArgument, "A model constructor cannot be asynchronous.");
                }

                return new TokenEvaluatorResult(SyntaxTreeTokenType.BeginMethod, TokenType.StatementSeparator, new AlgorithmClassConstructorDeclaration(parameters));
            }

            return new TokenEvaluatorResult(SyntaxTreeTokenType.BeginMethod, TokenType.StatementSeparator, new AlgorithmClassMethodDeclaration(identifier, isAsync, parameters));
        }

        #endregion
    }
}
