using Microsoft.AspNetCore.Components;
using MudBlazor;
using MudBlazor.Extensions;
using MudBlazor.Utilities;
using MudExtensions.Enums;

namespace MudExtensions
{
    public partial class MudComboBoxItem<T> : MudBaseSelectItem, IDisposable
    {
        protected string Classname => new CssBuilder("mud-combobox-item")
            .AddClass($"mud-combobox-item-{MudComboBox?.Dense.ToDescriptionString()}")
            .AddClass("mud-ripple", !DisableRipple && !Disabled)
            .AddClass("mud-combobox-item-gutters")
            .AddClass("mud-combobox-item-clickable")
            .AddClass("mud-combobox-item-hilight", Active && !Disabled)
            .AddClass("mud-combobox-item-hilight-selected", Active && Selected && !Disabled)
            .AddClass($"mud-selected-item mud-{MudComboBox?.Color.ToDescriptionString()}-text mud-{MudComboBox?.Color.ToDescriptionString()}-hover", Selected && !Disabled && !Active)
            .AddClass("mud-combobox-item-disabled", Disabled)
            .AddClass("mud-combobox-item-bordered", MudComboBox?.Bordered == true && Active)
            .AddClass($"mud-combobox-item-bordered-{MudComboBox?.Color.ToDescriptionString()}", MudComboBox?.Bordered == true && Selected)
            .AddClass("d-none", !Eligible)
            .AddClass(Class)
            .Build();

        protected string HighlighterClassname => MudComboBox is null ? null : new CssBuilder()
            .AddClass("mud-combobox-highlighter", MudComboBox.Highlight && string.IsNullOrWhiteSpace(MudComboBox.HighlightClass))
            .AddClass(MudComboBox?.HighlightClass, MudComboBox.Highlight)
            .Build();

        internal string ItemId { get; } = string.Concat("_", Guid.NewGuid().ToString().AsSpan(0, 8));

        /// <summary>
        /// The parent select component
        /// </summary>
        [CascadingParameter]
        MudComboBox<T> MudComboBox { get; set; }

        /// <summary>
        /// The text to display
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Behavior)]
        public string Text { get; set; }

        /// <summary>
        /// A user-defined option that can be selected
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public T Value { get; set; }

        /// <summary>
        /// The color of the text. It supports the theme colors.
        /// </summary>
        /// <remarks>The default is <see cref="MudComboBox.TextColor"/></remarks>
        [Parameter]
        [Category(CategoryTypes.List.Behavior)]
        public Color? TextColor { get; set; } = null;

        /// <summary>
        /// The color of the checked checkbox. It supports the theme colors.
        /// </summary>
        /// <remarks>The default is <see cref="MudComboBox.CheckBoxCheckedColor"/></remarks>
        [Parameter]
        [Category(CategoryTypes.List.Behavior)]
        public Color? CheckBoxCheckedColor { get; set; } = null;

        /// <summary>
        /// The color of the unchecked checkbox. It supports the theme colors.
        /// </summary>
        /// <remarks>The default is <see cref="MudComboBox.CheckBoxUnCheckedColor"/></remarks>
        [Parameter]
        [Category(CategoryTypes.Radio.Appearance)]
        public Color? CheckBoxUnCheckedColor { get; set; } = null;

        /// <summary>
        /// The size of the checkbox.
        /// </summary>
        /// <remarks>The default is <see cref="MudComboBox.CheckBoxSize"/></remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public Size? CheckBoxSize { get; set; } = null;


        protected internal bool Selected { get; set; }
        protected internal bool Active { get; set; }

        public void SetActive(bool isActive)
        {
            Active = isActive;
            //StateHasChanged();
        }

        [Parameter]
        public bool Eligible { get; set; } = true;

        protected string DisplayString
        {
            get
            {
                var converter = MudComboBox?.Converter;
                if (MudComboBox?.ItemPresenter == ValuePresenter.None)
                {
                    if (converter == null)
                        return Value.ToString();
                    return converter.Set(Value);
                }

                if (converter == null)
                    return $"{(string.IsNullOrWhiteSpace(Text) ? Value : Text)}";
                return !string.IsNullOrWhiteSpace(Text) ? Text : converter.Set(Value);
            }
        }

        public void ForceRender()
        {
            CheckEligible();
            StateHasChanged();
        }

        public async Task ForceUpdate()
        {
            SyncSelected();
            await InvokeAsync(StateHasChanged);
        }

        protected override void OnInitialized()
        {
            MudComboBox?.AddItem(this);
        }

        //bool? _oldMultiselection;
        //bool? _oldSelected;
        bool _selectedChanged = false;
        //bool? _oldEligible = true;
        //bool _eligibleChanged = false;
        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            //SyncSelected();
            //if (_oldSelected != Selected || _oldEligible != Eligible)
            //{
            //    _selectedChanged = true;
            //}
            //_oldSelected = Selected;
            CheckEligible();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
            if (_selectedChanged)
            {
                _selectedChanged = false;
                await InvokeAsync(StateHasChanged);
            }
        }

        protected internal void CheckEligible()
        {
            Eligible = IsEligible();
        }

        protected bool IsEligible()
        {
            if (MudComboBox is null)
                return true;

            if (!MudComboBox.Editable || MudComboBox.DisableFilter)
                return true;

            if (string.IsNullOrWhiteSpace(MudComboBox._searchString))
                return true;

            if (MudComboBox.SearchFunc is not null)
                return MudComboBox.SearchFunc.Invoke(Value, Text, MudComboBox.GetSearchString());

            if (!string.IsNullOrWhiteSpace(Text))
            {
                if (Text.Contains(MudComboBox._searchString ?? string.Empty, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            else
            {
                if (MudComboBox.Converter.Set(Value).Contains(MudComboBox._searchString ?? string.Empty, StringComparison.OrdinalIgnoreCase))
                    return true;
            }

            return false;
        }

        protected void SyncSelected()
        {
            if (MudComboBox is null)
                return;

            if (MudComboBox.MultiSelection && MudComboBox.SelectedValues.Contains(Value))
                Selected = true;

            else if (!MudComboBox.MultiSelection && ((MudComboBox.Value is null && Value is null) || MudComboBox.Value?.Equals(Value) == true))
                Selected = true;
            else
                Selected = false;
        }

        protected async Task HandleOnClick()
        {
            await MudComboBox.ToggleOption(this, !Selected);
            await InvokeAsync(StateHasChanged);
            await MudComboBox.FocusAsync();
            await OnClick.InvokeAsync();
        }

        public void Dispose()
        {
            try
            {
                MudComboBox?.RemoveItem(this);
            }
            catch { }
        }
    }
}
