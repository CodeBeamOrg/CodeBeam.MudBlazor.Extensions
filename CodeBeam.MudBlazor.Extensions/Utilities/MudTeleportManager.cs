using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace MudExtensions.Utilities
{
    public class MudTeleportManager
    {
        private IJSRuntime JSRuntime;

        public MudTeleportManager(IJSRuntime jsRuntime)
        {
            JSRuntime = jsRuntime;
        }

        public async Task<string> Teleport(ElementReference reference, string toTeleport)
        {
            if (string.IsNullOrEmpty(toTeleport))
            {
                return null;
            }
            if (toTeleport.StartsWith('.') == false)
            {
                toTeleport = "." + toTeleport;
            }
            var result = await JSRuntime.InvokeAsync<string>("mudTeleport.teleport", reference, toTeleport);
            return result;
        }

        public async Task RemoveFromDom(ElementReference reference)
        {
            await JSRuntime.InvokeVoidAsync("mudTeleport.removeFromDOM", reference);
        }

    }
}
