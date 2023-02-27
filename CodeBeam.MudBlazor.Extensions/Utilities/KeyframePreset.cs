using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MudExtensions.Utilities
{
    public static class KeyframePreset
    {
        // When making presets use "-val-" keyword to change it to real values with KeyframeBuilder.
        public static string Blur { get; set; } = "-webkit-filter: blur(-val-px); filter: blur(-val-px);";
        public static string Fade { get; set; } = "opacity: -val-;";
        public static string Flip { get; set; } = "-webkit-transform: rotateX(-val-deg); transform: rotateX(-val-deg);";
        public static string Rotate { get; set; } = "transform: rotate(-val-deg); transform: -webkit-rotate(-val-deg);";
        public static string RotateDiagonal { get; set; } = "-webkit-transform: rotate3d(1, 1, 0, -val-deg); transform: rotate3d(1, 1, 0, -val-deg);";
        public static string Scale { get; set; } = "-webkit-transform: scale(-val-); transform: scale(-val-);";
        public static string Shadow { get; set; } = "-webkit-box-shadow: 0 0 -val1-px 0 rgba(0, 0, 0, -val2-); box-shadow: 0 0 -val1-px 0 rgba(0, 0, 0, -val2-);";
        public static string ShadowInset { get; set; } = "-webkit-box-shadow: inset 0 0 -val1-px 0 rgba(0, 0, 0, -val2-); box-shadow: inset 0 0 -val1-px 0 rgba(0, 0, 0, -val2-);";
        public static string SlideX { get; set; } = "-webkit-transform: translateX(-val-px); transform: translateX(-val-px);";
        public static string SlideY { get; set; } = "-webkit-transform: translateY(-val-px); transform: translateY(-val-px);";
        public static string TextShadow { get; set; } = "text-shadow: 0 0 -val1-px rgba(0, 0, 0, -val2-);";
        public static string TextSpacing { get; set; } = "letter-spacing: -val-em;";
    }
}
