using Microsoft.AspNetCore.Components;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudExtensions
{
    public partial class MudStep : MudComponentBase
    {
        [CascadingParameter]
        protected MudStepper MudStepper { get; set; }

        [Parameter]
        public string Title { get; set; }

        /// <summary>
        /// If true the step is skippable.
        /// </summary>
        [Parameter]
        public bool Optional { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            MudStepper.AddStep(this);
        }
    }
}