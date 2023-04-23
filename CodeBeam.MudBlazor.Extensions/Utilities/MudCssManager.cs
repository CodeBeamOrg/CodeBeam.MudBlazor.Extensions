using Microsoft.JSInterop;
using MudBlazor.Extensions;
using System.ComponentModel;

namespace MudExtensions.Utilities
{
    public class MudCssManager
    {
        private IJSRuntime JSRuntime;

        public MudCssManager(IJSRuntime jsRuntime)
        {
            JSRuntime = jsRuntime;
        }

        public async Task SetCss(string className, CssProp cssProp, string value)
        {
            if (className == null)
            {
                return;
            }
            if (className.StartsWith('.') == false)
            {
                className = "." + className;
            }

            object[] parameters = new object[] { className, cssProp.ToDescriptionString(), value };
            await JSRuntime.InvokeVoidAsync("setcss", parameters);
        }

        public async Task SetCss(string className, string cssPropName, string value)
        {
            if (className == null)
            {
                return;
            }
            if (className.StartsWith('.') == false)
            {
                className = "." + className;
            }

            object[] parameters = new object[] { className, cssPropName, value };
            await JSRuntime.InvokeVoidAsync("setcss", parameters);
        }

        public async Task<string> GetCss(string className, CssProp cssProp)
        {
            if (className == null)
            {
                return null;
            }
            if (className.StartsWith('.') == false)
            {
                className = "." + className;
            }

            object[] parameters = new object[] { className, cssProp.ToDescriptionString() };
            var result = await JSRuntime.InvokeAsync<string>("getcss", parameters);
            return result;
        }

        public async Task<string> GetCss(string className, string cssPropName)
        {
            if (className == null)
            {
                return null;
            }
            if (className.StartsWith('.') == false)
            {
                className = "." + className;
            }

            object[] parameters = new object[] { className, cssPropName };
            var result = await JSRuntime.InvokeAsync<string>("getcss", parameters);
            return result;
        }
    }

    public enum CssProp
    {
        [Description("align-content")]
        AlignContent,
        [Description("align-items")]
        AlignItems,
        [Description("align-self")]
        AlignSelf,
        [Description("animation")]
        Animation,
        [Description("animation-delay")]
        AnimationDelay,
        [Description("animation-direction")]
        AnimationDirection,
        [Description("animation-duration")]
        AnimationDuration, 
        [Description("animation-fill-mode")]
        AnimationFillMode,
        [Description("animation-iteration-count")]
        AnimationIterationCount,
        [Description("animationname")]
        AnimationName,
        [Description("animation-play-state")]
        AnimationPlayState,
        [Description("animation-timing-function")]
        AnimationTimingFunction,
        [Description("backdrop-filter")]
        BackdropFilter,
        [Description("background")]
        Background,
        [Description("background-attachment")]
        BackgroundAttachment,
        [Description("background-blend-mode")]
        BackgroundBlendMode,
        [Description("background-clip")]
        BackgroundClip,
        [Description("background-color")]
        BackgroundColor,
        [Description("background-image")]
        BackgroundImage,
        [Description("background-origin")]
        BackgroundOrigin,
        [Description("background-position")]
        BackgroundPosition,
        [Description("background-repeat")]
        BackgroundRepeat,
        [Description("background-size")]
        BackgroundSize,
        [Description("border")]
        Border,
        [Description("border-image")]
        BorderImage,
        [Description("border-left")]
        BorderLeft,
        [Description("border-right")]
        BorderRight,
        [Description("border-top")]
        BorderTop,
        [Description("border-bottom")]
        BorderBottom,
        [Description("border-radius")]
        BorderRadius,
        [Description("border-spacing")]
        BorderSpacing,
        [Description("border-style")]
        BorderStyle,
        [Description("border-width")]
        BorderWidth,
        [Description("bottom")]
        Bottom,
        [Description("box-shadow")]
        BoxShadow,
        [Description("box-sizing")]
        BoxSizing,
        [Description("caret-color")]
        CaretColor,
        [Description("clip")]
        Clip,
        [Description("color")]
        Color,
        [Description("cursor")]
        Cursor,
        [Description("direction")]
        Direction,
        [Description("display")]
        Display,
        [Description("filter")]
        Filter,
        [Description("flex")]
        Flex,
        [Description("flex-basis")]
        FlexBasis,
        [Description("flexd-irection")]
        FlexDirection,
        [Description("flex-flow")]
        FlexFlow,
        [Description("flex-grow")]
        FlexGrow,
        [Description("flex-shrink")]
        FlexShrink,
        [Description("flex-wrap")]
        FlexWrap,
        [Description("float")]
        Float,
        [Description("font")]
        Font,
        [Description("font-family")]
        FontFamily,
        [Description("font-kerning")]
        FontKerning,
        [Description("font-size")]
        FontSize,
        [Description("font-size-adjust")]
        FontSizeAdjust,
        [Description("font-stretch")]
        FontStretch,
        [Description("font-style")]
        FontStyle,
        [Description("font-variant")]
        FontVariant,
        [Description("font-weight")]
        FontWeight,
        [Description("grid")]
        Grid,
        [Description("grid-area")]
        GridArea,
        [Description("grid-column")]
        GridColumn,
        [Description("grid-row")]
        GridRow,
        [Description("grid-template")]
        GridTemplate,
        [Description("grid-template-areas")]
        GridTemplateAreas,
        [Description("grid-template-columns")]
        GridTemplateColumns,
        [Description("grid-template-rows")]
        GridTemplateRows,
        [Description("height")]
        Height,
        [Description("hyphens")]
        Hyphens,
        [Description("justify-content")]
        JustifyContent,
        [Description("left")]
        Left,
        [Description("letter-spacing")]
        LetterSpacing,
        [Description("lineheight")]
        LineHeight,
        [Description("list-style")]
        ListStyle,
        [Description("margin")]
        Margin,
        [Description("margin-bottom")]
        MarginBottom,
        [Description("margin-left")]
        MarginLeft,
        [Description("margin-right")]
        MarginRight,
        [Description("margin-top")]
        MarginTop,
        [Description("max-height")]
        MaxHeight,
        [Description("max-width")]
        MaxWidth,
        [Description("min-height")]
        MinHeight,
        [Description("min-width")]
        MinWidth,
        [Description("object-fit")]
        ObjectFit,
        [Description("object-position")]
        ObjectPosition,
        [Description("opacity")]
        Opacity,
        [Description("order")]
        Order,
        [Description("outline")]
        Outline,
        [Description("overflow")]
        Overflow,
        [Description("overflow-x")]
        OverflowX,
        [Description("overflow-y")]
        OverflowY,
        [Description("padding")]
        Padding,
        [Description("padding-bottom")]
        PaddingBottom,
        [Description("padding-left")]
        PaddingLeft,
        [Description("padding-right")]
        PaddingRight,
        [Description("padding-top")]
        PaddingTop,
        [Description("pointer-events")]
        PointerEvents,
        [Description("position")]
        Position,
        [Description("right")]
        Right,
        [Description("top")]
        Top,
        [Description("transform")]
        Transform,
        [Description("transition")]
        Transition,
        [Description("user-select")]
        UserSelect,
        [Description("vertical-align")]
        VerticalAlign,
        [Description("visibility")]
        Visibility,
        [Description("white-space")]
        WhiteSpace,
        [Description("word-break")]
        WordBreak,
        [Description("word-spacing")]
        WordSpacing,
        [Description("word-wrap")]
        WordWrap,
        [Description("writing-mode")]
        WritingMode,
        [Description("width")]
        Width,
        [Description("z-index")]
        ZIndex,
    }

    public static class AlignContentCssProp
    {
        public static string Stretch { get; private set; } = "stretch";
        public static string Center { get; private set; } = "center";
        public static string FlexStart { get; private set; } = "flex-start";
        public static string FlexEnd { get; private set; } = "flex-end";
        public static string SpaceBetween { get; private set; } = "space-between";
        public static string SpaceAround { get; private set; } = "space-around";
        public static string Initial { get; private set; } = "initial";
        public static string Inherit { get; private set; } = "inherit";
    }

    public static class AlignItemsCssProp
    {
        public static string Stretch { get; private set; } = "stretch";
        public static string Center { get; private set; } = "center";
        public static string FlexStart { get; private set; } = "flex-start";
        public static string FlexEnd { get; private set; } = "flex-end";
        public static string Baseline { get; private set; } = "baseline";
        public static string Initial { get; private set; } = "initial";
        public static string Inherit { get; private set; } = "inherit";
    }

    public static class AlignSelfCssProp
    {
        public static string Auto { get; private set; } = "auto";
        public static string Stretch { get; private set; } = "stretch";
        public static string Center { get; private set; } = "center";
        public static string FlexStart { get; private set; } = "flex-start";
        public static string FlexEnd { get; private set; } = "flex-end";
        public static string Baseline { get; private set; } = "baseline";
        public static string Initial { get; private set; } = "initial";
        public static string Inherit { get; private set; } = "inherit";
    }

    public static class CursorCssProp
    {
        public static string Alias { get; private set; } = "alias";
        public static string AllScroll { get; private set; } = "all-scroll";
        public static string Auto { get; private set; } = "auto";
        public static string Cell { get; private set; } = "cell";
        public static string ContextMenu { get; private set; } = "context-menu";
        public static string ColResize { get; private set; } = "col-resize";
        public static string Copy { get; private set; } = "copy";
        public static string Crosshair { get; private set; } = "crosshair";
        public static string Default { get; private set; } = "default";
        public static string EResize { get; private set; } = "e-resize";
        public static string EwResize { get; private set; } = "ew-resize";
        public static string Grab { get; private set; } = "grab";
        public static string Grabbing { get; private set; } = "grabbing";
        public static string Help { get; private set; } = "help";
        public static string Move { get; private set; } = "move";
        public static string NResize { get; private set; } = "n-resize";
        public static string NeResize { get; private set; } = "ne-resize";
        public static string NeswResize { get; private set; } = "nesw-resize";
        public static string NsResize { get; private set; } = "ns-resize";
        public static string NwResize { get; private set; } = "nw-resize";
        public static string NwseResize { get; private set; } = "nwse-resize";
        public static string NoDrop { get; private set; } = "no-drop";
        public static string None { get; private set; } = "none";
        public static string NotAllowed { get; private set; } = "not-allowed";
        public static string Pointer { get; private set; } = "pointer";
        public static string Progress { get; private set; } = "progress";
        public static string RowResize { get; private set; } = "row-resize";
        public static string SResize { get; private set; } = "s-resize";
        public static string SeResize { get; private set; } = "se-resize";
        public static string SwResize { get; private set; } = "sw-resize";
        public static string Text { get; private set; } = "text";
        public static string Url { get; private set; } = "url";
        public static string VerticalText { get; private set; } = "vertical-text";
        public static string Wait { get; private set; } = "wait";
        public static string ZoomIn { get; private set; } = "zoom-in";
        public static string ZoomOut { get; private set; } = "zoom-out";
        public static string Initial { get; private set; } = "initial";
        public static string Inherit { get; private set; } = "inherit";
    }

    public static class DirectionCssProp
    {
        public static string Ltr { get; private set; } = "ltr";
        public static string Rtl { get; private set; } = "rtl";
        public static string Initial { get; private set; } = "initial";
        public static string Inherit { get; private set; } = "inherit";
    }

    public static class DisplayCssProp
    {
        public static string Inline { get; private set; } = "inline";
        public static string Block { get; private set; } = "block";
        public static string Flex { get; private set; } = "flex";
        public static string Grid { get; private set; } = "grid";
        public static string InlineBlock { get; private set; } = "inline-block";
        public static string InlineFlex { get; private set; } = "inline-flex";
        public static string InlineGrid { get; private set; } = "inline-grid";
        public static string InlineTable { get; private set; } = "inline-table";
        public static string Contents { get; private set; } = "contents";
        public static string Table { get; private set; } = "table";
        public static string TableRow { get; private set; } = "table-row";
        public static string TableColumn { get; private set; } = "table-column"; 
        public static string TableCell { get; private set; } = "table-cell";
        public static string TableCaption { get; private set; } = "table-caption";
        public static string None { get; private set; } = "none";
        public static string Initial { get; private set; } = "initial";
        public static string Inherit { get; private set; } = "inherit";
    }

    public static class FontStyleCssProp
    {
        public static string Normal { get; private set; } = "normal";
        public static string Italic { get; private set; } = "italic";
        public static string Oblique { get; private set; } = "oblique";
        public static string Initial { get; private set; } = "initial";
        public static string Inherit { get; private set; } = "inherit";
    }

    public static class HyphensCssProp
    {
        public static string None { get; private set; } = "none";
        public static string Manual { get; private set; } = "manual";
        public static string Auto { get; private set; } = "auto";
        public static string Initial { get; private set; } = "initial";
        public static string Inherit { get; private set; } = "inherit";
    }

    public static class JustifyContentCssProp
    {
        public static string Center { get; private set; } = "center";
        public static string FlexStart { get; private set; } = "flex-start";
        public static string FlexEnd { get; private set; } = "flex-end";
        public static string SpaceBetween { get; private set; } = "space-between";
        public static string SpaceAround { get; private set; } = "space-around";
        public static string Initial { get; private set; } = "initial";
        public static string Inherit { get; private set; } = "inherit";
    }

    public static class PointerEventsCssProp
    {
        public static string Auto { get; private set; } = "auto";
        public static string None { get; private set; } = "none";
        public static string VisiblePainted { get; private set; } = "visiblePainted";
        public static string VisibleFill { get; private set; } = "visibleFill";
        public static string VisibleStroke { get; private set; } = "visibleStroke";
        public static string Painted { get; private set; } = "painted";
        public static string Fill { get; private set; } = "fill";
        public static string Stroke { get; private set; } = "stroke";
        public static string All { get; private set; } = "all";
        public static string Initial { get; private set; } = "initial";
        public static string Inherit { get; private set; } = "inherit";
    }

    public static class PositionCssProp
    {
        public static string Static { get; private set; } = "static";
        public static string Absolute { get; private set; } = "absolute";
        public static string Relative { get; private set; } = "relative";
        public static string Fixed { get; private set; } = "fixed";
        public static string Sticky { get; private set; } = "sticky";
        public static string Initial { get; private set; } = "initial";
        public static string Inherit { get; private set; } = "inherit";
    }

    public static class UserSelectCssProp
    {
        public static string Auto { get; private set; } = "auto";
        public static string None { get; private set; } = "none";
        public static string All { get; private set; } = "all";
        public static string Text { get; private set; } = "text";
    }

    public static class WhiteSpaceCssProp
    {
        public static string Normal { get; private set; } = "normal";
        public static string Nowrap { get; private set; } = "nowrap";
        public static string Pre { get; private set; } = "pre";
        public static string Preline { get; private set; } = "pre-line";
        public static string Prewrap { get; private set; } = "pre-wrap";
        public static string Initial { get; private set; } = "initial";
        public static string Inherit { get; private set; } = "inherit";
    }

    public static class WordWrapCssProp
    {
        public static string Normal { get; private set; } = "normal";
        public static string BreakWord { get; private set; } = "break-word";
        public static string Initial { get; private set; } = "initial";
        public static string Inherit { get; private set; } = "inherit";
    }

    public static class WritingModeCssProp
    {
        public static string HorizontalTb { get; private set; } = "horizontal-tb";
        public static string VerticalRl { get; private set; } = "vertical-rl";
        public static string VerticalLr { get; private set; } = "vertical-lr";
    }
}
