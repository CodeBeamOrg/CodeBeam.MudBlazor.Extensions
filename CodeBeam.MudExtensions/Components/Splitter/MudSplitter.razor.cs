using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using MudBlazor.Extensions;
using MudBlazor.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MudBlazor.CategoryTypes;

namespace MudExtensions
{
    public partial class MudSplitter : MudComponentBase
    {

        MudSlider<double> _slider;

        protected string Classname => new CssBuilder("mud-splitter relative")
            .AddClass("mud-splitter-generate")
            .AddClass(Class)
            .Build();

        protected string ContentClassname => new CssBuilder("d-flex ma-2")
            .AddClass(ClassContent)
            .Build();

        /// <summary>
        /// The two contents' (sections) classes, seperated by space.
        /// </summary>
        [Parameter]
        public string ClassContent { get; set; }

        /// <summary>
        /// The two contents' (sections) styles, seperated by space.
        /// </summary>
        [Parameter]
        public string StyleContent { get; set; }

        /// <summary>
        /// If true, splitter bar goes vertical.
        /// </summary>
        [Parameter]
        public bool Horizontal { get; set; }

        [Parameter]
        public RenderFragment FirstContent { get; set; }

        [Parameter]
        public RenderFragment SecondContent { get; set; }

        protected override void OnParametersSet()
        {
            GetDimensions();
        }

        double _firstContentDimension = 50;
        double _secondContentDimension = 50;
        protected void GetDimensions()
        {
            if (_slider == null)
            {
                return;
            }
            _firstContentDimension = _slider.Value;
            _secondContentDimension = 100d - _firstContentDimension;
        }

    }
}
