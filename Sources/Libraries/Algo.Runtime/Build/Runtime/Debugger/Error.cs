using System;
using Algo.Runtime.Build.AlgorithmDOM.DOM;
using Newtonsoft.Json;


namespace Algo.Runtime.Build.Runtime.Debugger
{
    /// <summary>
    /// Provide information about an error during the execution of an algorithm
    /// </summary>
    public class Error
    {
        #region Properties

        /// <summary>
        /// Gets or sets the thrown exception
        /// </summary>   
        [JsonProperty]
        public Exception Exception { get; private set; }

        /// <summary>
        /// Gets or sets the ID of the <see cref="AlgorithmObject"/> that thrown the exception
        /// </summary>
        public string AlgorithmObjectId { get; set; }

        /// <summary>
        /// Gets or sets a description of the error
        /// </summary>
        public string ErrorDescription { get; set; }

        /// <summary>
        /// Gets or sets a description of a possible solution
        /// </summary>
        public string SolutionDescription { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initialize a new instance of <see cref="Error"/>
        /// </summary>
        /// <param name="exception">The exception thrown</param>
        public Error(Exception exception)
        {
            Exception = exception;
        }

        #endregion
    }
}
