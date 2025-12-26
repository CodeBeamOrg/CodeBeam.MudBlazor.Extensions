using System.ComponentModel;

namespace MudExtensions
{
#pragma warning disable CS1591
    public enum AnimationDirection
    {
        [Description("normal")]
        Normal,
        [Description("reverse")]
        Reverse,
        [Description("alternate")]
        Alternate,
        [Description("alternate-reverse")]
        AlternateReverse,
    }
}
