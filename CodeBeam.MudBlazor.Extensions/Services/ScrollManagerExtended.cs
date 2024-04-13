using Microsoft.JSInterop;
using MudBlazor.Extensions;

namespace MudExtensions.Services
{
    /// <summary>
    /// Inject with the AddMudBlazorScrollServices extension
    /// </summary>
    public interface IScrollManagerExtended
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="parentId"></param>
        /// <param name="childId"></param>
        /// <returns></returns>
        ValueTask ScrollToMiddleAsync(string parentId, string childId);
    }

    /// <summary>
    /// 
    /// </summary>
    public class ScrollManagerExtended : IScrollManagerExtended
    {
        /// <summary>
        /// 
        /// </summary>
        [Obsolete]
        public string? Selector { get; set; }
        private readonly IJSRuntime _jSRuntime;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="jSRuntime"></param>
        public ScrollManagerExtended(IJSRuntime jSRuntime)
        {
            _jSRuntime = jSRuntime;

        }

        /// <summary>
        /// Scroll to an url fragment
        /// </summary>
        /// <param name="id">The id of the selector that is going to be scrolled to</param>
        /// <param name="behavior">smooth or auto</param>
        /// <returns></returns>
        public ValueTask ScrollToFragmentAsync(string id, ScrollBehavior behavior) =>
            _jSRuntime.InvokeVoidAsync("mudScrollManager.scrollToFragment", id, behavior.ToDescriptionString());

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="behavior"></param>
        /// <returns></returns>
        [Obsolete]
        public async Task ScrollToFragment(string id, ScrollBehavior behavior) =>
            await ScrollToFragmentAsync(id, behavior);

        /// <summary>
        /// Scrolls to the coordinates of the element
        /// </summary>
        /// <param name="id">id of element</param>
        /// <param name="left">x coordinate</param>
        /// <param name="top">y coordinate</param>
        /// <param name="behavior">smooth or auto</param>
        /// <returns></returns>
        public ValueTask ScrollToAsync(string? id, int left, int top, ScrollBehavior behavior) =>
            _jSRuntime.InvokeVoidAsync("mudScrollManager.scrollTo", id, left, top, behavior.ToDescriptionString());

        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="top"></param>
        /// <param name="behavior"></param>
        /// <returns></returns>
        [Obsolete]
        public async Task ScrollTo(int left, int top, ScrollBehavior behavior) =>
            await ScrollToAsync(Selector, left, top, behavior);

        /// <summary>
        /// Scrolls to the top of the element
        /// </summary>
        /// <param name="id">id of element</param>
        /// <param name="scrollBehavior">smooth or auto</param>
        /// <returns></returns>
        public ValueTask ScrollToTopAsync(string id, ScrollBehavior scrollBehavior = ScrollBehavior.Auto) =>
            ScrollToAsync(id, 0, 0, scrollBehavior);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scrollBehavior"></param>
        /// <returns></returns>
        public async Task ScrollToTop(ScrollBehavior scrollBehavior = ScrollBehavior.Auto)
        {
#pragma warning disable CS0612 // Type or member is obsolete
            await ScrollToAsync(Selector, 0, 0, scrollBehavior);
#pragma warning restore CS0612 // Type or member is obsolete
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parentId"></param>
        /// <param name="childId"></param>
        /// <returns></returns>
        public ValueTask ScrollToMiddleAsync(string parentId, string childId) =>
            _jSRuntime.InvokeVoidAsync("mudScrollManagerExtended.scrollToMiddle", parentId, childId);
    }

    /// <summary>
    /// Smooth: scrolls in a smooth fashion;
    /// Auto: is immediate
    /// </summary>
    public enum ScrollBehavior
    {
        Smooth,
        Auto
    }
}
