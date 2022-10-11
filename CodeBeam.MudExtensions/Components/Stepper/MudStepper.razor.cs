using CodeBeam.MudExtensions.Utilities;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using MudBlazor.Extensions;
using MudBlazor.Utilities;
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
            .AddClass("cursor-pointer", Linear == false)
            .Build();

        protected string DashClassname => new CssBuilder("mud-stepper-header-dash flex-grow-1 mx-auto")
            .AddClass("mud-stepper-header-dash-completed")
            .Build();

        [Parameter]
        public int ActiveIndex { get; set; }

        /// <summary>
        /// If true, the header can not be clickable and users can step one by one.
        /// </summary>
        [Parameter]
        public bool Linear { get; set; }

        /// <summary>
        /// If total steps exceed this value, only the active step has title.
        /// </summary>
        [Parameter]
        public bool DisableHeaderText { get; set; }

        /// <summary>
        /// If true, disabled ripple effect when click on step headers.
        /// </summary>
        [Parameter]
        public bool DisableRipple { get; set; }

        [Parameter]
        public Color Color { get; set; } = Color.Default;

        [Parameter]
        public Variant Variant { get; set; }

        [Parameter]
        public bool Vertical { get; set; }

        [Parameter]
        public StepperLocalizedStrings LocalizedStrings { get; set; } = new();

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public RenderFragment CompletedContent { get; set; }

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

            //Steps.Add(step);
            //Steps = Steps;

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
            _isResultStep = false;
            await ActiveStepChanged.InvokeAsync();
            await _animate.Refresh();
        }

        protected async Task SetActiveIndex(int count)
        {
            if (_isResultStep)
            {
                ActiveIndex = Steps.Count;
            }
            if (Steps.Count - 1 < ActiveIndex + count)
            {
                ActiveIndex = Steps.Count - 1;
                if (IsAllStepsCompleted() == false)
                {
                    _isResultStep = true;
                }
            }
            else if (ActiveIndex + count < 0)
            {
                ActiveIndex = 0;
                _isResultStep = false;
            }
            else
            {
                if (IsAllStepsCompleted())
                {
                    ActiveIndex = Steps.Count - 1;
                    _isResultStep = true;
                }
                else
                {
                    ActiveIndex += count;
                    _isResultStep = false;
                }
            }
            await ActiveStepChanged.InvokeAsync();
            await _animate.Refresh();
        }

        internal bool _isResultStep = false;
        public async Task CompleteStep(int index, bool moveToNextStep = true)
        {
            Steps[index].SetComplete(true);
            if (moveToNextStep)
            {
                await SetActiveIndex(1);
            }
        }

        protected bool IsStepActive(MudStep step)
        {
            return Steps.IndexOf(step) == ActiveIndex;
        }

        public bool IsAllStepsCompleted()
        {
            return !Steps.Any(x => x.Completed == false && x.Optional == false);
        }

        protected int CompletedStepCount()
        {
            return Steps.Where(x => x.Completed).Count();
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

        public void Reset()
        {
            Steps.ForEach(x => x.SetComplete(false));
            ActiveIndex = 0;
            _isResultStep = false;
        }

    }
}
