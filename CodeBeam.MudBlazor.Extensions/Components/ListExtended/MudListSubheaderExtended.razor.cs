using Microsoft.AspNetCore.Components;
using MudBlazor;
using MudBlazor.Utilities;

namespace MudExtensions
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public partial class MudListSubheaderExtended<T> : MudComponentBase
    {
        /// <summary>
        /// 
        /// </summary>
        protected string? Classname =>
        new CssBuilder("mud-list-subheader-extended")
            .AddClass("mud-list-subheader-gutters-extended", !DisableGutters)
            .AddClass("mud-list-subheader-inset-extended", Inset)
            .AddClass("mud-list-subheader-secondary-background-extended", SecondaryBackground)
            .AddClass("mud-list-subheader-sticky-extended", Sticky)
            .AddClass("mud-list-subheader-sticky-dense-extended", Sticky && (MudListExtended != null && MudListExtended.DisablePadding))
            .AddClass(Class)
        .Build();

        /// <summary>
        /// 
        /// </summary>
        [CascadingParameter] protected MudListExtended<T?>? MudListExtended { get; set; }

        /// <summary>
        /// The child render fragment.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Behavior)]
        public RenderFragment? ChildContent { get; set; }

        /// <summary>
        /// Disables the left and right spaces.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Appearance)]
        public bool DisableGutters { get; set; }

        /// <summary>
        /// If true subheader has more padding. Default is false.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Appearance)]
        public bool Inset { get; set; }

        /// <summary>
        /// If true, subheader behaves sticky and remains on top until other subheader comes to top.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Appearance)]
        public bool Sticky { get; set; }

        /// <summary>
        /// If true, subheader has darken background.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Appearance)]
        public bool SecondaryBackground { get; set; }
    }
}
