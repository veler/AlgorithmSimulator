namespace PortableSimulator.Build.AlgorithmDOM.DOM
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    class AlgorithmStatementCollection : Collection<AlgorithmStatement>
    {
        #region Methods

        public void AddRange(IEnumerable<AlgorithmStatement> items)
        {
            foreach (var item in items)
                this.Add(item);
        }

        #endregion
    }
}
