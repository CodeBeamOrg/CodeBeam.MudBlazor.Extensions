using Microsoft.AspNetCore.Components;

namespace ComponentViewer.Components
{
    public partial class ComponentCard
    {
        [Inject] NavigationManager NavigationManager { get; set; }

        [Parameter]
        public string Title { get; set; }

        [Parameter]
        public string ComponentName { get; set; }

        [Parameter]
        public string Description { get; set; }

        private void NavigateComponentPage()
        {
            NavigationManager.NavigateTo($"/{(string.IsNullOrEmpty(ComponentName) ? Title.ToLowerInvariant() : ComponentName.ToLowerInvariant())}");
        }
    }
}
