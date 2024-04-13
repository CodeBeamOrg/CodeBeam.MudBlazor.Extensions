using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace MudExtensions.Utilities
{
    /// <summary>
    /// 
    /// </summary>
    public class MudTeleportManager
    {
        private IJSRuntime JSRuntime;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="jsRuntime"></param>
        public MudTeleportManager(IJSRuntime jsRuntime)
        {
            JSRuntime = jsRuntime;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reference"></param>
        /// <param name="toTeleport"></param>
        /// <returns></returns>
        public async Task<string?> Teleport(ElementReference reference, string? toTeleport)
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reference"></param>
        /// <returns></returns>
        public async Task RemoveFromDom(ElementReference reference)
        {
            await JSRuntime.InvokeVoidAsync("mudTeleport.removeFromDOM", reference);
        }

    }
}
