using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MudExtensions.Utilities
{
    /// <summary>
    /// Generates CSS animation keyframes for MudAnimate.
    /// </summary>
    public class KeyframeBuilder
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ticks"></param>
        /// <param name="values"></param>
        /// <param name="property"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static string Build(int ticks, List<string> values, string property, string defaultValue = "")
        {
            StringBuilder sb = new ();
            for (int i = 0; i < ticks; i++)
            {
                sb.Append($"{(i == 0 ? 0 : 100/i)}%");
                sb.Append($"{{ {property.Replace("-val-", values[i])} }}");
            }
            return sb.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ticks"></param>
        /// <param name="values"></param>
        /// <param name="property"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static string Build(int ticks, List<Tuple<string, string>> values, string property, string defaultValue = "")
        {
            StringBuilder sb = new();
            for (int i = 0; i < ticks; i++)
            {
                sb.Append($"{(i == 0 ? 0 : 100 / i)}%");
                sb.Append($"{{ {property.Replace("-val1-", values[i].Item1).Replace("-val2-", values[i].Item2)} }}");
            }
            return sb.ToString();
        }
    }
}
