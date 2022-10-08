using System.ComponentModel;

namespace MudExtensions
{
    public enum AnimationTiming
    {
        [Description("ease")]
        Ease,
        [Description("linear")]
        Linear,
        [Description("ease-in")]
        EaseIn,
        [Description("ease-out")]
        EaseOut,
        [Description("ease-in-out")]
        EaseInOut,
    }
}
