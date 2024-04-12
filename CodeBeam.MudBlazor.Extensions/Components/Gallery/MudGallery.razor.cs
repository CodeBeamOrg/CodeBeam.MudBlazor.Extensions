using Microsoft.AspNetCore.Components;
using MudBlazor;
using MudBlazor.Utilities;

namespace MudExtensions
{
    /// <summary>
    /// Gallery component.
    /// </summary>
    public partial class MudGallery : MudComponentBase
    {
        MudAnimate _animate = new();
        Guid _animateGuid = Guid.NewGuid();
        
        bool _visible = false;
        DialogOptions _dialogOptions = new() { NoHeader = true, FullWidth = true, MaxWidth = MaxWidth.Large, CloseOnEscapeKey = true };
        string? _selectedSrc;

        /// <summary>
        /// 
        /// </summary>
        protected string? Classname => new CssBuilder("d-flex flex-wrap")
            .AddClass(Class)
            .Build();

        /// <summary>
        /// 
        /// </summary>
        protected string? ImageStylename => new StyleBuilder()
            .AddStyle("width", $"{100 / ItemPerLine}%")
            .AddStyle("aspect-ratio", "1 / 1")
            .Build();

        /// <summary>
        /// CSS classes for selected image.
        /// </summary>
        [Parameter]
        public string? ClassSelectedImage { get; set; }

        /// <summary>
        /// CSS styles for selected image.
        /// </summary>
        [Parameter]
        public string? StyleSelectedImage { get; set; }

        /// <summary>
        /// Sets how many images show per gallery line. Default is 3.
        /// </summary>
        [Parameter]
        public int ItemPerLine { get; set; } = 3;

        /// <summary>
        /// If true, closes selected image on backdrop click. Default is true.
        /// </summary>
        [Parameter]
        public bool EnableBackdropClick { get; set; } = true;

        /// <summary>
        /// If true, disables the default animation on image changing.
        /// </summary>
        [Parameter]
        public bool EnableAnimation { get; set; } = true;

        /// <summary>
        /// If true, close button shows on selected imgae view.
        /// </summary>
        [Parameter]
        public bool ShowToolboxCloseButton { get; set; } = true;

        /// <summary>
        /// If true, toolbox buttons shows on selected imgae view.
        /// </summary>
        [Parameter]
        public bool ShowToolboxNavigationButtons { get; set; } = true;

        /// <summary>
        /// The max width for selected image. The remaining space fills with an overlay. Default is Medium.
        /// </summary>
        [Parameter]
        public MaxWidth MaxWidth { get; set; } = MaxWidth.Medium;

        /// <summary>
        /// Renderfragment for top section on selected view.
        /// </summary>
        [Parameter]
        public RenderFragment? ToolboxTopContent { get; set; }

        /// <summary>
        /// Renderfragment for bottom section on selected view.
        /// </summary>
        [Parameter]
        public RenderFragment? ToolboxBottomContent { get; set; }

        /// <summary>
        /// Gallery's image source.
        /// </summary>
        [Parameter]
        public List<string> ImageSource { get; set; } = new();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="src"></param>
        protected void ImageClick(string src)
        {
            _selectedSrc = src;
            _visible = true;
            StateHasChanged();
        }

        /// <summary>
        /// Change menu visible.
        /// </summary>
        /// <param name="visible"></param>
        public void ChangeMenu(bool visible)
        {
            _visible = visible;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        protected async Task SetAdjacentImage(int count)
        {
            if (_selectedSrc == null)
            {
                return;
            }
            int index = ImageSource.IndexOf(_selectedSrc);

            if (ImageSource.Count <= index + count || index + count < 0)
            {
                return;
            }

            if (EnableAnimation)
            {
                await _animate.Refresh();
            }
            _selectedSrc = ImageSource[index + count];
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int GetSelectedImageIndex()
        {
            return ImageSource.IndexOf(_selectedSrc ?? "");
        }

    }
}
