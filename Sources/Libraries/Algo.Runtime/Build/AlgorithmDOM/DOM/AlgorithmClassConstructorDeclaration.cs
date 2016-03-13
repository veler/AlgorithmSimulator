using Newtonsoft.Json;

namespace Algo.Runtime.Build.AlgorithmDOM.DOM
{
    /// <summary>
    /// Represents a method that will be run when the parent class is instanciate ti initialize the class.
    /// </summary>
    public class AlgorithmClassConstructorDeclaration : AlgorithmClassMethodDeclaration
    {
        #region Properties
        /// <summary>
        /// Gets a <see cref="AlgorithmDomType"/> used to identify the object without reflection
        /// </summary>
        internal override AlgorithmDomType DomType => AlgorithmDomType.ClassConstructorDeclaration;

        /// <summary>
        /// Gets or sets if the method is asynchronous or not.
        /// </summary>
        [JsonProperty]
        public new static bool IsAsync { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initialize a new instance of <see cref="AlgorithmClassConstructorDeclaration"/>
        /// </summary>
        public AlgorithmClassConstructorDeclaration(params AlgorithmParameterDeclaration[] arguments)
            : base("ctor", false, arguments)
        {
        }

        #endregion
    }
}
