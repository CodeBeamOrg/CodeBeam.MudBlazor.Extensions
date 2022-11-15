using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using MudBlazor.Utilities;
using MudExtensions.Enums;

namespace MudExtensions
{
    public partial class MudLoading : MudComponentBase
    {

        protected string TextClassname => new CssBuilder()
            .AddClass("mt-4")
            .AddClass(ClassText)
            .Build();

        bool _loading = true;
        /// <summary>
        /// Two way binded loading state.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public bool Loading 
        { 
            get => _loading; 
            set
            {
                if (_loading == value)
                {
                    return;
                }
                _loading = value;
                LoadingChanged.InvokeAsync(_loading).AndForget();
            }
        }

        /// <summary>
        /// Fires when loading changed.
        /// </summary>
        [Parameter]
        public EventCallback<bool> LoadingChanged { get; set; }

        /// <summary>
        /// If true, the background still remain visible, but user cannot interact them.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public bool Overlap { get; set; } = false;

        /// <summary>
        /// The text shows after the loading indicator.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public string Text { get; set; }

        /// <summary>
        /// CSS classes for the text, seperated by space.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public string ClassText { get; set; }

        /// <summary>
        /// If true, show a darken background.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public bool Darken { get; set; } = false;

        /// <summary>
        /// Set the indicator type. A middle placed circular or top placed linear progress.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public LoaderType LoaderType { get; set; } = LoaderType.Circular;

        /// <summary>
        /// Custom loader content. If it is set, the overlap, darken and loadertype parameters ignored.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public RenderFragment LoaderContent { get; set; }

        /// <summary>
        /// The child content.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public RenderFragment ChildContent { get; set; }

    }
}
