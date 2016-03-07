using System;
using System.Linq;
using System.Reflection;

namespace Algo.Runtime.ComponentModel
{
    internal static class EnumExtension
    {
        #region Methods

        public static string GetDescription<T>(this T enumerationValue) where T : struct
        {
            var memberInfo = enumerationValue.GetType().GetMember(enumerationValue.ToString());
            if (memberInfo != null && memberInfo.Length > 0)
            {
                var attrs = memberInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false).ToArray();

                if (attrs != null && attrs.Length > 0)
                {
                    return ((DescriptionAttribute)attrs[0]).Description;
                }
            }
            return enumerationValue.ToString();
        }

        #endregion
    }

    [AttributeUsage(AttributeTargets.Field)]
    internal class DescriptionAttribute : Attribute
    {
        public string Description { get; set; }

        public DescriptionAttribute(string description)
        {
            Description = description;
        }
    }
}
