using Newtonsoft.Json;

namespace Algo.Runtime.Build.AlgorithmDOM.DOM
{
    /// <summary>
    /// Represents the entry point method of a program
    /// </summary>
    public class AlgorithmEntryPointMethod : AlgorithmClassMethodDeclaration
    {
        #region Properties

        /// <summary>
        /// Gets a <see cref="AlgorithmDomType"/> used to identify the object without reflection
        /// </summary>
        internal override AlgorithmDomType DomType => AlgorithmDomType.EntryPointMethod;

        /// <summary>
        /// Gets or sets if the method is asynchronous or not.
        /// </summary>
        [JsonProperty]
        public new static bool IsAsync { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initialize a new instance of <see cref="AlgorithmEntryPointMethod"/>
        /// </summary>
        public AlgorithmEntryPointMethod()
            : base("Main", false)
        {
        }

        #endregion
    }
}
