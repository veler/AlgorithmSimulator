using Newtonsoft.Json;

namespace Algo.Runtime.ComponentModel
{
    /// <summary>
    /// Provide some extension methods for an <see cref="object"/>
    /// </summary>
    internal static class ObjectExtension
    {
        #region Methods

        /// <summary>
        /// Perform a deep Copy of the object, using Json as a serialisation method
        /// </summary>
        /// <typeparam name="T">The type of object being copied</typeparam>
        /// <param name="source">The object instance to copy</param>
        /// <returns>The copied object</returns>
        internal static T DeepClone<T>(this T source)
        {
            if (object.ReferenceEquals(source, null))
            {
                return default(T);
            }
            
            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(source));
        }

        #endregion
    }
}
