using System;

namespace Algo.Runtime.Build.AlgorithmDOM.DOM
{
    /// <summary>
    /// Represents a call to method from the CLR (i.e : System.IO.File.ReadAllText). If the goal is to call a mathod represented by AlgorithmDOM, please use <see cref="AlgorithmInvokeMethodExpression"/>
    /// </summary>
    public class AlgorithmInvokeCoreMethodExpression : AlgorithmInvokeMethodExpression
    {
        #region Fields

        private AlgorithmReferenceExpression _targetObject;

        #endregion

        #region Properties

        public override sealed AlgorithmReferenceExpression TargetObect
        {
            get
            {
                return _targetObject;

            }
            set
            {
                if (value is AlgorithmThisReferenceExpression)
                {
                    throw new ArgumentException("Unable to invoke a Core method from This which is not a Core class.", nameof(value));
                }
                _targetObject = value;
            }
        }

        /// <summary>
        /// Gets or sets an array that defines the type of each arguments of the method
        /// </summary>
        public Type[] ArgumentsTypes { get; set; }

        #endregion

        #region Constructors     

        /// <summary>
        /// Initialize a new instance of <see cref="AlgorithmInvokeCoreMethodExpression"/>
        /// </summary>
        public AlgorithmInvokeCoreMethodExpression()
        {
        }

        /// <summary>
        /// Initialize a new instance of <see cref="AlgorithmInvokeCoreMethodExpression"/>
        /// </summary>
        /// <param name="targetObject">A reference to a variable or to a class</param>
        /// <param name="methodName">The method name to call</param>
        /// <param name="argumentTypes">The array of types that defines the type of each arguments of the method</param>
        /// <param name="arguments">The arguments to pass during the call</param>
        public AlgorithmInvokeCoreMethodExpression(AlgorithmReferenceExpression targetObject, string methodName, Type[] argumentTypes, params AlgorithmExpression[] arguments)
            : base(targetObject, methodName, arguments)
        {
            TargetObect = targetObject;
            ArgumentsTypes = argumentTypes;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets a string representation of the reference
        /// </summary>
        /// <returns>String that reprensents the reference</returns>
        public override string ToString()
        {
            return $"{TargetObect}.{MethodName}()";
        }

        #endregion
    }
}
