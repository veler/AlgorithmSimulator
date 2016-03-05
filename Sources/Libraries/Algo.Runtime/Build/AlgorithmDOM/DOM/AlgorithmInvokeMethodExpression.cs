namespace Algo.Runtime.Build.AlgorithmDOM.DOM
{
    /// <summary>
    /// Represents a call to a method in an algorithm. If the goal is to call a method from the CLR, please use <see cref="AlgorithmInvokeCoreMethodExpression"/>
    /// </summary>
    public class AlgorithmInvokeMethodExpression : AlgorithmExpression
    {
        #region Properties

        /// <summary>
        /// Gets or sets the class reference that contains the method
        /// </summary>
        public virtual AlgorithmReferenceExpression TargetObect { get; set; }

        /// <summary>
        /// Gets or sets the name of the method to call
        /// </summary>
        public AlgorithmIdentifier MethodName { get; set; }

        /// <summary>
        /// Gets or sets an array of arguments to pass to the call
        /// </summary>
        public AlgorithmExpressionCollection Arguments { get; set; }

        /// <summary>
        /// Gets or sets whether a call to a asynchronous method should be done synchronously or not
        /// </summary>
        public bool Await { get; set; }

        #endregion

        #region Consturctors

        /// <summary>
        /// Initialize a new instance of <see cref="AlgorithmInvokeMethodExpression"/>.
        /// </summary>
        public AlgorithmInvokeMethodExpression()
        {
        }

        /// <summary>
        /// Initialize a new instance of <see cref="AlgorithmInvokeMethodExpression"/> and set the <see cref="TargetObject"/> property to <see cref="AlgorithmThisReferenceExpression"/>
        /// </summary>                                            
        /// <param name="methodName">The method name to call</param>
        /// <param name="arguments">The arguments to pass during the call</param>
        public AlgorithmInvokeMethodExpression(string methodName, params AlgorithmExpression[] arguments)
            : this(null, methodName, arguments)
        {
            TargetObect = new AlgorithmThisReferenceExpression();
        }

        /// <summary>
        /// Initialize a new instance of <see cref="AlgorithmInvokeMethodExpression"/>.
        /// </summary>
        /// <param name="targetObject">A reference to a variable or to a class</param>
        /// <param name="methodName">The method name to call</param>
        /// <param name="arguments">The arguments to pass during the call</param>
        public AlgorithmInvokeMethodExpression(AlgorithmReferenceExpression targetObject, string methodName, params AlgorithmExpression[] arguments)
        {
            TargetObect = targetObject;
            MethodName = new AlgorithmIdentifier(methodName);
            Arguments = new AlgorithmExpressionCollection();
            Arguments.AddRange(arguments);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets a string representation of the reference
        /// </summary>
        /// <returns>String that reprensents the reference</returns>
        public override string ToString()
        {
            return $"{TargetObect}.{MethodName.Identifier}()";
        }

        #endregion
    }
}
