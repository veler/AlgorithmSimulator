using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Algo.Runtime.Build.AlgorithmDOM.DOM
{
    /// <summary>
    /// Represents a collection of expressions
    /// </summary>
    public class AlgorithmExpressionCollection : Collection<AlgorithmExpression>
    {
        #region Methods

        /// <summary>
        /// Add a range of <see cref="AlgorithmExpression"/> item to the collection
        /// </summary>
        /// <param name="items">The items to add to the collection</param>
        public void AddRange(IEnumerable<AlgorithmExpression> items)
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
