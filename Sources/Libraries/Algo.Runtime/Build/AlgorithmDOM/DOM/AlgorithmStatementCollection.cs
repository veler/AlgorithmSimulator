using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Algo.Runtime.Build.AlgorithmDOM.DOM
{
    /// <summary>
    /// Represents a collection of statements
    /// </summary>
    public class AlgorithmStatementCollection : Collection<AlgorithmStatement>
    {
        #region Methods

        /// <summary>
        /// Add a range of <see cref="AlgorithmStatement"/> item to the collection
        /// </summary>
        /// <param name="items">The items to add to the collection</param>
        public void AddRange(IEnumerable<AlgorithmStatement> items)
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
