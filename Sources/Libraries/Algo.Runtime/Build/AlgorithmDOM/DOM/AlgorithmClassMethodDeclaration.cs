namespace Algo.Runtime.Build.AlgorithmDOM.DOM
{
    /// <summary>
    /// Represents a method declaration in a class in an algorithm
    /// </summary>
    public class AlgorithmClassMethodDeclaration : AlgorithmClassMember
    {
        #region Properties

        internal override AlgorithmDomType DomType => AlgorithmDomType.ClassMethodDeclaration;

        /// <summary>
        /// Gets of sets the statements in the method's body
        /// </summary>
        public AlgorithmStatementCollection Statements { get { return _statements; } set { _statements = value; } }

        /// <summary>
        /// Gets or sets a collection of arguments declaration
        /// </summary>
        public AlgorithmParameterDeclarationCollection Arguments { get { return _arguments; } set { _arguments = value; } }

        /// <summary>
        /// Gets or sets whether the method can be call asynchronously
        /// </summary>
        public bool IsAsync { get { return _isAsync; } set { _isAsync = value; } }

        #endregion

        #region Constructors

        /// <summary>
        /// Initialize a new instance of <see cref="AlgorithmClassMethodDeclaration"/>
        /// </summary>
        /// <param name="name">The name of the method</param>
        /// <param name="isAsync">Defines whether the method can be call asynchronously</param>
        /// <param name="arguments">The arguments declaration for this method</param>
        public AlgorithmClassMethodDeclaration(string name, bool isAsync, params AlgorithmParameterDeclaration[] arguments)
            : base(name)
        {
            Statements = new AlgorithmStatementCollection();
            IsAsync = isAsync;
            Arguments = new AlgorithmParameterDeclarationCollection();
            Arguments.AddRange(arguments);
        }

        #endregion
    }
}
