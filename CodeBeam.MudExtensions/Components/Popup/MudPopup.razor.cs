using Microsoft.AspNetCore.Components;
using MudBlazor;
using MudBlazor.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MudExtensions
{
    public partial class MudPopup : MudComponentBase
    {
        Guid _animationGuid = Guid.NewGuid();

        protected string Classname => new CssBuilder("mud-popup")
            .AddClass($"fixed mud-width-full gap-4 pa-4 popup-{_animationGuid}")
            .AddClass("mud-popup-center", PopupPosition == PopupPosition.Center)
            .AddClass("d-flex", (_breakpoint != Breakpoint.Xs && PopupPosition != PopupPosition.Center))
            .AddClass("align-center", PopupPosition == PopupPosition.Bottom || PopupPosition == PopupPosition.Top)
            .AddClass($"mud-elevation-{Elevation.ToString()}")
            .AddClass(Class)
            .Build();

        protected string Stylename => new StyleBuilder()
            .AddStyle("bottom", $"{Padding}px", PopupPosition == PopupPosition.Bottom)
            .AddStyle("top", $"{Padding}px", PopupPosition == PopupPosition.Top)
            .AddStyle("left", $"{Padding}px", PopupPosition == PopupPosition.Bottom || PopupPosition == PopupPosition.Top)
            .AddStyle("width", $"calc(100% - {Padding * 2}px)", PopupPosition == PopupPosition.Bottom || PopupPosition == PopupPosition.Top)
            .AddStyle(Style)
            .Build();

        [Parameter]
        public PopupPosition PopupPosition { get; set; } = PopupPosition.Bottom;

        bool _open = false;
        /// <summary>
        /// The popup's visible state.
        /// </summary>
        [Parameter]
        public bool Open 
        { 
            get => _open; 
            set
            {
                if (_open == value)
                {
                    return;
                }
                _open = value;
                OpenChanged.InvokeAsync(_open).AndForget();
            }
        }

        /// <summary>
        /// Popup's space between the borders. Has no effect on centered popups.
        /// </summary>
        [Parameter]
        public int Padding { get; set; }

        /// <summary>
        /// If true, popup appears with a fade animation.
        /// </summary>
        [Parameter]
        public bool EnableAnimation { get; set; } = true;

        /// <summary>
        /// The higher the number, the heavier the drop-shadow.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Paper.Appearance)]
        public int Elevation { set; get; } = 4;

        /// <summary>
        /// The icon at the start of the popup. Can be overridden with ChildContent.
        /// </summary>
        [Parameter]
        public string Icon { get; set; }

        /// <summary>
        /// The text coming after the icon of the popup. Can be overridden with ChildContent.
        /// </summary>
        [Parameter]
        public string Text { get; set; }

        /// <summary>
        /// Icon and text color.
        /// </summary>
        [Parameter]
        public Color Color { get; set; }

        /// <summary>
        /// Custom content for override everything.
        /// </summary>
        [Parameter]
        public RenderFragment ChildContent { get; set; }

        /// <summary>
        /// The action button section. If this renderfragment null, a close icon button will be appear.
        /// </summary>
        [Parameter]
        public RenderFragment ActionContent { get; set; }

        /// <summary>
        /// The MudLink content continues after the text.
        /// </summary>
        [Parameter]
        public RenderFragment LinkContent { get; set; }

        [Parameter]
        public EventCallback<bool> OpenChanged { get; set; }

        Breakpoint _breakpoint;
        protected void GetBreakpoint(Breakpoint breakpoint)
        {
            _breakpoint = breakpoint;
        }

    }
}
