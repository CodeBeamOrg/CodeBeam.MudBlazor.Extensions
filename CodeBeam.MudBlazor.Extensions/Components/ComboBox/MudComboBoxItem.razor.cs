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
            .AddClass("d-none", Eligible == false)
            .AddClass(Class)
            .Build();

        protected string HighlighterClassname => new CssBuilder()
            .AddClass("mud-combobox-highlighter", MudComboBox is not null && MudComboBox.Highlight && string.IsNullOrEmpty(MudComboBox.HighlightClass))
            .AddClass(MudComboBox?.HighlightClass, MudComboBox is not null && MudComboBox.Highlight)
            .Build();

        internal string ItemId { get; } = string.Concat("_", Guid.NewGuid().ToString().AsSpan(0, 8));

        /// <summary>
        /// The parent select component
        /// </summary>
        [CascadingParameter]
        internal MudComboBox<T> MudComboBox { get; set; }

        protected Typo GetTypo()
        {
            if (MudComboBox == null)
            {
                return Typo.body1;
            }

            if (MudComboBox.Dense == Dense.Slim || MudComboBox.Dense == Dense.Superslim)
            {
                return Typo.body2;
            }

            return Typo.body1;
        }

        ///// <summary>
        ///// Functional items does not hold values. If a value set on Functional item, it ignores by the MudSelect. They cannot be subject of keyboard navigation and selection.
        ///// </summary>
        //[Parameter]
        //[Category(CategoryTypes.List.Behavior)]
        //public bool IsFunctional { get; set; }

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

        protected internal bool Selected { get; set; }
        protected internal bool Active { get; set; }

        public void SetActive(bool isActive)
        {
            Active = isActive;
            StateHasChanged();
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
                    return $"{(string.IsNullOrEmpty(Text) ? Value : Text)}";
                return !string.IsNullOrEmpty(Text) ? Text : converter.Set(Value);
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
            MudComboBox?.Add(this);
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
            if (MudComboBox == null || MudComboBox.Editable == false || MudComboBox.DisableFilter == true)
            {
                return true;
            }

            if (string.IsNullOrEmpty(MudComboBox._searchString))
            {
                return true;
            }

            if (MudComboBox?.SearchFunc != null)
            {
                return MudComboBox.SearchFunc.Invoke(Value, Text, MudComboBox.GetSearchString());
            }

            if (string.IsNullOrEmpty(Text) == false)
            {
                if (Text.Contains(MudComboBox._searchString ?? string.Empty, StringComparison.CurrentCultureIgnoreCase))
                {
                    return true;
                }
            }
            else
            {
                if (MudComboBox.Converter.Set(Value).Contains(MudComboBox._searchString ?? string.Empty, StringComparison.CurrentCultureIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        protected void SyncSelected()
        {
            if (MudComboBox == null)
            {
                return;
            }

            if (MudComboBox.MultiSelection == true && MudComboBox.SelectedValues.Contains(Value))
            {
                Selected = true;
            }
            else if (MudComboBox.MultiSelection == false && ((MudComboBox.Value == null && Value == null) || MudComboBox.Value?.Equals(Value) == true))
            {
                Selected = true;
            }
            else
            {
                Selected = false;
            }
        }

        protected async Task HandleOnClick()
        {
            //Selected = !Selected;
            await MudComboBox.ToggleOption(this, !Selected);
            //await MudComboBox?.SelectOption(Value);
            await InvokeAsync(StateHasChanged);
            //if (MudComboBox.MultiSelection == false)
            //{
            //    await MudComboBox?.CloseMenu();
            //}
            //else
            //{
            //    await MudComboBox.FocusAsync();
            //}
            await MudComboBox.FocusAsync();
            await OnClick.InvokeAsync();
        }

        protected bool GetDisabledStatus()
        {
            if (MudComboBox?.ItemDisabledFunc != null)
            {
                return MudComboBox.ItemDisabledFunc(Value);
            }
            return Disabled;
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
