using Microsoft.AspNetCore.Components;
using MudBlazor;
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

        /// <summary>
        /// If true the requirement of the step is completed and shows an tick into the header.
        /// </summary>
        [Parameter]
        public bool Completed { get; set; }

        /// <summary>
        /// If true the step is skippable.
        /// </summary>
        [Parameter]
        public bool Optional { get; set; }

        [Parameter]
        public bool ResultStep { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            if (ResultStep == false)
            {
                MudStepper.AddStep(this);
            }
        }

        protected internal void SetComplete(bool value)
        {
            Completed = value;
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