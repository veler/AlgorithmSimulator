using Newtonsoft.Json;

namespace Algo.Runtime.Build.AlgorithmDOM.DOM
{
    /// <summary>
    /// Represents a reference to a variable in an algorithm
    /// </summary>
    public class AlgorithmPropertyReferenceExpression : AlgorithmReferenceExpression, IAlgorithmAssignable
    {
        #region Properties

        /// <summary>
        /// Gets a <see cref="AlgorithmDomType"/> used to identify the object without reflection
        /// </summary>
        internal override AlgorithmDomType DomType => AlgorithmDomType.PropertyReferenceExpression;

        /// <summary>
        /// Gets or sets the class reference or variable that contains the property
        /// </summary>
        [JsonProperty]
        public AlgorithmReferenceExpression TargetObect { get { return _targetObject; } set { _targetObject = value; } }

        /// <summary>
        /// Gets or sets the name of the variable
        /// </summary>
        [JsonProperty]
        public AlgorithmIdentifier PropertyName { get { return _propertyName; } set { _propertyName = value; } }

        #endregion

        #region Constuctors

        /// <summary>
        /// Initialize a new instance of <see cref="AlgorithmPropertyReferenceExpression"/>
        /// </summary>
        public AlgorithmPropertyReferenceExpression()
        {
        }

        /// <summary>
        /// Initialize a new instance of <see cref="AlgorithmPropertyReferenceExpression"/>
        /// </summary>
        /// <param name="targetObject">The class reference or variable that contains the property</param>
        /// <param name="name">The name of the variable we make reference</param>
        public AlgorithmPropertyReferenceExpression(AlgorithmReferenceExpression targetObject, string name)
        {
            TargetObect = targetObject;
            PropertyName = new AlgorithmIdentifier(name);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets a string representation of the reference
        /// </summary>
        /// <returns>String that reprensents the reference</returns>
        public override string ToString()
        {
            return $"{TargetObect}.{PropertyName.Identifier}";
        }

        #endregion
    }
}
