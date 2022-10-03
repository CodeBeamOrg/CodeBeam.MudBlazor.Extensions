using Microsoft.AspNetCore.Components;

namespace ComponentViewer.Wasm.Components
{
    public partial class ComponentCard
    {
        [Inject] NavigationManager NavigationManager { get; set; }

        [Parameter]
        public string Title { get; set; }

        [Parameter]
        public string Description { get; set; }

        private void NavigateComponentPage()
        {
            NavigationManager.NavigateTo($"/{Title.ToLower()}");
        }
    }
}
