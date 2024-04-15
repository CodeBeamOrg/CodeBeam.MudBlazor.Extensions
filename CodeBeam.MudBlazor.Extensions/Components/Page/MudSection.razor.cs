// Copyright (c) CodeBeam 2021
// CodeBeam.MudExtensions licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using MudBlazor.Utilities;

namespace MudExtensions
{
    /// <summary>
    /// 
    /// </summary>
    public partial class MudSection : MudComponentBase
    {
        /// <summary>
        /// 
        /// </summary>
        protected string? Classname =>
        new CssBuilder("mud-section")
            .AddClass($"mud-section-col-start-{Column.ToString()}")
            .AddClass($"mud-section-col-end-{(Column + ColSpan).ToString()}")
            .AddClass($"mud-section-row-start-{Row.ToString()}")
            .AddClass($"mud-section-row-end-{(Row + RowSpan).ToString()}")
            .AddClass(Class)
        .Build();

        /// <summary>
        /// 
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Item.Appearance)]
        public RenderFragment? ChildContent { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Item.Appearance)]
        public int Column { get; set; } = 1;

        /// <summary>
        /// 
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Item.Appearance)]
        public int ColSpan { get; set; } = 1;

        /// <summary>
        /// 
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Item.Appearance)]
        public int Row { get; set; } = 1;

        /// <summary>
        /// 
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Item.Appearance)]
        public int RowSpan { get; set; } = 1;

        /// <summary>
        /// 
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Item.Behavior)]
        public EventCallback<MouseEventArgs> OnClick { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Item.Behavior)]
        public bool OnClickStopPropagation { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Item.Behavior)]
        public EventCallback OnContextMenu { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Item.Behavior)]
        public bool OnContextMenuPreventDefault { get; set; }

        /// <summary>
        /// 
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
