using Microsoft.AspNetCore.Components;
using MudBlazor;
using MudExtensions.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudExtensions
{
    public partial class MudStep : MudComponentBase, IDisposable
    {

        [CascadingParameter]
        protected MudStepper MudStepper { get; set; }

        [Parameter]
        public string Title { get; set; }

        [Parameter]
        public StepStatus Status { get; set; } = StepStatus.Continued;

        /// <summary>
        /// If true the step is skippable.
        /// </summary>
        [Parameter]
        public bool Optional { get; set; }

        /// <summary>
        /// If true, the step show when the stepper is completed. There should be only one result step.
        /// </summary>
        [Parameter]
        public bool IsResultStep { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            if (IsResultStep == false)
            {
                MudStepper.AddStep(this);
            }
        }

        protected internal void SetStatus(StepStatus status)
        {
            Status = status;
        }

        public void Dispose()
        {
            try
            {
                MudStepper?.RemoveStep(this);
            }
            catch (Exception) { }
        }

    }
}