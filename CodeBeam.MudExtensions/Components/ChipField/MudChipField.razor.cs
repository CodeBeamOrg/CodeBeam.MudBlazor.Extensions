using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using MudBlazor.Utilities;
using MudExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace MudExtensions
{
    public partial class MudChipField<T> : MudTextFieldExtended<T>
    {
        MudTextFieldExtended<T> _textFieldExtendedReference;
        T _internalValue;

        [Parameter]
        public List<string> Values { get; set; }

        [Parameter]
        public EventCallback<List<string>> ValuesChanged { get; set; }

        [Parameter]
        public char Delimiter { get; set; }

        [Parameter]
        public Color ChipColor { get; set; }

        [Parameter]
        public Variant ChipVariant { get; set; }

        protected async Task HandleKeyDown(KeyboardEventArgs args)
        {
            if (args.Key == Delimiter.ToString() && _internalValue != null)
            {
                await SetChips();
                StateHasChanged();
            }

            if (args.Key == "Backspace" && _internalValue == null && Values.Any())
            {
                Values.RemoveAt(Values.Count - 1);
                await ValuesChanged.InvokeAsync(Values);
            }
        }

        protected async Task SetChips()
        {
            if (Values == null)
            {
                Values = new();
            }
            Values.Add(Converter.Set(_internalValue));
            await ValuesChanged.InvokeAsync(Values);
            if (RuntimeLocation.IsServerSide)
            {
                await _textFieldExtendedReference.BlurAsync();
            }
            else
            {
                await Task.Delay(10);
            }
            await _textFieldExtendedReference.Clear();
            if (RuntimeLocation.IsServerSide)
            {
                await _textFieldExtendedReference.FocusAsync();
            }
        }

        public async Task Closed(MudChip chip)
        {
            if (Disabled || ReadOnly)
            {
                return;
            }
            Values.Remove(chip.Text);
            await ValuesChanged.InvokeAsync(Values);
            await _textFieldExtendedReference.FocusAsync();
        }

    }
}
