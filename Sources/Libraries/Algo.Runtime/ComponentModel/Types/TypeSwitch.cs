using System;
using System.Reflection;

namespace Algo.Runtime.ComponentModel.Types
{
    /// <summary>
    /// Provide a way to perform a switch on object's type
    /// </summary>
    internal static class TypeSwitch
    {
        #region Methods

        /// <summary>
        /// Perform a switch on an object's type
        /// </summary>
        /// <param name="source">The object we try to check the type</param>
        /// <param name="cases">The cases of the switch, represented by an array of <see cref="CaseInfo"/> values</param>
        internal static void Switch(object source, params CaseInfo[] cases)
        {
            var type = source.GetType().GetTypeInfo();
            foreach (var entry in cases)
            {
                if (entry.IsDefault || entry.Target.GetTypeInfo().IsAssignableFrom(type))
                {
                    entry.Action(source);
                    break;
                }
            }
        }

        /// <summary>
        /// Perform a case on a type switch
        /// </summary>
        /// <typeparam name="T">Define the expected type for this switch</typeparam>
        /// <param name="action">The <see cref="Action"/> to perform when the T type match</param>
        /// <returns>Returns a <see cref="CaseInfo"/> that represents a case of the switch, with the test type and the action to perform.</returns>
        internal static CaseInfo Case<T>(Action action)
        {
            return new CaseInfo() { Action = x => action(), Target = typeof(T) };
        }

        /// <summary>
        /// Perform a case on a type switch
        /// </summary>
        /// <typeparam name="T">Define the expected type for this switch</typeparam>
        /// <param name="action">The <see cref="Action"/> to perform when the T type match</param>
        /// <returns>Returns a <see cref="CaseInfo"/> that represents a case of the switch, with the test type and the action to perform.</returns>
        internal static CaseInfo Case<T>(Action<T> action)
        {
            return new CaseInfo() { Action = x => action((T)x), Target = typeof(T) };
        }

        /// <summary>
        /// Perform a default case on a type switch
        /// </summary>
        /// <param name="action">The <see cref="Action"/> to perform when the default case run</param>
        /// <returns>Returns a <see cref="CaseInfo"/> that represents a case of the switch, with the action to perform when it's a default switch case.</returns>
        internal static CaseInfo Default(Action action)
        {
            return new CaseInfo() { Action = x => action(), IsDefault = true };
        }

        #endregion   
    }
}
