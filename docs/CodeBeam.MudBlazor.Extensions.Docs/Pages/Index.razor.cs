using Microsoft.AspNetCore.Components;
using MudBlazor;
using MudExtensions.Docs.Services;
using MudExtensions.Docs.Shared;
using static MudBlazor.Colors;

namespace MudExtensions.Docs.Pages
{
    public partial class Index
    {
        [CascadingParameter]
        MainLayout? MainLayout { get; set; }

        List<MudExtensionComponentInfo> _components = new();
        MudExtensionComponentInfo? _searchedComponent;
        MudAnimate _animate = new();

        bool _navigating = false;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            _components = DocsService.GetAllComponentInfo();
        }

        private string Version
        {
            get
            {
                var v = typeof(MudAnimate).Assembly.GetName().Version;
                return $"v {v?.Major}.{v?.Minor}.{v?.Build}";
            }
        }

        private async Task<IEnumerable<MudExtensionComponentInfo>> ComponentSearch(string value, CancellationToken token)
        {
            // In real life use an asynchronous function for fetching data from an api.
            await Task.Delay(1, token);

            // if text is null or empty, show complete list
            if (string.IsNullOrEmpty(value))
                return _components;
            return _components.Where(x => x?.Component?.Name.Contains(value, StringComparison.InvariantCultureIgnoreCase) == true || x?.Usage.ToDescriptionString().Equals(value, StringComparison.CurrentCultureIgnoreCase) == true || (value.ToLowerInvariant() == "material" && x.IsMaterial3 == true));
        }

        private async Task NavigateSelectedComponent()
        {
            _navigating = true;
            StateHasChanged();
            await Task.Delay(2000);
            NavigationManager.NavigateTo($"/{_searchedComponent?.Component?.Name.Replace("`1", null).ToLowerInvariant()}");
            if (_searchedComponent == null)
            {
                _navigating = false;
            }
        }

        private async Task HandleCompCardClick(MudExtensionComponentInfo component)
        {
            if (component == null)
            {
                return;
            }
            _searchedComponent = component;
            await NavigateSelectedComponent();
        }

        private string GetSectionIcon(ComponentUsage componentUsage)
        {
            if (componentUsage == ComponentUsage.Layout)
            {
                return Icons.Material.Filled.Dashboard;
            }
            else if (componentUsage == ComponentUsage.Button)
            {
                return Icons.Material.Filled.Dataset;
            }
            else if (componentUsage == ComponentUsage.Input)
            {
                return Icons.Material.Filled.AppRegistration;
            }
            else if (componentUsage == ComponentUsage.Display)
            {
                return Icons.Material.Filled.Ballot;
            }
            else if (componentUsage == ComponentUsage.Utility)
            {
                return Icons.Material.Filled.AutoAwesome;
            }

            return Icons.Material.Filled.Dashboard;
        }

    }
}
