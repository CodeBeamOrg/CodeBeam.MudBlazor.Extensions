using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using MudBlazor;
using MudExtensions.Base;

namespace MudExtensions
{
    /// <summary>
    /// The extended base input fundamentals.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class MudBaseInputExtended<T> : MudBaseInput<T>
    {
        private string? _userAttributesId = Identifier.Create("mudinputextended");
        private readonly string _componentId = Identifier.Create("mudinputextended");

        /// <summary>
        /// 
        /// </summary>
        protected MudBaseInputExtended()
        {
            Converter = new DefaultConverter<T>
            {
                Culture = GetCulture,
                Format = GetFormat
            };

            //using var registerScope = CreateRegisterScope();
            //_textState = registerScope.RegisterParameter<string?>(nameof(Text))
            //    .WithParameter(() => Text)
            //    .WithEventCallback(() => TextChanged)
            //    .WithChangeHandler(OnTextParameterChangedAsync);
            //_valueState = registerScope.RegisterParameter<T?>(nameof(Value))
            //    .WithParameter(() => Value)
            //    .WithEventCallback(() => ValueChanged)
            //    .WithChangeHandler(OnValueParameterChangedAsync);
            //_formatState = registerScope.RegisterParameter<string?>(nameof(Format))
            //    .WithParameter(() => Format)
            //    .WithChangeHandler(OnCultureAndFormatChangedAsync);
            //_inputIdState = registerScope.RegisterParameter<string?>(nameof(InputId))
            //    .WithParameter(() => InputId)
            //    .WithChangeHandler(UpdateInputIdStateAsync);
        }



        /// <summary>
        /// Fires on input.
        /// </summary>
        [Parameter] public EventCallback OnInput { get; set; }

        /// <summary>
        /// Fires on change.
        /// </summary>
        [Parameter] public EventCallback OnChange { get; set; }

        [Parameter]
        public EventCallback<MudBeforeInputEventArgs> OnBeforeInput { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the component has an adornment at the start.
        /// </summary>
        [Parameter]
        public bool HasAdornmentStart { get; set; }

        /// <summary>
        /// The Adornment if used. By default, it is set to None.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public RenderFragment? AdornmentStart { get; set; }

        /// <summary>
        /// The Adornment if used. By default, it is set to None.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public RenderFragment? AdornmentEnd { get; set; }

        /// <summary>
        /// CSS style of the child content.
        /// </summary>
        [Parameter]
        public string? ChildContentStyle { get; set; }

        /// <summary>
        /// If true disables paste to input component. Default is false.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public bool DisablePaste { get; set; }

        protected async Task InvokeBeforeInputAsync(MudBeforeInputEventArgs args)
        {
            _isFocused = true;
            await OnBeforeInputAsync(args);

            if (OnBeforeInput.HasDelegate)
                await OnBeforeInput.InvokeAsync(args);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override async Task OnCultureAndFormatChangedAsync()
        {
            await base.OnCultureAndFormatChangedAsync();
            await UpdateTextPropertyAsync(false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override async Task OnConverterChangedAsync()
        {
            await base.OnConverterChangedAsync();
            await UpdateTextPropertyAsync(false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected string? ResolveAriaDescribedBy() => GetAriaDescribedByString();

        [JSInvokable("OnBeforeInput")]
        public async Task<bool> OnBeforeInputFromJs(MudBeforeInputJsDto dto)
        {
            var args = new MudBeforeInputEventArgs
            {
                Data = dto.Data,
                InputType = dto.InputType,
                IsComposing = dto.IsComposing
            };

            await InvokeBeforeInputAsync(args);
            return args.PreventDefault;
        }

        protected virtual Task OnBeforeInputAsync(MudBeforeInputEventArgs args) => Task.CompletedTask;


    }
}
