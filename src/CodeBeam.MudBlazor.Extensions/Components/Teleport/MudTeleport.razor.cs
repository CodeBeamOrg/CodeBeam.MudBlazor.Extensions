using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using MudBlazor.Utilities;
using MudExtensions.Utilities;

namespace MudExtensions
{
    /// <summary>
    /// Teleport content to different containers.
    /// </summary>
    public partial class MudTeleport : MudComponentBase
    {
        private string _generatedClass = "teleport" + Guid.NewGuid().ToString().Substring(0, 8);

        /// <summary>
        /// 
        /// </summary>
        protected string? Classname => new CssBuilder()
            .AddClass(_generatedClass)
            .AddClass(Class)
            .Build();

        /// <summary>
        /// The class name that shows the parent which the teleport content will teleport.
        /// </summary>
        [Parameter] public string? To { get; set; }

        /// <summary>
        /// The class name that used to return teleport content to the container.
        /// </summary>
        [Parameter] public string? OwnClass { get; set; }

        /// <summary>
        /// If true teleported content returns the container, otherwise the content remains the last teleported place. Default is false.
        /// </summary>
        [Parameter] public bool ReturnWhenNotFound { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Parameter] public RenderFragment? ChildContent { get; set; }

        private ElementReference _ref;

        private string? _to;
        private bool _mustUpdate = true;

        /// <summary>
        /// 
        /// </summary>
        protected override void OnParametersSet()
        {
            // if `To` or `Disabled` has changed we must update the teleport
            if (To != null && (!To.Equals(_to)))
            {
                _to = To;
                _mustUpdate = true;
            }
            else
            {
                _mustUpdate = false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="firstRender"></param>
        /// <returns></returns>
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (_mustUpdate)
            {
                await Update();
            }
            await base.OnAfterRenderAsync(firstRender);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task Update()
        {
            var result = await MudTeleportManager.Teleport(_ref, To);
            if (result == "not found" && ReturnWhenNotFound == true)
            {
                await MudTeleportManager.Teleport(_ref, _generatedClass);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task Reset()
        {
            To = null;
            await MudTeleportManager.Teleport(_ref, _generatedClass);
            StateHasChanged();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async ValueTask DisposeAsync()
        {
            try
            {
                await MudTeleportManager.RemoveFromDom(_ref);
            }
            catch (JSDisconnectedException) { }
            catch (TaskCanceledException) { }
            catch (InvalidOperationException) { /* it throws in the server side project */ }
        }
    }
}
