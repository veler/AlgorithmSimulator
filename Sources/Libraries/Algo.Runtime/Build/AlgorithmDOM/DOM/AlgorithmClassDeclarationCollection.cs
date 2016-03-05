using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Algo.Runtime.Build.AlgorithmDOM.DOM
{
    /// <summary>
    /// Represents a collection of class declaration in an algorithm
    /// </summary>
    public class AlgorithmClassDeclarationCollection : Collection<AlgorithmClassDeclaration>
    {
        #region Methods

        /// <summary>
        /// Add a range of <see cref="AlgorithmClassDeclaration"/> item to the collection
        /// </summary>
        /// <param name="items">The items to add to the collection</param>
        public void AddRange(IEnumerable<AlgorithmClassDeclaration> items)
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
