using System;
using System.Collections.ObjectModel;

namespace Algo.Runtime.Build.AlgorithmDOM.DOM
{
    /// <summary>
    /// Represents a parameter declaration for a method in an algorithm
    /// </summary>
    public class AlgorithmParameterDeclaration : AlgorithmObject, IAlgorithmVariable
    {
        #region Properties

        /// <summary>
        /// Gets or sets the name of the argument 
        /// </summary>         
        public AlgorithmIdentifier Name { get; set; }

        /// <summary>
        /// Gets or sets whether the argument is of type <see cref="object"/> or <see cref="Collection{T}"/> of <see cref="object"/>
        /// </summary>
        public bool IsArray { get; set; }

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
