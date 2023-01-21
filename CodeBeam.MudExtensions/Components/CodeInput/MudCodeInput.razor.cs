using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using MudBlazor.Utilities;
using MudExtensions.Extensions;

namespace MudExtensions
{
    public partial class MudCodeInput<T> : MudFormComponent<T, string>
    {
        public MudCodeInput() : base(new DefaultConverter<T>()) { }

        protected string Classname =>
           new CssBuilder($"d-flex gap-{Spacing}")
           .AddClass(Class)
           .Build();

        protected string InputClassname =>
            new CssBuilder("justify-text-center")
                .AddClass("mud-code", Variant != Variant.Text)
                .AddClass(ClassInput)
                .Build();

        private List<MudTextField<T>> _elementReferences = new();

        /// <summary>
        /// The CSS classes for each input, seperated by space.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public string ClassInput { get; set; }

        /// <summary>
        /// Type of the input element. It should be a valid HTML5 input type.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public InputType InputType { get; set; } = InputType.Text;

        /// <summary>
        /// The value of the input.
        /// </summary>
        private T _theValue;
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public T Value
        {
            get => _theValue;
            set
            {
                if (Converter.Set(_theValue) == Converter.Set(value))
                {
                    return;
                }
                _theValue = value;
                SetValueFromOutside(_theValue).AndForgetExt();
            }
        }

        /// <summary>
        /// The event fires when value changed.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public EventCallback<T> ValueChanged { get; set; }

        private int _count;
        /// <summary>
        /// The number of text fields.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public int Count
        {
            get => _count;
            set
            {
                if (value == _count || value < 0)
                {
                    return;
                }

                if (12 < value)
                {
                    _count = 12;
                }
                else
                {
                    _count = value;
                }
            }
        }

        /// <summary>
        /// Determines the spacing between each input.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public int Spacing { get; set; } = 2;

        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public bool Disabled { get; set; }

        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public bool ReadOnly { get; set; }

        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public Variant Variant { get; set; } = Variant.Text;

        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public Margin Margin { get; set; }

        protected async Task HandleKeyDown(KeyboardEventArgs arg)
        {
            if (Disabled || ReadOnly)
            {
                return;
            }

            if (RuntimeLocation.IsClientSide)
            {
                await Task.Delay(10);
            }
            
            if (arg.Key == "Backspace" || arg.Key == "ArrowLeft")
            {
                await FocusPrevious();
                return;
            }

            if (arg.Key.Length == 1 || arg.Key == "ArrowRight")
            {
                await FocusNext();
            }
            
        }

        private int _lastFocusedIndex = 0;
        protected void CheckFocus(int count)
        {
            _lastFocusedIndex = count;
        }

        public async Task FocusNext()
        {
            if (_lastFocusedIndex >= Count - 1)
            {
                await _elementReferences[_lastFocusedIndex].BlurAsync();
                await _elementReferences[_lastFocusedIndex].FocusAsync();
                return;
            }
            await _elementReferences[_lastFocusedIndex + 1].FocusAsync();
        }

        public async Task FocusPrevious()
        {
            if (_lastFocusedIndex == 0)
            {
                await _elementReferences[_lastFocusedIndex].BlurAsync();
                await _elementReferences[_lastFocusedIndex].FocusAsync();
                return;
            }
            await _elementReferences[_lastFocusedIndex - 1].FocusAsync();
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            SyncReferences();
        }

        private void SyncReferences()
        {
            _elementReferences.Clear();
            for (int i = 0; i < 12; i++)
            {
                _elementReferences.Add(new MudTextField<T>());
            }
        }

        public async Task SetValue()
        {
            string result =  "";
            for (int i = 0; i < Count; i++)
            {
                var val = _elementReferences[i].Value?.ToString();
                if (val == null)
                {
                    continue;
                }

                result += val;
            }

            Value = Converter.Get(result);
            await ValueChanged.InvokeAsync(Value);
        }

        public async Task SetValueFromOutside(T value)
        {
            string val = Converter.Set(value);
            if (Count < val.Length)
            {
                val = val.Substring(0, Count);
            }
            Value = Converter.Get(val);
            for (int i = 0; i < Count; i++)
            {
                if (i < val.Length)
                {
                    await _elementReferences[i].SetText(val[i].ToString());
                }
                else
                {
                    await _elementReferences[i].SetText(null);
                }
            }

            await ValueChanged.InvokeAsync(Value);
            StateHasChanged();
        }

    }
}
