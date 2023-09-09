using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using MudBlazor;
using MudBlazor.Extensions;
using MudBlazor.Utilities;

namespace MudExtensions
{
    public partial class MudInputExtended<T> : MudBaseInputExtended<T>
    {
        [Inject] IJSRuntime JSRuntime { get; set; }

        protected string Classname => MudInputCssHelperExtended.GetClassname(this,
            () => HasNativeHtmlPlaceholder() || ForceShrink == true || !string.IsNullOrEmpty(Text) || AdornmentStart != null || !string.IsNullOrWhiteSpace(Placeholder) || !string.IsNullOrEmpty(Converter.Set(Value)));

        protected string InputClassname => MudInputCssHelperExtended.GetInputClassname(this);

        protected string AdornmentClassname => MudInputCssHelperExtended.GetAdornmentClassname(this);

        protected string AdornmentStartClassname =>
            new CssBuilder("mud-input-adornment mud-input-adornment-start-extended")
                .AddClass($"mud-input-{Variant.ToDescriptionString()}-extended")
                .AddClass($"mud-text", !string.IsNullOrEmpty(AdornmentText))
                .AddClass($"mud-input-root-filled-shrink", Variant == Variant.Filled)
                .Build();

        protected string AdornmentEndClassname =>
            new CssBuilder("mud-input-adornment mud-input-adornment-end-extended")
                .AddClass($"mud-input-{Variant.ToDescriptionString()}-extended")
                .AddClass($"mud-text", !string.IsNullOrEmpty(AdornmentText))
                .AddClass($"mud-input-root-filled-shrink", Variant == Variant.Filled)
                .Build();

        protected string ClearButtonClassname =>
                    new CssBuilder()
                    .AddClass("me-n1", Adornment == Adornment.End && !HideSpinButtons)
                    .AddClass("mud-icon-button-edge-end", Adornment == Adornment.End && HideSpinButtons)
                    .AddClass("me-6", Adornment != Adornment.End && !HideSpinButtons)
                    .AddClass("mud-icon-button-edge-margin-end", Adornment != Adornment.End && HideSpinButtons)
                    .Build();

        protected string ChildContentClassname =>
                    new CssBuilder()
                    .AddClass("d-inline", InputType == InputType.Hidden && ChildContent != null && ShowVisualiser == false)
                    .AddClass("d-none", !(InputType == InputType.Hidden && ChildContent != null && ShowVisualiser == false))
                    .Build();

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
            if (firstRender)
            {
                if (AutoSize)
                {
                    await JSRuntime.InvokeVoidAsync("auto_size", ElementReference);
                    StateHasChanged();
                }
            }
        }

        [Parameter] public bool ShowVisualiser { get; set; }
        [Parameter] public string DataVisualiserStyle { get; set; }

        /// <summary>
        /// Type of the input element. It should be a valid HTML5 input type.
        /// </summary>
        [Parameter] public InputType InputType { get; set; } = InputType.Text;

        internal override InputType GetInputType() => InputType;

        protected string InputTypeString => InputType.ToDescriptionString();

        protected Task OnInputHandler(ChangeEventArgs args)
        {
            if (!Immediate)
                return Task.CompletedTask;
            _isFocused = true;
            OnInput.InvokeAsync();
            if (AutoSize)
            {
                JSRuntime.InvokeVoidAsync("auto_size", ElementReference);
            }
            return SetTextAsync(args?.Value as string);
        }

        protected async Task OnChangeHandler(ChangeEventArgs args)
        {
            _internalText = args?.Value as string;
            await OnInternalInputChanged.InvokeAsync(args);
            if (!Immediate)
            {
                await SetTextAsync(args?.Value as string);
                if (AutoSize)
                {
                    await JSRuntime.InvokeVoidAsync("auto_size", ElementReference);
                }
                
                await OnChange.InvokeAsync();
            }
        }

        public virtual async Task ForceAutoSize()
        {
            await JSRuntime.InvokeVoidAsync("auto_size", ElementReference);
        }

        /// <summary>
        /// If true, automatically resize the height regard to the text. Needs Lines parameter to set more than 1.
        /// </summary>
        [Parameter] public bool AutoSize { get; set; }

        /// <summary>
        /// Fires on input.
        /// </summary>
        [Parameter] public EventCallback OnInput { get; set; }

        /// <summary>
        /// Fires on change.
        /// </summary>
        [Parameter] public EventCallback OnChange { get; set; }

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
        [Parameter] public RenderFragment ChildContent { get; set; }

        public ElementReference ElementReference { get; private set; }
        private ElementReference _elementReference1;

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

        public override ValueTask BlurAsync()
        {
            return ElementReference.MudBlurAsync();
        }

        public override ValueTask SelectAsync()
        {
            return ElementReference.MudSelectAsync();
        }

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
        /// Hides the spin buttons for <see cref="MudNumericField{T}"/>
        /// </summary>
        [Parameter] public bool HideSpinButtons { get; set; } = true;

        [Parameter] public RenderFragment DataVisualiser { get; set; }

        /// <summary>
        /// Show clear button.
        /// </summary>
        [Parameter] public bool Clearable { get; set; }

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

        private bool _showClearable;

        private void UpdateClearable(object value)
        {
            var showClearable = HasValue((T)value);
            if (Clearable != showClearable)
                Clearable = showClearable;
        }

        private bool GetClearable() => Clearable && ((Value is string stringValue && !string.IsNullOrWhiteSpace(stringValue)) || (Value is not string && Value is not null));

        protected override async Task UpdateTextPropertyAsync(bool updateValue)
        {
            await base.UpdateTextPropertyAsync(updateValue);
            if (Clearable)
                UpdateClearable(Text);
        }

        protected override async Task UpdateValuePropertyAsync(bool updateText)
        {
            await base.UpdateValuePropertyAsync(updateText);
            if (Clearable)
                UpdateClearable(Value);
        }

        protected virtual async Task ClearButtonClickHandlerAsync(MouseEventArgs e)
        {
            await SetTextAsync(string.Empty, updateValue: true);
            await ElementReference.FocusAsync();
            await OnClearButtonClick.InvokeAsync(e);
        }

        private string _internalText;

        public override async Task SetParametersAsync(ParameterView parameters)
        {
            await base.SetParametersAsync(parameters);
            //if (!_isFocused || _forceTextUpdate)
            //    _internalText = Text;
            if (RuntimeLocation.IsServerSide && TextUpdateSuppression)
            {
                // Text update suppression, only in BSS (not in WASM).
                // This is a fix for #1012
                if (!_isFocused || _forceTextUpdate)
                    _internalText = Text;
            }
            else
            {
                // in WASM (or in BSS with TextUpdateSuppression==false) we always update
                _internalText = Text;
            }
        }

        /// <summary>
        /// Sets the input text from outside programmatically
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public Task SetText(string text)
        {
            _internalText = text;
            return SetTextAsync(text);
        }


        // Certain HTML5 inputs (dates and color) have a native placeholder
        private bool HasNativeHtmlPlaceholder()
        {
            return GetInputType() is InputType.Color or InputType.Date or InputType.DateTimeLocal or InputType.Month
                or InputType.Time or InputType.Week;
        }
    }

}
