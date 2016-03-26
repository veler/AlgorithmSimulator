using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
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
            AddTerm("<identifierArray>", @"([_a-zA-Z][_a-zA-Z0-9]*(\[\])?)", 3, null);
            AddTerm("<string>", @"""([^""\\]|\\['""\\0abfnrtv]|\\x[a-fA-F0-9][a-fA-F0-9]{0,3})*""", 3, null);
            AddTerm("<character>", @"'([^'\\]|\\['""\\0abfnrtv]|\\x[a-fA-F0-9][a-fA-F0-9]{0,3})'", 3, null);
            AddTerm("<decimal>", @"-?[0-9\.]+", 3, null);

            AddTerm("<primitiveValue>", @"(<string>|<character>|<decimal>|\[(.*)?\])", 3, EvaluatePrimitiveValue);

            AddLeftParen("<leftParen>", @"\(", 1);
            AddRightParen("<rightParen>", @"\)", 1);
            AddLeftParen("<leftBracket>", @"\[", 1);
            AddRightParen("<rightBracket>", @"\]", 1);
            AddArgumentSeparator("<comma>", @"\s*,\s*", 1);

            AddStatementSeparator("<newLine>", @"\s*\n\s*", 0);
            AddStatementSeparator("<comment>", @"\s*#.*", 0);

            AddOperator("<+>", @"\+", 2, null);

            AddTerm("<variableDeclaration>", @"VARIABLE\s+<identifierArray>(\s+(=)\s*<primitiveValue>)?", 4, EvaluateVariableDeclaration); // TODO : variable initialization => VARIABLE myVar[] = ["item1", "item2"]

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

        private TokenEvaluatorResult EvaluateVariableDeclaration(string text, string[] splittedText, EvaluatorArgument evaluatorArgument)
        {
            AlgorithmPrimitiveExpression defaultValue = null;
            var variableName = splittedText[2];
            var isArray = variableName.EndsWith("[]");

            variableName = variableName.Replace("[]", string.Empty);

            if (splittedText.Length > 4)
            {
                var defaultValueStringIndex = -1;
                if (!isArray && splittedText[4] == "=")
                {
                    defaultValueStringIndex = 5;
                }
                else if (isArray && splittedText.Length > 5 && splittedText[5] == "=")
                {
                    defaultValueStringIndex = 6;
                }

                if (defaultValueStringIndex > -1)
                {
                    var primitiveValueRegex = GetRegexFromName("<primitiveValue>");
                    var defaultValueString = splittedText[defaultValueStringIndex];
                    defaultValue = (AlgorithmPrimitiveExpression)EvaluatePrimitiveValue(defaultValueString, primitiveValueRegex.Split(defaultValueString), evaluatorArgument).AlgorithmObject;

                    // ReSharper disable UseMethodIsInstanceOfType
                    // ReSharper disable UseIsOperator.1
                    if (isArray && defaultValue != null && !typeof(List<object>).IsAssignableFrom(defaultValue.Value.GetType()))
                    // ReSharper restore UseIsOperator.1
                    // ReSharper restore UseMethodIsInstanceOfType
                    {
                        throw new SyntaxErrorException(evaluatorArgument, $"An array value is expected for '{variableName}'");
                    }
                }
            }

            if (defaultValue == null && isArray)
            {
                defaultValue = new AlgorithmPrimitiveExpression(new List<object>());
            }

            // TODO : block
            if (!_inMethod && _inClass)
            {
                var propertyDeclaration = new AlgorithmClassPropertyDeclaration(variableName, isArray);
                propertyDeclaration.DefaultValue = defaultValue;
                return new TokenEvaluatorResult(SyntaxTreeTokenType.PropertyDeclaration, TokenType.StatementSeparator, propertyDeclaration);
            }
            if (_inMethod || _inProgram)
            {
                var variableDeclaration = new AlgorithmVariableDeclaration(variableName, isArray);
                variableDeclaration.DefaultValue = defaultValue;
                return new TokenEvaluatorResult(SyntaxTreeTokenType.VariableDeclaration, TokenType.StatementSeparator, variableDeclaration);
            }
            throw new ValidationException();
        }

        private TokenEvaluatorResult EvaluateFunctionDeclaration(string text, string[] splittedText, EvaluatorArgument evaluatorArgument)
        {
            var identifierArrayPattern = string.Format("^{0}$", GetRegexFromName("<identifierArray>"));
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

        private TokenEvaluatorResult EvaluatePrimitiveValue(string text, string[] splittedText, EvaluatorArgument evaluatorArgument)
        {
            return new TokenEvaluatorResult(SyntaxTreeTokenType.PrimitiveValue, TokenType.Unknow, new AlgorithmPrimitiveExpression(ParsePrimitiveValue(ref text, evaluatorArgument, false)));
        }

        private object ParsePrimitiveValue(ref string text, EvaluatorArgument evaluatorArgument, bool inArray)
        {
            int match;
            var stringRegex = new Regex("^" + GetRegexFromName("<string>"));
            var characterRegex = new Regex("^" + GetRegexFromName("<character>"));
            var decimalRegex = new Regex("^" + GetRegexFromName("<decimal>"));

            text = text.TrimStart();

            match = Match(stringRegex, text);
            if (match > 0)
            {
                var value = text.Substring(1, match - 2);
                text = text.Substring(match);
                return value;
            }

            match = Match(characterRegex, text);
            if (match > 0)
            {
                var character = text.Substring(1, match - 2);
                if (character.Length > 1)
                {
                    throw new SyntaxErrorException(evaluatorArgument, "Too may characters in character literal. Please use the \" instead of \' to define a string.");
                }
                text = text.Substring(match);
                return character;
            }

            match = Match(decimalRegex, text);
            if (match > 0)
            {
                var number = text.Substring(0, match);
                text = text.Substring(match);
                if (number.Contains("."))
                {
                    return decimal.Parse(number);
                }
                return long.Parse(number);
            }

            if (text.StartsWith("["))
            {
                object value;
                var list = new List<object>();

                text = text.Substring(1);

                do
                {
                    value = ParsePrimitiveValue(ref text, evaluatorArgument, true);
                    if (value != null)
                    {
                        list.Add(value);
                    }
                } while (value != null);

                return list;
            }

            if (text.StartsWith("]"))
            {
                return null;
            }

            if (inArray)
            {
                var argumentSeparatorRegex = new Regex("^" + GetRegexFromName("<comma>"));
                match = Match(argumentSeparatorRegex, text);
                if (match > 0)
                {
                    text = text.Substring(match);
                    return ParsePrimitiveValue(ref text, evaluatorArgument, true);
                }
            }

            throw new SyntaxErrorException(evaluatorArgument, "String, charachter or number expected.");
        }

        private int Match(Regex regex, string text)
        {
            var m = regex.Match(text);
            return m.Success ? m.Length : 0;
        }

        #endregion
    }
}
