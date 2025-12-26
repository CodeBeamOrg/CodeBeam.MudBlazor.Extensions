using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using MudBlazor.Utilities;

namespace MudExtensions
{
    /// <summary>
    /// 
    /// </summary>
    public partial class MudToggle : MudComponentBase
    {
        /// <summary>
        /// 
        /// </summary>
        protected string? Classname => new CssBuilder()
            .AddClass(Class, !Toggled)
            .AddClass(ClassToggled, Toggled)
            .AddClass(ClassCommon)
            .Build();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected string? GetStyle()
        {
            if (!Toggled)
            {
                return $"{StyleCommon} {Style}";
            }
            else
            {
                return $"{StyleCommon} {StyleToggled}";
            }

        }

        bool _toggled;
        /// <summary>
        /// 
        /// </summary>
        [Parameter]
        public bool Toggled
        { 
            get => _toggled; 
            set
            {
                if (_toggled == value)
                {
                    return;
                }
                _toggled = value;
                ToggledChanged.InvokeAsync().CatchAndLog();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [Parameter]
        public EventCallback<bool> ToggledChanged { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Parameter]
        public string? ClassCommon { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Parameter]
        public string? ClassToggled { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Parameter]
        public string? StyleToggled { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Parameter]
        public string? StyleCommon { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Parameter]
        public RenderFragment? ChildContent { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Parameter]
        public RenderFragment? ToggleContent { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Parameter]
        public EventCallback<MouseEventArgs> OnClick { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected async Task HandleOnClick()
        {
            await OnClick.InvokeAsync();
        }

    }
}
