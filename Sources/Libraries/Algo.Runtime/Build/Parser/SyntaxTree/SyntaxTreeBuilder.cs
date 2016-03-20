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

        private bool _programSet;
        private bool _inClass;
        private bool _inMethod;
        private bool _inProgram;

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
                    break;

                case SyntaxTreeTokenType.BeginBlock:
                    break;

                case SyntaxTreeTokenType.EndBlock:
                case SyntaxTreeTokenType.EndMethod:
                case SyntaxTreeTokenType.EndClass:
                case SyntaxTreeTokenType.EndProgram:
                    EndInstruction(argument);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void Dispose()
        {
            _classes.Clear();
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

            AlgorithmProgram.Classes.Add(classDefinition);

            _inClass = true;
            _classes.Add(classDefinition.Name.ToString(), argument);
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
                    throw new SyntaxErrorException(argument, "Program definition is missing.");
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

        #endregion
    }
}
