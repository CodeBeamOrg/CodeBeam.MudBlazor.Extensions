using Microsoft.AspNetCore.Components;
using MudBlazor;
using MudBlazor.Extensions;
using MudBlazor.Utilities;

namespace MudExtensions
{
    /// <summary>
    /// Represents an option of a select or multi-select. To be used inside MudSelect.
    /// </summary>
    public partial class MudSelectItemExtended<T> : MudComponentBase, IDisposable
    {
        private string GetCssClasses() => new CssBuilder()
            .AddClass(Class)
            .Build();

        private IMudSelectExtended? _parent;
        internal MudSelectExtended<T?>? MudSelectExtended => (MudSelectExtended<T?>?)IMudSelectExtended;
        /// <summary>
        /// 
        /// </summary>
        public MudListItemExtended<T> ListItem { get; set; } = new();
        internal string ItemId { get; } = Identifier.Create("selectItem_");

        private IMudShadowSelectExtended? _shadowParent;
        /// <summary>
        /// The parent select component
        /// </summary>
        [CascadingParameter]
        internal IMudSelectExtended? IMudSelectExtended
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
        public string? Text { get; set; }

        /// <summary>
        /// Select items with HideContent==true are only there to register their RenderFragment with the select but
        /// wont render and have no other purpose!
        /// </summary>
        [CascadingParameter(Name = "HideContent")]
        internal bool HideContent { get; set; }

        private void OnUpdateSelectionStateFromOutside(IEnumerable<T?>? selection)
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
        public T? Value { get; set; }

        /// <summary>
        /// The URL to navigate to when this item is clicked.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.General.ClickAction)]
        public string? Href { get; set; }

        /// <summary>
        /// The content within this item.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.General.Behavior)]
        public RenderFragment? ChildContent { get; set; }

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

        /// <summary>
        /// OnClick event.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public EventCallback OnClick { get; set; }

        /// <summary>
        /// Prevents the user from interacting with this item.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.General.Behavior)]
        public bool Disabled { get; set; }


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

        /// <summary>
        /// 
        /// </summary>
        protected string? DisplayString
        {
            get
            {
                if (MudSelectExtended == null)
                {
                    return $"{(string.IsNullOrEmpty(Text) ? Value : Text)}";
                }

                return !string.IsNullOrEmpty(Text) ? Text : MudSelectExtended.ConverterSetCore(Value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected async Task HandleOnClickAsync()
        {
            // Selection works on list. We arrange only popover state and some minor arrangements on click.
            await MudSelectExtended!.SelectOption(Value);
            await InvokeAsync(StateHasChanged);
            if (!MultiSelection)
            {
                await MudSelectExtended!.CloseMenu();
            }
            else
            {
                MudSelectExtended?.FocusAsync();
            }
            await OnClick.InvokeAsync();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected bool GetDisabledStatus()
        {
            if (MudSelectExtended?.ItemDisabledFunc != null)
            {
                return MudSelectExtended.ItemDisabledFunc(Value);
            }
            return Disabled;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            try
            {
                MudSelectExtended?.Remove(this);
                ((MudSelectExtended<T?>?)_shadowParent)?.UnregisterShadowItem(this);
            }
            catch (Exception) { }
        }
    }
}
