﻿using Newtonsoft.Json;

namespace Algo.Runtime.Build.AlgorithmDOM.DOM
{
    /// <summary>
    /// Represents a reference to a variable in an algorithm
    /// </summary>
    public class AlgorithmVariableReferenceExpression : AlgorithmReferenceExpression, IAlgorithmAssignable
    {
        #region Properties

        /// <summary>
        /// Gets a <see cref="AlgorithmDomType"/> used to identify the object without reflection
        /// </summary>
        internal override AlgorithmDomType DomType => AlgorithmDomType.VariableReferenceExpression;

        /// <summary>
        /// Gets or sets the name of the variable
        /// </summary>
        [JsonProperty]
        public AlgorithmIdentifier Name { get { return _name; } set { _name = value; } }

        #endregion

        #region Constuctors

        /// <summary>
        /// Initialize a new instance of <see cref="AlgorithmVariableReferenceExpression"/>
        /// </summary>
        public AlgorithmVariableReferenceExpression()
        {
        }

        /// <summary>
        /// Initialize a new instance of <see cref="AlgorithmVariableReferenceExpression"/>
        /// </summary>
        /// <param name="name">The name of the variable we make reference</param>
        public AlgorithmVariableReferenceExpression(string name)
        {
            Name = new AlgorithmIdentifier(name);
        }

        /// <summary>
        /// Initialize a new instance of <see cref="AlgorithmVariableReferenceExpression"/>
        /// </summary>
        /// <param name="variable"></param>
        public AlgorithmVariableReferenceExpression(AlgorithmVariableDeclaration variable)
        {
            Name = variable.Name;
        }

        #endregion

        #region Methods
        
        /// <summary>
        /// Gets a string representation of the reference
        /// </summary>
        /// <returns>String that reprensents the reference</returns>
        public override string ToString()
        {
            return Name.Identifier;
        }

        #endregion
    }
}
