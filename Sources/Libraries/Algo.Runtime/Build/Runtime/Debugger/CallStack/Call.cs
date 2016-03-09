using System.Collections.ObjectModel;
using Algo.Runtime.Build.AlgorithmDOM.DOM;
using Algo.Runtime.Build.Runtime.Memory;
using Newtonsoft.Json;

namespace Algo.Runtime.Build.Runtime.Debugger.CallStack
{
    public sealed class Call
    {
        #region Properties

        /// <summary>
        /// Gets or sets the <see cref="AlgorithmClassReferenceExpression"/>
        /// </summary>   
        [JsonProperty]
        public AlgorithmClassReferenceExpression ClassReference { get; private set; }

        /// <summary>
        /// Gets or sets the <see cref="AlgorithmInvokeMethodExpression"/>
        /// </summary>   
        [JsonProperty]
        public AlgorithmInvokeMethodExpression InvokeMethodExpression { get; private set; }

        /// <summary>
        /// Gets or sets the list of accessible variables
        /// </summary>   
        [JsonProperty]
        public ReadOnlyCollection<Variable> Variables { get; internal set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initialize a new instance of <see cref="Call"/>
        /// </summary>
        /// <param name="classReference">The reference to the class</param>
        /// <param name="invokeMethodExpression">The reference to the called method</param>
        internal Call(AlgorithmClassReferenceExpression classReference, AlgorithmInvokeMethodExpression invokeMethodExpression)
        {
            ClassReference = classReference;
            InvokeMethodExpression = invokeMethodExpression;
        }

        #endregion
    }
}
