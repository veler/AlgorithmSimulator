using System;
using System.Collections.Generic;
using System.Linq;
using Algo.Runtime.Build.AlgorithmDOM.DOM;
using Algo.Runtime.Build.Parser.Exceptions;

namespace Algo.Runtime.Build.Parser.SyntaxTree
{
    internal class SyntaxTreeBuilder : IDisposable
    {
        #region Fields

        private bool _entryPointSet;
        private bool _programSet;
        private bool _inClass;
        private bool _inMethod;
        private bool _inProgram;

        private AlgorithmClassDeclaration _currentClassDeclaration;

        private Dictionary<string, SyntaxTreeBuilderArgument> _classes;

        #endregion

        #region Properties

        internal string DefaultProgramName => "UnnamedProgram";

        internal AlgorithmProgram AlgorithmProgram { get; private set; }

        #endregion

        #region Constructors

        public SyntaxTreeBuilder()
        {
            _classes = new Dictionary<string, SyntaxTreeBuilderArgument>();
        }

        #endregion

        #region Methods

        internal void Reset()
        {
            _entryPointSet = false;
            _programSet = false;
            _inClass = false;
            _inMethod = false;
            _inProgram = false;

            _currentClassDeclaration = null;
            _classes.Clear();
        }

        internal void BuildSyntaxTree(SyntaxTreeBuilderArgument argument)
        {
            InitializeProgram();

            switch (argument.EvaluatorResult.CurrentSyntaxTreeTokenType)
            {
                case SyntaxTreeTokenType.Unknow:
                    break;

                case SyntaxTreeTokenType.BeginProgram:
                    SetProgram(argument);
                    break;

                case SyntaxTreeTokenType.BeginClass:
                    AddClassDeclaration(argument);
                    break;

                case SyntaxTreeTokenType.BeginMethod:
                    AddMethodDeclaration(argument);
                    break;

                case SyntaxTreeTokenType.BeginBlock:
                    break;

                case SyntaxTreeTokenType.EndBlock:
                case SyntaxTreeTokenType.EndMethod:
                case SyntaxTreeTokenType.EndClass:
                case SyntaxTreeTokenType.EndProgram:
                    EndInstruction(argument);
                    break;

                case SyntaxTreeTokenType.VariableDeclaration:
                    AddVariableDeclaration(argument);
                    break;

                case SyntaxTreeTokenType.PropertyDeclaration:
                    AddPropertyDeclaration(argument);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void Dispose()
        {
            Reset();
            _classes = null;
        }

        private void InitializeProgram()
        {
            if (AlgorithmProgram == null)
            {
                AlgorithmProgram = new AlgorithmProgram(DefaultProgramName);
            }
        }

        private void SetProgram(SyntaxTreeBuilderArgument argument)
        {
            if (_programSet || _inProgram)
            {
                throw new SyntaxErrorException(argument, "A program can be defined only one time.");
            }

            AlgorithmProgram.Name = ((AlgorithmProgram)argument.EvaluatorResult.AlgorithmObject).Name;
            _programSet = true;
            _inProgram = true;
        }

        private void AddClassDeclaration(SyntaxTreeBuilderArgument argument)
        {
            if (_inClass)
            {
                throw new SyntaxErrorException(argument, $"A class cannot be defined at this location. Please move this class outside of '{_classes.Last().Key}'.");
            }

            var classDefinition = (AlgorithmClassDeclaration)argument.EvaluatorResult.AlgorithmObject;

            if (_classes.ContainsKey(classDefinition.Name.ToString()))
            {
                var duplicatedClass = _classes[classDefinition.Name.ToString()];
                throw new SyntaxErrorException(argument, $"The class '{classDefinition.Name}' already exists in '{duplicatedClass.DocumentName}' line {duplicatedClass.LineNumber} : {duplicatedClass.LinePosition}");
            }

            _currentClassDeclaration = classDefinition;
            AlgorithmProgram.Classes.Add(classDefinition);

            _inClass = true;
            _classes.Add(classDefinition.Name.ToString(), argument);
        }

        private void AddMethodDeclaration(SyntaxTreeBuilderArgument argument)
        {
            if (_inMethod)
            {
                throw new SyntaxErrorException(argument, $"A method cannot be defined at this location. Please move this method outside of '{_currentClassDeclaration.Members.Last().Name}'.");
            }

            var methodDefinition = (AlgorithmClassMethodDeclaration)argument.EvaluatorResult.AlgorithmObject;

            if (_currentClassDeclaration.Members.Any(member => member.Name == methodDefinition.Name))
            {
                throw new SyntaxErrorException(argument, $"The method '{methodDefinition.Name}' already exists in '{_currentClassDeclaration.Name}'");
            }

            if (methodDefinition.DomType == AlgorithmDomType.EntryPointMethod)
            {
                if (_entryPointSet)
                {
                    throw new SyntaxErrorException(argument, "The entry point method has already been set.");
                }
                _entryPointSet = true;
            }

            _inMethod = true;
            _currentClassDeclaration.Members.Add(methodDefinition);
        }

        private void EndInstruction(SyntaxTreeBuilderArgument argument)
        {
            if (argument.EvaluatorResult.CurrentSyntaxTreeTokenType == SyntaxTreeTokenType.EndBlock)
            {
                // TODO
            }
            else if (argument.EvaluatorResult.CurrentSyntaxTreeTokenType == SyntaxTreeTokenType.EndMethod)
            {
                if (!_inMethod)
                {
                    throw new SyntaxErrorException(argument, "Method definition is missing.");
                }
                // TODO : detect a not closed block.
                _inMethod = false;
            }
            else if (argument.EvaluatorResult.CurrentSyntaxTreeTokenType == SyntaxTreeTokenType.EndClass)
            {
                if (!_inClass)
                {
                    throw new SyntaxErrorException(argument, "Class definition is missing.");
                }
                if (_inMethod) // TODO : block
                {
                    throw new SyntaxErrorException(argument, "A method or block has not been closed.");
                }
                _currentClassDeclaration = null;
                _inClass = false;
            }
            else if (argument.EvaluatorResult.CurrentSyntaxTreeTokenType == SyntaxTreeTokenType.EndProgram)
            {
                if (!_inProgram)
                {
                    throw new SyntaxErrorException(argument, "Program definition is missing.");
                }
                if (_inClass || _inMethod) // TODO : block
                {
                    throw new SyntaxErrorException(argument, "A class, method or block has not been closed.");
                }
                _inProgram = false;
            }
        }

        private void AddVariableDeclaration(SyntaxTreeBuilderArgument argument)
        {
            var variableDeclaration = (AlgorithmVariableDeclaration)argument.EvaluatorResult.AlgorithmObject;

            // TODO : block
            if (_inMethod)
            {
                AlgorithmProgram.Classes.Last().Members.Last()._statements.Add(variableDeclaration);
            }
            else
            {
                AlgorithmProgram.Variables.Add(variableDeclaration);
            }
        }

        private void AddPropertyDeclaration(SyntaxTreeBuilderArgument argument)
        {
            var propertyDeclaration = (AlgorithmClassPropertyDeclaration)argument.EvaluatorResult.AlgorithmObject;

            if (_inClass && _currentClassDeclaration != null)
            {
                _currentClassDeclaration.Members.Add(propertyDeclaration);
            }
            else
            {
                throw new SyntaxErrorException(argument, string.Empty);
            }
        }

        #endregion
    }
}
