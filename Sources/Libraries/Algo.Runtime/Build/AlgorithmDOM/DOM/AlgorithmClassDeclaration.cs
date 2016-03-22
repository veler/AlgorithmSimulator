using Newtonsoft.Json;

namespace Algo.Runtime.Build.AlgorithmDOM.DOM
{
    /// <summary>
    /// Represents a class declaration in an algorithm
    /// </summary>
    public class AlgorithmClassDeclaration : AlgorithmClassMember
    {
        #region Properties

        /// <summary>
        /// Gets a <see cref="AlgorithmDomType"/> used to identify the object without reflection
        /// </summary>
        internal override AlgorithmDomType DomType => AlgorithmDomType.ClassDeclaration;

        /// <summary>
        /// Gets or sets a collection of members (fields, methods...) for the class
        /// </summary>
        [JsonProperty]
        public AlgorithmClassMemberCollection Members { get { return _members; } set { _members = value; } }

        #endregion

        #region Constructors

        /// <summary>
        /// Initialize a new instance of <see cref="AlgorithmClassDeclaration"/>
        /// </summary>
        /// <param name="name">The name of the class</param>
        public AlgorithmClassDeclaration(string name)
            : base(name)
        {
            Members = new AlgorithmClassMemberCollection();
        }

        #endregion
    }
}
