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
        private MudSelectExtended<T?>? _registeredSelect;
        private bool _hasRegisteredPresentationMetadata;
        private T? _registeredValue;
        private string? _registeredDisplayString;
        private bool _registeredDisabled;
        private bool _registeredIsFunctional;
        private string? _registeredHref;
        private string? _registeredClass;
        private string? _registeredStyle;

        internal MudSelectExtended<T?>? MudSelectExtended => (MudSelectExtended<T?>?)_parent;

        /// <summary>
        /// 
        /// </summary>
        public MudListItemExtended<T> ListItem { get; set; } = new();
        internal string ItemId { get; } = Identifier.Create("selectItem_");

        /// <summary>
        /// The parent select component. Registration is reconciled in OnParametersSet so all
        /// cascading parameters (including HideContent) have been applied before side effects run.
        /// </summary>
        [CascadingParameter]
        internal IMudSelectExtended? IMudSelectExtended
        {
            get => _parent;
            set => _parent = value;
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

        /// <inheritdoc />
        protected override void OnParametersSet()
        {
            base.OnParametersSet();

            _parent?.CheckGenericTypeMatch(this);
            var select = MudSelectExtended;

            // Declarative ChildContent uses a metadata-only pass (HideContent=true). Collection-
            // backed items are always transient view components; ItemCollection and selected values
            // are authoritative regardless of whether the visible list itself is virtualized.
            var shouldRegister = select != null && select.ItemCollection == null && HideContent;
            var previousValue = _registeredValue;
            var presentationMetadataChanged = HasRegisteredPresentationMetadataChanged(select);

            if (!shouldRegister || !ReferenceEquals(_registeredSelect, select))
            {
                _registeredSelect?.Remove(this);
                _registeredSelect = null;
                _hasRegisteredPresentationMetadata = false;
            }

            if (shouldRegister && _registeredSelect == null)
            {
                select!.Add(this);
                _registeredSelect = select;
                CaptureRegisteredPresentationMetadata();
            }
            else if (shouldRegister && presentationMetadataChanged)
            {
                CaptureRegisteredPresentationMetadata();

                if (select!.IsSelectedPresentationValue(previousValue) ||
                    select.IsSelectedPresentationValue(Value))
                {
                    select.MarkDeclarativePresentationDirty();
                }
            }
        }

        private bool HasRegisteredPresentationMetadataChanged(MudSelectExtended<T?>? select)
        {
            if (!_hasRegisteredPresentationMetadata)
                return false;

            var valueChanged = select == null
                ? !EqualityComparer<T?>.Default.Equals(_registeredValue, Value)
                : !select.PresentationValuesEqual(_registeredValue, Value);

            return valueChanged
                || _registeredDisplayString != DisplayString
                || _registeredDisabled != Disabled
                || _registeredIsFunctional != IsFunctional
                || _registeredHref != Href
                || _registeredClass != Class
                || _registeredStyle != Style;
        }

        private void CaptureRegisteredPresentationMetadata()
        {
            _registeredValue = Value;
            _registeredDisplayString = DisplayString;
            _registeredDisabled = Disabled;
            _registeredIsFunctional = IsFunctional;
            _registeredHref = Href;
            _registeredClass = Class;
            _registeredStyle = Style;
            _hasRegisteredPresentationMetadata = true;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            try
            {
                _registeredSelect?.Remove(this);
                _registeredSelect = null;
            }
            catch (Exception) { }
        }
    }
}
