using Microsoft.AspNetCore.Components;
using MudBlazor;
using MudBlazor.Utilities;
using MudExtensions.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MudExtensions
{
    public partial class MudComboboxItem<T> : MudBaseSelectItem, IDisposable
    {
        protected string Classname => new CssBuilder("mud-list-item")
            .AddClass("mud-list-item-dense", (MudCombobox?.Dense) ?? false)
            .AddClass("mud-ripple", !DisableRipple && !Disabled)
            .AddClass($"mud-selected-item mud-{MudCombobox?.Color.ToDescriptionString()}-text mud-{MudCombobox?.Color.ToDescriptionString()}-hover", !Disabled)
            .AddClass("mud-list-item-disabled", Disabled)
            .AddClass(Class)
            .Build();

        private String GetCssClasses() => new CssBuilder()
            .AddClass(Class)
            .Build();

        //internal MudCombobox<T> MudCombobox;
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

        /// <summary>
        /// Select items with HideContent==true are only there to register their RenderFragment with the select but
        /// wont render and have no other purpose!
        /// </summary>
        [CascadingParameter(Name = "HideContent")]
        internal bool HideContent { get; set; }

        private void OnUpdateSelectionStateFromOutside(IEnumerable<T> selection)
        {
            if (selection == null)
                return;
            var old_is_selected = IsSelected;
            IsSelected = selection.Contains(Value);
            if (old_is_selected != IsSelected)
                InvokeAsync(StateHasChanged);
        }

        /// <summary>
        /// A user-defined option that can be selected
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public T Value { get; set; }

        /// <summary>
        /// Mirrors the MultiSelection status of the parent select
        /// </summary>
        protected bool MultiSelection
        {
            get
            {
                if (MudCombobox == null)
                    return false;
                return MudCombobox.MultiSelection;
            }
        }

        private bool _isSelected;
        internal bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected == value)
                    return;
                _isSelected = value;
            }
        }

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

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            MudCombobox?.Add(this);
        }

        protected void HandleOnClick()
        {
            // Selection works on list. We arrange only popover state and some minor arrangements on click.
            MudCombobox?.SelectOption(Value).AndForgetExt();
            InvokeAsync(StateHasChanged);
            if (!MultiSelection)
            {
                MudCombobox?.CloseMenu().AndForgetExt();
            }
            else
            {
                MudCombobox?.FocusAsync().AndForgetExt();
            }
            OnClick.InvokeAsync().AndForgetExt();
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
