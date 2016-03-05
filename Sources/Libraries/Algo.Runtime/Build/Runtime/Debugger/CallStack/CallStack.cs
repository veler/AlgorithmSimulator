using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Algo.Runtime.Build.Runtime.Debugger.CallStack
{
    public class CallStack : Collection<Call>
    {
        #region Methods

        /// <summary>
        /// Add a range of <see cref="Call"/> item to the collection
        /// </summary>
        /// <param name="items">The items to add to the collection</param>
        public void AddRange(IEnumerable<Call> items)
        {
            foreach (var item in items)
            {
                Add(item);
            }
        }

        #endregion
    }
}
