using Microsoft.AspNetCore.Components;
using MudBlazor;
using MudExtensions.Components.TypographyM3;

namespace MudExtensions
{
    /// <summary>
    /// 
    /// </summary>
    public partial class MudTypographyProvider : MudComponentBase
    {
        /// <summary>
        /// Typography definitions
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Item.Behavior)]
        public TypographyM3 TypographyM3 { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public MudTypographyProvider()
            :base()
        {
            TypographyM3 = new();
        }
    }
}
