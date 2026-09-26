using Bunit;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;
using MudExtensions.Services;

namespace MudExtensions.Benchmarks;

internal static class BenchmarkBunitContext
{
    public static BunitContext Create()
    {
        var context = new BunitContext();
        context.JSInterop.Mode = JSRuntimeMode.Loose;
        context.Services.AddMudServices(options =>
        {
            options.SnackbarConfiguration.ShowTransitionDuration = 0;
            options.SnackbarConfiguration.HideTransitionDuration = 0;
            options.PopoverOptions.CheckForPopoverProvider = false;
        });
        context.Services.AddMudExtensions();
        return context;
    }
}
