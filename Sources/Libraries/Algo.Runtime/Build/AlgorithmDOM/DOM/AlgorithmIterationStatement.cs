namespace Algo.Runtime.Build.AlgorithmDOM.DOM
{
    /// <summary>
    /// Represents an interation in an algorithm, typically represented by a for/while keyword.
    /// </summary>
    public class AlgorithmIterationStatement : AlgorithmStatement
    {
        #region Properties

        /// <summary>
        /// Gets a <see cref="AlgorithmDomType"/> used to identify the object without reflection
        /// </summary>
        internal override AlgorithmDomType DomType => AlgorithmDomType.IterationStatement;

        /// <summary>
        /// Gets or sets the statements in the iteration's body
        /// </summary>
        public AlgorithmStatementCollection Statements { get { return _statements; } set { _statements = value; } }

        /// <summary>
        /// Gets or sets the statement that initialize the iteration
        /// </summary>
        public AlgorithmStatement InitializationStatement { get { return _initializationStatement; } set { _initializationStatement = value; } }

        /// <summary>
        /// Gets or sets the statement that define the incrementation
        /// </summary>
        public AlgorithmStatement IncrementStatement { get { return _incrementStatement; } set { _incrementStatement = value; } }

        /// <summary>
        /// Gets or sets the test expression of the iteration
        /// </summary>
        public AlgorithmExpression Condition { get { return _condition; } set { _condition = value; } }

        /// <summary>
        /// Gets or sets a value that define whether the test expression will be run before of after the execution of the iteration's body
        /// </summary>
        public bool ConditionAfterBody { get { return _conditionAfterBody; } set { _conditionAfterBody = value; } }

        #endregion

        #region Constructors   

        /// <summary>
        /// Initialize a new instance of <see cref="AlgorithmIterationStatement"/>
        /// </summary>
        public AlgorithmIterationStatement()
        {  
        }

        /// <summary>
        /// Initialize a new instance of <see cref="AlgorithmIterationStatement"/>
        /// </summary>
        /// <param name="initializationStatement">The statement that initialize the iteration</param>
        /// <param name="incrementStatement">The statement that define the incrementation</param>
        /// <param name="condition">The test expression of the iteration</param>
        /// <param name="conditionAfterBody">This value defines whether the test expression will be run before of after the execution of the iteration's body</param>
        /// <param name="statements">The statements in the iteration's body</param>
        public AlgorithmIterationStatement(AlgorithmStatement initializationStatement, AlgorithmStatement incrementStatement, AlgorithmExpression condition, bool conditionAfterBody = false, AlgorithmStatementCollection statements = null)
        {      
            InitializationStatement = initializationStatement;
            IncrementStatement = incrementStatement;
            Condition = condition;
            ConditionAfterBody = conditionAfterBody;
            Statements = statements ?? new AlgorithmStatementCollection();
        }

        #endregion
    }
}
