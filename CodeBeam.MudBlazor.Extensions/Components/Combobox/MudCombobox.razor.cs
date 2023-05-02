﻿using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using MudBlazor.Services;
using MudBlazor.Utilities;
using MudExtensions.Enums;
using MudExtensions.Extensions;

namespace MudExtensions
{
    public partial class MudCombobox<T> : MudBaseInputExtended<T>
    {
        #region Constructor, Injected Services, Parameters, Fields

        public MudCombobox()
        {
            Adornment = Adornment.End;
            IconSize = Size.Medium;
            base.SkipUpdateProcessOnSetParameters = true;
        }

        [Inject] private IKeyInterceptorFactory KeyInterceptorFactory { get; set; }

        private string multiSelectionText;
        private IKeyInterceptor _keyInterceptor;

        public List<MudComboboxItem<T>> Items = new();
        protected internal List<MudComboboxItem<T>> EligibleItems { get; set; } = new();
        private MudInputExtended<T> _elementReference;
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

        private string _elementId = "combobox_" + Guid.NewGuid().ToString().Substring(0, 8);
        private string _popoverId = "comboboxpopover_" + Guid.NewGuid().ToString().Substring(0, 8);

        /// <summary>
        /// User class names for the input, separated by space
        /// </summary>
        [Category(CategoryTypes.FormComponent.Appearance)]
        [Parameter] public bool Editable { get; set; }

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

        /// <summary>
        /// Optional presentation template for items
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public RenderFragment<MudComboboxItem<T>> ItemTemplate { get; set; }

        /// <summary>
        /// Optional presentation template for selected items
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListAppearance)]
        public RenderFragment<MudComboboxItem<T>> ItemSelectedTemplate { get; set; }

        /// <summary>
        /// Optional presentation template for disabled items
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListAppearance)]
        public RenderFragment<MudComboboxItem<T>> ItemDisabledTemplate { get; set; }

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
        /// If true the active (hilighted) item select on tab key. Designed for only single selection. Default is true.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Selecting)]
        public bool SelectValueOnTab { get; set; } = true;

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

        /// <summary>
        /// If true, selected items doesn't have a selected background color.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Appearance)]
        public bool DisableSelectedItemStyle { get; set; }

        [Parameter]
        [Category(CategoryTypes.List.Behavior)]
        public string SearchBoxPlaceholder { get; set; }

        /// <summary>
        /// If true, compact vertical padding will be applied to all Select items.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListAppearance)]
        public bool Dense { get; set; }

        /// <summary>
        /// The Open Select Icon
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public string OpenIcon { get; set; } = Icons.Material.Filled.ArrowDropDown;

        /// <summary>
        /// Dropdown color of select. Supports theme colors.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public Color Color { get; set; } = Color.Primary;

        /// <summary>
        /// The Close Select Icon
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public string CloseIcon { get; set; } = Icons.Material.Filled.ArrowDropUp;

        /// <summary>
        /// The value presenter.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public ValuePresenter ValuePresenter { get; set; } = ValuePresenter.Text;

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
        public Func<T, string, bool> SearchFunc { get; set; }

        /// <summary>
        /// If not null, select items will automatically created regard to the collection.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public ICollection<T> ItemCollection { get; set; } = null;

        /// <summary>
        /// Allows virtualization. Only work is ItemCollection parameter is not null.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Behavior)]
        public bool Virtualize { get; set; }

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
        /// If true popover width will be the same as the select component.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public bool RelativeWidth { get; set; } = true;

        /// <summary>
        /// Sets the maxheight the Select can have when open.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListAppearance)]
        public int MaxHeight { get; set; } = 300;

        /// <summary>
        /// Set the anchor origin point to determen where the popover will open from.
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
        /// If true, the Select's input will not show any values that are not defined in the dropdown.
        /// This can be useful if Value is bound to a variable which is initialized to a value which is not in the list
        /// and you want the Select to show the label / placeholder instead.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public bool Strict { get; set; }

        /// <summary>
        /// Show clear button.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public bool Clearable { get; set; } = false;

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
        public bool SearchBoxAutoFocus { get; set; }

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
        /// Custom checked icon.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListAppearance)]
        public string CheckedIcon { get; set; } = Icons.Material.Filled.CheckBox;

        /// <summary>
        /// Custom unchecked icon.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListAppearance)]
        public string UncheckedIcon { get; set; } = Icons.Material.Filled.CheckBoxOutlineBlank;

        /// <summary>
        /// Custom indeterminate icon.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListAppearance)]
        public string IndeterminateIcon { get; set; } = Icons.Material.Filled.IndeterminateCheckBox;

        private bool _multiSelection = false;
        /// <summary>
        /// If true, multiple values can be selected via checkboxes which are automatically shown in the dropdown
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListBehavior)]
        public bool MultiSelection
        {
            get => _multiSelection;
            set
            {
                if (value != _multiSelection)
                {
                    _multiSelection = value;
                    //UpdateTextPropertyAsync(false).AndForgetExt();
                    //SyncMultiselectionValues(_multiSelection).AndForgetExt();
                }
            }
        }

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
                    SelectedValues = new HashSet<T>() { Value };
                }
                await SelectedValuesChanged.InvokeAsync(_selectedValues);
            }
            else 
            {
                await SetValueAsync(SelectedValues.LastOrDefault(), false);
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
                if (_selectedValues == null)
                    _selectedValues = new HashSet<T>(_comparer);
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
                //if (!MultiSelection)
                //{
                //    SetValueAsync(_selectedValues.FirstOrDefault()).AndForget();
                //}
                //else
                //{
                //    SetValueAsync(_selectedValues.LastOrDefault(), false).AndForget();
                //    UpdateTextPropertyAsync(false).AndForget();
                //}

                SelectedValuesChanged.InvokeAsync(new HashSet<T>(SelectedValues, _comparer)).AndForget();
                ForceUpdateItems().AndForgetExt();
                //Console.WriteLine("SelectedValues setter ended");
            }
        }

        private MudComboboxItem<T> _selectedItem;
        private HashSet<MudComboboxItem<T>> _selectedItems;

        protected internal MudComboboxItem<T> SelectedItem
        {
            get => _selectedItem;

            set
            {
                if (_selectedItem == value)
                {
                    return;
                }
                _selectedItem = value;
            }
        }

        protected internal IEnumerable<MudComboboxItem<T>> SelectedItems
        {
            get => _selectedItems;

            set
            {
                if (value == null && _selectedItems == null)
                {
                    return;
                }

                if (value != null && _selectedItems != null && _selectedItems.SetEquals(value))
                {
                    return;
                }
                _selectedItems = value == null ? null : value.ToHashSet();
            }
        }

        /// <summary>
        /// Fires when SelectedValues changes.
        /// </summary>
        [Parameter] public EventCallback<IEnumerable<T>> SelectedValuesChanged { get; set; }

        /// <summary>
        /// Should only be used for debugging and development purposes.
        /// </summary>
        [Parameter] public EventCallback<IEnumerable<MudComboboxItem<T>>> SelectedItemsChanged { get; set; }

        //protected async Task SetCustomizedTextAsync(string text, bool updateValue = true,
        //    List<T> selectedConvertedValues = null,
        //    Func<List<T>, string> multiSelectionTextFunc = null)
        //{
        //    // The Text property of the control is updated
        //    Text = multiSelectionTextFunc?.Invoke(selectedConvertedValues);

        //    // The comparison is made on the multiSelectionText variable
        //    if (multiSelectionText != text)
        //    {
        //        multiSelectionText = text;
        //        if (!string.IsNullOrWhiteSpace(multiSelectionText))
        //            Touched = true;
        //        if (updateValue)
        //            await UpdateValuePropertyAsync(false);
        //        await TextChanged.InvokeAsync(multiSelectionText);
        //    }
        //}

        protected Task UpdateDataVisualiserTextAsync()
        {
            List<string> textList = new List<string>();
            if (Items != null && Items.Any())
            {
                if (ItemCollection != null)
                {
                    foreach (var val in SelectedValues)
                    {
                        var collectionValue = ItemCollection.FirstOrDefault(x => x != null && (Comparer != null ? Comparer.Equals(x, val) : x.Equals(val)));
                        if (collectionValue != null)
                        {
                            textList.Add(Converter.Set(collectionValue));
                        }
                    }
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
                    _dataVisualiserText = MultiSelectionTextFunc.Invoke(SelectedValues.ToList());
                    //return SetCustomizedTextAsync(string.Join(Delimiter, textList),
                    //    selectedConvertedValues: SelectedValues.ToList(),
                    //    multiSelectionTextFunc: MultiSelectionTextFunc, updateValue: updateValue);
                    return Task.CompletedTask;
                }
                else
                {
                    //return SetTextAsync(string.Join(Delimiter, textList), updateValue: updateValue);
                    _dataVisualiserText = string.Join(Delimiter, textList);
                    return Task.CompletedTask;
                }
            }
            else
            {
                var item = Items.FirstOrDefault(x => Value == null ? x.Value == null : Comparer != null ? Comparer.Equals(Value, x.Value) : Value.Equals(x.Value));
                if (item == null)
                {
                    _dataVisualiserText = Converter.Set(Value);
                    //SetTextAsync(Converter.Set(Value), false);
                    return Task.CompletedTask;
                }
                //SetTextAsync((!string.IsNullOrEmpty(item.Text) ? item.Text : Converter.Set(item.Value)), updateValue: false);
                _dataVisualiserText = (!string.IsNullOrEmpty(item.Text) ? item.Text : Converter.Set(item.Value));
                return Task.CompletedTask;
            }
        }

        protected internal string _dataVisualiserText;

        private string GetSelectTextPresenter()
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

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            //UpdateIcon();
            _allSelected = GetAllSelectedState();
        }

        T _oldValue;
        bool _oldMultiselection = false;
        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();
            UpdateIcon();
            if (MultiSelection != _oldMultiselection)
            {
                await SyncMultiselectionValues(MultiSelection);
                await ForceRenderItems();
            }
            if ((Value == null && _oldValue != null) || (Value != null && Value.Equals(_oldValue) == false))
            {
                await ForceUpdateItems();
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
                await _keyInterceptor.Connect(_elementId, new KeyInterceptorOptions()
                {
                    //EnableLogging = true,
                    TargetClass = "mud-input-control",
                    Keys = {
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
                    },
                });
                _keyInterceptor.KeyDown += HandleKeyDown;
                _keyInterceptor.KeyUp += HandleKeyUp;
                await UpdateDataVisualiserTextAsync();
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
                }
                _keyInterceptor?.Dispose();
            }
        }

        #endregion


        #region Events (Key, Focus)

        protected internal async void HandleKeyDown(KeyboardEventArgs obj)
        {
            if (Disabled || ReadOnly)
                return;

            switch (obj.Key)
            {
                case "Tab":
                    await CloseMenu();
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
                    break;
                case " ":
                    await ToggleMenu();
                    break;
                case "Escape":
                    await CloseMenu();
                    break;
                case "Enter":
                case "NumpadEnter":
                    if (!MultiSelection)
                    {
                        if (!_isOpen)
                        {
                            await OpenMenu();
                        }
                        else
                        {
                            await CloseMenu();
                        }
                        break;
                    }
                    else
                    {
                        if (!_isOpen)
                        {
                            await OpenMenu();
                            break;
                        }
                        else
                        {
                            await _elementReference.SetText(Text);
                            break;
                        }
                    }
            }
            await OnKeyDown.InvokeAsync(obj);

        }

        protected internal async void HandleKeyUp(KeyboardEventArgs obj)
        {
            await OnKeyUp.InvokeAsync(obj);
        }

        protected internal async Task OnLostFocus(FocusEventArgs obj)
        {
            //if (_isOpen)
            //{
            //    // when the menu is open we immediately get back the focus if we lose it (i.e. because of checkboxes in multi-select)
            //    // otherwise we can't receive key strokes any longer
            //    _elementReference.FocusAsync().AndForget(TaskOption.Safe);
            //}
            //base.OnBlur.InvokeAsync(obj).AndForget();

            await OnBlurredAsync(obj);
        }

        public override ValueTask FocusAsync()
        {
            return _elementReference.FocusAsync();
        }

        public override ValueTask BlurAsync()
        {
            return _elementReference.BlurAsync();
        }

        public override ValueTask SelectAsync()
        {
            return _elementReference.SelectAsync();
        }

        public override ValueTask SelectRangeAsync(int pos1, int pos2)
        {
            return _elementReference.SelectRangeAsync(pos1, pos2);
        }

        #endregion


        #region PopoverState

        public async Task ToggleMenu()
        {
            if (Disabled || ReadOnly)
                return;
            if (_isOpen)
                await CloseMenu();
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

            //disable escape propagation: if selectmenu is open, only the select popover should close and underlying components should not handle escape key
            await _keyInterceptor.UpdateKey(new() { Key = "Escape", StopDown = "Key+none" });
            await OnOpen.InvokeAsync();
        }

        public async Task CloseMenu()
        {
            _isOpen = false;
            UpdateIcon();
            StateHasChanged();
            //if (focusAgain == true)
            //{
            //    StateHasChanged();
            //    await OnBlur.InvokeAsync(new FocusEventArgs());
            //    _elementReference.FocusAsync().AndForget(TaskOption.Safe);
            //    StateHasChanged();
            //}

            //enable escape propagation: the select popover was closed, now underlying components are allowed to handle escape key
            await _keyInterceptor.UpdateKey(new() { Key = "Escape", StopDown = "none" });

            await OnClose.InvokeAsync();
        }

        #endregion

        #region Item Registration & Selection

        protected internal async Task ToggleOption(MudComboboxItem<T> item, bool selected)
        {
            if (selected == false)
            {
                if (MultiSelection == false && Value.Equals(item.Value))
                {
                    if (ToggleSelection)
                    {
                        await SetValueAsync(default(T));
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
                    await SetValueAsync(item.Value, false);
                    
                }
                else if (SelectedValues.Contains(item.Value) == false)
                {
                    await SetValueAsync(item.Value, false);
                    SelectedValues = SelectedValues.Append(item.Value);
                    await SelectedValuesChanged.InvokeAsync(_selectedValues);
                    _allSelected = GetAllSelectedState();
                }
                item.Selected = true;
            }
            await UpdateDataVisualiserTextAsync();
            if (MultiSelection == false)
            {
                await CloseMenu();
            }
            else
            {
                await FocusAsync();
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

        public async Task BeginValidatePublic()
        {
            await BeginValidateAsync();
        }

        protected internal bool? Add(MudComboboxItem<T> item)
        {
            if (item == null)
                return false;
            bool? result = null;
            if (Items?.Select(x => x.Value).Contains(item.Value) == false)
            {
                Items.Add(item);
                result = true;
            }
            return result;
        }

        protected internal void Remove(MudComboboxItem<T> item)
        {
            if (Items == null)
            {
                return;
            }
            //Items.Remove(Items.FirstOrDefault(x => x.Value.Equals(item.Value)));
            Items.Remove(item);
        }

        #endregion


        #region Clear

        /// <summary>
        /// Extra handler for clearing selection.
        /// </summary>
        protected async ValueTask ClearButtonClickHandlerAsync(MouseEventArgs e)
        {
            await SetValueAsync(default, false);
            await SetTextAsync(default, false);
            _selectedValues.Clear();
            SelectedItem = null;
            SelectedItems = null;
            DeselectAllItems();
            await BeginValidateAsync();
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

        protected bool IsValueInList
        {
            get
            {
                if (Value == null)
                    return false;
                //return _shadowLookup.TryGetValue(Value, out var _);
                foreach (var value in Items.Select(x => x.Value))
                {
                    if (Comparer != null ? Comparer.Equals(value, Value) : value.Equals(Value)) //(Converter.Set(item.Value) == Converter.Set(Value))
                    {
                        return true;
                    }
                }
                return false;
            }
        }

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

        /// <summary>
        /// Fixes issue #4328
        /// Returns true when MultiSelection is true and it has selected values(Since Value property is not used when MultiSelection=true
        /// </summary>
        /// <param name="value"></param>
        /// <returns>True when component has a value</returns>
        protected override bool HasValue(T value)
        {
            if (MultiSelection)
                return SelectedValues?.Count() > 0;
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
                foreach (var item in Items)
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
                foreach (var item in Items)
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
            if (MultiSelection == true && SelectedValues.Count() == Items.Count)
            {
                return true;
            }

            if (MultiSelection == true && SelectedValues.Count() == 0)
            {
                return false;
            }

            return null;
        }

        protected async Task ForceRenderItems()
        {
            Items.ForEach(async(x) => await x.ForceRender());
        }

        protected async Task ForceUpdateItems()
        {
            Items.ForEach(async (x) => await x.ForceUpdate());
        }

    }
}
