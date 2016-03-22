using System;
using Newtonsoft.Json;

namespace Algo.Runtime.Build.AlgorithmDOM.DOM
{
    /// <summary>
    /// Represents a program
    /// </summary>
    public class AlgorithmProgram : AlgorithmObject
    {
        #region Fields

        private new string _name;

        #endregion

        #region Properties

        /// <summary>
        /// Gets a <see cref="AlgorithmDomType"/> used to identify the object without reflection
        /// </summary>
        internal override AlgorithmDomType DomType => AlgorithmDomType.Program;

        /// <summary>
        /// Gets or sets global variables accessibles anywhere in the program's classes and methods
        /// </summary>
        [JsonProperty]
        public AlgorithmVariableDeclarationCollection Variables { get; set; }

        /// <summary>
        /// Gets or sets the classes of the program
        /// </summary>
        [JsonProperty]
        public AlgorithmClassDeclarationCollection Classes { get; set; }

        /// <summary>
        /// Gets or sets the name of the program
        /// </summary>
        [JsonProperty]
        public string Name
        {
            get { return _name; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentNullException("The name cannot be null or containing only white spaces.");
                }
                _name = value;
            }
        }

        /// <summary>
        /// Gets or sets the name of the class that contains the entry point of the program
        /// </summary>
        public string EntryPointPath { get; private set; }

        #endregion

        #region Constructors

        /// <summary>    
        /// Initialize a new instance of <see cref="AlgorithmProgram"/>
        /// </summary> 
        /// <param name="name">The name of the program.</param>
        public AlgorithmProgram(string name)
        {
            Name = name;
            Classes = new AlgorithmClassDeclarationCollection();
            Variables = new AlgorithmVariableDeclarationCollection();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the number of <see cref="AlgorithmEntryPointMethod"/> in this <see cref="AlgorithmProgram"/>
        /// </summary>
        /// <returns>Returns the number of <see cref="AlgorithmEntryPointMethod"/></returns>
        public int GetEntryPointMethodCount()
        {
            var count = 0;

            foreach (var cl in Classes)
            {
                foreach (var member in cl.Members)
                {
                    if (member is AlgorithmEntryPointMethod)
                    {
                        count++;
                    }
                }
            }

            return count;
        }

        /// <summary>
        /// Gets the current entry point in the program
        /// </summary>
        /// <returns>Returns a <see cref="AlgorithmEntryPointMethod"/> corresponding to the entry point. Returns null if the entry point has not been found.</returns>
        public AlgorithmEntryPointMethod GetEntryPointMethod()
        {
            if (string.IsNullOrEmpty(EntryPointPath))
            {
                return null;
            }

            var path = EntryPointPath.Trim();
            var i = 0;
            AlgorithmEntryPointMethod result = null;

            while (i < Classes.Count && result == null)
            {
                if (string.Compare(Classes[i].Name.ToString().Trim(), path, StringComparison.CurrentCultureIgnoreCase) == 0)
                {
                    var j = 0;
                    while (j < Classes[i].Members.Count && result == null)
                    {
                        var method = Classes[i].Members[j] as AlgorithmEntryPointMethod;
                        if (method != null)
                        {
                            result = method;
                        }
                        j++;
                    }
                }
                i++;
            }

            return result;
        }

        /// <summary>
        /// Search and update, if necessary, the entry point path. If there is not only one entry point method found, a <see cref="RankException"/> is thrown.
        /// </summary>
        public void UpdateEntryPointPath()
        {
            var entryPointCount = GetEntryPointMethodCount();
            if (entryPointCount > 1)
            {
                throw new RankException($"There are {entryPointCount} entry point methods in the program. There must be only one.");
            }

            if (entryPointCount == 0)
            {
                throw new RankException("Unable to find any entry point method in the program. There must be only one.");
            }

            if (GetEntryPointMethod() != null)
            {
                return;
            }

            var updated = false;
            var i = 0;
            int j;

            EntryPointPath = string.Empty;

            while (i < Classes.Count && !updated)
            {
                j = 0;
                while (j < Classes[i].Members.Count && !updated)
                {
                    if (Classes[i].Members[j] is AlgorithmEntryPointMethod)
                    {
                        EntryPointPath = Classes[i].Name.ToString();
                        updated = true;
                    }
                    j++;
                }
                i++;
            }
        }

        #endregion
    }
}
