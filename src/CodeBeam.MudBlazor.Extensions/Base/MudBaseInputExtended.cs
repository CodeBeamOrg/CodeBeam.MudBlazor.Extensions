using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;

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

        /// <summary>
        /// Gets or sets a callback that is invoked before input is processed.
        /// </summary>
        /// <remarks>Use this callback to perform custom logic or validation before the input event is
        /// handled. This can be useful for intercepting or modifying input behavior in advanced scenarios
        /// </remarks>
        [Parameter]
        public EventCallback<BeforeInputEventArgs> OnBeforeInput { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the component has an adornment at the start.
        /// </summary>
        [Parameter]
        public bool HasAdornmentStart { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the component has an adornment at the end.
        /// </summary>
        [Parameter]
        public bool HasAdornmentEnd { get; set; }

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

        /// <summary>
        /// Invokes logic to be executed before input is processed, including raising the BeforeInput event if a
        /// delegate is assigned.
        /// </summary>
        /// <param name="args">The event data associated with the before input operation.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        protected async Task InvokeBeforeInputAsync(BeforeInputEventArgs args)
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [JSInvokable("OnBeforeInput")]
        public async Task<bool> OnBeforeInputFromJs(BeforeInputJsDto dto)
        {
            var args = new BeforeInputEventArgs
            {
                Data = dto.Data,
                InputType = dto.InputType ?? string.Empty,
                IsComposing = dto.IsComposing
            };

            await InvokeBeforeInputAsync(args);
            return args.PreventDefault;
        }

        /// <summary>
        /// Invoked before processing input, allowing for custom logic or validation to be performed asynchronously.
        /// </summary>
        /// <remarks>Override this method in a derived class to implement custom pre-processing or
        /// validation logic before input is handled. This method is called before the main input processing
        /// occurs.</remarks>
        /// <param name="args">An object containing event data for the input operation.</param>
        /// <returns>A task that represents the asynchronous operation. The default implementation returns a completed task.</returns>
        protected virtual Task OnBeforeInputAsync(BeforeInputEventArgs args) => Task.CompletedTask;
    }
}
