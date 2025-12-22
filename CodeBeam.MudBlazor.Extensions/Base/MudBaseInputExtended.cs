using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using MudBlazor.State;
using MudBlazor.Utilities;
using MudExtensions.Base;

namespace MudExtensions
{
    /// <summary>
    /// The extended base input fundamentals.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class MudBaseInputExtended<T> : MudFormComponent<T, string>
    {
        private bool _isDirty;
        private bool _validated;
        protected bool _isFocused;
        protected bool _forceTextUpdate;
        protected string? InputElementId => _inputIdState.Value;
        private string? _userAttributesId = Identifier.Create("mudinputextended");
        private readonly string _componentId = Identifier.Create("mudinputextended");

        /// <summary>
        /// 
        /// </summary>
        protected virtual bool SkipUpdateProcessOnSetParameters { get; set; }

        private readonly ParameterState<string?> _textState;
        private readonly ParameterState<T?> _valueState;
        private readonly ParameterState<string?> _formatState;
        private readonly ParameterState<string?> _inputIdState;

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

            using var registerScope = CreateRegisterScope();
            _textState = registerScope.RegisterParameter<string?>(nameof(Text))
                .WithParameter(() => Text)
                .WithEventCallback(() => TextChanged)
                .WithChangeHandler(OnTextParameterChangedAsync);
            _valueState = registerScope.RegisterParameter<T?>(nameof(Value))
                .WithParameter(() => Value)
                .WithEventCallback(() => ValueChanged)
                .WithChangeHandler(OnValueParameterChangedAsync);
            _formatState = registerScope.RegisterParameter<string?>(nameof(Format))
                .WithParameter(() => Format)
                .WithChangeHandler(OnCultureAndFormatChangedAsync);
            _inputIdState = registerScope.RegisterParameter<string?>(nameof(InputId))
                .WithParameter(() => InputId)
                .WithChangeHandler(UpdateInputIdStateAsync);
        }

        [CascadingParameter(Name = "ParentDisabled")] private bool ParentDisabled { get; set; }
        [CascadingParameter(Name = "ParentReadOnly")] private bool ParentReadOnly { get; set; }

        /// <summary>
        /// Disable input component if true. Default is false.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public bool Disabled { get; set; }

        /// <summary>
        /// Get the input component is disabled or not.
        /// </summary>
        /// <returns></returns>
        protected bool GetDisabledState() => Disabled || ParentDisabled;

        /// <summary>
        /// If true, the input will be read-only.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public bool ReadOnly { get; set; }

        /// <summary>
        /// Get the input component is readonly or not.
        /// </summary>
        /// <returns></returns>
        protected bool GetReadOnlyState() => ReadOnly || ParentReadOnly;

        /// <summary>
        /// Fires on input.
        /// </summary>
        [Parameter] public EventCallback OnInput { get; set; }

        /// <summary>
        /// Fires on change.
        /// </summary>
        [Parameter] public EventCallback OnChange { get; set; }

        /// <summary>
        /// The ID of the input element.
        /// </summary>
        /// <remarks>
        /// When set takes precedence over any internally generated IDs.
        /// </remarks>
        [Parameter, ParameterState]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public string? InputId { get; set; }

        /// <summary>
        /// Set the text-align on the component.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Text.Appearance)]
        public Typo Typo { get; set; } = Typo.subtitle1;

        /// <summary>
        /// If true, the input will take up the full width of its container.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public bool FullWidth { get; set; }

        /// <summary>
        /// If true, the input will update the Value immediately on typing.
        /// If false, the Value is updated only on Enter.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public bool Immediate { get; set; }

        /// <summary>
        /// If false, the input will not have an underline.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public bool Underline { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether the component has an adornment at the start.
        /// </summary>
        [Parameter]
        public bool HasAdornmentStart { get; set; }

        /// <summary>
        /// The HelperText will be displayed below the text field.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public string? HelperText { get; set; }

        /// <summary>
        /// If true, the helper text will only be visible on focus.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public bool HelperTextOnFocus { get; set; }

        /// <summary>
        /// Icon that will be used if Adornment is set to Start or End.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public string? AdornmentIcon { get; set; }

        /// <summary>
        /// Text that will be used if Adornment is set to Start or End, the Text overrides Icon.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public string? AdornmentText { get; set; }

        /// <summary>
        /// The Adornment if used. By default, it is set to None.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public Adornment Adornment { get; set; } = Adornment.None;

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
        /// The aria-label of the adornment.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public string? AdornmentAriaLabel { get; set; } = string.Empty;

        /// <summary>
        /// The validation is only triggered if the user has changed the input value at least once. By default, it is false
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public bool OnlyValidateIfDirty { get; set; } = false;

        /// <summary>
        /// If true shrinks label directly.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public bool ShrinkLabel { get; set; }

        /// <summary>
        /// The color of the adornment if used. It supports the theme colors.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public Color AdornmentColor { get; set; } = Color.Default;

        /// <summary>
        /// The Icon Size.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public Size IconSize { get; set; } = Size.Medium;

        /// <summary>
        /// Button click event if set and Adornment used.
        /// </summary>
        [Parameter] public EventCallback<MouseEventArgs> OnAdornmentClick { get; set; }

        /// <summary>
        /// Variant to use.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public Variant Variant { get; set; } = Variant.Text;

        /// <summary>
        ///  Will adjust vertical spacing.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public Margin Margin { get; set; } = Margin.None;

        /// <summary>
        /// The short hint displayed in the input before the user enters a value.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public string? Placeholder { get; set; }

        /// <summary>
        /// If set, will display the counter, value 0 will display current count but no stop count.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Validation)]
        public int? Counter { get; set; }

        /// <summary>
        /// Maximum number of characters that the input will accept
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Validation)]
        public int MaxLength { get; set; } = 524288;

        /// <summary>
        /// If string has value the label text will be displayed in the input, and scaled down at the top if the input has value.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public string? Label { get; set; }

        /// <summary>
        /// If true the input will focus automatically.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public bool AutoFocus { get; set; }

        /// <summary>
        ///  A multiline input (textarea) will be shown, if set to more than one line.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public int Lines { get; set; } = 1;

        /// <summary>
        ///  The text to be displayed.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Data)]
        public string? Text { get; set; }

        /// <summary>
        /// When TextUpdateSuppression is true (which is default) the text can not be updated by bindings while the component is focused in BSS (not WASM).
        /// This solves issue #1012: Textfield swallowing chars when typing rapidly
        /// If you need to update the input's text while it is focused you can set this parameter to false.
        /// Note: on WASM text update suppression is not active, so this parameter has no effect.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public bool TextUpdateSuppression { get; set; } = true;

        /// <summary>
        ///  Hints at the type of data that might be entered by the user while editing the input
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public virtual InputMode InputMode { get; set; } = InputMode.text;

        /// <summary>
        /// The pattern attribute, when specified, is a regular expression which the input's value must match in order for the value to pass constraint validation. It must be a valid JavaScript regular expression
        /// Not Supported in multline input
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Validation)]
        public virtual string? Pattern { get; set; }

        /// <summary>
        /// CSS style of the child content.
        /// </summary>
        [Parameter]
        public string? ChildContentStyle { get; set; }

        /// <summary>
        /// Sync the value, values and text, calls validation manually. Useful to call after user changes value or text programmatically.
        /// </summary>
        /// <returns></returns>
        public virtual async Task ForceUpdate()
        {
            await SetValueAsync(Value, force: true);
        }

        /// <summary>
        /// Derived classes need to override this if they can be something other than text
        /// </summary>
        internal virtual InputType GetInputType() { return InputType.Text; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="updateValue"></param>
        /// <returns></returns>
        protected virtual async Task SetTextAsync(string? text, bool updateValue = true)
        {
            if (Text != text)
            {
                Text = text;
                _validated = false;
                if (!string.IsNullOrWhiteSpace(Text))
                    Touched = true;
                if (updateValue)
                    await UpdateValuePropertyAsync(false);
                await TextChanged.InvokeAsync(Text);
            }
        }

        /// <summary>
        /// Text change hook for descendants. Called when Text needs to be refreshed from current Value property.
        /// </summary>
        protected virtual Task UpdateTextPropertyAsync(bool updateValue)
        {
            return SetTextAndUpdateValueAsync(ConvertSet(ReadValue), updateValue);
        }

        /// <summary>
        /// Focus to the element.
        /// </summary>
        /// <returns>The ValueTask</returns>
        public virtual ValueTask FocusAsync() => ValueTask.CompletedTask;

        /// <summary>
        /// Blur from the element.
        /// </summary>
        /// <returns></returns>
        public virtual ValueTask BlurAsync() => ValueTask.CompletedTask;

        /// <summary>
        /// Focus and select all text.
        /// </summary>
        /// <returns></returns>
        public virtual ValueTask SelectAsync() => ValueTask.CompletedTask;

        /// <summary>
        /// Focus and select partial text with given positions.
        /// </summary>
        /// <param name="pos1"></param>
        /// <param name="pos2"></param>
        /// <returns></returns>
        public virtual ValueTask SelectRangeAsync(int pos1, int pos2) => ValueTask.CompletedTask;

        /// <summary>
        /// Fired when the text value changes.
        /// </summary>
        [Parameter] public EventCallback<string?> TextChanged { get; set; }

        /// <summary>
        /// Fired when the element loses focus.
        /// </summary>
        [Parameter] public EventCallback<FocusEventArgs> OnBlur { get; set; }

        /// <summary>
        /// Fired when the element changes internally its text value.
        /// </summary>
        [Parameter]
        public EventCallback<string?> OnInternalInputChanged { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        protected internal virtual async Task OnBlurredAsync(FocusEventArgs obj)
        {
            if (ReadOnly)
            {
                return;
            }

            _isFocused = false;

            if (!OnlyValidateIfDirty || _isDirty)
            {
                Touched = true;
                if (_validated)
                    await OnBlur.InvokeAsync(obj);
                else
                    await BeginValidationAfterAsync(OnBlur.InvokeAsync(obj));
            }
        }

        /// <summary>
        /// Fired on the KeyDown event.
        /// </summary>
        [Parameter] public EventCallback<KeyboardEventArgs> OnKeyDown { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        protected virtual async Task InvokeKeyDownAsync(KeyboardEventArgs obj)
        {
            _isFocused = true;
            await OnKeyDown.InvokeAsync(obj);
        }

        /// <summary>
        /// Prevent the default action for the KeyDown event.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public bool KeyDownPreventDefault { get; set; }

        /// <summary>
        /// If true disables paste to input component. Default is false.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public bool DisablePaste { get; set; }

        /// <summary>
        /// Fired on the KeyUp event.
        /// </summary>
        [Parameter] public EventCallback<KeyboardEventArgs> OnKeyUp { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        protected virtual async Task InvokeKeyUpAsync(KeyboardEventArgs obj)
        {
            _isFocused = true;
            await OnKeyUp.InvokeAsync(obj);
        }

        /// <summary>
        /// Prevent the default action for the KeyUp event.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public bool KeyUpPreventDefault { get; set; }

        /// <summary>
        /// Fired when the Value property changes.
        /// </summary>
        [Parameter]
        public EventCallback<T> ValueChanged { get; set; }

        /// <summary>
        /// The value of this input element.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Data)]
        public T? Value
        {
            get => _value;
            set => _value = value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="updateText"></param>
        /// <param name="force"></param>
        /// <returns></returns>
        protected virtual async Task SetValueAsync(T? value, bool updateText = true, bool force = false)
        {
            if (!EqualityComparer<T?>.Default.Equals(Value, value) || force == true)
            {
                _isDirty = true;
                _validated = false;
                Value = value;
                await ValueChanged.InvokeAsync(Value);
                if (updateText)
                    await UpdateTextPropertyAsync(false);
                FieldChanged(Value);
                await BeginValidateAsync();
            }
        }

        /// <summary>
        /// Value change hook for descendants. Called when Value needs to be refreshed from current Text property.
        /// </summary>
        protected virtual Task UpdateValuePropertyAsync(bool updateText)
        {
            return SetValueAndUpdateTextAsync(ConvertGet(ReadText), updateText);
        }

        /// <summary>
        /// The format applied to values.
        /// </summary>
        /// <remarks>
        /// This property is passed into the <c>ToString()</c> method of the <see cref="Value"/> property, such as formatting <c>int</c>, <c>float</c>, <c>DateTime</c> and <c>TimeSpan</c> values.
        /// </remarks>
        [Parameter, ParameterState]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public string? Format { get; set; }

        protected override string? GetFormat() => _formatState.Value;

        protected override async Task OnCultureAndFormatChangedAsync()
        {
            await base.OnCultureAndFormatChangedAsync();
            await UpdateTextPropertyAsync(false);
        }

        protected override async Task OnConverterChangedAsync()
        {
            await base.OnConverterChangedAsync();
            await UpdateTextPropertyAsync(false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override async Task ValidateValue()
        {
            if (SubscribeToParentFormExtended)
            {
                try
                {
                    _validated = true;
                    await base.ValidateValue();
                }
                catch
                {

                }
                
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            // Because the way the Value setter is built, it won't cause an update if the incoming Value is
            // equal to the initial value. This is why we force an update to the Text property here.
            if (typeof(T) != typeof(string))
                await UpdateTextPropertyAsync(false);

            if (Label == null && For != null)
                Label = For.GetLabelString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="forceTextUpdate"></param>
        /// <returns></returns>
        public virtual async Task ForceRender(bool forceTextUpdate)
        {
            _forceTextUpdate = true;
            await UpdateTextPropertyAsync(false);
            StateHasChanged();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public override async Task SetParametersAsync(ParameterView parameters)
        {
            await base.SetParametersAsync(parameters);
            if (SkipUpdateProcessOnSetParameters == true)
            {
                return;
            }
            var hasText = parameters.Contains<string>(nameof(Text));
            var hasValue = parameters.Contains<T>(nameof(Value));

            // Refresh Value from Text
            if (hasText && !hasValue)
                await UpdateValuePropertyAsync(false);

            // Refresh Text from Value
            if (hasValue && !hasText)
            {
                var updateText = true;
                if (_isFocused && !_forceTextUpdate)
                {
                    // Text update suppression, only in BSS (not in WASM).
                    // This is a fix for #1012
                    if (RuntimeLocation.IsServerSide && TextUpdateSuppression)
                        updateText = false;
                }
                if (updateText)
                {
                    _forceTextUpdate = false;
                    await UpdateTextPropertyAsync(false);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="firstRender"></param>
        /// <returns></returns>
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            //Only focus automatically after the first render cycle!
            if (firstRender && AutoFocus)
            {
                await FocusAsync();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnParametersSet()
        {
            if (SubscribeToParentFormExtended)
                base.OnParametersSet();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override async Task ResetValueAsync()
        {
            await SetTextAsync(null, updateValue: true);
            _isDirty = false;
            _validated = false;
            await base.ResetValueAsync();
        }

        [CascadingParameter(Name = "SubscribeToParentFormExtended")]
        internal bool SubscribeToParentFormExtended { get; set; } = true;

        private async Task UpdateInputIdStateAsync()
        {
            if (_inputIdState.Value is not null)
            {
                return;
            }

            if (_userAttributesId is not null)
            {
                await _inputIdState.SetValueAsync(_userAttributesId);
                return;
            }

            await _inputIdState.SetValueAsync(_componentId);
        }

        private async Task OnValueParameterChangedAsync(ParameterChangedEventArgs<T?> arg)
        {
            _isDirty = true;
            _validated = false;

            // When Value changes from parent, update Text from Value
            // But only if Text is not also being set in the same parameter update
            // Check ParameterView to see if Text is also present
            if (!arg.ParameterView.Contains<string?>(nameof(Text)))
            {
                // Always update text when Value changes (TextUpdateSuppression removed)
                _forceTextUpdate = false;
                await UpdateTextPropertyAsync(false);
            }
        }

        private async Task OnTextParameterChangedAsync(ParameterChangedEventArgs<string?> arg)
        {
            _validated = false;

            if (!string.IsNullOrEmpty(arg.Value))
            {
                Touched = true;
            }

            // When Text changes from parent, update Value from Text using UpdateValuePropertyAsync
            // But only if Value is not also being set in the same parameter update
            // Check ParameterView to see if Value is also present
            if (!arg.ParameterView.Contains<T?>(nameof(Value)))
            {
                await UpdateValuePropertyAsync(updateText: false);
            }
        }

        protected internal string? ReadText => _textState.Value;
        protected Task SetTextAsync(string? text) => _textState.SetValueAsync(text);

        protected virtual async Task SetValueAndUpdateTextAsync(T? value, bool updateText = true, bool force = false)
        {
            var valueChanged = !EqualityComparer<T?>.Default.Equals(ReadValue, value);

            if (!valueChanged && !force)
            {
                return;
            }

            _isDirty = true;
            _validated = false;

            // Use ParameterState to set Value instead of direct assignment
            // This ensures proper parameter lifecycle management
            await _valueState.SetValueAsync(value);

            // If force is true but value hasn't changed, ParameterState won't fire the callback
            // so we need to manually invoke it to maintain backward compatibility
            if (force && !valueChanged)
            {
                await ValueChanged.InvokeAsync(value);
            }

            if (updateText)
            {
                await UpdateTextPropertyAsync(false);
            }

            FieldChanged(value);
            await BeginValidateAsync();
        }

        protected virtual async Task SetTextAndUpdateValueAsync(string? text, bool updateValue = true)
        {
            if (ReadText == text)
            {
                return;
            }

            _validated = false;

            if (!string.IsNullOrEmpty(text))
            {
                Touched = true;
            }

            await _textState.SetValueAsync(text);
            if (updateValue)
            {
                await UpdateValuePropertyAsync(false);
            }
        }
    }

    internal static class ParameterViewExtensions
    {
        public static bool Contains<T>(this ParameterView view, string parameterName)
        {
            return view.TryGetValue<T>(parameterName, out var _);
        }
    }
}
