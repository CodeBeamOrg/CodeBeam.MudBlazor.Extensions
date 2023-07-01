using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using MudBlazor.Utilities;
using MudExtensions;
using MudExtensions.Extensions;

namespace MudExtensions
{
    /// <summary>
    /// Represents an option of a select or multi-select. To be used inside MudSelect.
    /// </summary>
    public partial class MudSelectItemExtended<T> : MudBaseSelectItem, IDisposable
    {
        private String GetCssClasses() => new CssBuilder()
            .AddClass(Class)
            .Build();

        private IMudSelectExtended _parent;
        internal MudSelectExtended<T> MudSelectExtended => (MudSelectExtended<T>)IMudSelectExtended;
        public MudListItemExtended<T> ListItem { get; set; }
        internal string ItemId { get; } = "selectItem_"+Guid.NewGuid().ToString().Substring(0,8);

        /// <summary>
        /// The parent select component
        /// </summary>
        [CascadingParameter]
        internal IMudSelectExtended IMudSelectExtended
        {
            get => _parent;
            set
            {
                _parent = value;
                if (_parent == null)
                    return;
                _parent.CheckGenericTypeMatch(this);
                if (MudSelectExtended == null)
                    return;
                bool isSelected = MudSelectExtended.Add(this);
                if (_parent.MultiSelection)
                {
                    MudSelectExtended.SelectionChangedFromOutside += OnUpdateSelectionStateFromOutside;
                    InvokeAsync(() => OnUpdateSelectionStateFromOutside(MudSelectExtended.SelectedValues));
                }
                else
                {
                    IsSelected = isSelected;
                }
            }
        }

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

        private IMudShadowSelectExtended _shadowParent;
        [CascadingParameter]
        internal IMudShadowSelectExtended IMudShadowSelectExtended
        {
            get => _shadowParent;
            set
            {
                _shadowParent = value;
                ((MudSelectExtended<T>)_shadowParent)?.RegisterShadowItem(this);
            }
        }

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
                if (MudSelectExtended == null)
                    return false;
                return MudSelectExtended.MultiSelection;
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
                var converter = MudSelectExtended?.Converter;
                if (converter == null)
                    return $"{(string.IsNullOrEmpty(Text) ? Value : Text)}";
                return !string.IsNullOrEmpty(Text) ? Text : converter.Set(Value);
            }
        }

        protected void HandleOnClick()
        {
            // Selection works on list. We arrange only popover state and some minor arrangements on click.
            MudSelectExtended?.SelectOption(Value).AndForgetExt();
            InvokeAsync(StateHasChanged);
            if (!MultiSelection)
            {
                MudSelectExtended?.CloseMenu().AndForgetExt();
            }
            else
            {
                MudSelectExtended?.FocusAsync().AndForgetExt();
            }
            OnClick.InvokeAsync().AndForgetExt();
        }

        protected bool GetDisabledStatus()
        {
            if (MudSelectExtended?.ItemDisabledFunc != null)
            {
                return MudSelectExtended.ItemDisabledFunc(Value);
            }
            return Disabled;
        }

        public void Dispose()
        {
            try
            {
                MudSelectExtended?.Remove(this);
                ((MudSelectExtended<T>)_shadowParent)?.UnregisterShadowItem(this);
            }
            catch (Exception) { }
        }
    }
}
