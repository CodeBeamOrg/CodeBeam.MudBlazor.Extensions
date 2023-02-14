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
        protected string ChipClassname =>
           new CssBuilder("d-flex")
            .AddClass("flex-wrap", WrapChips)
            .AddClass("mt-5", Variant == Variant.Filled)
            .AddClass(ClassChip)
            .Build();

        MudTextFieldExtended<T> _textFieldExtendedReference;
        T _internalValue;

        [Parameter]
        public List<string> Values { get; set; }

        [Parameter]
        public EventCallback<List<string>> ValuesChanged { get; set; }

        [Parameter]
        public Size ChipSize { get; set; }

        [Parameter]
        public char Delimiter { get; set; }

        [Parameter]
        public string ClassChip { get; set; }

        [Parameter]
        public string StyleChip { get; set; }

        [Parameter]
        public Color ChipColor { get; set; }

        [Parameter]
        public Variant ChipVariant { get; set; }

        [Parameter]
        public bool WrapChips { get; set; }

        [Parameter]
        public int MaxChips { get; set; }

        [Parameter]
        public int ChipsMaxWidth { get; set; } = 80;

        protected async Task HandleKeyDown(KeyboardEventArgs args)
        {
            if (args.Key == Delimiter.ToString() && _internalValue != null)
            {
                await SetChips();
                StateHasChanged();
            }

            if (args.Key == "Backspace" && string.IsNullOrEmpty(Converter.Set(_internalValue)) && Values.Any())
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
