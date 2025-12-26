// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Bunit;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;
using MudExtensions.Docs.Services;
using MudExtensions.Services;

namespace MudExtensions.UnitTests.Components
{
    public abstract class BunitTest
    {
        protected BunitContext Context { get; private set; }

        [SetUp]
        public virtual void Setup()
        {
            Context = new();
            Context.JSInterop.Mode = JSRuntimeMode.Loose;
            Context.Services.AddMudServices(options =>
            {
                options.SnackbarConfiguration.ShowTransitionDuration = 0;
                options.SnackbarConfiguration.HideTransitionDuration = 0;
                options.PopoverOptions.CheckForPopoverProvider = false;
            });
            Context.Services.AddMudExtensions();
            Context.Services.AddScoped<MudExtensionsDocsService>();
        }

        [TearDown]
        public void TearDown()
        {
            try
            {
                Context.Dispose();
            }
            catch (Exception)
            {
                /*ignore*/
            }
        }

        protected async Task ImproveChanceOfSuccess(Func<Task> testAction)
        {
            for (int i = 0; i < 10; i++)
            {
                try
                {
                    await testAction();
                    return;
                }
                catch(Exception) { /*we don't care here*/ }
            }
            await testAction();
        }
    }
}
