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
        [Description("background")]
        Background,
        [Description("backgroundclip")]
        BackgroundClip,
        [Description("backgroundcolor")]
        BackgroundColor,
        [Description("backgroundimage")]
        BackgroundImage,
        [Description("border")]
        Border,
        [Description("borderradius")]
        BorderRadius,
        [Description("color")]
        Color,
        [Description("height")]
        Height,
        [Description("transition")]
        Transition,
        [Description("width")]
        Width,
    }
}
