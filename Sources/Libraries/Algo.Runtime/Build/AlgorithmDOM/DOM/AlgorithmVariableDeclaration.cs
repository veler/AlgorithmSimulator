using System.Collections.ObjectModel;

namespace Algo.Runtime.Build.AlgorithmDOM.DOM
{
    /// <summary>
    /// Represents a variable declaration in a statement. In this structure, variables will always be of type <see cref="object"/> or <see cref="Collection{T}"/> of <see cref="object"/>
    /// </summary>
    public class AlgorithmVariableDeclaration : AlgorithmStatement, IAlgorithmVariable
    {
        #region Properties

        /// <summary>
        /// Gets a <see cref="AlgorithmDomType"/> used to identify the object without reflection
        /// </summary>
        internal override AlgorithmDomType DomType => AlgorithmDomType.VariableDeclaration;

        /// <summary>
        /// Gets or sets the name of the variable 
        /// </summary>         
        public AlgorithmIdentifier Name { get { return _name; } set { _name = value; } }

        /// <summary>
        /// Gets or sets whether the variable is of type <see cref="object"/> or <see cref="Collection{T}"/> of <see cref="object"/>
        /// </summary>
        public bool IsArray { get { return _isArray; } set { _isArray = value; } }

        /// <summary>
        /// Gets of sets the default value of the variable
        /// </summary>
        public AlgorithmPrimitiveExpression DefaultValue { get { return _defaultValue; } set { _defaultValue = value; } }

        #endregion

        #region Constuctors

        /// <summary>
        /// Initialize a new instance of <see cref="AlgorithmVariableDeclaration"/>
        /// </summary>
        public AlgorithmVariableDeclaration()
        {
        }

        /// <summary>
        /// Initialize a new instance of <see cref="AlgorithmVariableDeclaration"/>
        /// </summary>
        /// <param name="name">The name of the variable</param>
        /// <param name="isArray">Define whether the variable is of type <see cref="object"/> or <see cref="Collection{T}"/> of <see cref="object"/></param>
        public AlgorithmVariableDeclaration(string name, bool isArray = false)
        {
            Name = new AlgorithmIdentifier(name);
            IsArray = isArray;
        }

        #endregion
    }
}
