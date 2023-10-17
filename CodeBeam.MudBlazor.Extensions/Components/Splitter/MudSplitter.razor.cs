using Microsoft.AspNetCore.Components;
using MudBlazor;
using MudBlazor.Extensions;
using MudBlazor.Utilities;

namespace MudExtensions
{
    public partial class MudSplitter : MudComponentBase
    {

        readonly Guid _styleGuid = Guid.NewGuid();
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
        /// The height of splitter. For example: "400px"
        /// </summary>
        /// <remarks>The default is 100%</remarks>
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
        [Obsolete("StyleContent is deprecated, please use property ContentStyle, StartContentStyle or EndContentStyle to set the style.")]
        [Parameter]
        public string StyleContent { get; set; }

        /// <summary>
        /// The style to apply to both content sections, seperated by space.
        /// </summary>
        [Parameter]
        public string ContentStyle { get; set; }

        /// <summary>
        /// The style of the <see cref="StartContent"/>, seperated by space. Overrules <see cref=" StyleContent"/>
        /// </summary>
        [Parameter]
        public string StartContentStyle { get; set; }

        /// <summary>
        /// The style of the <see cref="EndContent"/>, seperated by space. Overrules <see cref=" StyleContent"/>
        /// </summary>
        [Parameter]
        public string EndContentStyle { get; set; }

        /// <summary>
        /// The splitter bar's styles, seperated by space. The style string should end with: "!important;"
        /// </summary>
        [Obsolete("StyleBar is deprecated, please use property BarStyle to set the bar's style.")]
        [Parameter]
        public string StyleBar { get; set; } = "width:2px !important;";

        /// <summary>
        /// The splitter bar's styles, seperated by space. The style string should end with: "!important;"
        /// </summary>
        /// <remarks>The default is 2px</remarks>
        [Parameter]
        public string BarStyle { get; set; } = "width:2px !important;";

        /// <summary>
        /// The slide sensitivity that should between 0.01 and 10. Smaller values increase the smooth but reduce performance. Default is 0.1
        /// </summary>
        [Parameter]
        public double Sensitivity { get; set; } = 0.1d;

        /// <summary>
        /// If true, user can interact with splitter bar.
        /// </summary>
        /// <remarks>The default is true</remarks>
        [Parameter]
        public bool EnableSlide { get; set; } = true;

        /// <summary>
        /// Enables the default margin.
        /// </summary>
        /// <remarks>The default is true, which adds class: "ma-2"</remarks>
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
        /// <remarks>The default is 50</remarks>
        [Parameter]
        public double Dimension { get; set; } = 50;

        [Parameter]
        public EventCallback<double> DimensionChanged { get; set; }

        [Parameter]
        public EventCallback OnDoubleClicked { get; set; }


        string EffectiveStartStyle { get { return !string.IsNullOrWhiteSpace(StartContentStyle) ? StartContentStyle : !string.IsNullOrWhiteSpace(ContentStyle) ? ContentStyle : StyleContent; } }
        string EffectiveEndStyle { get { return !string.IsNullOrWhiteSpace(EndContentStyle) ? EndContentStyle : !string.IsNullOrWhiteSpace(ContentStyle) ? ContentStyle : StyleContent; } }
        string EffectiveHeight { get { return !string.IsNullOrWhiteSpace(Height) ? $"height:{Height} !important;" : null; } }
        string EffectiveBarStyle { get { return !string.IsNullOrWhiteSpace(BarStyle) ? BarStyle : StyleBar; } }
        string EffectiveColor { get { return $"background-color:var(--mud-palette-{(Color == Color.Default ? "action-default" : Color.ToDescriptionString())}) !important;"; } }


        protected Task UpdateDimension(double percentage)
        {
            Dimension = percentage;

            if (Dimension < 0)
                Dimension = 0;
            else if (Dimension > 100)
                Dimension = 100;

            if (DimensionChanged.HasDelegate)
                _ = DimensionChanged.InvokeAsync(percentage);

            return Task.CompletedTask;
        }
        
        Task OnDoubleClick()
        {
            if (OnDoubleClicked.HasDelegate)
                _ = OnDoubleClicked.InvokeAsync();

            return Task.CompletedTask;
        }
    }
}
