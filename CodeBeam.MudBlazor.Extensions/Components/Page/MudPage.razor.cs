// Copyright (c) CodeBeam 2021
// CodeBeam.MudExtensions licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using MudBlazor.Extensions;
using MudBlazor.Utilities;
using MudExtensions.Enums;

namespace MudExtensions
{
    /// <summary>
    /// 
    /// </summary>
    public partial class MudPage : MudComponentBase
    {
        /// <summary>
        /// 
        /// </summary>
        protected string? Classname =>
        new CssBuilder("mud-page")
            .AddClass($"mud-page-column-{Column.ToString()}")
            .AddClass($"mud-page-row-{Row.ToString()}")
            .AddClass($"mud-page-height-{FullScreen.ToDescriptionString()}")
            .AddClass(Class)
        .Build();

        /// <summary>
        /// 
        /// </summary>
        protected string? Stylename =>
        new StyleBuilder()
            .AddStyle("height", $"{Height}", !String.IsNullOrEmpty(Height))
            .AddStyle(Style)
        .Build();

        /// <summary>
        /// The content inside the component.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Item.Appearance)]
        public RenderFragment? ChildContent { get; set; }

        /// <summary>
        /// Number of columns. Default is 4.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Item.Appearance)]
        public int Column { get; set; } = 4;

        /// <summary>
        /// Number of rows. Default is 4.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Item.Appearance)]
        public int Row { get; set; } = 4;

        /// <summary>
        /// Determines MudPage's height.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Item.Appearance)]
        public FullScreen FullScreen { get; set; } = FullScreen.None;

        /// <summary>
        /// Determines MudPage's height with a string value.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Item.Appearance)]
        public string? Height { get; set; }

        /// <summary>
        /// Fires when click.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Item.Behavior)]
        public EventCallback<MouseEventArgs> OnClick { get; set; }

        /// <summary>
        /// If true stop progpagation on click.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Item.Behavior)]
        public bool OnClickStopPropagation { get; set; }

        /// <summary>
        /// Fires when context menu (ex. right click).
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Item.Behavior)]
        public EventCallback OnContextMenu { get; set; }

        /// <summary>
        /// If true prevent default context menu event.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Item.Behavior)]
        public bool OnContextMenuPreventDefault { get; set; }

        /// <summary>
        /// If true stop propagation context menu event.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Item.Behavior)]
        public bool OnContextMenuStopPropagation { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ev"></param>
        /// <returns></returns>
        protected async Task OnClickHandler(MouseEventArgs ev)
        {
            await OnClick.InvokeAsync(ev);
        }
    }
}
