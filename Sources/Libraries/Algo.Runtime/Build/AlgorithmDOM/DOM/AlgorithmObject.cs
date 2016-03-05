using System;
using Newtonsoft.Json;

namespace Algo.Runtime.Build.AlgorithmDOM.DOM
{
    /// <summary>
    /// Basic class for algorithm representation.
    /// </summary>
    public abstract class AlgorithmObject
    {
        #region Properties

        /// <summary>
        /// Gets a unique GUID to identify a part of an algorithm
        /// </summary>   
        [JsonProperty]
        public string Id { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initialize a new instance of <see cref="AlgorithmObject"/>
        /// </summary>
        protected AlgorithmObject()
        {
            Id = $"{Guid.NewGuid().ToString().ToLower()}-{Guid.NewGuid().ToString().ToLower()}";
        }

        #endregion
    }
}
