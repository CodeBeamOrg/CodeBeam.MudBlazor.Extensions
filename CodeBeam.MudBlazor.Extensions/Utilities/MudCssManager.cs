using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudExtensions
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
    }

    public enum CssProp
    {
        [Description("aligncontent")]
        AlignContent,
        [Description("alignitems")]
        AlignItems,
        [Description("alignself")]
        AlignSelf,
        [Description("animation")]
        Animation,
        [Description("animationdelay")]
        AnimationDelay,
        [Description("animationdirection")]
        AnimationDirection,
        [Description("animationduration")]
        AnimationDuration, 
        [Description("animationfillmode")]
        AnimationFillMode,
        [Description("animationiterationcount")]
        AnimationIterationCount,
        [Description("animationname")]
        AnimationName,
        [Description("animationplaystate")]
        AnimationPlayState,
        [Description("animationtimingfunction")]
        AnimationTimingFunction,
        [Description("background")]
        Background,
        [Description("backgroundattachment")]
        BackgroundAttachment,
        [Description("backgroundblendmode")]
        BackgroundBlendMode,
        [Description("backgroundclip")]
        BackgroundClip,
        [Description("backgroundcolor")]
        BackgroundColor,
        [Description("backgroundimage")]
        BackgroundImage,
        [Description("backgroundorigin")]
        BackgroundOrigin,
        [Description("backgroundposition")]
        BackgroundPosition,
        [Description("backgroundrepeat")]
        BackgroundRepeat,
        [Description("backgroundsize")]
        BackgroundSize,
        [Description("border")]
        Border,
        [Description("borderimage")]
        BorderImage,
        [Description("borderleft")]
        BorderLeft,
        [Description("borderright")]
        BorderRight,
        [Description("bordertop")]
        BorderTop,
        [Description("borderbottom")]
        BorderBottom,
        [Description("borderradius")]
        BorderRadius,
        [Description("borderspacing")]
        BorderSpacing,
        [Description("borderstyle")]
        BorderStyle,
        [Description("borderwidth")]
        BorderWidth,
        [Description("bottom")]
        Bottom,
        [Description("boxshadow")]
        BoxShadow,
        [Description("boxsizing")]
        BoxSizing,
        [Description("caretcolor")]
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
        [Description("flexbasis")]
        FlexBasis,
        [Description("flexdirection")]
        FlexDirection,
        [Description("flexflow")]
        FlexFlow,
        [Description("flexgrow")]
        FlexGrow,
        [Description("flexshrink")]
        FlexShrink,
        [Description("flexwrap")]
        FlexWrap,
        [Description("float")]
        Float,
        [Description("font")]
        Font,
        [Description("fontfamily")]
        FontFamily,
        [Description("fontkerning")]
        FontKerning,
        [Description("fontsize")]
        FontSize,
        [Description("fontsizeadjust")]
        FontSizeAdjust,
        [Description("fontstretch")]
        FontStretch,
        [Description("fontstyle")]
        FontStyle,
        [Description("fontvariant")]
        FontVariant,
        [Description("fontweight")]
        FontWeight,
        [Description("grid")]
        Grid,
        [Description("gridarea")]
        GridArea,
        [Description("gridcolumn")]
        GridColumn,
        [Description("gridrow")]
        GridRow,
        [Description("gridtemplate")]
        GridTemplate,
        [Description("gridtemplateareas")]
        GridTemplateAreas,
        [Description("gridtemplatecolumns")]
        GridTemplateColumns,
        [Description("gridtemplaterows")]
        GridTemplateRows,
        [Description("height")]
        Height,
        [Description("hyphens")]
        Hyphens,
        [Description("justifycontent")]
        JustifyContent,
        [Description("left")]
        Left,
        [Description("letterspacing")]
        LetterSpacing,
        [Description("lineheight")]
        LineHeight,
        [Description("liststyle")]
        ListStyle,
        [Description("margin")]
        Margin,
        [Description("marginbottom")]
        MarginBottom,
        [Description("marginleft")]
        MarginLeft,
        [Description("marginright")]
        MarginRight,
        [Description("margintop")]
        MarginTop,
        [Description("maxheight")]
        MaxHeight,
        [Description("maxwidth")]
        MaxWidth,
        [Description("minheight")]
        MinHeight,
        [Description("minwidth")]
        MinWidth,
        [Description("objectfit")]
        ObjectFit,
        [Description("objectposition")]
        ObjectPosition,
        [Description("opacity")]
        Opacity,
        [Description("order")]
        Order,
        [Description("outline")]
        Outline,
        [Description("overflow")]
        Overflow,
        [Description("overflowx")]
        OverflowX,
        [Description("overflowy")]
        OverflowY,
        [Description("padding")]
        Padding,
        [Description("paddingbottom")]
        PaddingBottom,
        [Description("paddingleft")]
        PaddingLeft,
        [Description("paddingright")]
        PaddingRight,
        [Description("paddingtop")]
        PaddingTop,
        [Description("pointerevents")]
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
        [Description("userselect")]
        UserSelect,
        [Description("verticalalign")]
        VerticalAlign,
        [Description("visibility")]
        Visibility,
        [Description("whitespace")]
        WhiteSpace,
        [Description("wordbreak")]
        WordBreak,
        [Description("wordspacing")]
        WordSpacing,
        [Description("wordwrap")]
        WordWrap,
        [Description("writingmode")]
        WritingMode,
        [Description("width")]
        Width,
        [Description("zindex")]
        ZIndex,
    }
}
