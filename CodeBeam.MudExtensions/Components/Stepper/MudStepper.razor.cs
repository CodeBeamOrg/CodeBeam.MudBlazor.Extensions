using CodeBeam.MudExtensions.Utilities;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudExtensions
{
    public partial class MudStepper : MudComponentBase
    {
        MudAnimate _animate;

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
                _steps = value;
            }
        }

        internal void AddStep(MudStep step)
        {
            _steps.Add(step);

            StateHasChanged();
        }

        protected async Task SetActiveIndex(MudStep step)
        {
            ActiveIndex = Steps.IndexOf(step);
            await ActiveStepChanged.InvokeAsync();
            await _animate.Refresh();
        }

        protected async Task SetActiveIndex(int count)
        {
            if (Steps.Count - 1 < ActiveIndex + count)
            {
                ActiveIndex = Steps.Count - 1;
            }
            else if (ActiveIndex + count < 0)
            {
                ActiveIndex = 0;
            }
            else
            {
                ActiveIndex += count;
            }
            await ActiveStepChanged.InvokeAsync();
            await _animate.Refresh();
        }

        protected bool IsStepActive(MudStep step)
        {
            return Steps.IndexOf(step) == ActiveIndex;
        }

        protected string GetNextButtonString()
        {
            if (Steps.Count - 1 == ActiveIndex)
            {
                return LocalizedStrings.Finish;
            }
            else
            {
                return LocalizedStrings.Next;
            }
        }

        protected void UpdateAvatars()
        {
            foreach (MudStep step in Steps)
            {

            }
        }

    }
}
