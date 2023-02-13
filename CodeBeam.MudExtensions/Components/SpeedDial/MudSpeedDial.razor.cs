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
    public partial class MudSpeedDial : MudComponentBase
    {
        Guid _animationGuid = Guid.NewGuid();

        protected string StackClassname => new CssBuilder("ma-2")
            .AddClass($"speedDial-{_animationGuid}")
            .AddClass("flex-column-reverse", Origin == Origin.BottomCenter || Origin == Origin.BottomRight || Origin == Origin.BottomLeft)
            .Build();

        protected string Stylename => new StyleBuilder()
            .AddStyle("bottom", $"{Padding}px", Origin == Origin.BottomCenter || Origin == Origin.BottomRight || Origin == Origin.BottomLeft)
            .AddStyle("top", $"{Padding}px", Origin == Origin.TopCenter || Origin == Origin.TopRight || Origin == Origin.TopLeft)
            .AddStyle("right", $"{Padding}px", !(Origin == Origin.BottomLeft || Origin == Origin.CenterLeft || Origin == Origin.TopLeft))
            .AddStyle("left", $"{Padding}px", Origin == Origin.BottomLeft || Origin == Origin.CenterLeft || Origin == Origin.TopLeft)
            .AddStyle("left", "50%", Origin == Origin.BottomCenter || Origin == Origin.TopCenter || Origin == Origin.CenterCenter)
            .AddStyle("width", "fit-content")
            .AddStyle(Style)
            .Build();

        [Parameter]
        public bool Open { get; set; }

        [Parameter]
        public bool OpenOnHover { get; set; } = true;

        [Parameter]
        public bool OpenOnClick { get; set; } = true;

        [Parameter]
        public int Padding { get; set; }

        [Parameter]
        public EventCallback OnMainButtonClick { get; set; }

        [Parameter]
        public string Icon { get; set; } = Icons.Material.Filled.Add;

        [Parameter]
        public string IconOnOpen { get; set; }

        [Parameter]
        public Size Size { get; set; } = Size.Large;

        [Parameter]
        public Color Color { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        Origin _origin = Origin.BottomRight;
        [Parameter]
        public Origin Origin
        {
            get => _origin;
            set
            {
                if (_origin == value) return;
                _origin = value;
                UpdateOrigin();
            }
        }

        protected string GetIcon()
        {
            if (Open && !string.IsNullOrEmpty(IconOnOpen))
            {
                return IconOnOpen;
            }
            return Icon;
        }

        protected void ChangeMenu(bool open)
        {
            Open = open;
            StateHasChanged();
        }

        public void ToggleMenu()
        {
            ChangeMenu(!Open);
        }

        public void OpenMenu()
        {
            ChangeMenu(true);
        }

        public void CloseMenu()
        {
            ChangeMenu(false);
        }

        protected async Task MainButtonClick()
        {
            if (OpenOnClick)
            {
                ToggleMenu();
            }
            await OnMainButtonClick.InvokeAsync();
        }

        bool _rootMouseEnter;
        bool _popoverMouseEnter;
        protected void RootMouseEnter()
        {
            if (!OpenOnHover)
            {
                return;
            }
            _rootMouseEnter = true;
            OpenMenu();

        }

        protected void PopoverMouseEnter()
        {
            if (!OpenOnHover)
            {
                return;
            }
            _popoverMouseEnter = true;
        }

        protected async Task PopoverMouseLeave()
        {
            if (!OpenOnHover)
            {
                return;
            }
            _popoverMouseEnter = false;
            await WaitToClose();
        }

        protected async Task WaitToClose()
        {
            if (!OpenOnHover)
            {
                return;
            }
            _rootMouseEnter = false;
            await Task.Delay(100);
            if (!_popoverMouseEnter && !_rootMouseEnter)
            {
                CloseMenu();
            }
        }

        bool _row = false;
        Origin _anchorOrigin = Origin.TopCenter;
        Origin _transformOrigin = Origin.BottomCenter;
        protected void UpdateOrigin()
        {
            if (Origin == Origin.BottomRight || Origin == Origin.BottomCenter || Origin == Origin.BottomLeft)
            {
                _row = false;
                _anchorOrigin = Origin.TopCenter;
                _transformOrigin = Origin.BottomCenter;
            }
            else if (Origin == Origin.TopRight || Origin == Origin.TopCenter || Origin == Origin.TopLeft)
            {
                _row = false;
                _anchorOrigin = Origin.BottomCenter;
                _transformOrigin = Origin.TopCenter;
            }
            else if (Origin == Origin.CenterLeft || Origin == Origin.CenterCenter || Origin == Origin.CenterRight)
            {
                _row = true;
                _anchorOrigin = Origin.TopCenter;
                _transformOrigin = Origin.BottomCenter;
            }
            //else if (Origin == Origin.CenterRight)
            //{
            //    _row = true;
            //    _anchorOrigin = Origin.CenterRight;
            //    _transformOrigin = Origin.CenterLeft;
            //}
            StateHasChanged();
        }
    }
}
