using Microsoft.AspNetCore.Components;
using MudBlazor;
using MudBlazor.Utilities;

namespace MudExtensions
{
    public partial class MudComboboxItem<T> : MudBaseSelectItem, IDisposable
    {
        protected string Classname => new CssBuilder("mud-list-item-extended")
            .AddClass("mud-list-item-dense", (MudCombobox?.Dense) ?? false)
            .AddClass("mud-ripple", !DisableRipple && !Disabled)
            .AddClass("mud-list-item-gutters-extended")
            .AddClass("mud-list-item-clickable-extended")
            .AddClass("mud-list-item-dense-extended", MudCombobox?.Dense == true)
            .AddClass($"mud-selected-item mud-{MudCombobox?.Color.ToDescriptionString()}-text mud-{MudCombobox?.Color.ToDescriptionString()}-hover", Selected && !Disabled)
            .AddClass("mud-list-item-disabled", Disabled)
            .AddClass("d-none", Eligible == false)
            .AddClass(Class)
            .Build();

        internal string ItemId { get; } = "comboboxItem_" + Guid.NewGuid().ToString().Substring(0, 8);

        /// <summary>
        /// The parent select component
        /// </summary>
        [CascadingParameter]
        internal MudCombobox<T> MudCombobox { get; set; }

        /// <summary>
        /// Functional items does not hold values. If a value set on Functional item, it ignores by the MudSelect. They cannot be subject of keyboard navigation and selection.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Behavior)]
        public bool IsFunctional { get; set; }

        /// <summary>
        /// The text to display
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Behavior)]
        public string Text { get; set; }

        //private void OnUpdateSelectionStateFromOutside(IEnumerable<T> selection)
        //{
        //    if (selection == null)
        //        return;
        //    var old_is_selected = Selected;
        //    Selected = selection.Contains(Value);
        //    if (old_is_selected != Selected)
        //        InvokeAsync(StateHasChanged);
        //}

        /// <summary>
        /// A user-defined option that can be selected
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public T Value { get; set; }

        /// <summary>
        /// Mirrors the MultiSelection status of the parent select
        /// </summary>
        //protected bool MultiSelection
        //{
        //    get
        //    {
        //        if (MudCombobox == null)
        //            return false;
        //        return MudCombobox.MultiSelection;
        //    }
        //}

        protected internal bool Selected { get; set; }

        [Parameter]
        public bool Eligible { get; set; } = true;

        protected string DisplayString
        {
            get
            {
                var converter = MudCombobox?.Converter;
                if (converter == null)
                    return $"{(string.IsNullOrEmpty(Text) ? Value : Text)}";
                return !string.IsNullOrEmpty(Text) ? Text : converter.Set(Value);
            }
        }

        protected override void OnInitialized()
        {
            MudCombobox?.Add(this);
        }

        bool? _oldSelected;
        bool _selectedChanged = false;
        bool? _oldEligible = true;
        bool _eligibleChanged = false;
        protected override void OnParametersSet()
        {
            base.OnParametersSetAsync();
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

        protected void CheckEligible()
        {
            Eligible = IsEligible();
        }

        protected bool IsEligible()
        {
            if (MudCombobox == null || MudCombobox.Editable == false)
            {
                return true;
            }

            if (MudCombobox.Text == null && MudCombobox.Value == null)
            {
                return true;
            }

            if (Text != null)
            {
                if (Text.Contains(MudCombobox.Text ?? MudCombobox.Converter.Set(MudCombobox.Value), StringComparison.CurrentCultureIgnoreCase))
                {
                    return true;
                }
            }
            else
            {
                if (MudCombobox.Converter.Set(Value).Contains(MudCombobox.Text ?? MudCombobox.Converter.Set(MudCombobox.Value), StringComparison.CurrentCultureIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        protected void SyncSelected()
        {
            if (MudCombobox == null)
            {
                return;
            }

            if (MudCombobox.SelectedValues.Contains(Value)) 
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
            await MudCombobox.ToggleOption(this, !Selected);
            //await MudCombobox?.SelectOption(Value);
            await InvokeAsync(StateHasChanged);
            //if (MudCombobox.MultiSelection == false)
            //{
            //    await MudCombobox?.CloseMenu();
            //}
            //else
            //{
            //    await MudCombobox.FocusAsync();
            //}
            await OnClick.InvokeAsync();
        }

        protected bool GetDisabledStatus()
        {
            if (MudCombobox?.ItemDisabledFunc != null)
            {
                return MudCombobox.ItemDisabledFunc(Value);
            }
            return Disabled;
        }

        public void Dispose()
        {
            try
            {
                MudCombobox?.Remove(this);
            }
            catch (Exception) { }
        }
    }
}
