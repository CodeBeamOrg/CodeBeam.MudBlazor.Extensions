using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using MudBlazor.Services;
using MudBlazor.Utilities;
using MudExtensions.Enums;
using MudExtensions.Extensions;
using MudExtensions.Services;
using System.Runtime.InteropServices;

namespace MudExtensions
{
    public partial class MudComboBox<T> : MudBaseInputExtended<T>
    {
        #region Constructor, Injected Services, Parameters, Fields

        public MudComboBox()
        {
            Adornment = Adornment.End;
            IconSize = Size.Medium;
            //base.SkipUpdateProcessOnSetParameters = true;
        }

        [Inject] IKeyInterceptorFactory KeyInterceptorFactory { get; set; }
        [Inject] IScrollManagerExtended ScrollManagerExtended { get; set; }
        [Inject] IScrollManager ScrollManager { get; set; }

        protected internal void SetSearchString(T value)
        {
            _searchString = Converter.Set(value);
        }

        internal string _searchString { get; set; }
        private readonly string multiSelectionText;
        private IKeyInterceptor _keyInterceptor;
        static readonly KeyInterceptorOptions _keyInterceptorOptions = new()
        {
            //EnableLogging = true,
            TargetClass = "mud-input-control",
            Keys =
            {
                new KeyOptions { Key=" ", PreventDown = "key+none" }, //prevent scrolling page, toggle open/close
                new KeyOptions { Key="ArrowUp", PreventDown = "key+none" }, // prevent scrolling page, instead hilight previous item
                new KeyOptions { Key="ArrowDown", PreventDown = "key+none" }, // prevent scrolling page, instead hilight next item
                new KeyOptions { Key="Home", PreventDown = "key+none" },
                new KeyOptions { Key="End", PreventDown = "key+none" },
                new KeyOptions { Key="Escape" },
                new KeyOptions { Key="Enter", PreventDown = "key+none" },
                new KeyOptions { Key="NumpadEnter", PreventDown = "key+none" },
                new KeyOptions { Key="a", PreventDown = "key+ctrl" }, // select all items instead of all page text
                new KeyOptions { Key="A", PreventDown = "key+ctrl" }, // select all items instead of all page text
                new KeyOptions { Key="/./", SubscribeDown = true, SubscribeUp = true }, // for our users
            }
        };

        public List<MudComboBoxItem<T>> Items { get; set; } = new();
        internal MudComboBoxItem<T> _lastActivatedItem;
        protected internal List<MudComboBoxItem<T>> EligibleItems { get; set; } = new();
        private MudInputExtended<string> _inputReference;
        private MudTextFieldExtended<string> _searchbox;
        internal bool _isOpen;
        protected internal string _currentIcon { get; set; }

        protected string Classname =>
            new CssBuilder("mud-select-extended")
            .AddClass(Class)
            .Build();

        protected string InputClassname =>
            new CssBuilder("mud-select-input-extended")
            .AddClass(InputClass)
            .Build();

        protected string PopoverClassname =>
            new CssBuilder()
            .AddClass("d-none", _isOpen == false)
            .AddClass(PopoverClass)
            .Build();

        protected string MockInputStylename =>
            new StyleBuilder()
            .AddStyle("height: auto")
            .AddStyle("min-height: 1.1876em")
            .AddStyle("display", "inline", Value != null || SelectedValues.Any())
            .Build();

        private readonly string _elementId = string.Concat("combobox_", Guid.NewGuid().ToString().AsSpan(0, 8));
        private readonly string _popoverId = string.Concat("comboboxpopover_", Guid.NewGuid().ToString().AsSpan(0, 8));

        /// <summary>
        /// If true, combobox goes to autocomplete mode.
        /// </summary>
        [Category(CategoryTypes.FormComponent.Appearance)]
        [Parameter] public bool Editable { get; set; }

        /// <summary>
        /// If true, all items are eligible regarding what user search in textfield. Default is false.
        /// </summary>
        [Category(CategoryTypes.FormComponent.Appearance)]
        [Parameter] public bool DisableFilter { get; set; } = false;

        /// <summary>
        /// If true, searched text has highlight.
        /// </summary>
        [Category(CategoryTypes.FormComponent.Appearance)]
        [Parameter] public bool Highlight { get; set; }

        /// <summary>
        /// Overrides the highlight class.
        /// </summary>
        [Category(CategoryTypes.FormComponent.Appearance)]
        [Parameter] public string HighlightClass { get; set; }

        /// <summary>
        /// If true, selected or active items in popover has border.
        /// </summary>
        [Category(CategoryTypes.FormComponent.Appearance)]
        [Parameter] public bool Bordered { get; set; }

        /// <summary>
        /// User class names for the input, separated by space
        /// </summary>
        [Category(CategoryTypes.FormComponent.Appearance)]
        [Parameter] public string InputClass { get; set; }

        /// <summary>
        /// User style names for the input, separated by space
        /// </summary>
        [Category(CategoryTypes.FormComponent.Appearance)]
        [Parameter] public string InputStyle { get; set; }

        /// <summary>
        /// Fired when dropdown opens.
        /// </summary>
        [Category(CategoryTypes.FormComponent.Behavior)]
        [Parameter] public EventCallback OnOpen { get; set; }

        /// <summary>
        /// Fired when dropdown closes.
        /// </summary>
        [Category(CategoryTypes.FormComponent.Behavior)]
        [Parameter] public EventCallback OnClose { get; set; }

        /// <summary>
        /// Add the MudSelectItems here
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListBehavior)]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        [Category(CategoryTypes.FormComponent.ListBehavior)]
        public RenderFragment PopoverStartContent { get; set; }

        [Parameter]
        [Category(CategoryTypes.FormComponent.ListBehavior)]
        public RenderFragment PopoverEndContent { get; set; }

        [Parameter]
        [Category(CategoryTypes.FormComponent.ListBehavior)]
        public RenderFragment NoItemsContent { get; set; }

        /// <summary>
        /// Optional presentation template for items
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public RenderFragment<MudComboBoxItem<T>> ItemTemplate { get; set; }

        /// <summary>
        /// Optional presentation template for selected items
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListAppearance)]
        public RenderFragment<MudComboBoxItem<T>> ItemSelectedTemplate { get; set; }

        /// <summary>
        /// Optional presentation template for disabled items
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListAppearance)]
        public RenderFragment<MudComboBoxItem<T>> ItemDisabledTemplate { get; set; }

        /// <summary>
        /// Function to be invoked when checking whether an item should be disabled or not. Works both with renderfragment and ItemCollection approach.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListBehavior)]
        public Func<T, bool> ItemDisabledFunc { get; set; }

        /// <summary>
        /// Classname for item template or chips.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListBehavior)]
        public string TemplateClass { get; set; }

        /// <summary>
        /// If true the active (hilighted) item select on tab key. Designed for only single selection. Default is false.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Selecting)]
        public bool SelectValueOnTab { get; set; }

        /// <summary>
        /// User class names for the popover, separated by space
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListAppearance)]
        public string PopoverClass { get; set; }

        /// <summary>
        /// User class names for the popover, separated by space
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListAppearance)]
        public bool DisablePopoverPadding { get; set; }

        [Parameter]
        [Category(CategoryTypes.List.Behavior)]
        public string SearchBoxPlaceholder { get; set; }

        /// <summary>
        /// If true, compact vertical padding will be applied to all Select items.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListAppearance)]
        public Dense Dense { get; set; } = Dense.Standard;

        /// <summary>
        /// The Open Select Icon
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public string OpenIcon { get; set; } = Icons.Material.Filled.ArrowDropDown;

        /// <summary>
        /// The Close Select Icon
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public string CloseIcon { get; set; } = Icons.Material.Filled.ArrowDropUp;

        /// <summary>
        /// Dropdown color of select. Supports theme colors.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public Color Color { get; set; } = Color.Primary;

        /// <summary>
        /// The input's visual.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public ValuePresenter InputPresenter { get; set; } = ValuePresenter.Text;

        /// <summary>
        /// The items' visual in popover.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public ValuePresenter ItemPresenter { get; set; } = ValuePresenter.Text;

        /// <summary>
        /// If true, shows checkbox when multiselection is true. Default is true.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListBehavior)]
        public bool ShowCheckbox { get; set; } = true;

        /// <summary>
        /// If set to true and the MultiSelection option is set to true, a "select all" checkbox is added at the top of the list of items.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListBehavior)]
        public bool SelectAll { get; set; }

        /// <summary>
        /// Sets position of the Select All checkbox
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Appearance)]
        public SelectAllPosition SelectAllPosition { get; set; } = SelectAllPosition.BeforeSearchBox;

        /// <summary>
        /// Define the text of the Select All option.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListAppearance)]
        public string SelectAllText { get; set; } = "Select All";

        /// <summary>
        /// Function to define a customized multiselection text.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public Func<List<T>, string> MultiSelectionTextFunc { get; set; }

        /// <summary>
        /// Custom search func for searchbox. If doesn't set, it search with "Contains" logic by default. Passed value and searchString values.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListBehavior)]
        public Func<T, string, string, bool> SearchFunc { get; set; }

        /// <summary>
        /// If not null, select items will automatically created regard to the collection.
        /// </summary>
        //[Parameter]
        //[Category(CategoryTypes.FormComponent.Behavior)]
        //public ICollection<T> ItemCollection { get; set; } = null;

        /// <summary>
        /// Allows virtualization. Only work is ItemCollection parameter is not null.
        /// </summary>
        //[Parameter]
        //[Category(CategoryTypes.List.Behavior)]
        //public bool Virtualize { get; set; }

        /// <summary>
        /// If true, chips has close button and remove from SelectedValues when pressed the close button.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Behavior)]
        public bool ChipCloseable { get; set; }

        [Parameter]
        [Category(CategoryTypes.List.Behavior)]
        public string ChipClass { get; set; }

        [Parameter]
        [Category(CategoryTypes.List.Behavior)]
        public Variant ChipVariant { get; set; } = Variant.Filled;

        [Parameter]
        [Category(CategoryTypes.List.Behavior)]
        public Size ChipSize { get; set; } = Size.Small;

        /// <summary>
        /// Parameter to define the delimited string separator.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public string Delimiter { get; set; } = ", ";

        /// <summary>
        /// If true popover width will be the same as the combobox component.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public bool RelativeWidth { get; set; } = true;

        /// <summary>
        /// Sets the maxheight of the popover.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListAppearance)]
        public int MaxHeight { get; set; } = 300;

        /// <summary>
        /// Set the anchor origin point to determine where the popover will open from.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListAppearance)]
        public Origin AnchorOrigin { get; set; } = Origin.BottomCenter;

        /// <summary>
        /// Sets the transform origin point for the popover.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListAppearance)]
        public Origin TransformOrigin { get; set; } = Origin.TopCenter;

        /// <summary>
        /// If false, combobox allows value from out of bound.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public bool Strict { get; set; } = true;

        /// <summary>
        /// Show clear button.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public bool Clearable { get; set; }

        /// <summary>
        /// If true, shows a searchbox for filtering items. Only works with ItemCollection approach.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Behavior)]
        public bool SearchBox { get; set; }

        /// <summary>
        /// If true, the search-box will be focused when the dropdown is opened.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Behavior)]
        public bool SearchBoxAutoFocus { get; set; } = true;

        /// <summary>
        /// If true, the search-box has a clear icon.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Behavior)]
        public bool SearchBoxClearable { get; set; }

        /// <summary>
        /// If true, prevent scrolling while dropdown is open.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListBehavior)]
        public bool LockScroll { get; set; } = false;

        /// <summary>
        /// Button click event for clear button. Called after text and value has been cleared.
        /// </summary>
        [Parameter] public EventCallback<MouseEventArgs> OnClearButtonClick { get; set; }

        /// <summary>
        /// Custom indeterminate icon.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListAppearance)]
        public string IndeterminateIcon { get; set; } = Icons.Material.Filled.IndeterminateCheckBox;

        /// <summary>
        /// If true, multiple values can be selected via checkboxes which are automatically shown in the dropdown
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListBehavior)]
        public bool MultiSelection { get; set; }

        [Parameter]
        [Category(CategoryTypes.FormComponent.ListBehavior)]
        public bool ToggleSelection { get; set; }

        protected async Task SyncMultiselectionValues(bool singleToMultiselection)
        {
            if (singleToMultiselection == true)
            {
                if (Value == null)
                {
                    SelectedValues = new HashSet<T>();
                }
                else
                {
                    if (Value is string && string.IsNullOrEmpty(Converter.Set(Value)))
                    {
                        SelectedValues = new HashSet<T>();
                    }
                    else
                    {
                        SelectedValues = new HashSet<T>() { Value };
                    }

                }
                await SelectedValuesChanged.InvokeAsync(_selectedValues);
            }
            else
            {
                await SetValueAsync(SelectedValues.LastOrDefault(), false);
                _searchString = Converter.Set(SelectedValues.LastOrDefault());
            }
        }

        private IEqualityComparer<T> _comparer;
        /// <summary>
        /// The Comparer to use for comparing selected values internally.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public IEqualityComparer<T> Comparer
        {
            get => _comparer;
            set
            {
                if (_comparer == value)
                    return;
                _comparer = value;
                // Apply comparer and refresh selected values
                _selectedValues = new HashSet<T>(_selectedValues, _comparer);
                SelectedValues = _selectedValues;
            }
        }

        private Func<T, string> _toStringFunc = x => x?.ToString();
        /// <summary>
        /// Defines how values are displayed in the drop-down list
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListBehavior)]
        public Func<T, string> ToStringFunc
        {
            get => _toStringFunc;
            set
            {
                if (_toStringFunc == value)
                    return;
                _toStringFunc = value;
                Converter = new Converter<T>
                {
                    SetFunc = _toStringFunc ?? (x => x?.ToString()),
                };
            }
        }

        #endregion


        #region Values, Texts & Items

        private HashSet<T> _selectedValues;
        /// <summary>
        /// Set of selected values. If MultiSelection is false it will only ever contain a single value. This property is two-way bindable.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Data)]
        public IEnumerable<T> SelectedValues
        {
            get
            {
                _selectedValues ??= new HashSet<T>(_comparer);
                return _selectedValues;
            }
            set
            {
                var set = value ?? new HashSet<T>(_comparer);
                if (value == null && _selectedValues == null)
                {
                    return;
                }
                if (value != null && _selectedValues != null && _selectedValues.SetEquals(value))
                {
                    return;
                }
                if (SelectedValues.Count() == set.Count() && _selectedValues.All(x => set.Contains(x, _comparer)))
                    return;

                _selectedValues = new HashSet<T>(set, _comparer);

                SelectedValuesChanged.InvokeAsync(new HashSet<T>(SelectedValues, _comparer)).AndForget();
                ForceUpdateItems().AndForgetExt();
                //Console.WriteLine("SelectedValues setter ended");
            }
        }

        /// <summary>
        /// Fires when SelectedValues changes.
        /// </summary>
        [Parameter] public EventCallback<IEnumerable<T>> SelectedValuesChanged { get; set; }

        protected Task UpdateDataVisualiserTextAsync()
        {
            var textList = new List<string>();
            if (Items != null && Items.Any())
            {
                if (false) // ItemCollection != null
                {
                    //foreach (var val in SelectedValues)
                    //{
                    //    var collectionValue = ItemCollection.FirstOrDefault(x => x != null && (Comparer != null ? Comparer.Equals(x, val) : x.Equals(val)));
                    //    if (collectionValue != null)
                    //    {
                    //        textList.Add(Converter.Set(collectionValue));
                    //    }
                    //}
                }
                else
                {
                    foreach (var val in SelectedValues)
                    {
                        if (!Strict && !Items.Select(x => x.Value).Contains(val))
                        {
                            textList.Add(ToStringFunc != null ? ToStringFunc(val) : Converter.Set(val));
                            continue;
                        }
                        var item = Items.FirstOrDefault(x => x != null && (x.Value == null ? val == null : Comparer != null ? Comparer.Equals(x.Value, val) : x.Value.Equals(val)));
                        if (item != null)
                        {
                            textList.Add(!string.IsNullOrEmpty(item.Text) ? item.Text : Converter.Set(item.Value));
                        }
                    }
                }
            }

            // when multiselection is true, we return
            // a comma separated list of selected values
            if (MultiSelection)
            {
                if (MultiSelectionTextFunc != null)
                {
                    if (!SelectedValues.Any())
                    {
                        _dataVisualiserText = null;
                        return Task.CompletedTask;
                    }
                    _dataVisualiserText = MultiSelectionTextFunc.Invoke(SelectedValues.ToList());
                    return Task.CompletedTask;
                }
                else
                {
                    _dataVisualiserText = string.Join(Delimiter, textList);
                    return Task.CompletedTask;
                }
            }
            else
            {
                var item = Items.FirstOrDefault(x => Value == null ? x.Value == null : Comparer != null ? Comparer.Equals(Value, x.Value) : Value.Equals(x.Value));
                _dataVisualiserText = item is null
                    ? Converter.Set(Value)
                    : (!string.IsNullOrEmpty(item.Text) ? item.Text : Converter.Set(item.Value));

                return Task.CompletedTask;
            }
        }

        protected async Task UpdateComboBoxValueAsync(T value, bool updateText = true, bool updateSearchString = false, bool force = false)
        {
            await SetValueAsync(value, updateText, force);
            if (updateSearchString)
            {
                _searchString = Converter.Set(Value);
                await _inputReference.SetText(_searchString);
            }
        }

        protected internal string _dataVisualiserText;

        protected internal string GetPresenterText()
        {
            return _dataVisualiserText;
        }

        #endregion


        #region Lifecycle Methods

        protected override void OnInitialized()
        {
            base.OnInitialized();
            UpdateIcon();
            if (!MultiSelection && Value != null)
            {
                _selectedValues = new HashSet<T>(_comparer) { Value };
            }
            else if (MultiSelection && SelectedValues != null)
            {
                // TODO: Check this line again
                SetValueAsync(SelectedValues.FirstOrDefault()).AndForget();
            }

        }

        bool _oldShowCheckbox = true;
        bool _oldBordered;
        Dense _oldDense = Dense.Standard;
        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            if (_oldShowCheckbox != ShowCheckbox ||
                _oldBordered != Bordered ||
                _oldDense != Dense)
            {
                ForceRenderItems();
            }
            _oldShowCheckbox = ShowCheckbox;
            _oldBordered = Bordered;
            _oldDense = Dense;
            _allSelected = GetAllSelectedState();
        }

        bool _firstRendered = false;
        T _oldValue;
        bool _oldMultiselection = false;
        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();
            UpdateIcon();
            if (MultiSelection != _oldMultiselection)
            {
                if (_firstRendered == true)
                {
                    await SyncMultiselectionValues(MultiSelection);
                }
                ForceRenderItems();
                if (MultiSelection == true)
                {
                    _searchString = null;
                }
                else
                {
                    DeselectAllItems();
                    if (Value is not null)
                        Items.FirstOrDefault(x => x.Value.Equals(Value)).Selected = true;
                }
            }
            if ((Value == null && _oldValue != null) || (Value != null && Value.Equals(_oldValue) == false))
            {
                await ForceUpdateItems();
                if (MultiSelection == false)
                {
                    _searchString = Converter.Set(Value);
                    if (_inputReference != null)
                    {
                        await _inputReference?.SetText(_searchString);
                    }
                }
            }
            await UpdateDataVisualiserTextAsync();
            _oldMultiselection = MultiSelection;
            _oldValue = Value;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {

            if (firstRender)
            {
                _keyInterceptor = KeyInterceptorFactory.Create();
                await _keyInterceptor.Connect(_elementId, _keyInterceptorOptions);
                _keyInterceptor.KeyDown += HandleKeyDown;
                _keyInterceptor.KeyUp += HandleKeyUp;
                await UpdateDataVisualiserTextAsync();
                _firstRendered = true;
                StateHasChanged();
            }
            //Console.WriteLine("Select rendered");
            await base.OnAfterRenderAsync(firstRender);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                if (_keyInterceptor != null)
                {
                    _keyInterceptor.KeyDown -= HandleKeyDown;
                    _keyInterceptor.KeyUp -= HandleKeyUp;
                    _keyInterceptor.Dispose();
                    _keyInterceptor = null;
                }

                Items.Clear();
            }
        }

        #endregion


        #region Events (Key, Focus)

        protected internal async void HandleKeyDown(KeyboardEventArgs obj)
        {
            if (Disabled || ReadOnly)
                return;

            var key = obj.Key.ToLowerInvariant();
            if (Editable == false && key.Length == 1 && key != " " && !(obj.CtrlKey || obj.ShiftKey || obj.AltKey || obj.MetaKey))
            {
                await ActivateFirstItem(key);
                return;
            }

            switch (obj.Key)
            {
                case "Tab":
                    if (MultiSelection == false && SelectValueOnTab)
                    {
                        await ToggleOption(_lastActivatedItem, true);
                    }
                    await CloseMenu();
                    break;
                case "a":
                case "A":
                    if (obj.CtrlKey)
                    {
                        if (MultiSelection)
                        {
                            await SelectAllItems();
                        }
                    }
                    break;
                case "Home":
                    await ActivateFirstItem();
                    break;
                case "End":
                    await ActivateLastItem();
                    break;
                case "ArrowUp":
                    if (obj.AltKey)
                    {
                        await CloseMenu();
                    }
                    else if (!_isOpen)
                    {
                        await OpenMenu();
                    }
                    else
                    {
                        await ActivateAdjacentItem(-1);
                    }
                    break;
                case "ArrowDown":
                    if (obj.AltKey)
                    {
                        await OpenMenu();
                    }
                    else if (!_isOpen)
                    {
                        await OpenMenu();
                    }
                    else
                    {
                        await ActivateAdjacentItem(1);
                    }
                    break;
                case " ":
                    await ToggleMenu();
                    break;
                case "Escape":
                    await CloseMenu();
                    break;
                case "Enter":
                case "NumpadEnter":
                    if (MultiSelection == false)
                    {
                        if (_isOpen == false)
                        {
                            await OpenMenu();
                        }
                        else
                        {
                            await ToggleOption(_lastActivatedItem, !_lastActivatedItem?.Selected ?? true);
                        }
                        break;
                    }
                    else
                    {
                        if (_isOpen == false)
                        {
                            await OpenMenu();
                            break;
                        }
                        else
                        {
                            await ToggleOption(_lastActivatedItem, !_lastActivatedItem?.Selected ?? true);
                            //await _inputReference.SetText(Text);
                            if (_lastActivatedItem?.Selected == false)
                            {
                                _lastActivatedItem?.SetActive(true);
                            }
                            break;
                        }
                    }
            }
            await OnKeyDown.InvokeAsync(obj);
        }

        protected internal async Task SearchBoxHandleKeyDown(KeyboardEventArgs obj)
        {
            if (Disabled)
                return;
            switch (obj.Key)
            {
                case "Escape":
                case "ArrowUp":
                case "ArrowDown":
                case "Home":
                case "End":
                case "Enter":
                case "NumpadEnter":
                    HandleKeyDown(obj);
                    break;
                case "Tab":
                    await ActivateFirstItem();
                    await FocusAsync();
                    StateHasChanged();
                    break;
            }

        }

        protected internal void SearchBoxHandleKeyUp(KeyboardEventArgs obj)
        {
            ForceRenderItems();
        }

        protected internal async void HandleKeyUp(KeyboardEventArgs obj)
        {
            ForceRenderItems();
            await OnKeyUp.InvokeAsync(obj);
        }

        protected internal async Task HandleOnBlur(FocusEventArgs obj)
        {
            if (!MultiSelection)
            {
                if (Strict)
                {
                    // Check if the user-provided search string is an exact (case-insensitive) match against an item in the collection.
                    var item = Items.FirstOrDefault(x => Converter.Set(x.Value).Equals(_searchString, StringComparison.OrdinalIgnoreCase));
                    if (item is not null)
                        await ToggleOption(item, true);

                    // No item equals the user-provided search string.
                    else
                    {
                        // Restore the previous selected item, if any.
                        if (Value is not null)
                        {
                            item = Items.FirstOrDefault(x => x.Value.Equals(Value));
                            if (item is not null)
                                await ToggleOption(item, true);

                            // Remove non-matching search string.
                            else
                                await Clear();
                        }

                        // There was no previous selected item. Remove non-matching search string.
                        else
                            await Clear();
                    }
                }
                else
                    await UpdateComboBoxValueAsync(Converter.Get(_searchString), updateText: true, updateSearchString: true);
            }
            await OnBlurredAsync(obj);
        }

        protected internal void HandleInternalValueChanged(string val)
        {
            _searchString = val;
        }

        public override ValueTask FocusAsync()
        {
            return _inputReference.FocusAsync();
        }

        public override ValueTask BlurAsync()
        {
            return _inputReference.BlurAsync();
        }

        public override ValueTask SelectAsync()
        {
            return _inputReference.SelectAsync();
        }

        public override ValueTask SelectRangeAsync(int pos1, int pos2)
        {
            return _inputReference.SelectRangeAsync(pos1, pos2);
        }

        #endregion


        #region PopoverState

        public async Task ToggleMenu()
        {
            if (Disabled || ReadOnly)
                return;
            if (_isOpen)
            {
                await CloseMenu();
            }
            else
            {
                await OpenMenu();
            }
        }

        public async Task OpenMenu()
        {
            if (Disabled || ReadOnly)
                return;
            _isOpen = true;
            UpdateIcon();
            StateHasChanged();

            if (Editable)
            {
                if (MultiSelection)
                {
                    if (SearchBoxAutoFocus)
                    {
                        await Task.Delay(1);
                        await _searchbox.SelectAsync();
                    }
                }
                else
                    await _inputReference.SelectAsync();
            }

            // Disable escape propagation: if ComboBox menu is open, only the ComboBox popover should close and underlying components should not handle escape key.
            await _keyInterceptor.UpdateKey(new() { Key = "Escape", StopDown = "Key+none" });

            if (MultiSelection)
            {
                _lastActivatedItem = Items.LastOrDefault(x => x.Value.Equals(SelectedValues.LastOrDefault()));
            }
            else
            {
                _lastActivatedItem = Items.FirstOrDefault(x => x.Value.Equals(Value));
            }
            _allSelected = GetAllSelectedState();
            await ScrollToMiddleAsync(_lastActivatedItem);
            await OnOpen.InvokeAsync();
        }

        public async Task CloseMenu()
        {
            _isOpen = false;
            UpdateIcon();
            DeactiveAllItems();
            StateHasChanged();

            // Enable escape propagation: The ComboBox popover was closed, no underlying components are allowed to handle escape key.
            await _keyInterceptor.UpdateKey(new() { Key = "Escape", StopDown = "none" });

            await OnClose.InvokeAsync();
        }

        #endregion


        #region Item Registration & Selection

        protected internal async Task ToggleOption(MudComboBoxItem<T> item, bool selected)
        {
            if (item == null)
            {
                return;
            }

            if (selected == false)
            {
                if (MultiSelection == false && Value?.Equals(item.Value) == true)
                {
                    if (ToggleSelection)
                    {
                        await UpdateComboBoxValueAsync(default, updateText: true, updateSearchString: true);
                        item.Selected = false;
                    }
                }
                else if (MultiSelection == true && SelectedValues.Contains(item.Value))
                {
                    SelectedValues = SelectedValues.Where(x => x.Equals(item.Value) == false);
                    await SetValueAsync(SelectedValues.LastOrDefault(), false);
                    item.Selected = false;
                    _allSelected = GetAllSelectedState();
                }
            }
            else
            {
                if (MultiSelection == false)
                {
                    DeselectAllItems();
                    await UpdateComboBoxValueAsync(item.Value, updateText: true, updateSearchString: true);
                }
                else if (SelectedValues.Contains(item.Value) == false)
                {
                    await SetValueAsync(item.Value, false);
                    SelectedValues = SelectedValues.Append(item.Value);
                    await SelectedValuesChanged.InvokeAsync(_selectedValues);
                    _allSelected = GetAllSelectedState();

                    //await Task.Delay(1);
                }
                item.Selected = true;
            }
            DeactiveAllItems();
            _lastActivatedItem = item;
            await UpdateDataVisualiserTextAsync();
            if (MultiSelection == false)
            {
                await CloseMenu();
            }
            else
            {
                //await FocusAsync();
            }
        }

        protected void DeselectAllItems()
        {
            Items.ForEach(x => x.Selected = false);
        }

        public override async Task ForceUpdate()
        {
            await base.ForceUpdate();
            if (!MultiSelection)
            {
                SelectedValues = new HashSet<T>(_comparer) { Value };
            }
            else
            {
                await SelectedValuesChanged.InvokeAsync(new HashSet<T>(SelectedValues, _comparer));
            }
        }

        //public async Task BeginValidatePublic()
        //{
        //    await BeginValidateAsync();
        //}

        protected internal bool? Add(MudComboBoxItem<T> item)
        {
            if (item == null)
                return false;
            bool? result = null;
            if (Items.Select(x => x.Value).Contains(item.Value) == false)
            {
                Items.Add(item);
                if (MultiSelection == true && SelectedValues.Contains(item.Value))
                {
                    item.Selected = true;
                }
                result = true;
            }
            return result;
        }

        protected internal void RemoveItem(MudComboBoxItem<T> item) => Items.Remove(item);

        #endregion


        #region Clear

        /// <summary>
        /// Extra handler for clearing selection.
        /// </summary>
        protected async Task ClearButtonClickHandlerAsync(MouseEventArgs e)
        {
            await UpdateComboBoxValueAsync(default);
            _searchString = null;
            await SetTextAsync(default, false);
            _selectedValues.Clear();
            DeselectAllItems();
            await BeginValidateAsync();
            await ForceUpdateItems();
            StateHasChanged();
            await SelectedValuesChanged.InvokeAsync(new HashSet<T>(SelectedValues, _comparer));
            await OnClearButtonClick.InvokeAsync(e);
        }

        /// <summary>
        /// Clear the selection
        /// </summary>
        public async Task Clear()
        {
            await SetValueAsync(default, false);
            _searchString = null;
            await SetTextAsync(default, false);
            _selectedValues.Clear();
            await BeginValidateAsync();
            StateHasChanged();
            await SelectedValuesChanged.InvokeAsync(new HashSet<T>(SelectedValues, _comparer));
        }

        protected override void ResetValue()
        {
            base.ResetValue();
            SelectedValues = null;
        }

        #endregion

        protected void UpdateIcon()
        {
            _currentIcon = !string.IsNullOrWhiteSpace(AdornmentIcon) ? AdornmentIcon : _isOpen ? CloseIcon : OpenIcon;
        }

        //public void CheckGenericTypeMatch(object select_item)
        //{
        //    var itemT = select_item.GetType().GenericTypeArguments[0];
        //    if (itemT != typeof(T))
        //        throw new GenericTypeMismatchException("MudSelectExtended", "MudSelectItemExtended", typeof(T), itemT);
        //}

        protected override bool HasValue(T value)
        {
            if (MultiSelection)
                return SelectedValues.Any();
            else
                return base.HasValue(value);
        }

        protected async Task ChipClosed(MudChip chip)
        {
            if (chip == null || SelectedValues == null)
            {
                return;
            }
            //SelectedValues = SelectedValues.Where(x => Converter.Set(x)?.ToString() != chip.Value?.ToString());
            SelectedValues = SelectedValues.Where(x => x.Equals(chip.Value) == false);
            await SelectedValuesChanged.InvokeAsync(SelectedValues);
        }

        private bool? _allSelected;
        protected async Task SelectAllItems()
        {
            if (_allSelected == null || _allSelected == false)
            {
                SelectedValues = new List<T>();
                foreach (var item in Items.Where(x => x.Eligible))
                {
                    item.Selected = true;
                    SelectedValues = SelectedValues.Append(item.Value);
                }
                await SelectedValuesChanged.InvokeAsync(SelectedValues);
                await SetValueAsync(SelectedValues.LastOrDefault(), false);
                _allSelected = true;
            }
            else
            {
                foreach (var item in Items.Where(x => x.Selected))
                {
                    item.Selected = false;
                }
                SelectedValues = null;
                await SelectedValuesChanged.InvokeAsync(SelectedValues);
                _allSelected = false;
            }
        }

        protected bool? GetAllSelectedState()
        {
            if (MultiSelection)
            {
                var count = SelectedValues.Count();
                if (count == 0)
                    return false;

                if (count == Items.Count)
                    return true;

            }
            return null;
        }

        protected internal void ForceRenderItems()
        {
            Items.ForEach((x) => x.ForceRender());
            StateHasChanged();
        }

        protected internal async Task ForceUpdateItems()
        {
            Items.ForEach(async (x) => await x.ForceUpdate());
        }

        #region Active (Hilight)

        //protected int GetActiveItemIndex()
        //{
        //    if (_lastActivatedItem == null)
        //    {
        //        var a = Items.FindIndex(x => x.Active == true);
        //        return a;
        //    }
        //    else
        //    {
        //        var a = Items.FindIndex(x => _lastActivatedItem.Value == null ? x.Value == null : Comparer != null ? Comparer.Equals(_lastActivatedItem.Value, x.Value) : _lastActivatedItem.Value.Equals(x.Value));
        //        return a;
        //    }
        //}

        protected int GetActiveProperItemIndex()
        {
            var properItems = GetEnabledAndEligibleItems();
            if (properItems.Any())
            {
                if (_lastActivatedItem == null)
                {
                    var a = properItems.FindIndex(x => x.Active == true);
                    return a;
                }
                else
                {
                    var a = properItems.FindIndex(x => _lastActivatedItem.Value == null ? x.Value == null : Comparer != null ? Comparer.Equals(_lastActivatedItem.Value, x.Value) : _lastActivatedItem.Value.Equals(x.Value));
                    return a;
                }
            }
            return -1;
        }

        //protected T GetActiveItemValue()
        //{
        //    if (_lastActivatedItem == null)
        //    {
        //        return Items.FirstOrDefault(x => x.Active == true).Value;
        //    }
        //    else
        //    {
        //        return _lastActivatedItem.Value;
        //    }
        //}

        //protected internal void UpdateLastActivatedItem(T value)
        //{
        //    _lastActivatedItem = Items.FirstOrDefault(x => value == null ? x.Value == null : Comparer != null ? Comparer.Equals(value, x.Value) : value.Equals(x.Value));
        //}

        protected void DeactiveAllItems()
        {
            foreach (var item in CollectionsMarshal.AsSpan(Items))
            {
                if (item.Active)
                    item.SetActive(false);
            }
        }

        [Obsolete("Please use ActivateFirstItem() method")]
        public Task ActiveFirstItem(string startChar = null) => ActivateFirstItem(startChar);
        public async Task ActivateFirstItem(string startsWith = null)
        {
            var item = Items.FirstOrDefault();
            if (item is null || item.Disabled)
                return;

            DeactiveAllItems();
            if (string.IsNullOrWhiteSpace(startsWith))
            {
                item = GetEnabledAndEligibleItems().FirstOrDefault();
                if (item is not null)
                {
                    item.SetActive(true);
                    await ScrollToMiddleAsync(item);
                }
                _lastActivatedItem = item;
                return;
            }

            if (Editable)
                return;

            var foundItems = Items.Where(x => (x.Text ?? Converter.Set(x.Value) ?? string.Empty).StartsWith(startsWith, StringComparison.OrdinalIgnoreCase)).ToList();
            if (!foundItems.Any())
            {
                if (_lastActivatedItem is not null)
                {
                    _lastActivatedItem.SetActive(true);
                    await ScrollToMiddleAsync(_lastActivatedItem);
                }
                return;
            }

            item = foundItems.FirstOrDefault(x => x == _lastActivatedItem);
            if (item is null)
            {
                item = foundItems[0];
                item.SetActive(true);
                await ScrollToMiddleAsync(item);
                _lastActivatedItem = item;
                return;
            }

            if (item == foundItems.LastOrDefault())
            {
                item = foundItems[0];
                item.SetActive(true);
                await ScrollToMiddleAsync(item);
                _lastActivatedItem = item;
            }
            else
            {
                item = foundItems[foundItems.IndexOf(item) + 1];
                item.SetActive(true);
                await ScrollToMiddleAsync(item);
                _lastActivatedItem = item;
            }
        }

        [Obsolete("Please use ActivateAdjacentItem() method")]
        public Task ActiveAdjacentItem(int changeCount) => ActivateAdjacentItem(changeCount);
        public async Task ActivateAdjacentItem(int changeCount)
        {
            if (Items == null || Items.Count == 0)
                return;

            var properItems = GetEnabledAndEligibleItems();
            int index = GetActiveProperItemIndex();
            if (properItems.Count == 1 && index == 0)
            {
                properItems[0].SetActive(true);
                _lastActivatedItem = properItems[0];
            }
            else if (index + changeCount >= properItems.Count || 0 > index + changeCount)
            {
                return;
            }

            DeactiveAllItems();
            properItems[index + changeCount].SetActive(true);
            _lastActivatedItem = properItems[index + changeCount];
            await ScrollToMiddleAsync(Items[index + changeCount]);
        }

        [Obsolete("Please use ActivateLastItem() method")]
        public Task ActiveLastItem() => ActivateLastItem();
        public async Task ActivateLastItem()
        {
            if (!(Items.Count > 0))
                return;

            DeactiveAllItems();
            var item = GetEnabledAndEligibleItems().LastOrDefault();
            if (item is not null)
            {
                item.SetActive(true);
                await ScrollToMiddleAsync(item);
            }
            _lastActivatedItem = item;
        }

        #endregion

        protected internal List<MudComboBoxItem<T>> GetEnabledAndEligibleItems() => Items.Where(x => !x.Disabled && x.Eligible).ToList();

        protected bool HasEligibleItems()
        {
            foreach (var item in CollectionsMarshal.AsSpan(Items))
            {
                if (item.Eligible)
                    return true;
            }
            return false;
        }

        protected internal string GetSearchString() => _searchString;

        protected Typo GetTypo() => Dense == Dense.Slim || Dense == Dense.Superslim ? Typo.body2 : Typo.body1;

        protected internal ValueTask ScrollToMiddleAsync(MudComboBoxItem<T> item) => item is not null ? ScrollManagerExtended.ScrollToMiddleAsync(_popoverId, item.ItemId) : ValueTask.CompletedTask;
    }
}
