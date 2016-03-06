namespace Algo.Runtime.Build.AlgorithmDOM
{
    /// <summary>
    /// Provide a sets of methods used to check if some rules are respected in an algorithm made with AlgorithmDOM
    /// </summary>
    internal sealed class Utils
    {
        #region Methods

        /// <summary>
        /// Verify if a namespace is valid for a basic AlgorithmDOM
        /// </summary>
        /// <param name="identifier">The namespace to check</param>
        /// <returns>Returns false if the namespace contains any white spaces.</returns>
        internal static bool IsValidNamespace(string identifier)
        {
            return !identifier.Contains(" ");
        }

        #endregion
    }
}
