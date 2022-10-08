using System.ComponentModel;

namespace MudExtensions
{
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
