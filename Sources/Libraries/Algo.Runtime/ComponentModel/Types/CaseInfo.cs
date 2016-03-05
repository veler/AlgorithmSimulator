using System;

namespace Algo.Runtime.ComponentModel.Types
{
    /// <summary>
    /// Represents a case in a <see cref="TypeSwitch"/>
    /// </summary>
    sealed internal class CaseInfo
    {
        #region Properties   

        /// <summary>
        /// Gets or sets whether the case is a default case or not
        /// </summary>
        internal bool IsDefault { get; set; }

        /// <summary>
        /// If <see cref="IsDefault"/> is <value>False</value>, gets or sets the targeted type of the type switch case
        /// </summary>
        internal Type Target { get; set; }

        /// <summary>
        /// Gets or sets the action to perform when the case match
        /// </summary>
        internal Action<object> Action { get; set; }

        #endregion
    }
}
