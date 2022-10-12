using CodeBeam.MudExtensions.Utilities;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using MudBlazor.Extensions;
using MudBlazor.Utilities;
using MudExtensions.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using static MudBlazor.Colors;

namespace MudExtensions
{
    public partial class MudStepper : MudComponentBase
    {
        MudAnimate _animate;

        protected string HeaderClassname => new CssBuilder("d-flex align-center mud-stepper-header gap-4 pa-2")
            .AddClass("mud-ripple", DisableRipple == false && Linear == false)
            .AddClass("cursor-pointer mud-stepper-header-non-linear", Linear == false)
            .AddClass("flex-column", HeaderTextView == HeaderTextView.NewLine)
            .Build();
        
        protected string GetDashClassname(MudStep step)
        {
            return new CssBuilder("mud-stepper-header-dash flex-grow-1 mx-auto")
                .AddClass("mud-stepper-header-dash-completed", step.Status != StepStatus.Continued)
                .AddClass("mud-stepper-header-dash-vertical", Vertical)
                .AddClass("mt-5", HeaderTextView == HeaderTextView.NewLine)
                .Build();
        }

        [Parameter]
        public int ActiveIndex { get; set; }

        /// <summary>
        /// If true, the header can not be clickable and users can step one by one.
        /// </summary>
        [Parameter]
        public bool Linear { get; set; }

        /// <summary>
        /// If true, disables ripple effect when click on step headers.
        /// </summary>
        [Parameter]
        public bool DisableRipple { get; set; }

        /// <summary>
        /// If true, disables the default animation on step changing.
        /// </summary>
        [Parameter]
        public bool DisableAnimation { get; set; }

        [Parameter]
        public Color Color { get; set; } = Color.Default;

        [Parameter]
        public Variant Variant { get; set; }

        [Parameter]
        public HeaderTextView HeaderTextView { get; set; } = HeaderTextView.All;

        [Parameter]
        public bool Vertical { get; set; }

        [Parameter]
        public StepperLocalizedStrings LocalizedStrings { get; set; } = new();

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        /// <summary>
        /// Overrides the action buttons (previous, next etc.) with custom render fragment.
        /// </summary>
        [Parameter]
        public RenderFragment ActionContent { get; set; }

        [Parameter]
        public EventCallback<int> ActiveStepChanged { get; set; }

        List<MudStep> _steps = new();

        public List<MudStep> Steps
        {
            get => _steps;
            protected set
            {
                if (_steps.Equals(value))
                {
                    return;
                }
                if (_steps.Select(x => x.GetHashCode()).Contains(value.GetHashCode()))
                {
                    return;
                }
                _steps = value;
            }
        }

        internal void AddStep(MudStep step)
        {
            _steps.Add(step);

            StateHasChanged();
        }

        internal void RemoveStep(MudStep step)
        {
            Steps.Remove(step);

            StateHasChanged();
        }

        protected async Task SetActiveIndex(MudStep step)
        {
            ActiveIndex = Steps.IndexOf(step);
            await ActiveStepChanged.InvokeAsync();
            if (_animate != null)
            {
                await _animate.Refresh();
            }
        }

        protected async Task SetActiveIndex(int count, bool firstCompleted = false)
        {
            if (firstCompleted == true)
            {
                ActiveIndex = Steps.Count;
            }
            if (ActiveIndex + count < 0)
            {
                ActiveIndex = 0;
            }
            else if (ActiveIndex == Steps.Count - 1 && IsAllStepsCompleted() == false)
            {
                ActiveIndex = Steps.IndexOf(Steps.FirstOrDefault(x => x.Status == StepStatus.Continued));
            }
            else
            {
                ActiveIndex += count;
            }
            await ActiveStepChanged.InvokeAsync();
            if (_animate != null)
            {
                await _animate.Refresh();
            }
        }

        public async Task CompleteStep(int index, bool moveToNextStep = true)
        {
            Steps[index].SetStatus(StepStatus.Completed);
            if (IsAllStepsCompleted())
            {
                await SetActiveIndex(0, true);
            }
            else if (moveToNextStep)
            {
                await SetActiveIndex(1);
            }
        }

        public async Task SkipStep(int index, bool moveToNextStep = true)
        {
            Steps[index].SetStatus(StepStatus.Skipped);
            if (moveToNextStep)
            {
                await SetActiveIndex(1);
            }
        }

        public void MoveNextStep()
        {

        }

        protected bool IsStepActive(MudStep step)
        {
            return Steps.IndexOf(step) == ActiveIndex;
        }

        public bool IsAllStepsCompleted()
        {
            return !Steps.Any(x => x.Status == Enums.StepStatus.Continued);
        }

        protected int CompletedStepCount()
        {
            return Steps.Where(x => x.Status != Enums.StepStatus.Continued).Count();
        }

        protected string GetNextButtonString()
        {
            if (Steps.Count - 1 == CompletedStepCount())
            {
                return LocalizedStrings.Finish;
            }
            else
            {
                return LocalizedStrings.Next;
            }
        }

        internal bool ShowResultStep()
        {
            if (IsAllStepsCompleted() && ActiveIndex == Steps.Count)
            {
                return true;
            }

            return false;
        }

        public void Reset()
        {
            Steps.ForEach(x => x.SetStatus(StepStatus.Continued));
            ActiveIndex = 0;
        }

    }
}
