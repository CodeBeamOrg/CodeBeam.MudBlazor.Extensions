using Microsoft.AspNetCore.Components;
using MudBlazor;
using MudBlazor.Extensions;
using MudBlazor.Utilities;
using MudExtensions.Enums;
using MudExtensions.Utilities;

namespace MudExtensions
{
    public partial class MudStepper : MudComponentBase
    {
        MudAnimate _animate;
        Guid _animateGuid = Guid.NewGuid();

        protected string HeaderClassname => new CssBuilder("d-flex align-center gap-4 pa-2")
            .AddClass("cursor-pointer mud-stepper-header-non-linear", !Linear)
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

        protected string GetStepperStyle()
        {
            var count = Steps.Count * 2;
            string style = $"display: grid;grid-template-columns: repeat({count}, 1fr);";
            return style;
        }

        protected string GetStepPercent()
        {
            var dPercent = 100.0 / Steps.Count;

            string percent = $"width:{dPercent}%";

            return percent;
        }

        protected string GetProgressLinearStyle()
        {
            var colEnd = Steps.Count * 2;
            string style = $"z-index: -10;grid-column-start: 2; grid-column-end: {colEnd};grid-row: 1 / -1;display: inline-grid;top:26px";
            return style;
        }

        private int _activeIndex;
        internal int ActiveIndex 
        {
            get => _activeIndex; 
            set
            {
                _activeIndex = value;
                ProgressValue = _activeIndex * (100.0 / (Steps.Count - 1));
            }
        }

        internal double ProgressValue;

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
        /// If true, disables built-in "previous" step action button.
        /// </summary>
        [Parameter]
        public bool DisablePreviousButton { get; set; }

        /// <summary>
        /// If true, disables built-in "next" step action button.
        /// </summary>
        [Parameter]
        public bool DisableNextButton { get; set; }

        /// <summary>
        /// If true, disables built-in "skip" step action button.
        /// </summary>
        [Parameter]
        public bool DisableSkipButton { get; set; }

        /// <summary>
        /// If true, disables built-in "completed"/"skipped" step result indictors shown in the actions panel.
        /// </summary>
        [Parameter]
        public bool DisableStepResultIndicator { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Parameter]
        public bool MobileView { get; set; }

        /// <summary>
        /// If true, a linear loading indicator shows under the header.
        /// </summary>
        [Parameter]
        public bool Loading { get; set; }

        /// <summary>
        /// A static content that always show with all steps.
        /// </summary>
        [Parameter]
        public RenderFragment StaticContent { get; set; }

        /// <summary>
        /// If true, action buttons have icons instead of text to gain more space.
        /// </summary>
        [Parameter]
        public bool IconActionButtons { get; set; }

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
        /// Custom content to be shown between the "previous" and "next" action buttons.
        /// </summary>
        /// <remarks>
        /// If set, you must also supply a <code><MudSpacer /></code> somewhere in your render fragment
        /// to ensure that the built-in action buttons are aligned correctly.
        /// </remarks>
        [Parameter]
        public RenderFragment ActionContent { get; set; }

        [Parameter]
        public EventCallback<int> ActiveStepChanged { get; set; }

        [Obsolete("Use PreventStepChangeAsync instead.")]
        [Parameter]
        public Func<StepChangeDirection, bool> PreventStepChange { get; set; }

        [Parameter]
        public Func<StepChangeDirection, Task<bool>> PreventStepChangeAsync { get; set; }

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
            if (!step.IsResultStep)
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

        protected internal async Task SetActiveIndex(MudStep step)
        {
            if (_animate != null)
            {
                await _animate.Refresh();
            }
            ActiveIndex = Steps.IndexOf(step);
            await ActiveStepChanged.InvokeAsync(ActiveIndex);
        }

        public async Task SetActiveIndex(int count, bool firstCompleted = false, bool skipPreventProcess = false)
        {
            var stepChangeDirection = (
                count == 0 ? StepChangeDirection.None :
                    count >= 1 ? StepChangeDirection.Forward :
                        StepChangeDirection.Backward
            );

            if (!skipPreventProcess && PreventStepChange != null && PreventStepChange.Invoke(stepChangeDirection))
            {
                return;
            }

            if (PreventStepChangeAsync != null)
            {
                var result = await PreventStepChangeAsync.Invoke(stepChangeDirection);
                if (result == true)
                {
                    return;
                }
            }
            

            int backupActiveIndex = ActiveIndex;
            if (_animate != null)
            {
                await _animate.Refresh();
            }

            if (ActiveIndex == Steps.Count - 1 && !HasResultStep() && 0 < count)
            {
                return;
            }
            else if (firstCompleted)
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
            else if (ActiveIndex == Steps.Count - 1 && !IsAllStepsCompleted() && 0 < count)
            {
                ActiveIndex = Steps.IndexOf(Steps.FirstOrDefault(x => x.Status == StepStatus.Continued));
            }
            else
            {
                ActiveIndex += count;
            }

            if (backupActiveIndex != ActiveIndex)
            {
                await ActiveStepChanged.InvokeAsync(ActiveIndex);
            }
        }

        public async Task SetActiveStepByIndex(int index, bool firstCompleted = false, bool skipPreventProcess = false)
        {
            var stepChangeDirection = (
                index == ActiveIndex ? StepChangeDirection.None :
                    index > ActiveIndex ? StepChangeDirection.Forward :
                        StepChangeDirection.Backward
            );

            if (!skipPreventProcess && PreventStepChange != null && PreventStepChange.Invoke(stepChangeDirection))
            {
                return;
            }

            if (ActiveIndex == index || index < 0 || Steps.Count < index)
            {
                return;
            }

            if (Steps.Count == index && IsAllStepsCompleted() == false)
            {
                return;
            }

            if (_animate != null)
            {
                await _animate.Refresh();
            }

            ActiveIndex = index;
            await ActiveStepChanged.InvokeAsync(ActiveIndex);
        }

        public async Task CompleteStep(int index, bool moveToNextStep = true)
        {
            var isActiveStep = (index == ActiveIndex);
            if (isActiveStep)
            {
                var stepChangeDirection = (moveToNextStep ? StepChangeDirection.Forward : StepChangeDirection.None);
                if (PreventStepChange != null && PreventStepChange.Invoke(stepChangeDirection))
                {
                    return;
                }

                if (PreventStepChangeAsync != null)
                {
                    var result = await PreventStepChangeAsync.Invoke(stepChangeDirection);
                    if (result == true)
                    {
                        return;
                    }
                }
            }

            Steps[index].SetStatus(StepStatus.Completed);
            if (IsAllStepsCompleted())
            {
                await SetActiveIndex(1, true, true);
            }
            else if (isActiveStep && moveToNextStep)
            {
                await SetActiveIndex(1, skipPreventProcess: true);
            }
        }

        public async Task SkipStep(int index, bool moveToNextStep = true)
        {
            var isActiveStep = (index == ActiveIndex);
            if (isActiveStep)
            {
                var stepChangeDirection = (moveToNextStep ? StepChangeDirection.Forward : StepChangeDirection.None);
                if (PreventStepChange != null && PreventStepChange.Invoke(stepChangeDirection))
                {
                    return;
                }

                if (PreventStepChangeAsync != null)
                {
                    var result = await PreventStepChangeAsync.Invoke(stepChangeDirection);
                    if (result == true)
                    {
                        return;
                    }
                }
            }

            Steps[index].SetStatus(StepStatus.Skipped);
            if (isActiveStep && moveToNextStep)
            {
                await SetActiveIndex(1, skipPreventProcess: true);
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
