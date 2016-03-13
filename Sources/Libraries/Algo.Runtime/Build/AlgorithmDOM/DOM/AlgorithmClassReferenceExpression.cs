using System;

namespace Algo.Runtime.Build.AlgorithmDOM.DOM
{
    /// <summary>
    /// Represents a reference to a class in an algorithm
    /// </summary>
    public class AlgorithmClassReferenceExpression : AlgorithmReferenceExpression
    {
        #region Properties

        /// <summary>
        /// Gets a <see cref="AlgorithmDomType"/> used to identify the object without reflection
        /// </summary>
        internal override AlgorithmDomType DomType => AlgorithmDomType.ClassReferenceExpression;

        /// <summary>
        /// Gets or sets the full namespace path that contains the class
        /// </summary>
        public string Namespace
        {
            get { return _namespace; }
            set
            {
                if (!Utils.IsValidNamespace(value))
                {
                    throw new ArgumentException("The namespace cannot contains any white spaces.");
                }
                _namespace = value;
            }
        }

        /// <summary>
        /// Gets or sets the name of the class
        /// </summary>
        public AlgorithmIdentifier ClassName { get { return _className; } set { _className = value; } }

        /// <summary>
        /// Gets or sets the type that correspond to the class
        /// </summary>
        public Type Type { get { return _type; } set { _type = value; } }

        #endregion

        #region Constructors

        /// <summary>
        /// Initliaze a new instance of <see cref="AlgorithmClassReferenceExpression"/>
        /// </summary> 
        public AlgorithmClassReferenceExpression()
        {
        }

        /// <summary>
        /// Initliaze a new instance of <see cref="AlgorithmClassReferenceExpression"/>
        /// </summary>                             
        /// <param name="type">The type that correspond to the class</param>
        public AlgorithmClassReferenceExpression(Type type)
        {
            Type = type;
        }

        /// <summary>
        /// Initliaze a new instance of <see cref="AlgorithmClassReferenceExpression"/>
        /// </summary>                                                                          
        /// <param name="className">The name of the class</param>
        public AlgorithmClassReferenceExpression(string className)
            : this(string.Empty, className)
        {
        }

        /// <summary>
        /// Initliaze a new instance of <see cref="AlgorithmClassReferenceExpression"/>
        /// </summary>
        /// <param name="namespacePath">The full namespace path that contains the class</param>
        /// <param name="className">The name of the class</param>
        public AlgorithmClassReferenceExpression(string namespacePath, string className)
        {
            Namespace = namespacePath;
            ClassName = new AlgorithmIdentifier(className);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets a string representation of the object 
        /// </summary>
        /// <returns>Returns the full path to the class (namespace + class name)</returns>
        public override string ToString()
        {
            if (Type != null)
            {
                return Type.FullName;
            }
            if (string.IsNullOrWhiteSpace(Namespace))
            {
                return ClassName.ToString();
            }
            return $"{Namespace}.{ClassName}";
        }

        #endregion
    }
}
