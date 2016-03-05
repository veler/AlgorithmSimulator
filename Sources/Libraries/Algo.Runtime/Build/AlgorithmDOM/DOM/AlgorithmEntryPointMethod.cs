using Newtonsoft.Json;

namespace Algo.Runtime.Build.AlgorithmDOM.DOM
{
    /// <summary>
    /// Represents the entry point method of a program
    /// </summary>
    public class AlgorithmEntryPointMethod : AlgorithmClassMethodDeclaration
    {
        #region Properties

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
