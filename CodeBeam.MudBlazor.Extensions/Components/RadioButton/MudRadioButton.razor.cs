using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using MudBlazor.Extensions;
using MudBlazor.Services;
using MudBlazor.Utilities;
using CategoryAttribute = MudBlazor.CategoryAttribute;
using Color = MudBlazor.Color;
using Size = MudBlazor.Size;

namespace MudExtensions
{
	public partial class MudRadioButton<T> : MudComponentBase, IDisposable
	{
		[CascadingParameter(Name = "RightToLeft")] public bool RightToLeft { get; set; }

		protected string Classname =>
		new CssBuilder("mud-radio-button")
			.AddClass($"mud-disabled", IsDisabled)
			.AddClass($"mud-readonly", MudRadioButtonGroup?.GetReadOnlyState())
			.AddClass($"mud-radio-button-content-placement-{ConvertPlacement(Placement).ToDescriptionString()}")
			.AddClass(Class)
			.AddClass("d-flex flex-grow-1")
			.Build();

		protected string ButtonClassname =>
		new CssBuilder("mud-button-root")
			.AddClass($"mud-ripple mud-ripple-radio", !DisableRipple && !Disabled && !(MudRadioButtonGroup?.GetDisabledState() ?? false) && !(MudRadioButtonGroup?.GetReadOnlyState() ?? false))
			.AddClass($"mud-{Color.ToDescriptionString()}-text hover:mud-{Color.ToDescriptionString()}-hover", UnCheckedColor == null || (UnCheckedColor != null && Checked == true))
			.AddClass($"mud-{UnCheckedColor?.ToDescriptionString()}-text hover:mud-{UnCheckedColor?.ToDescriptionString()}-hover", UnCheckedColor != null && Checked == false)
			.AddClass($"mud-radio-button-dense", Dense)
			.AddClass($"mud-disabled", IsDisabled)
			.AddClass($"mud-readonly", MudRadioButtonGroup?.GetReadOnlyState())
			.AddClass($"mud-checked", Checked)
			.AddClass("mud-error-text", MudRadioButtonGroup?.HasErrors)
			.AddClass("d-flex flex-grow-1")
			.Build();

		protected string ChildSpanClassName =>
		new CssBuilder("mud-radio-button-content mud-typography mud-typography-body1")
			.AddClass("mud-error-text", MudRadioButtonGroup.HasErrors)
			.Build();

		private IMudRadioButtonGroup _parent;

		/// <summary>
		/// The parent Radio Group
		/// </summary>
		[CascadingParameter]
		internal IMudRadioButtonGroup IMudRadioButtonGroup
		{
			get => _parent;
			set
			{
				_parent = value;
				if (_parent == null)
					return;
				_parent.CheckGenericTypeMatch(this);
			}
		}

		internal MudRadioButtonGroup<T> MudRadioButtonGroup => (MudRadioButtonGroup<T>)IMudRadioButtonGroup;

		private Placement ConvertPlacement(Placement placement)
		{
			return placement switch
			{
				Placement.Left => RightToLeft ? Placement.End : Placement.Start,
				Placement.Right => RightToLeft ? Placement.Start : Placement.End,
				_ => placement
			};
		}

		/// <summary>
		/// The color of the component. It supports the theme colors.
		/// </summary>
		[Parameter]
		[Category(CategoryTypes.Radio.Appearance)]
		public Color Color { get; set; } = Color.Default;

		/// <summary>
		/// The base color of the component in its none active/unchecked state. It supports the theme colors.
		/// </summary>
		[Parameter]
		[Category(CategoryTypes.Radio.Appearance)]
		public Color? UnCheckedColor { get; set; } = null;

		/// <summary>
		/// The position of the child content.
		/// </summary>
		[Parameter]
		[Category(CategoryTypes.Radio.Behavior)]
		public Placement Placement { get; set; } = Placement.End;

		/// <summary>
		/// The value to associate to the button.
		/// </summary>
		[Parameter]
		[Category(CategoryTypes.Radio.Behavior)]
		public T Option { get; set; }

		/// <summary>
		/// If true, compact padding will be applied.
		/// </summary>
		[Parameter]
		[Category(CategoryTypes.Radio.Appearance)]
		public bool Dense { get; set; }

		/// <summary>
		/// The Size of the component.
		/// </summary>
		[Parameter]
		[Category(CategoryTypes.Radio.Appearance)]
		public Size Size { get; set; } = Size.Medium;

		/// <summary>
		/// If true, disables ripple effect.
		/// </summary>
		[Parameter]
		[Category(CategoryTypes.Radio.Appearance)]
		public bool DisableRipple { get; set; }

		/// <summary>
		/// If true, the button will be disabled.
		/// </summary>
		[Parameter]
		[Category(CategoryTypes.Radio.Behavior)]
		public bool Disabled { get; set; }
		private bool IsDisabled => Disabled || (MudRadioButtonGroup?.GetDisabledState() ?? false);

		/// <summary>
		/// Child content of component.
		/// </summary>
		[Parameter]
		[Category(CategoryTypes.Radio.Behavior)]
		public RenderFragment ChildContent { get; set; }

		internal bool Checked { get; private set; }

		internal void SetChecked(bool value)
		{
			if (Checked != value)
			{
				Checked = value;
				StateHasChanged();
			}
		}

		public void Select()
		{
			MudRadioButtonGroup?.SetSelectedRadioAsync(this).AndForget();
		}

		internal Task OnClick()
		{
			if (IsDisabled || (MudRadioButtonGroup?.GetReadOnlyState() ?? false))
				return Task.CompletedTask;
			if (MudRadioButtonGroup != null)
				return MudRadioButtonGroup.SetSelectedRadioAsync(this);

			return Task.CompletedTask;
		}

		protected internal async Task HandleKeyDownAsync(KeyboardEventArgs keyboardEventArgs)
		{
			if (IsDisabled || (MudRadioButtonGroup?.GetReadOnlyState() ?? false))
				return;
			switch (keyboardEventArgs.Key)
			{
				case "Enter":
				case "NumpadEnter":
				case " ":
					Select();
					break;
				case "Backspace":
					// Should be async on newer versions of MudBlazor.
					MudRadioButtonGroup?.Reset();

					break;
			}
		}

		protected override async Task OnInitializedAsync()
		{
			await base.OnInitializedAsync();

			if (MudRadioButtonGroup != null)
				await MudRadioButtonGroup.RegisterRadioAsync(this);
		}

		public void Dispose()
		{
			MudRadioButtonGroup?.UnregisterRadio(this);
			_keyInterceptor?.Dispose();
		}

		private IKeyInterceptor _keyInterceptor;
		[Inject] private IKeyInterceptorFactory _keyInterceptorFactory { get; set; }

		private readonly string _elementId = "radio" + Guid.NewGuid().ToString()[..8];

		protected override async Task OnAfterRenderAsync(bool firstRender)
		{
			if (firstRender)
			{
				_keyInterceptor = _keyInterceptorFactory.Create();
				await _keyInterceptor.Connect(_elementId, new KeyInterceptorOptions()
				{
					TargetClass = "mud-button-root",
					Keys = {
						new KeyOptions { Key=" ", PreventDown = "key+none", PreventUp = "key+none" }, // prevent scrolling page
                        new KeyOptions { Key="Enter", PreventDown = "key+none" },
						new KeyOptions { Key="NumpadEnter", PreventDown = "key+none" },
						new KeyOptions { Key="Backspace", PreventDown = "key+none" },
					},
				});
			}

			await base.OnAfterRenderAsync(firstRender);
		}
	}
}
