using Microsoft.AspNetCore.Components;
using MudBlazor;
using MudBlazor.State;
using MudBlazor.Utilities;

namespace MudExtensions
{
    /// <summary>
    /// 
    /// </summary>
    public partial class MudSpeedDial : MudComponentBase
    {
        /// <summary>
        /// MudLoading constructor.
        /// </summary>
        public MudSpeedDial()
        {
            using var registerScope = CreateRegisterScope();
            _origin = registerScope.RegisterParameter<Origin>(nameof(Origin))
                .WithParameter(() => Origin)
                .WithChangeHandler(UpdateOrigin);
        }

        private readonly ParameterState<Origin> _origin;

        Guid _animationGuid = Guid.NewGuid();

        /// <summary>
        /// 
        /// </summary>
        protected string? StackClassname => new CssBuilder("ma-2")
            .AddClass($"speedDial-{_animationGuid}")
            .AddClass("flex-column-reverse", Origin == Origin.BottomCenter || Origin == Origin.BottomRight || Origin == Origin.BottomLeft)
            .Build();

        /// <summary>
        /// 
        /// </summary>
        protected string? Stylename => new StyleBuilder()
            .AddStyle("bottom", $"{Padding}px", Origin == Origin.BottomCenter || Origin == Origin.BottomRight || Origin == Origin.BottomLeft)
            .AddStyle("top", $"{Padding}px", Origin == Origin.TopCenter || Origin == Origin.TopRight || Origin == Origin.TopLeft)
            .AddStyle("right", $"{Padding}px", !(Origin == Origin.BottomLeft || Origin == Origin.CenterLeft || Origin == Origin.TopLeft))
            .AddStyle("left", $"{Padding}px", Origin == Origin.BottomLeft || Origin == Origin.CenterLeft || Origin == Origin.TopLeft)
            .AddStyle("left", "50%", Origin == Origin.BottomCenter || Origin == Origin.TopCenter || Origin == Origin.CenterCenter)
            .AddStyle("width", "fit-content")
            .AddStyle(Style)
            .Build();

        /// <summary>
        /// 
        /// </summary>
        [Parameter]
        public string? PopoverClass { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Parameter]
        public bool Open { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Parameter]
        public bool OpenOnHover { get; set; } = true;

        /// <summary>
        /// 
        /// </summary>
        [Parameter]
        public bool OpenOnClick { get; set; } = true;

        /// <summary>
        /// 
        /// </summary>
        [Parameter]
        public bool CloseWhenClick { get; set; } = false;

        /// <summary>
        /// 
        /// </summary>
        [Parameter]
        public int Padding { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Parameter]
        public EventCallback OnMainButtonClick { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Parameter]
        public string? Icon { get; set; } = Icons.Material.Filled.Add;

        /// <summary>
        /// 
        /// </summary>
        [Parameter]
        public string? IconOnOpen { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Parameter]
        public Size Size { get; set; } = Size.Large;

        /// <summary>
        /// 
        /// </summary>
        [Parameter]
        public Color Color { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Parameter]
        public RenderFragment? ChildContent { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Parameter]
        public RenderFragment? ActivatorContent { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Parameter]
        public Origin Origin { get; set; } = Origin.BottomRight;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected string? GetIcon()
        {
            if (Open && !string.IsNullOrEmpty(IconOnOpen))
            {
                return IconOnOpen;
            }
            return Icon;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="open"></param>
        protected void ChangeMenu(bool open)
        {
            Open = open;
            StateHasChanged();
        }

        /// <summary>
        /// 
        /// </summary>
        public void ToggleMenu()
        {
            ChangeMenu(!Open);
        }

        /// <summary>
        /// 
        /// </summary>
        public void OpenMenu()
        {
            ChangeMenu(true);
        }

        /// <summary>
        /// 
        /// </summary>
        public void CloseMenu()
        {
            ChangeMenu(false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
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
        /// <summary>
        /// 
        /// </summary>
        protected void RootMouseEnter()
        {
            if (!OpenOnHover)
            {
                return;
            }
            _rootMouseEnter = true;
            OpenMenu();

        }

        /// <summary>
        /// 
        /// </summary>
        protected void PopoverMouseEnter()
        {
            if (!OpenOnHover)
            {
                return;
            }
            _popoverMouseEnter = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected async Task PopoverMouseLeave()
        {
            if (!OpenOnHover)
            {
                return;
            }
            _popoverMouseEnter = false;
            await WaitToClose();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// 
        /// </summary>
        protected void HandlePopoverClick()
        {
            if (CloseWhenClick)
            {
                ChangeMenu(false);
            }
        }

        bool _row = false;
        Origin _anchorOrigin = Origin.TopCenter;
        Origin _transformOrigin = Origin.BottomCenter;
        /// <summary>
        /// 
        /// </summary>
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
