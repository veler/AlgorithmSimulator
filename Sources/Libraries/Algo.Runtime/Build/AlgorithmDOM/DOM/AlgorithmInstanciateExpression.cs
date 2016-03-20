using Newtonsoft.Json;

namespace Algo.Runtime.Build.AlgorithmDOM.DOM
{
    /// <summary>
    /// Represents an object creation, typically represtented as a new expression. If the goal is to create an object from the CLR, please use <see cref="AlgorithmInstanciateCoreExpression"/>
    /// </summary>
    public class AlgorithmInstanciateExpression : AlgorithmExpression
    {
        #region Properties

        /// <summary>
        /// Gets a <see cref="AlgorithmDomType"/> used to identify the object without reflection
        /// </summary>
        internal override AlgorithmDomType DomType => AlgorithmDomType.InstanciateExpression;

        /// <summary>
        /// Gets or sets a reference to the class to instanciate
        /// </summary>
        [JsonProperty]
        public AlgorithmClassReferenceExpression CreateType { get { return _createType; } set { _createType = value; } }

        /// <summary>
        /// Gets or sets the arguments to pass in the class's constructor
        /// </summary>
        [JsonProperty]
        public AlgorithmExpressionCollection Arguments { get { return _argumentsExpression; } set { _argumentsExpression = value; } }

        #endregion

        #region Constructors

        /// <summary>
        /// Initialize a new instance of <see cref="AlgorithmInstanciateExpression"/>
        /// </summary>
        public AlgorithmInstanciateExpression()
        {  
        }

        /// <summary>
        /// Initialize a new instance of <see cref="AlgorithmInstanciateExpression"/>
        /// </summary>
        /// <param name="createType">Reference to the class to instanciate</param>
        /// <param name="arguments">Arguments to pass in the class's constructor</param>
        public AlgorithmInstanciateExpression(AlgorithmClassReferenceExpression createType, params AlgorithmExpression[] arguments)
        {
            CreateType = createType;
            Arguments = new AlgorithmExpressionCollection();
            Arguments.AddRange(arguments);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets a string representation of the reference
        /// </summary>
        /// <returns>String that reprensents the reference</returns>
        public override string ToString()
        {
            return $"new {CreateType}()";
        }

        #endregion
    }
}
