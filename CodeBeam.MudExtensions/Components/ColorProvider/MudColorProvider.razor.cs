using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudBlazor;

namespace MudExtensions
{
    public partial class MudColorProvider : MudComponentBase
    {

        public string GetRGBString(string hex, int percentage = 40)
        {
            if (string.IsNullOrEmpty(hex) || hex.Length < 6)
            {
                return null;
            }

            if (hex.StartsWith("#"))
            {
                hex = hex.Substring(1);
            }

            string r = hex.Substring(0, 2);
            string g = hex.Substring(2, 2);
            string b = hex.Substring(4, 2);

            return $"rgb({HexToRgb(r, percentage)}, {HexToRgb(g, percentage)}, {HexToRgb(b, percentage)})";
        }

        protected int HexToRgb(string s, int percentage = 40)
        {
            return ConvertRGBTone(SingleHexValue(s[0].ToString()) * 16 + SingleHexValue(s[1].ToString()), percentage);
        }

        protected int ConvertRGBTone(int val, double percentage)
        {
            if (percentage == 40)
            {
                return val;
            }
            else if (percentage < 40)
            {
                //double standardDecrement = 25.5d * ((40d - percentage) / 10d);
                //double deviationDecrement = (val / 6d) - 25.5d;
                //int semiVal = (int)Math.Round(val - standardDecrement - (deviationDecrement * ((percentage - 40d) / 10)));
                //if (semiVal < 0)
                //{
                //    semiVal = 0;
                //}
                //return semiVal;
                double processTime = (40 - percentage) / 10;
                return (int)(val - val * 0.15 * processTime);
            }
            else
            {
                //double standardIncrement = 25.5d * ((percentage - 40d) / 10d);
                //if (255 < val + standardIncrement)
                //{
                //    return 255;
                //}

                //double deviationIncrement = ((255d - val) / 6d) - 25.5d;
                //int semiVal = (int)Math.Round(val + standardIncrement + (deviationIncrement * ((percentage - 40d) / 10)));

                //if (255 < semiVal)
                //{
                //    semiVal = 255;
                //}
                //return semiVal;

                double processTime = (percentage - 40) / 10;
                return (int)(val + (255 -  val) * 0.15 * processTime);
            }
        }

        protected int SingleHexValue(string s)
        {
            switch (s)
            {
                case "0":
                    return 0;
                case "1":
                    return 1;
                case "2":
                    return 2;
                case "3":
                    return 3;
                case "4":
                    return 4;
                case "5":
                    return 5;
                case "6":
                    return 6;
                case "7":
                    return 7;
                case "8":
                    return 8;
                case "9":
                    return 9;
                case "a":
                case "A":
                    return 10;
                case "b":
                case "B":
                    return 11;
                case "c":
                case "C":
                    return 12;
                case "d":
                case "D":
                    return 13;
                case "e":
                case "E":
                    return 14;
                case "f":
                case "F":
                    return 15;
                case "g":
                case "G":
                    return 16;
                default:
                    return 0;
            }
        }

    }
}
