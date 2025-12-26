using Microsoft.AspNetCore.Components;

namespace MudExtensions
{
    /// <summary>
    /// 
    /// </summary>
    public partial class MudStepTemplate : ComponentBase
    {
        /// <summary>
        /// 
        /// </summary>
        [Parameter]
        public MudStepExtended? Step { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public MudStepTemplate()
            : base()
        {

        }
    }
}
