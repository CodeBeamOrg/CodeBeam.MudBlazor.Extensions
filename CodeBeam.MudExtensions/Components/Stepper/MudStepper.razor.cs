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
        [Parameter]
        public int ActiveIndex { get; set; }

        [Parameter]
        public bool Vertical { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

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

        protected void SetActiveIndex(MudStep step)
        {
            ActiveIndex = Steps.IndexOf(step);
        }
    }
}
