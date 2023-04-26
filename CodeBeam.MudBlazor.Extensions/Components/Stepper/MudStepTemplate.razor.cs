using Microsoft.AspNetCore.Components;

namespace MudExtensions
{
    public partial class MudStepTemplate : ComponentBase
    {
        [Parameter]
        public MudStep? Step { get; set; }
        public MudStepTemplate()
            : base()
        {

        }
    }
}
