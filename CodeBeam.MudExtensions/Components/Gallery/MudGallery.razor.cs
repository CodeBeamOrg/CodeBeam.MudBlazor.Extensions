using CodeBeam.MudExtensions.Utilities;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using MudBlazor.Extensions;
using MudBlazor.Utilities;
using MudExtensions.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using static MudBlazor.Colors;

namespace MudExtensions
{
    public partial class MudGallery : MudComponentBase
    {
        MudAnimate _animate;
        Guid _animateGuid = Guid.NewGuid();
        
        bool _visible = false;
        DialogOptions _dialogOptions = new() { NoHeader = true, FullWidth = true, MaxWidth = MaxWidth.Large, CloseOnEscapeKey = true };
        string _selectedSrc;

        protected string DialogClassname => new CssBuilder("mud-gallery-dialog d-flex align-center justify-center mud-width-full")
            .Build();

        protected string ImageStylename => new StyleBuilder()
            .AddStyle("width", $"{100 / ItemPerLine}%")
            .AddStyle("aspect-ratio", "1 / 1")
            .Build();

        [Parameter]
        public int ItemPerLine { get; set; } = 3;

        /// <summary>
        /// The mini image square's width and height.
        /// </summary>
        [Parameter]
        public int ImageDimension { get; set; }

        /// <summary>
        /// Provides CSS classes for the step content.
        /// </summary>
        [Parameter]
        public string ContentClass { get; set; }

        /// <summary>
        /// Provides CSS styles for the step content.
        /// </summary>
        [Parameter]
        public string ContentStyle { get; set; }



        /// <summary>
        /// If true, disables ripple effect when click on step headers.
        /// </summary>
        [Parameter]
        public bool DisableRipple { get; set; }

        /// <summary>
        /// If true, disables the default animation on step changing.
        /// </summary>
        [Parameter]
        public bool DisableAnimation { get; set; }

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
        /// Overrides the action buttons (previous, next etc.) with custom render fragment.
        /// </summary>
        [Parameter]
        public RenderFragment ActionContent { get; set; }

        [Parameter]
        public EventCallback<int> ActiveStepChanged { get; set; }

        [Parameter]
        public List<string> ImageSource { get; set; }

        protected void ImageClick(string src)
        {
            _selectedSrc = src;
            _visible = true;
            StateHasChanged();
        }


    }
}
