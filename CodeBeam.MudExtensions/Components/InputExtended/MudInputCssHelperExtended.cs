using System;
using MudBlazor;
using MudBlazor.Extensions;
using MudBlazor.Utilities;

namespace MudExtensions
{
    internal static class MudInputCssHelperExtended
    {
        public static string GetClassname<T>(MudBaseInputExtended<T> baseInput, Func<bool> shrinkWhen) =>
            new CssBuilder("mud-input")
                .AddClass($"mud-input-{baseInput.Variant.ToDescriptionString()}")
                .AddClass($"mud-input-margin-{baseInput.Margin.ToDescriptionString()}", when: () => baseInput.Margin != Margin.None)
                .AddClass("mud-input-underline", when: () => baseInput.DisableUnderLine == false && baseInput.Variant != Variant.Outlined)
                .AddClass("mud-shrink", when: shrinkWhen)
                .AddClass("mud-disabled", baseInput.Disabled)
                .AddClass("mud-input-error", baseInput.HasErrors)
                //.AddClass("d-flex align-center")
                .AddClass("mud-ltr", baseInput.GetInputType() == InputType.Email || baseInput.GetInputType() == InputType.Telephone)
                .AddClass(baseInput.Class)
                .Build();

        public static string GetInputClassname<T>(MudBaseInputExtended<T> baseInput) =>
            new CssBuilder("mud-input-slot")
                .AddClass("mud-input-root")
                .AddClass($"mud-input-root-{baseInput.Variant.ToDescriptionString()}")
                .AddClass($"mud-input-root-margin-{baseInput.Margin.ToDescriptionString()}", when: () => baseInput.Margin != Margin.None)
                .AddClass("ms-4", baseInput.AdornmentStart != null && baseInput.Variant == Variant.Text)
                .AddClass(baseInput.Class)
                .Build();

        public static string GetAdornmentClassname<T>(MudBaseInputExtended<T> baseInput) =>
            new CssBuilder("mud-input-adornment")
                .AddClass($"mud-input-adornment-start", baseInput.AdornmentStart != null)
                .AddClass($"mud-input-adornment-end", baseInput.AdornmentEnd != null)
                .AddClass($"mud-text", !string.IsNullOrEmpty(baseInput.AdornmentText))
                .AddClass($"mud-input-root-filled-shrink", baseInput.Variant == Variant.Filled)
                .AddClass(baseInput.Class)
                .Build();
    }
}
