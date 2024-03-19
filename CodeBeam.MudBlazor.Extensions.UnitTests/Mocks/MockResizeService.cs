using System;
using System.Threading.Tasks;
using MudBlazor;
using MudBlazor.Services;

namespace MudExtensions.UnitTests.Mocks
{
#pragma warning disable CS1998 // Justification - Implementing IResizeListenerService
    [Obsolete("Replaced by IBrowserViewportService. Remove in v7.")]
    public class MockResizeService : IResizeService
    {
        private int _width, _height;

        internal void ApplyScreenSize(int width, int height)
        {
            _width = width;
            _height = height;

            OnResized?.Invoke(this, new BrowserWindowSize()
            {
                Width = _width,
                Height = _height
            });
            OnBreakpointChanged?.Invoke(this, GetBreakpointInternal());
        }

#nullable enable
#pragma warning disable CS0414 // justification implementing interface  
        public event EventHandler<BrowserWindowSize>? OnResized;
        public event EventHandler<Breakpoint>? OnBreakpointChanged;
#pragma warning restore CS0414 
#nullable disable
        public async ValueTask<BrowserWindowSize> GetBrowserWindowSize()
        {
            return new BrowserWindowSize()
            {
                Width = _width,
                Height = _height
            };
        }

        public async Task<bool> IsMediaSize(Breakpoint breakpoint)
        {
            if (breakpoint == Breakpoint.None)
                return false;

            return IsMediaSize(breakpoint, await GetBreakpoint());
        }

        public bool IsMediaSize(Breakpoint breakpoint, Breakpoint reference)
        {
            if (breakpoint == Breakpoint.None)
                return false;

            return breakpoint switch
            {
                Breakpoint.Xs => reference == Breakpoint.Xs,
                Breakpoint.Sm => reference == Breakpoint.Sm,
                Breakpoint.Md => reference == Breakpoint.Md,
                Breakpoint.Lg => reference == Breakpoint.Lg,
                Breakpoint.Xl => reference == Breakpoint.Xl,
                // * and down
                Breakpoint.SmAndDown => reference <= Breakpoint.Sm,
                Breakpoint.MdAndDown => reference <= Breakpoint.Md,
                Breakpoint.LgAndDown => reference <= Breakpoint.Lg,
                // * and up
                Breakpoint.SmAndUp => reference >= Breakpoint.Sm,
                Breakpoint.MdAndUp => reference >= Breakpoint.Md,
                Breakpoint.LgAndUp => reference >= Breakpoint.Lg,
                _ => false,
            };
        }

        public async Task<Breakpoint> GetBreakpoint() => GetBreakpointInternal();

        [Obsolete($"Use {nameof(SubscribeAsync)} instead. This will be removed in v7.")]
        public Task<Guid> Subscribe(Action<BrowserWindowSize> callback) => SubscribeAsync(callback);

        public Task<Guid> SubscribeAsync(Action<BrowserWindowSize> callback) => Task.FromResult(new Guid());

        [Obsolete($"Use {nameof(SubscribeAsync)} instead. This will be removed in v7.")]
        public Task<Guid> Subscribe(Action<BrowserWindowSize> callback, ResizeOptions options) => SubscribeAsync(callback, options);

        public Task<Guid> SubscribeAsync(Action<BrowserWindowSize> callback, ResizeOptions options) => Task.FromResult(new Guid());

        [Obsolete($"Use {nameof(UnsubscribeAsync)} instead. This will be removed in v7.")]
        public Task<bool> Unsubscribe(Guid subscriptionId) => UnsubscribeAsync(subscriptionId);

        public Task<bool> UnsubscribeAsync(Guid subscriptionId) => Task.FromResult(true);

        private Breakpoint GetBreakpointInternal()
        {
            if (_width >= BreakpointGlobalOptions.DefaultBreakpointDefinitions[Breakpoint.Xl])
                return Breakpoint.Xl;
            else if (_width >= BreakpointGlobalOptions.DefaultBreakpointDefinitions[Breakpoint.Lg])
                return Breakpoint.Lg;
            else if (_width >= BreakpointGlobalOptions.DefaultBreakpointDefinitions[Breakpoint.Md])
                return Breakpoint.Md;
            else if (_width >= BreakpointGlobalOptions.DefaultBreakpointDefinitions[Breakpoint.Sm])
                return Breakpoint.Sm;
            else
                return Breakpoint.Xs;
        }

        public ValueTask DisposeAsync()
        {
            OnResized = null;
            OnBreakpointChanged = null;
            return ValueTask.CompletedTask;
        }
    }
#pragma warning restore CS1998

    internal class BreakpointGlobalOptions
    {
        /// <summary>
        /// Default  breakpoint definitions
        /// </summary>
        internal static Dictionary<Breakpoint, int> DefaultBreakpointDefinitions { get; set; } = new()
        {
            [Breakpoint.Xxl] = 2560,
            [Breakpoint.Xl] = 1920,
            [Breakpoint.Lg] = 1280,
            [Breakpoint.Md] = 960,
            [Breakpoint.Sm] = 600,
            [Breakpoint.Xs] = 0,
        };

        /// <summary>
        /// Retrieves the default or user-defined breakpoint definitions based on the provided <paramref name="options"/>.
        /// If user-defined breakpoint definitions are available in the <paramref name="options"/>, a copy is returned to prevent unintended modifications.
        /// Otherwise, the default <see cref="DefaultBreakpointDefinitions"/> breakpoint definitions are returned.
        /// </summary>
        /// <param name="options">The resize options containing breakpoint definitions, if any.</param>
        /// <returns>A dictionary containing the breakpoint definitions.</returns>
        internal static Dictionary<Breakpoint, int> GetDefaultOrUserDefinedBreakpointDefinition(ResizeOptions options)
        {
            if (options.BreakpointDefinitions is not null && options.BreakpointDefinitions.Count != 0)
            {
                // Copy as we don't want any unexpected modification
                return options.BreakpointDefinitions.ToDictionary(entry => entry.Key, entry => entry.Value);
            }

            return DefaultBreakpointDefinitions.ToDictionary(entry => entry.Key, entry => entry.Value);
        }
    }

}
