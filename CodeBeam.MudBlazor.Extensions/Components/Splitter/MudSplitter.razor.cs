using Microsoft.AspNetCore.Components;
using MudBlazor;
using MudBlazor.Extensions;
using MudBlazor.Utilities;

namespace MudExtensions
{
    public partial class MudSplitter : MudComponentBase
    {

        Guid _styleGuid = Guid.NewGuid();
        MudSlider<double> _slider;

        protected string Classname => new CssBuilder("mud-splitter")
            .AddClass($"border-solid border-2 mud-border-{Color.ToDescriptionString()}", Bordered)
            .AddClass($"mud-splitter-generate mud-splitter-generate-{_styleGuid}")
            .AddClass(Class)
            .Build();

        protected string ContentClassname => new CssBuilder($"mud-splitter-content mud-splitter-content-{_styleGuid} d-flex")
            .AddClass("ma-2", EnableMargin)
            .AddClass(ClassContent)
            .Build();

        protected string SliderClassname => new CssBuilder($"mud-splitter-thumb mud-splitter-thumb-{_styleGuid} mud-splitter-track")
            .AddClass("mud-splitter-thumb-disabled", EnableSlide == false)
            .Build();

        /// <summary>
        /// The two contents' (sections) classes, seperated by space.
        /// </summary>
        [Parameter]
        public string ClassContent { get; set; }

        //string _height;
        /// <summary>
        /// The height of splitter.
        /// </summary>
        [Parameter]
        public string Height { get; set; }

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
        public string StyleBar { get; set; } = "width:2px !important;";

        /// <summary>
        /// The slide sensitivity that should between 0.01 and 10. Smaller values increase the smooth but reduce performance. Default is 0.1
        /// </summary>
        [Parameter]
        public double Sensitivity { get; set; } = 0.1d;

        [Obsolete("DisableSlide is deprecated, please use property EnableSlide to set Slide.")]
        [Parameter]
        public bool DisableSlide
        {
            get { return !EnableSlide; }
            set { EnableSlide = !value; }
        }
        /// <summary>
        /// If true, user can interact with splitter bar.
        /// Default is true.
        /// </summary>
        [Parameter]
        public bool EnableSlide { get; set; } = true;

        [Obsolete("DisableMargin is deprecated, please use property EnableMargin to set Margin.")]
        [Parameter]
        public bool DisableMargin
        {
            get { return !EnableMargin; }
            set { EnableMargin = !value; }
        }
        /// <summary>
        /// Enables the default margin.
        /// Default is true.
        /// </summary>
        [Parameter]
        public bool EnableMargin { get; set; } = true;


        ///// <summary>
        ///// If true, splitter bar goes vertical.
        ///// </summary>
        //[Parameter]
        //public bool Horizontal { get; set; }

        [Parameter]
        public RenderFragment StartContent { get; set; }

        [Parameter]
        public RenderFragment EndContent { get; set; }


        /// <summary>
        /// The start content's percentage.
        /// Default is 50.
        /// </summary>
        [Parameter]
        public double Dimension { get; set; } = 50;

        [Parameter]
        public EventCallback<double> DimensionChanged { get; set; }

        [Parameter]
        public EventCallback OnDoubleClicked { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            await UpdateDimension(Dimension);
        }

        protected async Task UpdateDimension(double percentage)
        {
            Dimension = percentage;
            if (DimensionChanged.HasDelegate)
                await DimensionChanged.InvokeAsync(percentage);
        }

        /// <summary>
        /// Updates the dimension with given the start content's percentage
        /// </summary>
        /// <param name="percentage"></param>
        [Obsolete("SetDimensions is deprecated, please use property Dimension to set start content's percentage.")]
        public Task SetDimensions(double percentage) => UpdateDimension(percentage);

        async Task OnDoubleClick()
        {
            if (OnDoubleClicked.HasDelegate)
                await OnDoubleClicked.InvokeAsync();
        }
    }
}
