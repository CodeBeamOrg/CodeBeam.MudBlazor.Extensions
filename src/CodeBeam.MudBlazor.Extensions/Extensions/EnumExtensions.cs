using System.ComponentModel;

namespace MudExtensions
{
    /// <summary>
    /// 
    /// </summary>
    public static class EnumExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToDescriptionString(this Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            if (field is null)
            {
                return value.ToString().ToLower();
            }

            var attributes = Attribute.GetCustomAttributes(field, typeof(DescriptionAttribute), false) as DescriptionAttribute[];

            return attributes is { Length: > 0 }
                ? attributes[0].Description
                : value.ToString().ToLower();
        }

    }
}
