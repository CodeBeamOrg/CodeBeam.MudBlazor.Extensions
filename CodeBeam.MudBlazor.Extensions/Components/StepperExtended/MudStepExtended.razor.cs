using Microsoft.AspNetCore.Components;
using MudBlazor;
using MudBlazor.Utilities;

namespace MudExtensions
{
    /// <summary>
    /// 
    /// </summary>
    public partial class MudStepExtended : MudComponentBase, IDisposable
    {
        /// <summary>
        /// 
        /// </summary>
        protected string? Classname => new CssBuilder()
            .AddClass("d-none", ((MudStepperExtended.ActiveIndex < MudStepperExtended.Steps.Count && MudStepperExtended.Steps[MudStepperExtended.ActiveIndex] != this) || (MudStepperExtended.ShowResultStep() && !IsResultStep)) || (IsResultStep && !MudStepperExtended.ShowResultStep()))
            .AddClass(Class)
            .Build();

        /// <summary>
        /// 
        /// </summary>
        [CascadingParameter]
        public MudStepperExtended MudStepperExtended { get; set; } = new();

        /// <summary>
        /// Step text to show on header.
        /// </summary>
        [Parameter]
        public string? Title { get; set; }

        private int _order;
        /// <summary>
        /// The order of the step.
        /// </summary>
        [Parameter]
        public int Order 
        {
            get => _order; 
            set
            {
                _order = value;
                MudStepperExtended?.ReorderSteps();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsActive
        {
            get
            {
                return MudStepperExtended?.ActiveIndex == this.Number;
            }
        }


        StepStatus _status = StepStatus.Continued;
        /// <summary>
        /// The step status flag to show step is continued, skipped or completed. Do not set it directly unless you know what you do exactly.
        /// </summary>
        [Parameter]
        public StepStatus Status
        { 
            get => _status; 
            set
            {
                if (_status == value)
                {
                    return;
                }
                _status = value;
                StatusChanged.InvokeAsync(_status).AndForget();
            }
        }

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

        /// <summary>
        /// 
        /// </summary>
        [Parameter]
        public RenderFragment? ChildContent { get; set; }

        /// <summary>
        /// Fires when step status changed.
        /// </summary>
        [Parameter]
        public EventCallback<StepStatus> StatusChanged { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Parameter]
        public int Number { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Parameter]
        public RenderFragment<MudStepExtended>? Template { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Parameter]
        public string? Icon { get; set; }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnInitialized()
        {
            base.OnInitialized();

            MudStepperExtended.AddStep(this);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="status"></param>
        protected internal void SetStatus(StepStatus status)
        {
            Status = status;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            try
            {
                MudStepperExtended?.RemoveStep(this);
            }
            catch (Exception) { }
        }

    }
}