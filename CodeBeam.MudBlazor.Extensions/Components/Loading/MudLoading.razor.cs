using Microsoft.AspNetCore.Components;
using MudBlazor;
using MudBlazor.State;
using MudBlazor.Utilities;

namespace MudExtensions
{
    /// <summary>
    /// Loading component that covers a loading content with a parameter.
    /// </summary>
    public partial class MudLoading : MudComponentBase
    {
        /// <summary>
        /// MudLoading constructor.
        /// </summary>
        public MudLoading()
        {
            using var registerScope = CreateRegisterScope();
            _loading = registerScope.RegisterParameter<bool>(nameof(Loading))
                .WithParameter(() => Loading)
                .WithEventCallback(() => LoadingChanged);
        }

        private readonly ParameterState<bool> _loading;

        /// <summary>
        /// 
        /// </summary>
        protected string? TextClassname => new CssBuilder()
            .AddClass("mt-4")
            .AddClass(TextClass)
            .Build();
        
        /// <summary>
        /// Two way binded loading state.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public bool Loading { get; set; }

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
        public string? Text { get; set; }

        /// <summary>
        /// CSS classes for the text, seperated by space.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public string? TextClass { get; set; }

        /// <summary>
        /// CSS classes for the progress component, seperated by space.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public string? LoaderClass { get; set; }

        /// <summary>
        /// CSS style for the text.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public string? TextStyle { get; set; }
        
        /// <summary>
        /// CSS style for the progress component.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public string? LoaderStyle { get; set; }

        /// <summary>
        /// If true, show a darken background.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public bool Darken { get; set; } = false;

        /// <summary>
        /// The color of loading indicator. Default is Color.Primary.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public Color Color { get; set; } = Color.Primary;

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
        public RenderFragment? LoaderContent { get; set; }

        /// <summary>
        /// The child content.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public RenderFragment? ChildContent { get; set; }

    }
}
