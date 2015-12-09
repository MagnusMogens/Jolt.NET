using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace Jolt.NET.Helper
{
    public static class EnumUtils
    {
        /// <summary>
        /// Parses the enum description.
        /// </summary>
        /// <typeparam name="T">The parsing enum type.</typeparam>
        /// <exception cref="InvalidOperationException">Is thrown whenever the given type is no enum type.</exception>
        public static T ParseEnumDescription<T>(this string description, StringComparison comparisonType = StringComparison.OrdinalIgnoreCase)
        {
            var type = typeof(T);
            if (!type.GetTypeInfo().IsEnum) throw new InvalidOperationException();

            foreach (var field in type.GetFields())
            {
                var attribute = field.GetCustomAttributes(typeof(DisplayAttribute), false)
                                     .SingleOrDefault() as DisplayAttribute;

                if (attribute != null)
                {
                    if (attribute.GetDescription().Equals(description))
                        return (T)field.GetValue(null);
                }
                else
                {
                    if (field.Name == description)
                        return (T)field.GetValue(null);
                }
            }

            return default(T);
        }
        
        public static string GetDescription(this Enum value)
        {
            if (value == null)
                throw new ArgumentException("Parameter value has to be provided");

            var attribute = value.GetType()
                                 .GetField(value.ToString())
                                 .GetCustomAttributes(typeof(DisplayAttribute), false)
                                 .SingleOrDefault() as DisplayAttribute;

            return attribute?.GetDescription() ?? value.ToString();
        }
    }
}
