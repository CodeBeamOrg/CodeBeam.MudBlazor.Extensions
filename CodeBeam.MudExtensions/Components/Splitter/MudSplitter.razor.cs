using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using MudBlazor.Components.Highlighter;
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

        Guid _styleGuid = Guid.NewGuid();
        MudSlider<double> _slider;

        protected string Classname => new CssBuilder("mud-splitter")
            .AddClass($"border-solid border-8 mud-border-{Color.ToDescriptionString()}", Bordered == true)
            .AddClass($"mud-splitter-generate mud-splitter-generate-{_styleGuid}")
            .AddClass(Class)
            .Build();

        protected string ContentClassname => new CssBuilder($"mud-splitter-content mud-splitter-content-{_styleGuid} d-flex")
            .AddClass("ma-2", !DisableMargin)
            .AddClass(ClassContent)
            .Build();

        protected string SliderClassname => new CssBuilder($"mud-splitter-thumb mud-splitter-thumb-{_styleGuid} mud-splitter-track")
            .Build();

        /// <summary>
        /// The two contents' (sections) classes, seperated by space.
        /// </summary>
        [Parameter]
        public string ClassContent { get; set; }

        string _height;
        /// <summary>
        /// The height of splitter.
        /// </summary>
        [Parameter]
        public string Height 
        { 
            get => _height; 
            set
            {
                if (value == _height)
                {
                    return;
                }
                _height = value;
                UpdateDimensions().AndForget();
            }
        }

        /// <summary>
        /// The height of splitter.
        /// </summary>
        [Parameter]
        public Color Color { get; set; }

        /// <summary>
        /// If true, splitter has borders.
        /// </summary>
        [Parameter]
        public bool Bordered { get; set; }

        /// <summary>
        /// The two contents' (sections) styles, seperated by space.
        /// </summary>
        [Parameter]
        public string StyleContent { get; set; }

        /// <summary>
        /// The splitter bar's styles, seperated by space. All styles have to include !important and end with ';'
        /// </summary>
        [Parameter]
        public string StyleBar { get; set; }

        /// <summary>
        /// The slide sensitivity that should between 0.01 and 10. Smaller values increase the smooth but reduce performance. Default is 0.1
        /// </summary>
        [Parameter]
        public double Sensitivity { get; set; } = 0.1d;

        /// <summary>
        /// If true, user cannot interact with splitter bar.
        /// </summary>
        [Parameter]
        public bool DisableSlide { get; set; }

        /// <summary>
        /// Disables the default margin.
        /// </summary>
        [Parameter]
        public bool DisableMargin { get; set; }

        ///// <summary>
        ///// If true, splitter bar goes vertical.
        ///// </summary>
        //[Parameter]
        //public bool Horizontal { get; set; }

        [Parameter]
        public RenderFragment StartContent { get; set; }

        [Parameter]
        public RenderFragment EndContent { get; set; }

        [Parameter]
        public EventCallback DimensionChanged { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            await UpdateDimensions();
        }

        double _firstContentDimension = 50;
        double _secondContentDimension = 50;
        protected async Task UpdateDimensions()
        {
            if (_slider == null)
            {
                return;
            }
            _firstContentDimension = _slider.Value;
            _secondContentDimension = 100d - _firstContentDimension;
            await DimensionChanged.InvokeAsync();
        }

        public double GetStartContentPercentage() => _firstContentDimension;

        /// <summary>
        /// Updates the dimension with given the start content's percentage
        /// </summary>
        /// <param name="percentage"></param>
        public async Task SetDimensions(double percentage)
        {
            _slider.Value = percentage;
            await UpdateDimensions();
        }

    }
}
