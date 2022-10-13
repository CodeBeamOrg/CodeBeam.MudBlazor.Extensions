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
        Guid _animateGuid = Guid.NewGuid();

        protected string HeaderClassname => new CssBuilder("d-flex align-center mud-stepper-header gap-4 pa-2")
            .AddClass("mud-ripple", DisableRipple == false && Linear == false)
            .AddClass("cursor-pointer mud-stepper-header-non-linear", Linear == false)
            .AddClass("flex-column", HeaderTextView == HeaderTextView.NewLine)
            .Build();

        protected string ContentClassname => new CssBuilder($"mud-stepper-ani-{_animateGuid.ToString()}")
            .AddClass(ContentClass)
            .Build();

        protected string GetDashClassname(MudStep step)
        {
            return new CssBuilder("mud-stepper-header-dash flex-grow-1 mx-auto")
                .AddClass("mud-stepper-header-dash-completed", step.Status != StepStatus.Continued)
                //.AddClass("mud-stepper-header-dash-vertical", Vertical)
                .AddClass("mt-5", HeaderTextView == HeaderTextView.NewLine)
                //.AddClass("dash-tiny", Vertical && ActiveIndex != Steps.IndexOf(step))
                .AddClass($"mud-stepper-border-{Color.ToDescriptionString()}")
                .Build();
        }

        internal int ActiveIndex { get; set; }

        /// <summary>
        /// Provides CSS classes for the step content.
        /// </summary>
        [Parameter]
        public string ContentClass { get; set; }

        /// <summary>
        /// Provides CSS styles for the step content.
        /// </summary>
        [Parameter]
        public string ContentStyle { get; set; }

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

        /// <summary>
        /// The predefined Mud color for header and action buttons.
        /// </summary>
        [Parameter]
        public Color Color { get; set; } = Color.Default;

        /// <summary>
        /// The variant for header and action buttons.
        /// </summary>
        [Parameter]
        public Variant Variant { get; set; }

        /// <summary>
        /// Choose header text view. Default is all.
        /// </summary>
        [Parameter]
        public HeaderTextView HeaderTextView { get; set; } = HeaderTextView.All;

        // TODO
        //[Parameter]
        //public bool Vertical { get; set; }

        /// <summary>
        /// A class for provide all local strings at once.
        /// </summary>
        [Parameter]
        public StepperLocalizedStrings LocalizedStrings { get; set; } = new();

        /// <summary>
        /// The child content where MudSteps should be inside.
        /// </summary>
        [Parameter]
        public RenderFragment ChildContent { get; set; }

        /// <summary>
        /// Overrides the action buttons (previous, next etc.) with custom render fragment.
        /// </summary>
        [Parameter]
        public RenderFragment ActionContent { get; set; }

        [Parameter]
        public EventCallback<int> ActiveStepChanged { get; set; }

        [Parameter]
        public Func<bool> PreventStepChange { get; set; }

        List<MudStep> _steps = new();
        List<MudStep> _allSteps = new();
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
            _allSteps.Add(step);
            if (step.IsResultStep == false)
            {
                Steps.Add(step);
            }

            StateHasChanged();
        }

        internal void RemoveStep(MudStep step)
        {
            Steps.Remove(step);
            _allSteps.Remove(step);
            StateHasChanged();
        }

        protected async Task SetActiveIndex(MudStep step)
        {
            if (_animate != null)
            {
                await _animate.Refresh();
            }
            ActiveIndex = Steps.IndexOf(step);
            await ActiveStepChanged.InvokeAsync();
        }

        protected async Task SetActiveIndex(int count, bool firstCompleted = false)
        {
            if (PreventStepChange.Invoke() == true)
            {
                return;
            }

            int backupActiveIndex = ActiveIndex;
            if (_animate != null)
            {
                await _animate.Refresh();
            }

            if (ActiveIndex == Steps.Count - 1 && HasResultStep() == false && 0 < count)
            {
                return;
            }
            else if (firstCompleted == true)
            {
                if (HasResultStep())
                {
                    ActiveIndex = Steps.Count;
                }
            }
            else if (ActiveIndex + count < 0)
            {
                ActiveIndex = 0;
            }
            else if (ActiveIndex == Steps.Count - 1 && IsAllStepsCompleted() == false && 0 < count)
            {
                ActiveIndex = Steps.IndexOf(Steps.FirstOrDefault(x => x.Status == StepStatus.Continued));
            }
            else
            {
                ActiveIndex += count;
            }

            if (backupActiveIndex != ActiveIndex)
            {
                await ActiveStepChanged.InvokeAsync();
            }
        }

        public async Task CompleteStep(int index, bool moveToNextStep = true)
        {
            if (PreventStepChange.Invoke() == true)
            {
                return;
            }

            Steps[index].SetStatus(StepStatus.Completed);
            if (IsAllStepsCompleted())
            {
                await SetActiveIndex(1, true);
            }
            else if (moveToNextStep)
            {
                await SetActiveIndex(1);
            }
        }

        public async Task SkipStep(int index, bool moveToNextStep = true)
        {
            if (PreventStepChange.Invoke() == true)
            {
                return;
            }

            Steps[index].SetStatus(StepStatus.Skipped);
            if (moveToNextStep)
            {
                await SetActiveIndex(1);
            }
        }

        protected bool IsStepActive(MudStep step)
        {
            return Steps.IndexOf(step) == ActiveIndex;
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

        protected internal bool ShowResultStep()
        {
            if (IsAllStepsCompleted() && ActiveIndex == Steps.Count)
            {
                return true;
            }

            return false;
        }

        protected internal bool HasResultStep()
        {
            return _allSteps.Any(x => x.IsResultStep);
        }

        public bool IsAllStepsCompleted()
        {
            return !Steps.Any(x => x.Status == Enums.StepStatus.Continued);
        }

        public int GetActiveIndex()
        {
            return ActiveIndex;
        }

        public void Reset()
        {
            Steps.ForEach(x => x.SetStatus(StepStatus.Continued));
            ActiveIndex = 0;
        }

    }
}
