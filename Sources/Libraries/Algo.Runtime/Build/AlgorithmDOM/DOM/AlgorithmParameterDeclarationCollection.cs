using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Algo.Runtime.Build.AlgorithmDOM.DOM
{
    /// <summary>
    /// Represents a collection of parameter declaration
    /// </summary>
    public class AlgorithmParameterDeclarationCollection : Collection<AlgorithmParameterDeclaration>
    {
        #region Methods

        /// <summary>
        /// Add a range of <see cref="AlgorithmParameterDeclaration"/> item to the collection
        /// </summary>
        /// <param name="items">The items to add to the collection</param>
        public void AddRange(IEnumerable<AlgorithmParameterDeclaration> items)
        {
            if (items == null)
            {
                return;
            }

            foreach (var item in items)
            {
                Add(item);
            }
        }

        #endregion
    }
}
