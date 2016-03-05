using System;

namespace Algo.Runtime.Build.AlgorithmDOM.DOM
{
    /// <summary>
    /// Represents an identifier
    /// </summary>
    public class AlgorithmIdentifier : AlgorithmObject
    {
        #region Fields

        private string _identifier;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the identifier
        /// </summary>         
        public string Identifier
        {
            get { return _identifier; }
            set
            {
                if (!IsValidIdentifier(value))
                {
                    throw new ArgumentException("An identifier cannot be null or only containing white spaces.");
                }
                _identifier = value;
            }
        }

        #endregion

        #region Contructors

        /// <summary>
        /// Initialize a new instance of <see cref="AlgorithmIdentifier"/>
        /// </summary>
        /// <param name="identifier">The identifier</param>
        public AlgorithmIdentifier(string identifier)
        {
            Identifier = identifier;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Verify if an identifier is valid for a basic AlgorithmDOM
        /// </summary>
        /// <param name="identifier">The identifier to check</param>
        /// <returns>Returns false if the identifier is null of contains only white spaces.</returns>
        public bool IsValidIdentifier(string identifier)
        {
            return !String.IsNullOrWhiteSpace(identifier);
        }

        /// <summary>
        /// Gets a string representation of the object 
        /// </summary>
        /// <returns>Returns the identifier</returns>
        public override string ToString()
        {
            return _identifier;
        }

        #endregion
    }
}
