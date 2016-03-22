using System;
using Newtonsoft.Json;

namespace Algo.Runtime.Build.AlgorithmDOM.DOM
{
    /// <summary>
    /// Basic class for algorithm representation.
    /// </summary>
    public abstract class AlgorithmObject
    {
        #region Fields

        protected internal AlgorithmExpression _leftExpression;
        protected internal AlgorithmExpression _rightExpression;
        protected internal AlgorithmBinaryOperatorType _operator;
        protected internal AlgorithmClassMemberCollection _members;
        protected internal AlgorithmIdentifier _name;
        protected internal AlgorithmStatementCollection _statements;
        protected internal AlgorithmParameterDeclarationCollection _arguments;
        protected internal AlgorithmExpressionCollection _argumentsExpression;
        protected internal bool _isAsync;
        protected internal bool _isArray;
        protected internal AlgorithmPrimitiveExpression _defaultValue;
        protected internal string _namespace;
        protected internal AlgorithmIdentifier _className;
        protected internal Type _type;
        protected internal AlgorithmExpression _condition;
        protected internal AlgorithmStatementCollection _trueStatements;
        protected internal AlgorithmStatementCollection _falseStatements;
        protected internal AlgorithmExpression _expression;
        protected internal string _identifier;
        protected internal AlgorithmClassReferenceExpression _createType;
        protected internal AlgorithmReferenceExpression _targetObject;
        protected internal Type[] _argumentsTypes;
        protected internal AlgorithmIdentifier _methodName;
        protected internal bool _await;
        protected internal AlgorithmStatement _initializationStatement;
        protected internal AlgorithmStatement _incrementStatement;
        protected internal bool _conditionAfterBody;
        protected internal object _value;
        protected internal AlgorithmIdentifier _propertyName;
        protected internal string _code;
        protected internal AlgorithmExpression _indice;

        #endregion

        #region Properties

        /// <summary>
        /// Gets a <see cref="AlgorithmDomType"/> used to identify the object without reflection
        /// </summary>
        internal abstract AlgorithmDomType DomType { get; }

        /// <summary>
        /// Gets a unique GUID to identify a part of an algorithm
        /// </summary>   
        [JsonProperty]
        public string Id { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initialize a new instance of <see cref="AlgorithmObject"/>
        /// </summary>
        protected AlgorithmObject()
        {
            Id = $"{Guid.NewGuid().ToString().ToLower()}-{Guid.NewGuid().ToString().ToLower()}";
        }

        #endregion
    }
}
