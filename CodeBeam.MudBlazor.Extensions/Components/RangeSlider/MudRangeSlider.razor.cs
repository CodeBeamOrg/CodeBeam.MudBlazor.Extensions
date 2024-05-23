using Microsoft.AspNetCore.Components;
using MudBlazor;
using MudBlazor.State;
using MudBlazor.Utilities;
using System.Globalization;
using System.Numerics;

namespace MudExtensions
{
    /// <summary>
    /// Mud slider with range abilities.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public partial class MudRangeSlider<T> : MudComponentBase where T : struct, INumber<T>
    {

        private readonly ParameterState<T> _value;
        private readonly ParameterState<T> _upperValue;
        private readonly ParameterState<T?> _slideableMin;
        private readonly ParameterState<T?> _slideableMax;

        /// <summary>
        /// 
        /// </summary>
        public MudRangeSlider()
        {
            using var registerScope = CreateRegisterScope();
            _value = registerScope.RegisterParameter<T>(nameof(Value))
                .WithParameter(() => Value)
                .WithEventCallback(() => ValueChanged)
                .WithChangeHandler(OnValueParameterChanged);
            _upperValue = registerScope.RegisterParameter<T>(nameof(UpperValue))
                .WithParameter(() => UpperValue)
                .WithEventCallback(() => UpperValueChanged)
                .WithChangeHandler(OnUpperValueParameterChanged);
            _slideableMin = registerScope.RegisterParameter<T?>(nameof(SlideableMin))
                .WithParameter(() => SlideableMin)
                .WithChangeHandler(OnSlideableMinChanged);
            _slideableMax = registerScope.RegisterParameter<T?>(nameof(SlideableMax))
                .WithParameter(() => SlideableMax)
                .WithChangeHandler(OnSlideableMaxChanged);
        }

        private async Task OnValueParameterChanged()
        {
            if (Range && Convert.ToDecimal(_value.Value) + Convert.ToDecimal(MinDistance) >= Convert.ToDecimal(_upperValue.Value))
            {
                await _value.SetValueAsync(_upperValue.Value - MinDistance);
            }

            if (_slideableMin.Value != null && _value.Value < _slideableMin.Value)
            {
                await _value.SetValueAsync((T)_slideableMin.Value);
            }
        }

        private async Task OnUpperValueParameterChanged()
        {
            if (Range && Convert.ToDecimal(_upperValue.Value) - Convert.ToDecimal(MinDistance) <= Convert.ToDecimal(_value.Value))
            {
                await _upperValue.SetValueAsync(_value.Value + MinDistance);
            }

            if (_slideableMax.Value != null && _slideableMax.Value < _upperValue.Value)
            {
                await _upperValue.SetValueAsync((T)_slideableMax.Value);
            }
        }

        private async Task OnSlideableMinChanged()
        {
            if (_slideableMin.Value != null && _value.Value <_slideableMin.Value)
            {
                await _value.SetValueAsync((T)_slideableMin.Value);
            }
        }

        private async Task OnSlideableMaxChanged()
        {
            if (_slideableMax.Value != null && _slideableMax.Value < _upperValue.Value)
            {
                await _upperValue.SetValueAsync((T)_slideableMax.Value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected string? Classname =>
            new CssBuilder("mud-slider")
                .AddClass($"mud-slider-{Size.ToDescriptionString()}")
                .AddClass($"mud-slider-{Color.ToDescriptionString()}")
                .AddClass("mud-slider-vertical", Vertical)
                .AddClass(Class)
                .Build();

        /// <summary>
        /// If this is a Range Slider
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Slider.Validation)]
        public bool Range { get; set; } = true;

        /// <summary>
        /// Custom text for ValueLabel
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Slider.Validation)]
        public string? LabelText { get; set; }

        /// <summary>
        /// Custom text for upper ValueLabel
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Slider.Validation)]
        public string? UpperLabelText { get; set; }

        /// <summary>
        /// The minimum allowed value of the slider. Should not be equal to max.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Slider.Validation)]
        public T Min { get; set; } = T.Zero;

        /// <summary>
        /// The minimum value can slider thumb has.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Slider.Validation)]
        public T? SlideableMin { get; set; }

        /// <summary>
        /// The maximum allowed value of the slider. Should not be equal to min.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Slider.Validation)]
        public T Max { get; set; } = T.CreateTruncating(100);

        /// <summary>
        /// The minimum value can slider thumb has.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Slider.Validation)]
        public T? SlideableMax { get; set; }

        /// <summary>
        /// The minimum distance between the upper and lower values
        /// </summary>
        /// 
        [Parameter]
        [Category(CategoryTypes.Slider.Validation)]
        public T MinDistance { get; set; } = T.One;

        /// <summary>
        /// How many steps the slider should take on each move.
        /// </summary>
        /// 
        [Parameter]
        [Category(CategoryTypes.Slider.Validation)]
        public T Step { get; set; } = T.One;

		/// <summary>
		/// If true, the slider will be disabled.
		/// </summary>
		/// 
		[Parameter]
		[Category(CategoryTypes.Slider.Behavior)]
		public bool Disabled { get; set; } = false;

		/// <summary>
		/// If true and <seealso cref="Range"/>, the slider's min value will be disabled.
		/// </summary>
		/// 
		[Parameter]
		[Category(CategoryTypes.Slider.Behavior)]
		public bool DisableMin { get; set; } = false;

		/// <summary>
		/// If true and <seealso cref="Range"/>, the slider's max value will be disabled.
		/// </summary>
		/// 
		[Parameter]
		[Category(CategoryTypes.Slider.Behavior)]
		public bool DisableMax { get; set; } = false;

		/// <summary>
		/// Child content of component.
		/// </summary>
		[Parameter]
        [Category(CategoryTypes.Slider.Behavior)]
        public RenderFragment? ChildContent { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Slider.Behavior)]
        public Converter<T?> Converter { get; set; } = new DefaultConverter<T?>() { Culture = CultureInfo.InvariantCulture };

        /// <summary>
        /// Fires when value changed.
        /// </summary>
        [Parameter] public EventCallback<T> ValueChanged { get; set; }

        /// <summary>
        /// Fires when upper value changed.
        /// </summary>
        [Parameter] public EventCallback<T> UpperValueChanged { get; set; }

        /// <summary>
        /// Value of the component.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Slider.Data)]
        public T Value { get; set; } = T.Zero;

        /// <summary>
        /// If range set, holds the higher value.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Slider.Data)]
        public T UpperValue { get; set; } = T.CreateTruncating(50);

        /// <summary>
        /// The color of the component. It supports the Primary, Secondary and Tertiary theme colors.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Slider.Appearance)]
        public Color Color { get; set; } = Color.Primary;

        /// <summary>
        /// 
        /// </summary>
        protected string? DisplayText
        {
            get
            {
                if (!Range) return Converter.Set(_value.Value);

                //if both lower and upper are not set then it is any
                if ((Convert.ToDouble(_value.Value) == Convert.ToDouble(Min)) &&
                (Convert.ToDouble(_upperValue.Value) == Convert.ToDouble(Max) || Convert.ToDouble(_upperValue.Value) == 0))
                    return $"{Min} - {Max}";

                string displayText = $"{_value.Value} - {_upperValue.Value}";

                //If lower is min or not defined 
                if (Convert.ToDouble(_value.Value) == Convert.ToDouble(Min))
                    displayText = $"{Min} - {_upperValue.Value}";

                //if upper is max or not defined
                if (Convert.ToDouble(_upperValue.Value) == Convert.ToDouble(Max) || Convert.ToDouble(_upperValue.Value) == 0)
                    displayText = $"{_value.Value} - {Max}";                                

                return displayText;
            }
        }

        /// <summary>
        /// If true, displays the Values below the slider
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Slider.Appearance)]
        public bool Display { get; set; } = false;

        /// <summary>
        /// If true, the dragging the slider will update the Value immediately.
        /// If false, the Value is updated only on releasing the handle.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Slider.Behavior)]
        public bool Immediate { get; set; } = true;

        /// <summary>
        /// If true, displays the slider vertical.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Slider.Appearance)]
        public bool Vertical { get; set; } = false;

        /// <summary>
        /// If true, displays tick marks on the track.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Slider.Appearance)]
        public bool TickMarks { get; set; } = false;

        /// <summary>
        /// Labels for tick marks, will attempt to map the labels to each step in index order.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Slider.Appearance)]
        public string[]? TickMarkLabels { get; set; }

        /// <summary>
        /// Labels for tick marks, will attempt to map the labels to each step in index order.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Slider.Appearance)]
        public Size Size { get; set; } = Size.Small;

        /// <summary>
        /// The variant to use.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Button.Appearance)]
        public Variant Variant { get; set; } = Variant.Text;

        /// <summary>
        /// Displays the value over the slider thumb.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Button.Appearance)]
        public bool ValueLabel { get; set; }

        private int _tickMarkCount = 0;
        /// <summary>
        /// 
        /// </summary>
        protected override void OnParametersSet()
        {
            if (TickMarks)
            {
                var min = Convert.ToDouble(Min);
                var max = Convert.ToDouble(Max);
                var step = Convert.ToDouble(Step);

                _tickMarkCount = 1 + (int)((max - min) / step);
            }

            //if (Range)
            //{
            //    //if no Value was set or no Upper Value set, default to min and max
            //    if (string.IsNullOrEmpty(_value) && !string.IsNullOrEmpty(Converter.Set(Min)))
            //    {
            //        _value = Converter.Set(Min);
            //        ValueChanged.InvokeAsync(Value);
            //    }

            //    if (string.IsNullOrEmpty(_upperValue) && !string.IsNullOrEmpty(Converter.Set(Max)))
            //    {
            //        _upperValue = Converter.Set(Max);
            //        UpperValueChanged.InvokeAsync(UpperValue);
            //    }
            //}
            base.OnParametersSet();
        }

        private double CalculateWidth()
        {
            var min = Convert.ToDouble(Min);
            var max = Convert.ToDouble(Max);
            var value = Convert.ToDouble(_value.Value);

            if (Range)
            {
                value = (Convert.ToDouble(_upperValue.Value) + min - Convert.ToDouble(_value.Value));
            }

            var result = 100.0 * (value - min) / (max - min);
            result = Math.Min(Math.Max(0, result), 100);

            return Math.Round(result, 2);
        }

        private double CalculateLeft()
        {
            var min = Convert.ToDouble(Min);
            var max = Convert.ToDouble(Max);
            var value = Convert.ToDouble(_value.Value);
            var result = 100.0 * (value - min) / (max - min);
            result = Math.Min(Math.Max(0, result), 100);

            return Math.Round(result, 2);
        }

        private string GetValueText => _value.Value.ToString(null, CultureInfo.InvariantCulture);

        private string GetUpperValueText => _upperValue.Value.ToString(null, CultureInfo.InvariantCulture);

        private async Task SetValueTextAsync(string? text)
        {
            if (T.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out var result))
            {
                await _value.SetValueAsync(result);
            }
        }

        private async Task SetUpperValueTextAsync(string? text)
        {
            if (T.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out var result))
            {
                await _upperValue.SetValueAsync(result);
            }
        }

    }
}
