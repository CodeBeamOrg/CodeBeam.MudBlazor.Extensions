using System.ComponentModel;

namespace MudExtensions.Enums
{
    public enum AnimationType
    {
        [Description("blur")]
        Blur,
        [Description("fade")]
        Fade,
        [Description("flip")]
        Flip,
        [Description("rotate")]
        Rotate,
        [Description("rotate-diagonal")]
        RotateDiagonal,
        [Description("scale")]
        Scale,
        [Description("shadow")]
        Shadow,
        [Description("shadow-inset")]
        ShadowInset,
        [Description("slide-x")]
        SlideX,
        [Description("slide-y")]
        SlideY,
        [Description("text-shadow")]
        TextShadow,
        [Description("text-spacing")]
        TextSpacing,
    }
}
