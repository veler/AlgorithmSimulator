﻿using System.Collections.ObjectModel;

namespace Algo.Runtime.Build.AlgorithmDOM.DOM
{
    /// <summary>
    /// Represents a property declaration in a class in an algorithm
    /// </summary>
    public class AlgorithmClassPropertyDeclaration : AlgorithmClassMember, IAlgorithmVariable
    {
        #region Properties

        internal override AlgorithmDomType DomType => AlgorithmDomType.ClassPropertyDeclaration;

        /// <summary>
        /// Gets or sets whether the property is of type <see cref="object"/> or <see cref="Collection{T}"/> of <see cref="object"/>
        /// </summary>
        public bool IsArray { get { return _isArray; } set { _isArray = value; } }

        /// <summary>
        /// Gets of sets the default value of the property
        /// </summary>
        public AlgorithmPrimitiveExpression DefaultValue { get { return _defaultValue; } set { _defaultValue = value; } }

        #endregion

        #region Constuctors

        /// <summary>
        /// Initialize a new instance of <see cref="AlgorithmClassPropertyDeclaration"/>
        /// </summary>
        /// <param name="name">The name of the property</param>
        /// <param name="isArray">Define whether the property is of type <see cref="object"/> or <see cref="Collection{T}"/> of <see cref="object"/></param>
        public AlgorithmClassPropertyDeclaration(string name, bool isArray = false)
            : base(name)
        {
            IsArray = isArray;
        }

        #endregion
    }
}
