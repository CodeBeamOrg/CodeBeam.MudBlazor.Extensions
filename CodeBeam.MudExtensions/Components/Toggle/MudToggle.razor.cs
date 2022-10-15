using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
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
    public partial class MudToggle : MudComponentBase
    {

        protected string Classname => new CssBuilder()
            .AddClass(Class, Toggled == false)
            .AddClass(ClassToggled, Toggled == true)
            .AddClass(ClassCommon)
            .Build();

        protected string GetStyle()
        {
            if (Toggled == false)
            {
                return $"{StyleCommon} {Style}";
            }
            else
            {
                return $"{StyleCommon} {StyleToggled}";
            }

        }

        bool _toggled;
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
                ToggledChanged.InvokeAsync().AndForget();
            }
        }

        [Parameter]
        public EventCallback<bool> ToggledChanged { get; set; }

        [Parameter]
        public string ClassCommon { get; set; }

        [Parameter]
        public string ClassToggled { get; set; }

        [Parameter]
        public string StyleToggled { get; set; }

        [Parameter]
        public string StyleCommon { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public RenderFragment ToggleContent { get; set; }

        [Parameter]
        public EventCallback<MouseEventArgs> OnClick { get; set; }

        protected async Task HandleOnClick()
        {
            await OnClick.InvokeAsync();
        }

    }
}
