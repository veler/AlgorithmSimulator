using Newtonsoft.Json;

namespace Algo.Runtime.Build.AlgorithmDOM.DOM
{
    /// <summary>
    /// Represents a method that will be run when the parent class is instanciate ti initialize the class.
    /// </summary>
    public class AlgorithmClassConstructorDeclaration : AlgorithmClassMethodDeclaration
    {
        #region Properties

        internal override AlgorithmDomType DomType => AlgorithmDomType.ClassConstructorDeclaration;

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
