using System.Collections.ObjectModel;
using Algo.Runtime.Build.AlgorithmDOM.DOM;

namespace Algo.Runtime.Build.AlgorithmDOM
{
    interface IAlgorithmVariable
    {
        /// <summary>
        /// Gets or sets the name of the variable 
        /// </summary>         
        AlgorithmIdentifier Name { get; set; }

        /// <summary>
        /// Gets or sets whether the variable is of type <see cref="object"/> or <see cref="Collection{T}"/> of <see cref="object"/>
        /// </summary>
        bool IsArray { get; set; }

        /// <summary>
        /// Gets of sets the default value of the variable
        /// </summary>
        AlgorithmPrimitiveExpression DefaultValue { get; set; }
    }
}
