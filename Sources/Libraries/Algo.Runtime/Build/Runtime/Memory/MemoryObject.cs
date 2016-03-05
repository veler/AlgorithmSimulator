using System;
using System.Collections;
using System.Reflection;
using System.Text;
using Algo.Runtime.Build.Runtime.Debugger;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Algo.Runtime.Build.Runtime.Memory
{
    /// <summary>
    /// Provide a representation of an object in the memory and the size of this object.
    /// </summary>
    public abstract class MemoryObject : MemoryTraceObject
    {
        #region Fields

        private object _value;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the size of the value in the memory
        /// </summary>    
        [JsonProperty]
        private ulong Size { get; set; }

        /// <summary>
        /// Gets or sets the value in the memory
        /// </summary> 
        public object Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
                if (MemTrace)
                {
                    UpdateSizeAsync();
                }
            }
        }

        #endregion

        #region Constructors

        /// <summary>   
        /// Initialize a new instance of <see cref="MemoryObject"/>
        /// </summary>
        internal MemoryObject()
            : this(false, null)
        {
        }

        /// <summary>
        /// Initialize a new instance of <see cref="MemoryObject"/>
        /// </summary>
        /// <param name="memTrace">Defines whether the size of the value should be determinated each time the value change</param>
        /// <param name="value">Sets a value</param>
        internal MemoryObject(bool memTrace, object value = null)
            : base(memTrace)
        {
            Value = value;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Get the defined size of a <see cref="Type"/>
        /// </summary>
        /// <param name="type">The Type</param>
        /// <returns>Returns a <see cref="uint"/> corresponding to the size of the type</returns>
        private uint SizeOf(Type type)
        {
            var method = typeof(Earlz.BareMetal.BareMetal).GetMethod("SizeOf");
            return Convert.ToUInt32((int)(method.MakeGenericMethod(type).Invoke(null, null)));
        }

        /// <summary>
        /// Calculate the number of bytes used by an <see cref="object"/>
        /// </summary>
        /// <param name="value">The object we want to calculate the number of bytes</param>
        /// <returns>Returns a <see cref="ulong"/> corresponding to the number of bytes</returns>
        private ulong SizeOf(object value)
        {
            if (value == null)
            {
                return 0;
            }

            if (value is string)
            {
                return Convert.ToUInt64(Encoding.UTF8.GetByteCount(value.ToString()));
            }

            var valueType = value.GetType();
            ulong size = SizeOf(valueType);

            try
            {
                var items = value as IEnumerable;
                if (items != null)
                {
                    Type genericType;
                    var arraySize = 0;
                    var sizeProperty = valueType.GetProperty("Count") ?? valueType.GetProperty("Length");

                    if (valueType.IsArray)
                    {
                        genericType = valueType.GetElementType();
                    }
                    else
                    {
                        var genericArgs = valueType.GetGenericArguments();
                        if (genericArgs.Length > 0)
                        {
                            genericType = valueType.GetGenericArguments()[0];
                        }
                        else
                        {
                            var baseType = valueType.GetTypeInfo().BaseType;
                            if (baseType != null)
                            {
                                genericArgs = baseType.GetGenericArguments();
                                if (genericArgs.Length > 0)
                                {
                                    genericType = baseType.GetGenericArguments()[0];
                                }
                                else
                                {
                                    genericType = typeof(object);
#if DEBUG
                                    //  throw new Exception("Must not go there. Try to fix that.");
#endif
                                }
                            }
                            else
                            {
                                genericType = typeof(object);
#if DEBUG
                                //   throw new Exception("Must not go there. Try to fix that.");
#endif
                            }
                        }
                    }

                    if (sizeProperty != null)
                    {
                        arraySize = (int)sizeProperty.GetValue(items);
                    }

                    if (genericType != typeof(object))
                    {
                        return Convert.ToUInt64(arraySize * SizeOf(genericType)) + SizeOf(valueType);
                    }

                    foreach (var item in items)
                    {
                        size += SizeOf(item);
                    }
                    return size;
                }

                if (valueType.GetTypeInfo().IsValueType)
                {
                    return size;
                }

                const BindingFlags flags =
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
                foreach (var property in valueType.GetFields(flags))
                {
                    if (property.DeclaringType != typeof(Delegate))
                    {
                        size += SizeOf(property.GetValue(value));
                    }
                }
            }
            catch
            {

            }

            return size;
        }

        /// <summary>
        /// Determines the size of the value in the memory in bytes
        /// </summary>
        /// <returns>A <see cref="Task"/></returns>
        private Task UpdateSizeAsync()
        {
            return Task.Run(() =>
            {
                if (Value == null)
                {
                    Size = 0;
                }
                else
                {
                    Size = SizeOf(Value);
                }
            });
        }

        /// <summary>
        /// Gets the current size of the value in memory in bytes
        /// </summary>
        /// <returns>Returns the size of the value</returns>
        public async Task<ulong> GetSizeAsync()
        {
            if (!MemTrace)
            {
                await UpdateSizeAsync();
            }
            return Size;
        }

        #endregion
    }
}
