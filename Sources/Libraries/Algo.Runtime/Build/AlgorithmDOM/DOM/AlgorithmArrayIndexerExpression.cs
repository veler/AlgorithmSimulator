using Newtonsoft.Json;

namespace Algo.Runtime.Build.AlgorithmDOM.DOM
{
    /// <summary>
    /// Represents a reference to an index of an array.
    /// </summary>
    public class AlgorithmArrayIndexerExpression : AlgorithmReferenceExpression, IAlgorithmAssignable
    {
        #region Properties

        /// <summary>
        /// Gets a <see cref="AlgorithmDomType"/> used to identify the object without reflection
        /// </summary>
        internal override AlgorithmDomType DomType => AlgorithmDomType.ArrayIndexerExpression;

        /// <summary>
        /// Gets or sets the reference to the array
        /// </summary>
        [JsonProperty]
        public AlgorithmReferenceExpression TargetObject { get { return _targetObject; } set { _targetObject = value; } }

        /// <summary>
        /// Gets or sets the index of the array.
        /// </summary>
        [JsonProperty]
        public AlgorithmExpression Indice { get { return _indice; } set { _indice = value; } }

        #endregion

        #region Constuctors

        /// <summary>
        /// Initialize a new instance of <see cref="AlgorithmArrayIndexerExpression"/>
        /// </summary>
        public AlgorithmArrayIndexerExpression()
        {
        }

        /// <summary>
        /// Initialize a new instance of <see cref="AlgorithmArrayIndexerExpression"/>
        /// </summary>
        /// <param name="targetObject">the reference to the array</param>
        public AlgorithmArrayIndexerExpression(AlgorithmReferenceExpression targetObject)
        {
            TargetObject = targetObject;
        }

        /// <summary>
        /// Initialize a new instance of <see cref="AlgorithmArrayIndexerExpression"/>
        /// </summary>
        /// <param name="targetObject">the reference to the array</param>
        /// <param name="indice">the index of the array.</param>
        public AlgorithmArrayIndexerExpression(AlgorithmReferenceExpression targetObject, AlgorithmExpression indice)
        {
            TargetObject = targetObject;
            Indice = indice;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets a string representation of the reference
        /// </summary>
        /// <returns>String that reprensents the reference</returns>
        public override string ToString()
        {
            return $"{TargetObject}[{Indice}]";
        }

        #endregion
    }
}
