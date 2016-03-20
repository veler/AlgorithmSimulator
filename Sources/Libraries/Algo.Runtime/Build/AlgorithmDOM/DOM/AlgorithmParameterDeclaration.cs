using System;
using System.Collections.ObjectModel;
using Newtonsoft.Json;

namespace Algo.Runtime.Build.AlgorithmDOM.DOM
{
    /// <summary>
    /// Represents a parameter declaration for a method in an algorithm
    /// </summary>
    public class AlgorithmParameterDeclaration : AlgorithmObject, IAlgorithmVariable
    {
        #region Properties

        /// <summary>
        /// Gets a <see cref="AlgorithmDomType"/> used to identify the object without reflection
        /// </summary>
        internal override AlgorithmDomType DomType => AlgorithmDomType.ParameterDeclaration;

        /// <summary>
        /// Gets or sets the name of the argument 
        /// </summary>      
        [JsonProperty]
        public AlgorithmIdentifier Name { get { return _name; } set { _name = value; } }

        /// <summary>
        /// Gets or sets whether the argument is of type <see cref="object"/> or <see cref="Collection{T}"/> of <see cref="object"/>
        /// </summary>
        [JsonProperty]
        public bool IsArray { get { return _isArray; } set { _isArray = value; } }

        /// <summary>
        /// Gets of sets the default value of the variable
        /// </summary>
        public AlgorithmPrimitiveExpression DefaultValue
        {
            get { return null; }
            set { throw new NotSupportedException(); }
        }

        #endregion

        #region Constuctors

        /// <summary>
        /// Initialize a new instance of <see cref="AlgorithmParameterDeclaration"/>
        /// </summary>
        public AlgorithmParameterDeclaration()
        {
        }

        /// <summary>
        /// Initialize a new instance of <see cref="AlgorithmParameterDeclaration"/>
        /// </summary>
        /// <param name="name">The name of the argument</param>
        /// <param name="isArray">Define whether the argument is of type <see cref="object"/> or <see cref="Collection"/> of <see cref="object"/></param>
        public AlgorithmParameterDeclaration(string name, bool isArray = false)
        {
            Name = new AlgorithmIdentifier(name);
            IsArray = isArray;
        }

        #endregion
    }
}
