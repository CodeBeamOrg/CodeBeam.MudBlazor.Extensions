using Microsoft.JSInterop;
using MudBlazor;
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
        /// Scrolls to the coordinates of the element.
        /// </summary>
        /// <param name="id">id of element</param>
        /// <param name="left">x coordinate</param>
        /// <param name="top">y coordinate</param>
        /// <param name="behavior">smooth or auto</param>
        /// <returns></returns>
        public ValueTask ScrollToAsync(string id, int left, int top, ScrollBehavior behavior) =>
            _jSRuntime.InvokeVoidAsync("mudScrollManager.scrollTo", id, left, top, behavior.ToDescriptionString());

        /// <summary>
        /// Scrolls the first instance of the selector into view.
        /// </summary>
        /// <param name="selector"></param>
        /// <param name="behavior"></param>
        /// <returns></returns>
        public ValueTask ScrollIntoViewAsync(string selector, ScrollBehavior behavior) =>
            _jSRuntime.InvokeVoidAsync("mudScrollManager.scrollIntoView", selector, behavior.ToDescriptionString());

        /// <summary>
        /// Scrolls to the top of the element.
        /// </summary>
        /// <param name="id">id of element</param>
        /// <param name="scrollBehavior">smooth or auto</param>
        /// <returns></returns>
        public ValueTask ScrollToTopAsync(string id, ScrollBehavior scrollBehavior = ScrollBehavior.Auto) =>
            ScrollToAsync(id, 0, 0, scrollBehavior);

        /// <summary>
        /// Scroll to the bottom of the element (or if not found to the bottom of the page).
        /// </summary>
        /// <param name="id">id of element of null to scroll to page bottom</param>
        /// <param name="behavior">smooth or auto</param>
        /// <returns></returns>
        public ValueTask ScrollToBottomAsync(string id, ScrollBehavior behavior) =>
            _jSRuntime.InvokeVoidAsync("mudScrollManager.scrollToBottom", id, behavior.ToDescriptionString());

        /// <summary>
        /// 
        /// </summary>
        /// <param name="elementId"></param>
        /// <returns></returns>
        public ValueTask ScrollToYearAsync(string elementId) =>
            _jSRuntime.InvokeVoidAsync("mudScrollManager.scrollToYear", elementId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="elementId"></param>
        /// <returns></returns>
        public ValueTask ScrollToListItemAsync(string elementId) =>
            _jSRuntime.InvokeVoidAsync("mudScrollManager.scrollToListItem", elementId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="selector"></param>
        /// <param name="cssClass"></param>
        /// <returns></returns>
        public ValueTask LockScrollAsync(string selector = "body", string cssClass = "scroll-locked") =>
            _jSRuntime.InvokeVoidAsync("mudScrollManager.lockScroll", selector, cssClass);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="selector"></param>
        /// <param name="cssClass"></param>
        /// <returns></returns>
        public ValueTask UnlockScrollAsync(string selector = "body", string cssClass = "scroll-locked") =>
            _jSRuntime.InvokeVoidAsyncIgnoreErrors("mudScrollManager.unlockScroll", selector, cssClass);

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
#pragma warning disable CS1591
        Smooth,
        Auto
    }
}
