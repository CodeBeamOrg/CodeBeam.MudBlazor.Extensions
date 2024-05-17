using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using MudBlazor.State;
using MudBlazor.Utilities;

namespace MudExtensions
{
    /// <summary>
    /// Inputs which each input box can contain only one character.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public partial class MudCodeInput<T> : MudFormComponent<T, string>
    {
        /// <summary>
        /// MudCodeInput constructor.
        /// </summary>
        public MudCodeInput() : base(new DefaultConverter<T>())
        {
            using var registerScope = CreateRegisterScope();
            _theValue = registerScope.RegisterParameter<T?>(nameof(Value))
                .WithParameter(() => Value)
                .WithEventCallback(() => ValueChanged)
                .WithChangeHandler(OnValueChanged);
            _count = registerScope.RegisterParameter<int>(nameof(Count))
                .WithParameter(() => Count)
                .WithChangeHandler(OnCountChanged);
        }

        private readonly ParameterState<T?> _theValue;
        private readonly ParameterState<int> _count;

        private async Task OnValueChanged()
        {
            await SetValueFromOutside(_theValue.Value);
        }

        private async Task OnCountChanged()
        {
            if (_count.Value < 0)
            {
                await _count.SetValueAsync(0);
            }

            if (12 < _count.Value)
            {
                await _count.SetValueAsync(12);
            }
        }

        /// <summary>
        /// Protected classes.
        /// </summary>
        protected string? Classname =>
           new CssBuilder($"d-flex gap-{Spacing}")
           .AddClass(Class)
           .Build();

        /// <summary>
        /// Protected classes.
        /// </summary>
        protected string? InputClassname =>
            new CssBuilder("justify-text-center")
                .AddClass("mud-code", Variant != Variant.Text)
                .AddClass(InputClass)
                .Build();

        private List<MudTextFieldExtended<T>> _elementReferences = new();

        /// <summary>
        /// The CSS classes for each input, seperated by space.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public string? InputClass { get; set; }

        /// <summary>
        /// Type of the input element. It should be a valid HTML5 input type.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public InputType InputType { get; set; } = InputType.Text;

        /// <summary>
        /// The value of the input.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public T? Value { get; set; }

        /// <summary>
        /// The event fires when value changed.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public EventCallback<T?> ValueChanged { get; set; }

        /// <summary>
        /// The number of text fields.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public int Count { get; set; }

        /// <summary>
        /// Determines the spacing between each input.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public int Spacing { get; set; } = 2;

        /// <summary>
        /// If true disables the component. Default is false.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public bool Disabled { get; set; }

        /// <summary>
        /// If true removes all interactivity of the component. Default is false.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public bool ReadOnly { get; set; }

        /// <summary>
        /// Variant of the component.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public Variant Variant { get; set; } = Variant.Text;

        /// <summary>
        /// Margin of the component that determines component size.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public Margin Margin { get; set; }

        private async Task OnInputHandler()
        {
            await FocusNext();
        }

        /// <summary>
        /// Protected keydown event.
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
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

            if (arg.Key == "ArrowRight")
            {
                await FocusNext();
            }
            
        }

        private int _lastFocusedIndex = 0;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="count"></param>
        protected void CheckFocus(int count)
        {
            _lastFocusedIndex = count;
        }

        /// <summary>
        /// Focuses next input box.
        /// </summary>
        /// <returns></returns>
        public async Task FocusNext()
        {
            if (_lastFocusedIndex >= _count.Value - 1)
            {
                await _elementReferences[_lastFocusedIndex].BlurAsync();
                await _elementReferences[_lastFocusedIndex].FocusAsync();
                return;
            }
            await _elementReferences[_lastFocusedIndex + 1].FocusAsync();
        }

        /// <summary>
        /// Focuses previous input box.
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// 
        /// </summary>
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
                _elementReferences.Add(new MudTextFieldExtended<T>());
            }
        }

        /// <summary>
        /// Set value for the input boxes.
        /// </summary>
        /// <returns></returns>
        public async Task SetValue()
        {
            string result =  "";
            for (int i = 0; i < _count.Value; i++)
            {
                var val = _elementReferences[i].Value?.ToString();
                if (val == null)
                {
                    continue;
                }

                result += val;
            }

            await _theValue.SetValueAsync(Converter.Get(result));
        }

        /// <summary>
        /// Call this method to set value programmatically.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public async Task SetValueFromOutside(T? value)
        {
            string? val = Converter.Set(value);
            if (_count.Value < val?.Length)
            {
                val = val.Substring(0, _count.Value);
            }
            await _theValue.SetValueAsync(Converter.Get(val));
            for (int i = 0; i < _count.Value; i++)
            {
                if (i < val?.Length)
                {
                    await _elementReferences[i].SetText(val[i].ToString());
                }
                else
                {
                    await _elementReferences[i].SetText(null);
                }
            }
        }

    }
}
