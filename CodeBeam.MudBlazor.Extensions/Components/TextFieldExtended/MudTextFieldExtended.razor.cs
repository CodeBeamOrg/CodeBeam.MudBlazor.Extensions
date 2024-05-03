using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using MudBlazor.Utilities;

namespace MudExtensions
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public partial class MudTextFieldExtended<T> : MudDebouncedInputExtended<T>
    {
        /// <summary>
        /// 
        /// </summary>
        protected string? Classname =>
           new CssBuilder("mud-input-input-control")
           .AddClass(Class)
           .Build();

        /// <summary>
        /// 
        /// </summary>
        [CascadingParameter]
        public bool SubscribeToParentForm2 { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public MudInputExtended<string> InputReference { get; private set; } = new();
        private MudMask _maskReference = new();

        /// <summary>
        /// If true, automatically resize the height regard to the text. Needs Lines parameter to set more than 1.
        /// </summary>
        [Parameter] public bool AutoSize { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task ForceAutoSize()
        {
            await InputReference.ForceAutoSize();
        }

        /// <summary>
        /// The render fragment for child content.
        /// </summary>
        [Parameter] public RenderFragment? DataVisualiser { get; set; }

        /// <summary>
        /// Type of the input element. It should be a valid HTML5 input type.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public InputType InputType { get; set; } = InputType.Text;

        internal override InputType GetInputType() => InputType;

        private string GetCounterText() => Counter == null ? string.Empty : (Counter == 0 ? (string.IsNullOrEmpty(Text) ? "0" : $"{Text.Length}") : ((string.IsNullOrEmpty(Text) ? "0" : $"{Text.Length}") + $" / {Counter}"));

        /// <summary>
        /// Show clear button.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public bool Clearable { get; set; } = false;

        /// <summary>
        /// 
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public bool ShowVisualiser { get; set; }

        /// <summary>
        /// Button click event for clear button. Called after text and value has been cleared.
        /// </summary>
        [Parameter] public EventCallback<MouseEventArgs> OnClearButtonClick { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override ValueTask FocusAsync()
        {
            if (_mask == null)
                return InputReference.FocusAsync();
            else
                return _maskReference.FocusAsync();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override ValueTask BlurAsync()
        {
            if (_mask == null)
                return InputReference.BlurAsync();
            else
                return _maskReference.BlurAsync();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override ValueTask SelectAsync()
        {
            if (_mask == null)
                return InputReference.SelectAsync();
            else
                return _maskReference.SelectAsync();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pos1"></param>
        /// <param name="pos2"></param>
        /// <returns></returns>
        public override ValueTask SelectRangeAsync(int pos1, int pos2)
        {
            if (_mask == null)
                return InputReference.SelectRangeAsync(pos1, pos2);
            else
                return _maskReference.SelectRangeAsync(pos1, pos2);
        }

        /// <summary>
        /// 
        /// </summary>
        protected override async Task ResetValueAsync()
        {
            if (_mask == null)
                await InputReference.ResetAsync();
            else
                await _maskReference.ResetAsync();
            await base.ResetValueAsync();
        }

        /// <summary>
        /// Clear the text field, set Value to default(T) and Text to null
        /// </summary>
        /// <returns></returns>
        public Task Clear()
        {
            if (_mask == null)
                return InputReference.SetText(null);
            else
                return _maskReference.Clear();
        }

        /// <summary>
        /// Sets the input text from outside programmatically
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public async Task SetText(string text)
        {
            if (_mask == null)
            {
                if (InputReference != null)
                    await InputReference.SetText(text);
                return;
            }
            await _maskReference.Clear();
            //_maskReference.OnPaste(text);
        }


        private IMask? _mask = null;

        /// <summary>
        /// Provide a masking object. Built-in masks are PatternMask, MultiMask, RegexMask and BlockMask
        /// Note: when Mask is set, TextField will ignore some properties such as Lines, Pattern or HideSpinButtons, OnKeyDown and OnKeyUp, etc.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.General.Data)]
        public IMask? Mask
        {
            get => _maskReference?.Mask ?? _mask; // this might look strange, but it is absolutely necessary due to how MudMask works.
            set
            {
                _mask = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="updateText"></param>
        /// <param name="force"></param>
        /// <returns></returns>
        protected override Task SetValueAsync(T? value, bool updateText = true, bool force = false)
        {
            if (_mask != null)
            {
                var textValue = Converter.Set(value);
                _mask.SetText(textValue);
                textValue = Mask?.GetCleanText();
                value = Converter.Get(textValue);
            }
            return base.SetValueAsync(value, updateText, force);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="updateValue"></param>
        /// <returns></returns>
        protected override Task SetTextAsync(string? text, bool updateValue = true)
        {
            if (_mask != null)
            {
                _mask.SetText(text);
                text = _mask.Text;
            }
            return base.SetTextAsync(text, updateValue);
        }

        private async Task OnMaskedValueChanged(string s)
        {
            await SetTextAsync(s);
        }

    }
}
