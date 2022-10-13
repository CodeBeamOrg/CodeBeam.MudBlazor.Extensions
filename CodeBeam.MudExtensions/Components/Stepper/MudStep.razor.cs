using Microsoft.AspNetCore.Components;
using MudBlazor;
using MudBlazor.Utilities;
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

        protected string Classname => new CssBuilder()
            .AddClass("d-none", ((MudStepper.ActiveIndex < MudStepper.Steps.Count && MudStepper.Steps[MudStepper.ActiveIndex] != this) || (MudStepper.ShowResultStep() && IsResultStep == false)) || (IsResultStep && MudStepper.ShowResultStep() == false))
            .AddClass(Class)
            .Build();

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

            MudStepper.AddStep(this);
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