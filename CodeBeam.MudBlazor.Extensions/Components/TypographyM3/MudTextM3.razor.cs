using Microsoft.AspNetCore.Components;
using MudBlazor;
using MudBlazor.Extensions;
using MudBlazor.Utilities;
using MudExtensions.Enums;

namespace MudExtensions
{
#nullable enable
    public partial class MudTextM3 : MudComponentBase
    {
        protected string ClassName => new CssBuilder("mud-typographym3")
            .AddClass($"mud-typographym3-{Typo.ToDescriptionString()}-{Size.ToDescriptionString()}") // .mud-typographym3-#{$style}-#{$size}
            .AddClass($"mud-{Color.ToDescriptionString()}-text", Color != Color.Default && Color != Color.Inherit)
            .AddClass("mud-typography-gutterbottom", GutterBottom)
            .AddClass($"mud-typography-align-{ConvertAlign(Align).ToDescriptionString()}", Align != Align.Inherit)
            .AddClass("mud-typography-display-inline", Inline)
            .AddClass(Class)
            .Build();

        protected string StyleString => new StyleBuilder()
            .AddStyle(Style)
            .Build();

        private Align ConvertAlign(Align align)
        {
            return align switch
            {
                Align.Start => RightToLeft ? Align.Right : Align.Left,
                Align.End => RightToLeft ? Align.Left : Align.Right,
                _ => align
            };
        }

        [CascadingParameter(Name = "RightToLeft")]
        public bool RightToLeft { get; set; }

        /// <summary>
        /// Set the text-align on the component.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Text.Appearance)]
        public TypoM3 Typo { get; set; } = TypoM3.Body;
        /// <summary>
        /// Set the text-align on the component.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Text.Appearance)]
        public Size Size { get; set; } = Size.Large; 

        /// <summary>
        /// Child content of component.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Text.Behavior)]
        public RenderFragment? ChildContent { get; set; }

        /// <summary>
        /// Set the text-align on the component.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Text.Appearance)]
        public Align Align { get; set; } = Align.Inherit;

        /// <summary>
        /// The color of the component. It supports the theme colors.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Text.Appearance)]
        public Color Color { get; set; } = Color.Inherit;

        /// <summary>
        /// If true, the text will have a bottom margin.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Text.Appearance)]
        public bool GutterBottom { get; set; } = false;

        /// <summary>
        /// If true, Sets display inline
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Text.Appearance)]
        public bool Inline { get; set; }
    }
}