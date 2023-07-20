using Microsoft.AspNetCore.Components;
using MudBlazor;
using MudBlazor.Utilities;
using MudBlazor.Utilities.Exceptions;
using CategoryAttribute = MudBlazor.CategoryAttribute;

namespace MudExtensions
{
	public partial class MudRadioButtonGroup<T> : MudFormComponent<T, T>, IMudRadioButtonGroup
	{
		public MudRadioButtonGroup() : base(new MudBlazor.Converter<T, T>()) { }

		private MudRadioButton<T> _selectedRadio;

		private readonly HashSet<MudRadioButton<T>> _radios = new();

		protected string Classname =>
		new CssBuilder("mud-input-control-boolean-input")
			.AddClass(Class)
			.AddClass("d-flex")
			.AddClass("flex-grow-1")
			.Build();

		private string GetInputClass() =>
		new CssBuilder("mud-radio-button-group")
			.AddClass(InputClass)
			.AddClass("d-flex")
			.AddClass("flex-grow-1")
			.Build();

		/// <summary>
		/// User class names for the input, separated by space
		/// </summary>
		[Parameter]
		[Category(CategoryTypes.Radio.Appearance)]
		public string InputClass { get; set; }

		/// <summary>
		/// User style definitions for the input
		/// </summary>
		[Parameter]
		[Category(CategoryTypes.Radio.Appearance)]
		public string InputStyle { get; set; }

		[Parameter]
		[Category(CategoryTypes.Radio.Behavior)]
		public RenderFragment ChildContent { get; set; }

		[Parameter]
		[Category(CategoryTypes.Radio.Behavior)]
		public string Name { get; set; } = Guid.NewGuid().ToString();

		public void CheckGenericTypeMatch(object select_item)
		{
			var itemT = select_item.GetType().GenericTypeArguments[0];
			if (itemT != typeof(T))
				throw new GenericTypeMismatchException("MudRadioButtonGroup", "MudRadio", typeof(T), itemT);
		}

		/// <summary>
		/// If true, the input will be disabled.
		/// </summary>
		[Parameter]
		[Category(CategoryTypes.FormComponent.Behavior)]
		public bool Disabled { get; set; }
		[CascadingParameter(Name = "ParentDisabled")] private bool ParentDisabled { get; set; }
		internal bool GetDisabledState() => Disabled || ParentDisabled; //internal because the MudRadio reads this value directly

		/// <summary>
		/// If true, the input will be read-only.
		/// </summary>
		[Parameter]
		[Category(CategoryTypes.FormComponent.Behavior)]
		public bool ReadOnly { get; set; }
		[CascadingParameter(Name = "ParentReadOnly")] private bool ParentReadOnly { get; set; }
		internal bool GetReadOnlyState() => ReadOnly || ParentReadOnly; //internal because the MudRadio reads this value directly

		[Parameter]
		[Category(CategoryTypes.Radio.Data)]
		public T SelectedOption
		{
			get => _value;
			set => SetSelectedOptionAsync(value, true).AndForget();
		}

		protected async Task SetSelectedOptionAsync(T option, bool updateRadio)
		{
			if (!OptionEquals(_value, option))
			{
				_value = option;

				if (updateRadio)
					await SetSelectedRadioAsync(_radios.FirstOrDefault(r => OptionEquals(r.Option, _value)), false);

				await SelectedOptionChanged.InvokeAsync(_value);

				await BeginValidateAsync();
				FieldChanged(_value);
			}
		}

		[Parameter]
		public EventCallback<T> SelectedOptionChanged { get; set; }

		internal Task SetSelectedRadioAsync(MudRadioButton<T> radio)
		{
			Touched = true;
			return SetSelectedRadioAsync(radio, true);
		}

		protected async Task SetSelectedRadioAsync(MudRadioButton<T> radio, bool updateOption)
		{
			// Select a new radio or toggle the one that was already selected.
			_selectedRadio = _selectedRadio == radio ? null : radio;

			foreach (var item in _radios.ToArray())
				item.SetChecked(item == _selectedRadio);

			if (updateOption)
				await SetSelectedOptionAsync(GetOptionOrDefault(_selectedRadio), false);
		}

		internal Task RegisterRadioAsync(MudRadioButton<T> radio)
		{
			_radios.Add(radio);

			if (_selectedRadio == null)
			{
				if (OptionEquals(radio.Option, _value))
					return SetSelectedRadioAsync(radio, false);
			}

			return Task.CompletedTask;
		}

		internal void UnregisterRadio(MudRadioButton<T> radio)
		{
			_radios.Remove(radio);

			if (_selectedRadio == radio)
				_selectedRadio = null;
		}

		protected override Task ResetValueAsync()
		{
			if (_selectedRadio != null)
			{
				_selectedRadio.SetChecked(false);
				_selectedRadio = null;
			}

			return base.ResetValueAsync();
		}

		private static T GetOptionOrDefault(MudRadioButton<T> radio)
		{
			return radio != null ? radio.Option : default;
		}

		private static bool OptionEquals(T option1, T option2)
		{
			return EqualityComparer<T>.Default.Equals(option1, option2);
		}
	}
}
