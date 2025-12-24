using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using MudBlazor;
using MudBlazor.Utilities;

namespace MudExtensions
{
    /// <summary>
    /// The extended input component.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public partial class MudInputExtended<T> : MudBaseInputExtended<T>
    {
        [Inject] IJSRuntime JSRuntime { get; set; } = null!;

        /// <summary>
        /// 
        /// </summary>
        protected string? Classname => MudInputCssHelperExtended.GetClassname(this,
            () => HasNativeHtmlPlaceholder() || ShrinkLabel == true || !string.IsNullOrEmpty(ReadText) || !string.IsNullOrWhiteSpace(Placeholder) || HasValue(ReadValue));

        /// <summary>
        /// 
        /// </summary>
        protected string? InputClassname => MudInputCssHelperExtended.GetInputClassname(this);

        /// <summary>
        /// 
        /// </summary>
        protected string? AdornmentClassname => MudInputCssHelperExtended.GetAdornmentClassname(this);

        /// <summary>
        /// 
        /// </summary>
        protected string? AdornmentStartClassname =>
            new CssBuilder("mud-input-adornment")
                .AddClass("mud-input-adornment-start-extended", HasAdornmentStart)
                .AddClass($"mud-input-{Variant.ToDescriptionString()}-extended")
                .AddClass($"mud-text", !string.IsNullOrEmpty(AdornmentText))
                .AddClass($"mud-input-root-filled-shrink", Variant == Variant.Filled)
                .Build();

        /// <summary>
        /// 
        /// </summary>
        protected string? AdornmentEndClassname =>
            new CssBuilder("mud-input-adornment")
                .AddClass("mud-input-adornment-end-extended", HasAdornmentEnd)
                .AddClass($"mud-input-{Variant.ToDescriptionString()}-extended")
                .AddClass($"mud-text", !string.IsNullOrEmpty(AdornmentText))
                .AddClass($"mud-input-root-filled-shrink", Variant == Variant.Filled)
                .Build();

        /// <summary>
        /// 
        /// </summary>
        protected string? ClearButtonClassname =>
                    new CssBuilder()
                    .AddClass("me-n1", Adornment == Adornment.End && !HideSpinButtons)
                    .AddClass("mud-icon-button-edge-end", Adornment == Adornment.End && HideSpinButtons)
                    .AddClass("me-6", Adornment != Adornment.End && !HideSpinButtons)
                    .AddClass("mud-icon-button-edge-margin-end", Adornment != Adornment.End && HideSpinButtons)
                    .Build();

        /// <summary>
        /// 
        /// </summary>
        protected string? ChildContentClassname =>
                    new CssBuilder()
                    .AddClass("d-inline", InputType == InputType.Hidden && ChildContent != null && ShowVisualiser == false)
                    .AddClass("d-none", !(InputType == InputType.Hidden && ChildContent != null && ShowVisualiser == false))
                    .Build();

        private bool _beforeInputAttached;
        private DotNetObjectReference<MudInputExtended<T>>? _dotNetRef;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="firstRender"></param>
        /// <returns></returns>
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
            if (firstRender)
            {
                if (AutoSize)
                {
                    if (JSRuntime != null)
                    {
                        await JSRuntime.InvokeVoidAsync("auto_size", ElementReference);
                    }
                    StateHasChanged();
                }

                if (!_beforeInputAttached)
                {
                    _beforeInputAttached = true;
                    _dotNetRef = DotNetObjectReference.Create(this);

                    await JSRuntime.InvokeVoidAsync("mudBeforeInput.attach", ElementReference, _dotNetRef);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [Parameter] public bool ShowVisualiser { get; set; }

        /// <summary>
        /// CSS styles for data visualiser container.
        /// </summary>
        [Parameter] public string? DataVisualiserStyle { get; set; }

        /// <summary>
        /// Type of the input element. It should be a valid HTML5 input type.
        /// </summary>
        [Parameter] public InputType InputType { get; set; } = InputType.Text;


        /// <summary>
        /// 
        /// </summary>
        protected string? InputTypeString => InputType.ToDescriptionString();

        private async Task OnInputOrOnChangeAsync(string? input)
        {
            if (Immediate)
            {
                await OnInputHandler(input);
                await OnInput.InvokeAsync(input);
            }
            else
            {
                await OnChangeHandler(input);
                await OnChange.InvokeAsync(input);
            }

            if (AutoSize && JSRuntime != null)
            {
                await JSRuntime.InvokeVoidAsync("auto_size", ElementReference);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        protected async Task OnInputHandler(string? args)
        {
            _isFocused = true;
            _internalText = args;
            await OnInternalInputChanged.InvokeAsync(args);
            await SetTextAndUpdateValueAsync(args);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        protected async Task OnChangeHandler(string? args)
        {
            _internalText = args;
            await OnInternalInputChanged.InvokeAsync(args);
            await SetTextAndUpdateValueAsync(args);
        }

        /// <summary>
        /// Forces input height to fit the text content.
        /// </summary>
        /// <returns></returns>
        public virtual async Task ForceAutoSize()
        {
            if (JSRuntime != null)
            {
                await JSRuntime.InvokeVoidAsync("auto_size", ElementReference);
            }
        }

        /// <summary>
        /// If true, automatically resize the height regard to the text. Needs Lines parameter to set more than 1.
        /// </summary>
        [Parameter] public bool AutoSize { get; set; }

        /// <summary>
        /// Paste hook for descendants.
        /// </summary>
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously

        protected virtual async Task OnPaste(ClipboardEventArgs args)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            // do nothing
            return;
        }

        /// <summary>
        /// ChildContent of the MudInput will only be displayed if InputType.Hidden and if its not null.
        /// </summary>
        [Parameter] public RenderFragment? ChildContent { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ElementReference ElementReference { get; private set; }
        private ElementReference _elementReference1;

        /// <summary>
        /// Focuses component.
        /// </summary>
        /// <returns></returns>
        public override async ValueTask FocusAsync()
        {
            try
            {
                if (InputType == InputType.Hidden && ChildContent != null)
                    await _elementReference1.FocusAsync();
                else
                    await ElementReference.FocusAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine("MudInput.FocusAsync: " + e.Message);
            }
        }

        /// <summary>
        /// Blur from component.
        /// </summary>
        /// <returns></returns>
        public override ValueTask BlurAsync()
        {
            return ElementReference.MudBlurAsync();
        }

        /// <summary>
        /// Focus and select all text.
        /// </summary>
        /// <returns></returns>
        public override ValueTask SelectAsync()
        {
            return ElementReference.MudSelectAsync();
        }

        /// <summary>
        /// Focus and select partial text described with positions.
        /// </summary>
        /// <param name="pos1"></param>
        /// <param name="pos2"></param>
        /// <returns></returns>
        public override ValueTask SelectRangeAsync(int pos1, int pos2)
        {
            return ElementReference.MudSelectRangeAsync(pos1, pos2);
        }

        /// <summary>
        /// Invokes the callback when the Up arrow button is clicked when the input is set to <see cref="InputType.Number"/>.
        /// Note: use the optimized control <see cref="MudNumericField{T}"/> if you need to deal with numbers.
        /// </summary>
        [Parameter] public EventCallback OnIncrement { get; set; }

        /// <summary>
        /// Invokes the callback when the Down arrow button is clicked when the input is set to <see cref="InputType.Number"/>.
        /// Note: use the optimized control <see cref="MudNumericField{T}"/> if you need to deal with numbers.
        /// </summary>
        [Parameter] public EventCallback OnDecrement { get; set; }

        /// <summary>
        /// Hides the spin buttons.
        /// </summary>
        [Parameter] public bool HideSpinButtons { get; set; } = true;

        /// <summary>
        /// DavaVisualiser content.
        /// </summary>
        [Parameter] public RenderFragment? DataVisualiser { get; set; }

        /// <summary>
        /// Show clear button.
        /// </summary>
        [Parameter] public bool Clearable { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Parameter] public bool ForceClearable { get; set; }

        /// <summary>
        /// Button click event for clear button. Called after text and value has been cleared.
        /// </summary>
        [Parameter] public EventCallback<MouseEventArgs> OnClearButtonClick { get; set; }

        /// <summary>
        /// Mouse wheel event for input.
        /// </summary>
        [Parameter] public EventCallback<WheelEventArgs> OnMouseWheel { get; set; }

        /// <summary>
        /// Custom clear icon.
        /// </summary>
        [Parameter] public string ClearIcon { get; set; } = Icons.Material.Filled.Clear;

        /// <summary>
        /// Custom numeric up icon.
        /// </summary>
        [Parameter] public string NumericUpIcon { get; set; } = Icons.Material.Filled.KeyboardArrowUp;

        /// <summary>
        /// Custom numeric down icon.
        /// </summary>
        [Parameter] public string NumericDownIcon { get; set; } = Icons.Material.Filled.KeyboardArrowDown;

        private Size GetButtonSize() => Margin == Margin.Dense ? Size.Small : Size.Medium;

        //private bool _showClearable;

        private void UpdateClearable(object? value)
        {
            var showClearable = HasValue((T?)value);
            if (Clearable != showClearable)
                Clearable = showClearable;
        }

        //private bool GetClearable() => Clearable && ((ReadValue is string stringValue && !string.IsNullOrWhiteSpace(stringValue)) || (ReadValue is not string && ReadValue is not null));

        private bool ShowClearButton()
        {
            if (GetDisabledState())
            {
                return false;
            }

            if (!Clearable)
            {
                return false;
            }

            if (GetReadOnlyState())
            {
                return false;
            }

            if (ReadValue is string stringValue)
            {
                return !string.IsNullOrWhiteSpace(stringValue);
            }

            return ReadValue is not string and not null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="updateValue"></param>
        /// <returns></returns>
        //protected override async Task UpdateTextPropertyAsync(bool updateValue)
        //{
        //    await base.UpdateTextPropertyAsync(updateValue);
        //    if (Clearable)
        //        UpdateClearable(ReadText);
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="updateText"></param>
        /// <returns></returns>
        //protected override async Task UpdateValuePropertyAsync(bool updateText)
        //{
        //    await base.UpdateValuePropertyAsync(updateText);
        //    if (Clearable)
        //        UpdateClearable(ReadValue);
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        protected virtual async Task ClearButtonClickHandlerAsync(MouseEventArgs e)
        {
            await SetTextAndUpdateValueAsync(string.Empty, updateValue: true);
            await ElementReference.FocusAsync();
            await OnClearButtonClick.InvokeAsync(e);
        }

        private string? _internalText;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public override async Task SetParametersAsync(ParameterView parameters)
        {
            await base.SetParametersAsync(parameters);
            //if (!_isFocused || _forceTextUpdate)
            //    _internalText = Text;
            _internalText = ReadText;
        }

        /// <summary>
        /// Sets the input text from outside programmatically
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public Task SetText(string? text)
        {
            _internalText = text;
            return SetTextAndUpdateValueAsync(text);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected virtual bool HasNativeHtmlPlaceholder()
        {
            return InputType switch
            {
                InputType.Color => true,
                InputType.Date => true,
                InputType.DateTimeLocal => true,
                InputType.Month => true,
                InputType.Time => true,
                InputType.Week => true,
                _ => false
            };
        }

    }
}
