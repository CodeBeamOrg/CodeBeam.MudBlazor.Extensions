using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using MudBlazor.Utilities;

namespace MudExtensions
{
    /// <summary>
    /// A Mud input component has special features to working with chips.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public partial class MudChipField<T> : MudTextFieldExtended<T>
    {
        /// <summary>
        /// Protected classes.
        /// </summary>
        protected string? ChipClassname =>
           new CssBuilder("d-flex")
            .AddClass("flex-wrap", WrapChips)
            .AddClass("mt-5", Variant == Variant.Filled)
            .Build();

        /// <summary>
        /// Protected styles.
        /// </summary>
        protected string? ChipStylename =>
           new StyleBuilder()
            .AddStyle("max-width", $"{ChipsMaxWidth}%")
            .Build();

        MudTextFieldExtended<T> _textFieldExtendedReference = new();
        T? _internalValue;

        /// <summary>
        /// /The list of values.
        /// </summary>
        [Parameter]
        public List<string>? Values { get; set; }

        /// <summary>
        /// Fires when values changed
        /// </summary>
        [Parameter]
        public EventCallback<List<string>> ValuesChanged { get; set; }

        /// <summary>
        /// If false, pressing delimeter key has no effect if the value is already in values. Default is false.
        /// </summary>
        [Parameter]
        public bool AllowSameValues { get; set; }

        /// <summary>
        /// Determines chip size with small, medium or large values.
        /// </summary>
        [Parameter]
        public Size ChipSize { get; set; }

        /// <summary>
        /// The char that created a new chip with current value.
        /// </summary>
        [Parameter]
        public string? Delimiter { get; set; } = " ";

        /// <summary>
        /// CSS classes of the chips, seperated by space.
        /// </summary>
        [Parameter]
        public string? ClassChip { get; set; }

        /// <summary>
        /// CSS styles of the chips.
        /// </summary>
        [Parameter]
        public string? StyleChip { get; set; }

        /// <summary>
        /// Color of the chips.
        /// </summary>
        [Parameter]
        public Color ChipColor { get; set; }

        /// <summary>
        /// Variant of the chips.
        /// </summary>
        [Parameter]
        public Variant ChipVariant { get; set; }

        /// <summary>
        /// If true, the chips that exceed width goes to the below line.
        /// </summary>
        [Parameter]
        public bool WrapChips { get; set; }

        /// <summary>
        /// Determines that chips have close button. Default is true.
        /// </summary>
        [Parameter]
        public bool Closeable { get; set; } = true;

        /// <summary>
        /// Removes last created chip value when press Backspace. Default is true.
        /// </summary>
        [Parameter]
        public bool BackspaceChipRemoval { get; set; } = true;

        /// <summary>
        /// Maximum chip count. Set 0 to unlimited. Default is 0.
        /// </summary>
        [Parameter]
        public int MaxChips { get; set; }

        /// <summary>
        /// Max width for each chip as integer value. Default is 80.
        /// </summary>
        [Parameter]
        public int ChipsMaxWidth { get; set; } = 80;

        /// <summary>
        /// Protected keydown event.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        protected internal async Task HandleKeyDown(KeyboardEventArgs args)
        {
            var result = args.Key;
            if (result.Equals(Delimiter, StringComparison.InvariantCultureIgnoreCase) && _internalValue != null)
            {
                if (AllowSameValues == false && Values?.Contains(Converter.Set(_internalValue) ?? string.Empty) == true)
                {
                    await Task.Delay(10);
                    _internalValue = Converter.Get(Converter.Set(_internalValue)?.Replace(result, null).ToString());
                    await SetValueAsync(_internalValue);
                    StateHasChanged();
                    return;
                }
                await SetChips();
                StateHasChanged();
            }

            if (args.Key == "Backspace" && string.IsNullOrEmpty(Converter.Set(_internalValue)) && Values != null && Values.Any() && BackspaceChipRemoval == true)
            {
                Values.RemoveAt(Values.Count - 1);
                await ValuesChanged.InvokeAsync(Values);
            }
            await Task.Delay(10);
            await SetValueAsync(_internalValue);
            await OnKeyDown.InvokeAsync(args);
        }

        /// <summary>
        /// Protected keyup event.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        protected async Task HandleKeyUp(KeyboardEventArgs args)
        {
            await OnKeyUp.InvokeAsync(args);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected async Task SetChips()
        {
            if (_internalValue == null)
            {
                return;
            }

            if (Values == null)
            {
                Values = new();
            }
            Values.Add(Converter.Set(_internalValue) ?? "");
            await ValuesChanged.InvokeAsync(Values);
            if (RuntimeLocation.IsServerSide)
            {
                await _textFieldExtendedReference.BlurAsync();
            }
            else
            {
                await Task.Delay((int)DebounceInterval + 10);
            }
            await _textFieldExtendedReference.Clear();
            if (RuntimeLocation.IsServerSide)
            {
                await _textFieldExtendedReference.FocusAsync();
            }
        }

        /// <summary>
        /// Remove process of the specified chip.
        /// </summary>
        /// <param name="chip"></param>
        /// <returns></returns>
        public async Task Closed(MudChip<T> chip)
        {
            if (Disabled || ReadOnly)
            {
                return;
            }
            Values?.Remove(chip.Text ?? "");
            await ValuesChanged.InvokeAsync(Values);
            await _textFieldExtendedReference.FocusAsync();
        }

        /// <summary>
        /// Clear the text field.
        /// </summary>       
        /// <returns></returns>
        public async Task ClearTextField()
        {            
           await _textFieldExtendedReference.Clear();
        }

    }
}
