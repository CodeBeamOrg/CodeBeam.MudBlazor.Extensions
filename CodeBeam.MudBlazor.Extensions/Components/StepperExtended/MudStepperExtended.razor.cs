using MudExtensions.Utilities;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using MudBlazor.Extensions;
using MudBlazor.Utilities;

namespace MudExtensions
{
    /// <summary>
    /// Stepper component with extended features.
    /// </summary>
    public partial class MudStepperExtended : MudComponentBase
    {
        #region Parameters and Properties

        MudAnimate _animate = new();
        Guid _animateGuid = Guid.NewGuid();

        /// <summary>
        /// 
        /// </summary>
        protected string? HeaderClassname => new CssBuilder("d-flex align-center mud-stepper-header gap-4 pa-3")
            .AddClass("mud-ripple", Ripple && !Linear)
            .AddClass("cursor-pointer mud-stepper-header-non-linear", !Linear)
            .AddClass("flex-column", !Vertical)
            .AddClass("flex-row", Vertical)
            .Build();

        /// <summary>
        /// 
        /// </summary>
        protected string? ContentClassname => new CssBuilder($"mud-stepper-content-extended mud-stepper-ani-{_animateGuid.ToString()}")
            .AddClass(ContentClass)
            .Build();

        /// <summary>
        /// 
        /// </summary>
        protected string? ActionClassname => new CssBuilder("d-flex gap-4 mud-stepper-actions-extended")
            .AddClass("justify-center", StepperActionsJustify == StepperActionsJustify.Center)
            .AddClass("justify-end", StepperActionsJustify == StepperActionsJustify.End)
            .AddClass(ActionClass)
            .Build();

        /// <summary>
        /// 
        /// </summary>
        protected string? AvatarStylename => new StyleBuilder()
            .AddStyle("z-index: 20")
            .AddStyle("background-color", "var(--mud-palette-background)", Variant == Variant.Outlined)
            .Build();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected string? GetMobileStyle()
        {
            if(Vertical)
            {
                return "grid-column:1;margin-inline-start:22px;";
            }
            else
            {
                return "grid-row:1;margin-top:22px;";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected string? GetStepperStyle()
        {
            var count = Steps.Count * 2;
            if (Vertical)
            {
                return $"display:grid;grid-template-rows:repeat({count}, 1fr);";
            }
            else
            {
                return $"display:grid;grid-template-columns:repeat({count}, 1fr);";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected string? GetStepperSubStyle()
        {
            if (Vertical)
            {
                return "grid-row-start:1;grid-row-end:-1;flex-direction:column;grid-column:1;list-style:none;display:flex;";
            }
            else
            {
                return "grid-column-start:1;grid-column-end:-1;flex-direction:row;grid-row:1;list-style:none;display:flex;";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected string? GetStepPercent()
        {
            var dPercent = (100.0 / Steps.Count).ToInvariantString();
            if (Vertical)
            {
                return $"height:{dPercent}%";
            }
            else
            {
                return $"width:{dPercent}%";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected string? GetStepClass()
        {
            if (Vertical)
            {
                return $"d-flex";
            }
            else
            {
                return $"";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected string? GetProgressLinearStyle()
        {
            var end = Steps.Count * 2;
            if (Vertical)
            {
                return $"grid-row-start:2;grid-row-end:{end};grid-column:1/-1;display:inline-grid;left:{(HeaderSize == Size.Medium ? 30 : HeaderSize == Size.Large ? 38 : 22)}px;z-index:10;transform:rotateX(180deg);";
            }
            else
            {
                return $"grid-column-start:2;grid-column-end:{end};grid-row:1/-1;display:inline-grid;top:{(HeaderSize == Size.Medium ? 30 : HeaderSize == Size.Large ? 38 : 22)}px;{(HeaderSize == Size.Small ? "height:2px;" : HeaderSize == Size.Medium ? "height:3px;" : null)}{(MobileView ? "margin-inline-start:40px;" : null)}z-index:10";
            }
        }

        private int _activeIndex;
        internal int ActiveIndex
        {
            get => _activeIndex;
            set
            {
                _activeIndex = value;
                UpdateProgressValue();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
            if (firstRender)
            {
                if (HasIntroStep())
                    _activeIndex = -1;
                StateHasChanged();
            }
        }


        internal double ProgressValue;
        /// <summary>
        /// 
        /// </summary>
        protected void UpdateProgressValue()
        {
            ProgressValue = _activeIndex * (100.0 / (Steps.Count - 1));
        }

        /// <summary>
        /// Provides CSS classes for the step content.
        /// </summary>
        [Parameter]
        public string? ContentClass { get; set; }

        /// <summary>
        /// Provides CSS styles for the step content.
        /// </summary>
        [Parameter]
        public string? ContentStyle { get; set; }

        /// <summary>
        /// Provides CSS classes for the step actions.
        /// </summary>
        [Parameter]
        public string? ActionClass { get; set; }

        /// <summary>
        /// Determines how action buttons justified.
        /// </summary>
        [Parameter]
        public StepperActionsJustify StepperActionsJustify { get; set; }

        /// <summary>
        /// If true, the header can not be clickable and users can step one by one.
        /// </summary>
        [Parameter]
        public bool Linear { get; set; }

        /// <summary>
        /// If true, disables ripple effect when click on step headers.
        /// </summary>
        [Parameter]
        public bool Ripple { get; set; } = true;

        /// <summary>
        /// If true, disables the default animation on step changing.
        /// </summary>
        [Parameter]
        public bool Animation { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether action controls are displayed in the component.
        /// </summary>
        [Parameter]
        public bool ShowActions { get; set; } = true;

        /// <summary>
        /// If true, disables built-in "previous" step action button.
        /// </summary>
        [Parameter]
        public bool ShowPreviousButton { get; set; } = true;

        /// <summary>
        /// If true, disables built-in "next" step action button.
        /// </summary>
        [Parameter]
        public bool ShowNextButton { get; set; } = true;

        /// <summary>
        /// If true, disables built-in "skip" step action button.
        /// </summary>
        [Parameter]
        public bool ShowSkipButton { get; set; } = true;

        /// <summary>
        /// If true, disables built-in "completed"/"skipped" step result indictors shown in the actions panel.
        /// </summary>
        [Parameter]
        public bool ShowStepResultIndicator { get; set; } = true;

        /// <summary>
        /// 
        /// </summary>
        [Parameter]
        public bool MobileView { get; set; }

        /// <summary>
        /// Gets or sets the breakpoint at which the component automatically switches to mobile layout. Overrides MobileView parameter.
        /// </summary>
        /// <remarks>Use this property to define the responsive threshold for mobile-specific rendering.
        /// The value determines at which screen size the component adapts its layout for mobile devices.</remarks>
        [Parameter]
        public Breakpoint? MobileBreakpoint { get; set; }

        /// <summary>
        /// If true, a linear loading indicator shows under the header.
        /// </summary>
        [Parameter]
        public bool Loading { get; set; }

        /// <summary>
        /// A static content that always show with all steps.
        /// </summary>
        [Parameter]
        public RenderFragment? StaticContent { get; set; }

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
        /// Choose header badge view. Default is all.
        /// </summary>
        [Parameter]
        public HeaderBadgeView HeaderBadgeView { get; set; } = HeaderBadgeView.All;

        /// <summary>
        /// Choose header text view. Default is all.
        /// </summary>
        [Parameter]
        public Size HeaderSize { get; set; } = Size.Medium;

        /// <summary>
        /// Choose header text view. Default is all.
        /// </summary>
        [Parameter]
        public HeaderTextView HeaderTextView { get; set; } = HeaderTextView.None;

        /// <summary>
        /// Choose header alignment
        /// </summary>
        [Parameter]
        public bool Vertical { get; set; }

        /// <summary>
        /// A class for provide all local strings at once.
        /// </summary>
        [Parameter]
        public StepperLocalizedStrings LocalizedStrings { get; set; } = new();

        /// <summary>
        /// The child content where MudSteps should be inside.
        /// </summary>
        [Parameter]
        public RenderFragment? ChildContent { get; set; }

        /// <summary>
        /// Custom content to be shown between the "previous" and "next" action buttons.
        /// </summary>
        /// <remarks>
        /// If set, you must also supply a <code><MudSpacer /></code> somewhere in your render fragment
        /// to ensure that the built-in action buttons are aligned correctly.
        /// </remarks>
        [Parameter]
        public RenderFragment? ActionContent { get; set; }

        /// <summary>
        /// Fires when active step changed.
        /// </summary>
        [Parameter]
        public EventCallback<int> ActiveStepChanged { get; set; }

        /// <summary>
        /// Runs a task to prevent step change. Has change direction (backwards or forwards) and target index and returns a bool value.
        /// </summary>
        [Parameter]
        public Func<StepChangeDirection, int, Task<bool>>? PreventStepChangeAsync { get; set; }
        
        /// <summary>
        /// Gets or sets a delegate that is invoked asynchronously before the finishing action occurs. The delegate
        /// should return a task that resolves to <see langword="true"/> to allow the action to proceed, or <see
        /// langword="false"/> to cancel it.
        /// </summary>
        /// <remarks>If the delegate is <see langword="null"/>, the finishing action proceeds without
        /// additional checks. The asynchronous operation can be used to perform validation, confirmation dialogs, or
        /// other pre-finish logic.</remarks>
        [Parameter]
        public Func<Task<bool>>? BeforeFinishedAsync { get; set; }


        /// <summary>
        /// Gets or sets the callback that is invoked when the operation has finished.
        /// </summary>
        /// <remarks>Use this property to specify an action to perform after the component completes its
        /// process. The callback is triggered when the operation concludes, allowing you to execute custom logic such
        /// as updating the UI or notifying other components.</remarks>
        [Parameter]
        public EventCallback OnFinished { get; set; }


        List<MudStepExtended> _steps = new();
        List<MudStepExtended> _allSteps = new();
        /// <summary>
        /// 
        /// </summary>
        public List<MudStepExtended> Steps
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

        #endregion

        #region Step Management Methods
        internal void AddStep(MudStepExtended step)
        {
            if (step.IsResultStep)
            {
                step.SetStatus(StepStatus.Completed);
                if (_allSteps.Any(x => x.IsResultStep && x != step))
                    throw new InvalidOperationException("Only one ResultStep is allowed.");
            }
            _allSteps.Add(step);
            if (!step.IsResultStep && !step.IsIntroStep)
            {
                Steps.Add(step);
                ReorderSteps();
            }

            UpdateProgressValue();
            StateHasChanged();
        }

        internal void RemoveStep(MudStepExtended step)
        {
            Steps.Remove(step);
            _allSteps.Remove(step);
            UpdateProgressValue();
            StateHasChanged();
        }

        /// <summary>
        /// 
        /// </summary>
        public void ReorderSteps()
        {
            Steps = Steps.OrderBy(x => x.Order).ToList();
        }

        /// <summary>
        /// Marks the specified step as completed and optionally advances to the next step in the sequence.
        /// </summary>
        /// <remarks>If the specified step is the last remaining step, the method triggers any
        /// finalization logic before marking the step as completed. If step change prevention or finalization callbacks
        /// are configured, their results may prevent the completion or advancement. This method is typically used in
        /// multi-step workflows to manage progression and completion logic.</remarks>
        /// <param name="index">The zero-based index of the step to complete. Must be within the valid range of steps.</param>
        /// <param name="moveToNextStep">Specifies whether to automatically move to the next step after completing the current one. The default value
        /// is <see langword="true"/>.</param>
        /// <returns>A task that represents the asynchronous operation. The task completes when the step has been marked as
        /// completed and any subsequent actions have finished.</returns>
        public async Task CompleteStep(int index, bool moveToNextStep = true)
        {
            if (ActiveIndex == -1)
                return;

            if (ActiveIndex == Steps.Count)
                return;

            bool isActiveStep = (index == ActiveIndex);

            if (isActiveStep)
            {
                var direction = moveToNextStep ? StepChangeDirection.Forward : StepChangeDirection.None;
                if (PreventStepChangeAsync != null)
                {
                    var prevent = await PreventStepChangeAsync.Invoke(direction, index + 1);
                    if (prevent)
                        return;
                }
            }

            bool isLastStep = IsLastRemainingStep(index);

            if (isLastStep)
            {
                if (BeforeFinishedAsync != null)
                {
                    bool canContinue = await BeforeFinishedAsync.Invoke();
                    if (!canContinue)
                        return;
                }

                Steps[index].SetStatus(StepStatus.Completed);

                int backupIndex = ActiveIndex;
                await OnFinished.InvokeAsync();

                if (ActiveIndex != backupIndex)
                    return;

                if (HasResultStep())
                {
                    await GoToStepAsync(Steps.Count, skipPrevent: true);
                }

                return;
            }

            Steps[index].SetStatus(StepStatus.Completed);

            if (isActiveStep && moveToNextStep)
            {
                await GoNextStepAsync(skipPrevent: true);
            }
        }

        /// <summary>
        /// Marks the step at the specified <paramref name="index"/> as skipped.
        /// Optionally advances to the next step when <paramref name="moveToNextStep"/> is <c>true</c>.
        /// </summary>
        /// <remarks>
        /// If the step at <paramref name="index"/> is the active step and a step-change prevention callback
        /// (<see cref="PreventStepChangeAsync"/>) is provided, that callback is invoked before skipping.
        /// If the step is the last remaining incomplete step, this method will invoke the
        /// <see cref="BeforeFinishedAsync"/> callback (if provided) and then trigger <see cref="OnFinished"/>.
        /// After finishing, if a result step exists the component will navigate to it.
        /// </remarks>
        /// <param name="index">Zero-based index of the step to mark as skipped.</param>
        /// <param name="moveToNextStep">
        /// When <c>true</c> (the default), and the skipped step is the currently active step, the component will
        /// advance to the next appropriate step after skipping.
        /// </param>
        /// <returns>A task that represents the asynchronous skip operation.</returns>
        public async Task SkipStep(int index, bool moveToNextStep = true)
        {
            if (ActiveIndex == -1)
                return;

            if (ActiveIndex == Steps.Count)
                return;

            bool isActiveStep = (index == ActiveIndex);

            if (isActiveStep)
            {
                var direction = moveToNextStep ? StepChangeDirection.Forward : StepChangeDirection.None;
                if (PreventStepChangeAsync != null)
                {
                    bool prevent = await PreventStepChangeAsync.Invoke(direction, index + 1);
                    if (prevent)
                        return;
                }
            }

            bool isLastStep = IsLastRemainingStep(index);

            if (isLastStep)
            {
                if (BeforeFinishedAsync != null)
                {
                    bool canContinue = await BeforeFinishedAsync.Invoke();
                    if (!canContinue)
                        return;
                }

                Steps[index].SetStatus(StepStatus.Skipped);

                int backupIndex = ActiveIndex;
                await OnFinished.InvokeAsync();

                if (ActiveIndex != backupIndex)
                    return;

                if (HasResultStep())
                {
                    await GoToStepAsync(Steps.Count, skipPrevent: true);
                }

                return;
            }

            Steps[index].SetStatus(StepStatus.Skipped);

            if (isActiveStep && moveToNextStep)
            {
                await GoNextStepAsync(skipPrevent: true);
            }
        }


        #endregion

        #region Step Navigation Methods

        /// <summary>
        /// Central navigation method for stepper transitions.
        /// All public navigation APIs should call this method.
        /// </summary>
        protected async Task NavigateToStepAsync(int targetIndex, bool skipPrevent)
        {
            int stepCount = Steps.Count;

            if (HasIntroStep() && targetIndex == -1)
            {
                if (!skipPrevent && PreventStepChangeAsync is not null)
                {
                    bool prevented = await PreventStepChangeAsync.Invoke(
                        StepChangeDirection.Backward, targetIndex
                    );
                    if (prevented)
                        return;
                }

                ActiveIndex = -1;
                await ActiveStepChanged.InvokeAsync(ActiveIndex);
                return;
            }

            if (targetIndex < 0)
                targetIndex = HasIntroStep() ? -1 : 0;

            bool isResultStepTarget = (targetIndex == stepCount);

            if (targetIndex > stepCount && !isResultStepTarget)
                return;

            if (isResultStepTarget && !IsAllStepsCompleted())
                return;

            if (!skipPrevent && PreventStepChangeAsync is not null)
            {
                StepChangeDirection direction = StepChangeDirection.None;
                if (targetIndex > ActiveIndex)
                    direction = StepChangeDirection.Forward;
                else if (targetIndex < ActiveIndex)
                    direction = StepChangeDirection.Backward;

                bool prevented = await PreventStepChangeAsync.Invoke(direction, targetIndex);
                if (prevented)
                    return;
            }

            if (targetIndex == ActiveIndex)
                return;

            if (Animation && _animate != null)
                await _animate.Refresh();

            int backupIndex = ActiveIndex;
            ActiveIndex = targetIndex;

            if (!isResultStepTarget && ActiveIndex < Steps.Count)
            {
                var step = Steps[ActiveIndex];

                if (step.Status == StepStatus.NotStarted)
                    step.SetStatus(StepStatus.Continued);
            }

            if (backupIndex != ActiveIndex)
                await ActiveStepChanged.InvokeAsync(ActiveIndex);
        }

        /// <summary>
        /// Asynchronously navigates to the specified step in the workflow.
        /// </summary>
        /// <param name="index">The zero-based index of the step to navigate to. Must be within the valid range of steps.</param>
        /// <param name="skipPrevent">If <see langword="true"/>, bypasses any checks or conditions that would normally prevent navigation to the
        /// specified step; otherwise, enforces all navigation rules.</param>
        /// <returns>A task that represents the asynchronous navigation operation.</returns>
        public Task GoToStepAsync(int index, bool skipPrevent = false)
        {
            return NavigateToStepAsync(index, skipPrevent);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="skipPrevent"></param>
        /// <returns></returns>
        public Task GoByIndexAsync(int offset, bool skipPrevent = false)
        {
            int target = ActiveIndex + offset;
            return NavigateToStepAsync(target, skipPrevent);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="step"></param>
        /// <param name="skipPrevent"></param>
        /// <returns></returns>
        public Task GoToStepByReferenceAsync(MudStepExtended step, bool skipPrevent = false)
        {
            int index = Steps.IndexOf(step);
            if (index < 0)
                return Task.CompletedTask;

            return NavigateToStepAsync(index, skipPrevent);
        }

        /// <summary>
        /// Advances to the next step in the workflow asynchronously, or navigates to the first unfinished step if the
        /// workflow is at its final step.
        /// </summary>
        /// <remarks>If the workflow is already at the last step, this method navigates to the first step
        /// that is not completed or skipped. If all steps are finished, goes to result step if set.</remarks>
        /// <param name="skipPrevent">If set to <see langword="true"/>, bypasses any checks that would normally prevent navigation to the next
        /// step. The default is <see langword="false"/>.</param>
        /// <returns>A task that represents the asynchronous operation. The task completes when navigation to the appropriate
        /// step is finished.</returns>
        public async Task GoNextStepAsync(bool skipPrevent = false)
        {
            if (ActiveIndex == -1 && HasIntroStep())
            {
                await NavigateToStepAsync(0, skipPrevent);
                return;
            }

            if (ActiveIndex < Steps.Count - 1)
            {
                await NavigateToStepAsync(ActiveIndex + 1, skipPrevent);
                return;
            }

            var firstUnfinished = Steps.FirstOrDefault(x =>
                x.Status != StepStatus.Completed &&
                x.Status != StepStatus.Skipped);

            if (firstUnfinished != null)
            {
                int targetIndex = Steps.IndexOf(firstUnfinished);
                await NavigateToStepAsync(targetIndex, skipPrevent);
                return;
            }

            if (HasResultStep())
            {
                await NavigateToStepAsync(Steps.Count, skipPrevent);
            }
        }

        /// <summary>
        /// Navigates asynchronously to the previous step in the sequence, if the current step is not the first.
        /// </summary>
        /// <param name="skipPrevent">If set to <see langword="true"/>, bypasses any checks or conditions that would normally prevent navigation
        /// to the previous step.</param>
        /// <returns>A task that represents the asynchronous navigation operation.</returns>
        public async Task GoPreviousStepAsync(bool skipPrevent = false)
        {
            if (ActiveIndex == 0 && HasIntroStep())
            {
                await NavigateToStepAsync(-1, skipPrevent);
                return;
            }

            if (ActiveIndex <= 0)
                return;

            await NavigateToStepAsync(ActiveIndex - 1, skipPrevent);
        }

        #endregion

        #region The Obsoletes

        [Obsolete("Use GoToStepByReferenceAsync() instead.")]
        protected internal async Task SetActiveIndex(MudStepExtended step, bool skipPreventProcess = false)
        {
            await SetActiveStepByIndex(Steps.IndexOf(step), skipPreventProcess: skipPreventProcess);
        }

        [Obsolete("Use GoNextAsync/GoPreviousAsync or GoToStepAsync instead.")]
        public async Task SetActiveIndex(int count, bool firstCompleted = false, bool skipPreventProcess = false)
        {
            var stepChangeDirection = (
                count == 0 ? StepChangeDirection.None :
                    count >= 1 ? StepChangeDirection.Forward :
                        StepChangeDirection.Backward
            );

            if (skipPreventProcess == false && PreventStepChangeAsync != null)
            {
                var result = await PreventStepChangeAsync.Invoke(stepChangeDirection, ActiveIndex + count);
                if (result == true)
                {
                    return;
                }
            }

            int backupActiveIndex = ActiveIndex;
            if (_animate != null && Animation == true)
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
                var nextUnfinished = Steps.FirstOrDefault(x => x.Status != StepStatus.Completed && x.Status != StepStatus.Skipped);

                if (nextUnfinished != null)
                {
                    ActiveIndex = Steps.IndexOf(nextUnfinished);
                }
            }
            else
            {
                ActiveIndex += count;
            }

            if (backupActiveIndex != ActiveIndex && ActiveIndex < Steps.Count)
            {
                var step = Steps[ActiveIndex];
                if (step.Status == StepStatus.NotStarted)
                {
                    step.SetStatus(StepStatus.Continued);
                }
            }

            if (backupActiveIndex != ActiveIndex)
            {
                await ActiveStepChanged.InvokeAsync(ActiveIndex);
            }
        }

        [Obsolete("Use GoNextAsync/GoPreviousAsync or GoToStepAsync instead.")]
        public async Task SetActiveStepByIndex(int index, bool firstCompleted = false, bool skipPreventProcess = false)
        {
            var stepChangeDirection = (
                index == ActiveIndex ? StepChangeDirection.None :
                    index > ActiveIndex ? StepChangeDirection.Forward :
                        StepChangeDirection.Backward
            );

            if (skipPreventProcess == false && PreventStepChangeAsync != null)
            {
                var result = await PreventStepChangeAsync.Invoke(stepChangeDirection, index);
                if (result == true)
                {
                    return;
                }
            }

            if (ActiveIndex == index || index < 0 || Steps.Count < index)
            {
                return;
            }

            if (Steps.Count == index && IsAllStepsCompleted() == false)
            {
                return;
            }

            if (_animate != null && Animation == true)
            {
                await _animate.Refresh();
            }

            ActiveIndex = index;
            if (index < Steps.Count)
            {
                var step = Steps[index];
                if (step.Status == StepStatus.NotStarted)
                {
                    step.SetStatus(StepStatus.Continued);
                }
            }

            await ActiveStepChanged.InvokeAsync(ActiveIndex);
        }

        #endregion

        #region Logic Checks

        /// <summary>
        /// 
        /// </summary>
        /// <param name="step"></param>
        /// <returns></returns>
        protected bool IsStepActive(MudStepExtended step)
        {
            return Steps.IndexOf(step) == ActiveIndex;
        }

        /// <summary>
        /// Determines whether the step at the specified index is the last remaining incomplete step.
        /// </summary>
        /// <remarks>A step is considered incomplete if its status is neither Completed nor Skipped. If
        /// there are no incomplete steps, the method returns false.</remarks>
        /// <param name="index">The zero-based index of the step to evaluate within the collection of steps.</param>
        /// <returns>true if the step at the specified index is the only incomplete step remaining; otherwise, false.</returns>
        protected internal bool IsLastRemainingStep(int index)
        {
            var incompleteSteps = Steps
                .Where(x => x.Status != StepStatus.Completed && x.Status != StepStatus.Skipped)
                .ToList();

            if (incompleteSteps.Count == 0)
                return false;

            return incompleteSteps.Count == 1 && Steps.IndexOf(incompleteSteps[0]) == index;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected string? GetNextButtonString()
        {
            return IsLastRemainingStep(ActiveIndex)
                ? LocalizedStrings.Finish
                : LocalizedStrings.Next;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected internal bool ShowResultStep()
        {
            if (IsAllStepsCompleted() && ActiveIndex == Steps.Count)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Determines whether the collection contains result step.
        /// </summary>
        /// <returns>true if an result step exists in the collection; otherwise, false.</returns>
        protected internal bool HasResultStep()
        {
            return _allSteps.Any(x => x.IsResultStep);
        }

        /// <summary>
        /// Determines whether the collection contains introductory step.
        /// </summary>
        /// <returns>true if an introductory step exists in the collection; otherwise, false.</returns>
        protected internal bool HasIntroStep()
        {
            return _allSteps.Any(x => x.IsIntroStep);
        }

        /// <summary>
        /// Determines whether all steps in the collection have been completed or skipped.
        /// </summary>
        /// <remarks>This method evaluates the status of each step in the Steps collection. It is useful
        /// for checking whether a process or workflow has finished all required actions, including those that were
        /// intentionally skipped.</remarks>
        /// <returns>true if every step has a status of Completed or Skipped; otherwise, false.</returns>
        public bool IsAllStepsCompleted()
        {
            return Steps.All(x => x.Status == StepStatus.Completed || x.Status == StepStatus.Skipped);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int GetActiveIndex()
        {
            return ActiveIndex;
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        public void Reset()
        {
            Steps.ForEach(x => x.SetStatus(StepStatus.NotStarted));
            if (HasIntroStep())
            {
                ActiveIndex = -1;
            }
            else
            {
                ActiveIndex = 0;
            }
            UpdateProgressValue();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="status"></param>
        public void SetStepStatus(int index, StepStatus status)
        {
            Steps[index].SetStatus(status);
        }

        /// <summary>
        /// Update all component and render again.
        /// </summary>
        public void ForceRender()
        {
            UpdateProgressValue();
            StateHasChanged();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        protected virtual Task OnBeforeNavigate(int from, int to) => Task.CompletedTask;

        /// <summary>
        /// Handles changes to the specified breakpoint and updates the mobile view state accordingly.
        /// </summary>
        /// <remarks>This method updates the mobile view only if a mobile breakpoint is defined. It is
        /// typically called when the application's layout needs to respond to breakpoint changes, such as during window
        /// resizing or device orientation changes.</remarks>
        /// <param name="breakpoint">The breakpoint value that triggered the change. Determines whether the mobile view should be enabled or
        /// disabled.</param>
        protected void OnBreakpointChanged(Breakpoint breakpoint)
        {
            if (MobileBreakpoint.HasValue && breakpoint <= MobileBreakpoint )
            {
                MobileView = true;
            }
            else
            {
                MobileView = false;
            }
            StateHasChanged();
        }

    }
}
