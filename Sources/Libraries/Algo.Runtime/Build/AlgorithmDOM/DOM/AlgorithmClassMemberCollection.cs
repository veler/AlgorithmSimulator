using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Algo.Runtime.Build.AlgorithmDOM.DOM
{
    /// <summary>
    /// Represents a collection of class member in an algorithm
    /// </summary>
    public class AlgorithmClassMemberCollection : Collection<AlgorithmClassMember>
    {
        #region Methods

        /// <summary>
        /// Add a range of <see cref="AlgorithmClassMember"/> item to the collection
        /// </summary>
        /// <param name="items">The items to add to the collection</param>
        public void AddRange(IEnumerable<AlgorithmClassMember> items)
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
