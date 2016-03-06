namespace Algo.Runtime.Build.AlgorithmDOM.DOM
{
    /// <summary>
    /// Represents a member in a class
    /// </summary>
    public abstract class AlgorithmClassMember : AlgorithmObject
    {
        #region Properties

        /// <summary>
        /// Gets or sets the name of the member.
        /// </summary>
        public AlgorithmIdentifier Name { get { return _name; } set { _name = value; } }

        #endregion

        #region Constructors

        /// <summary>
        /// Initialize a new instance of <see cref="AlgorithmClassMember"/>
        /// </summary>
        /// <param name="name">The name of the member</param>
        public AlgorithmClassMember(string name)
        {
            Name = new AlgorithmIdentifier(name);
        }

        #endregion
    }
}
