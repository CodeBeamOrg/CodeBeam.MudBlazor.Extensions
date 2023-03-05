using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using MudBlazor.Services;
using MudBlazor.Utilities;
using MudExtensions.Enums;
using MudExtensions.Services;
using static MudBlazor.CategoryTypes;
using static MudBlazor.Colors;

namespace MudExtensions
{
    public partial class MudListExtended<T> : MudComponentBase, IDisposable
    {
        #region Parameters, Fields, Injected Services

        [Inject] IKeyInterceptorFactory KeyInterceptorFactory { get; set; }
        [Inject] IScrollManagerExtended ScrollManagerExtended { get; set; }

        // Fields used in more than one place (or protected and internal ones) are shown here.
        // Others are next to the relevant parameters. (Like _selectedValue)
        private string _elementId = "list_" + Guid.NewGuid().ToString().Substring(0, 8);
        private List<MudListItemExtended<T>> _items = new();
        private List<MudListExtended<T>> _childLists = new();
        internal MudListItemExtended<T> _lastActivatedItem;
        internal bool? _allSelected = false;
        private string _searchString;

        protected string Classname =>
        new CssBuilder("mud-list-extended")
           .AddClass("mud-list-padding-extended", !DisablePadding)
          .AddClass(Class)
        .Build();

        protected string Stylename =>
        new StyleBuilder()
            .AddStyle("max-height", $"{MaxItems * (!Dense ? 48 : 36) + (DisablePadding ? 0 : 16)}px", MaxItems != null)
            .AddStyle("overflow-y", "auto", MaxItems != null)
            .AddStyle(Style)
            .Build();

        [CascadingParameter] protected MudSelectExtended<T> MudSelectExtended { get; set; }
        [CascadingParameter] protected MudAutocomplete<T> MudAutocomplete { get; set; }
        [CascadingParameter] protected MudListExtended<T> ParentList { get; set; }

        /// <summary>
        /// The color of the selected List Item.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Appearance)]
        public Color Color { get; set; } = Color.Primary;

        /// <summary>
        /// Child content of component.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Behavior)]
        public RenderFragment ChildContent { get; set; }

        /// <summary>
        /// Optional presentation template for items
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListBehavior)]
        public RenderFragment<MudListItemExtended<T>> ItemTemplate { get; set; }

        /// <summary>
        /// Optional presentation template for selected items
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListBehavior)]
        public RenderFragment<MudListItemExtended<T>> ItemSelectedTemplate { get; set; }

        /// <summary>
        /// Optional presentation template for disabled items
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListBehavior)]
        public RenderFragment<MudListItemExtended<T>> ItemDisabledTemplate { get; set; }

        /// <summary>
        /// Optional presentation template for select all item
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListBehavior)]
        public RenderFragment SelectAllTemplate { get; set; }

        [Parameter]
        [Category(CategoryTypes.List.Behavior)]
        public DefaultConverter<T> Converter { get; set; } = new DefaultConverter<T>();

        private IEqualityComparer<T> _comparer;
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
                if (_selectedValues == null)
                {
                    return;
                }
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
                Converter = new DefaultConverter<T>
                {
                    SetFunc = _toStringFunc ?? (x => x?.ToString()),
                };
            }
        }

        /// <summary>
        /// Predefined enumerable items. If its not null, creates list items automatically.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Behavior)]
        public ICollection<T> ItemCollection { get; set; } = null;

        /// <summary>
        /// Custom search func for searchbox. If doesn't set, it search with "Contains" logic by default. Passed value and searchString values.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListBehavior)]
        public Func<T, string, bool> SearchFunc { get; set; }

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
        /// SearchBox's CSS classes, seperated by space.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Behavior)]
        public string ClassSearchBox { get; set; }

        [Parameter]
        [Category(CategoryTypes.List.Behavior)]
        public string SearchBoxPlaceholder { get; set; }

        /// <summary>
        /// Allows virtualization. Only work if ItemCollection parameter is not null.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Behavior)]
        public bool Virtualize { get; set; }

        /// <summary>
        /// Set max items to show in list. Other items can be scrolled. Works if list items populated with ItemCollection parameter.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Behavior)]
        public int? MaxItems { get; set; } = null;

        /// <summary>
        /// Overscan value for Virtualized list. Default is 2.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Behavior)]
        public int OverscanCount { get; set; } = 2;

        private bool _multiSelection = false;
        /// <summary>
        /// Allows multi selection and adds MultiSelectionComponent for each list item.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Behavior)]
        public bool MultiSelection
        {
            get => _multiSelection;

            set
            {
                if (ParentList != null)
                {
                    _multiSelection = ParentList.MultiSelection;
                    return;
                }
                if (_multiSelection == value)
                {
                    return;
                }
                _multiSelection = value;
                if (!_setParametersDone)
                {
                    return;
                }
                if (!_multiSelection)
                {
                    if (!_centralCommanderIsProcessing)
                    {
                        HandleCentralValueCommander("MultiSelectionOff");
                    }

                    UpdateSelectedStyles();
                }
            }
        }

        /// <summary>
        /// The MultiSelectionComponent's placement. Accepts Align.Start and Align.End
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Behavior)]
        public Align MultiSelectionAlign { get; set; } = Align.Start;

        /// <summary>
        /// The component which shows as a MultiSelection check.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Behavior)]
        public MultiSelectionComponent MultiSelectionComponent { get; set; } = MultiSelectionComponent.CheckBox;

        /// <summary>
        /// Set true to make the list items clickable. This is also the precondition for list selection to work.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Selecting)]
        public bool Clickable { get; set; }

        /// <summary>
        /// If true the active (hilighted) item select on tab key. Designed for only single selection. Default is true.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Selecting)]
        public bool SelectValueOnTab { get; set; } = true;

        /// <summary>
        /// If true, vertical padding will be removed from the list.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Appearance)]
        public bool DisablePadding { get; set; }

        /// <summary>
        /// If true, selected items doesn't have a selected background color.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Appearance)]
        public bool DisableSelectedItemStyle { get; set; }

        /// <summary>
        /// If true, compact vertical padding will be applied to all list items.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Appearance)]
        public bool Dense { get; set; }

        /// <summary>
        /// If true, the left and right padding is removed on all list items.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Appearance)]
        public bool DisableGutters { get; set; }

        /// <summary>
        /// If true, will disable the list item if it has onclick.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Behavior)]
        public bool Disabled { get; set; }

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
        /// If true, change background color to secondary for all nested item headers.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Appearance)]
        public bool SecondaryBackgroundForNestedItemHeader { get; set; }

        /// <summary>
        /// Fired on the KeyDown event.
        /// </summary>
        [Parameter] public EventCallback<KeyboardEventArgs> OnKeyDown { get; set; }

        /// <summary>
        /// Fired on the OnFocusOut event.
        /// </summary>
        [Parameter] public EventCallback<FocusEventArgs> OnFocusOut { get; set; }

        /// <summary>
        /// Fired on the OnDoubleClick event.
        /// </summary>
        [Parameter] public EventCallback<ListItemClickEventArgs<T>> OnDoubleClick { get; set; }

        #endregion


        #region Values & Items (Core: Be careful if you change something inside the region, it affects all logic and also Select and Autocomplete)

        bool _centralCommanderIsProcessing = false;
        bool _centralCommanderResultRendered = false;
        // CentralCommander has a simple aim: Prevent racing conditions. It has two mechanism to do this:
        // (1) When this method is running, it doesn't allow to run a second one. That guarantees to different value parameters can not call this method at the same time.
        // (2) When this method runs once, prevents all value setters until OnAfterRender runs. That guarantees to have proper values.
        protected void HandleCentralValueCommander(string changedValueType, bool updateStyles = true)
        {
            if (!_setParametersDone)
            {
                return;
            }
            if (_centralCommanderIsProcessing)
            {
                return;
            }
            _centralCommanderIsProcessing = true;

            if (changedValueType == nameof(SelectedValue))
            {
                if (!MultiSelection)
                {
                    SelectedValues = new HashSet<T>(_comparer) { SelectedValue };
                    UpdateSelectedItem();
                }
            }
            else if (changedValueType == nameof(SelectedValues))
            {
                if (MultiSelection)
                {
                    SelectedValue = SelectedValues == null ? default(T) : SelectedValues.LastOrDefault();
                    UpdateSelectedItem();
                }
            }
            else if (changedValueType == nameof(SelectedItem))
            {
                if (!MultiSelection)
                {
                    SelectedItems = new HashSet<MudListItemExtended<T>>() { SelectedItem };
                    UpdateSelectedValue();
                }
            }
            else if (changedValueType == nameof(SelectedItems))
            {
                if (MultiSelection)
                {
                    SelectedItem = SelectedItems == null ? null : SelectedItems.LastOrDefault();
                    UpdateSelectedValue();
                }
            }
            else if (changedValueType == "MultiSelectionOff")
            {
                SelectedValue = SelectedValues == null ? default(T) : SelectedValues.FirstOrDefault();
                var items = CollectAllMudListItems(true);
                SelectedValues = SelectedValue == null ? null : new HashSet<T>(_comparer) { SelectedValue };
                UpdateSelectedItem();
            }

            _centralCommanderResultRendered = false;
            _centralCommanderIsProcessing = false;
            if (updateStyles)
            {
                UpdateSelectedStyles();
            }
        }

        protected internal void UpdateSelectedItem()
        {
            var items = CollectAllMudListItems(true);

            if (MultiSelection && (SelectedValues == null || SelectedValues.Count() == 0))
            {
                SelectedItem = null;
                SelectedItems = null;
                return;
            }

            SelectedItem = items.FirstOrDefault(x => SelectedValue == null ? x.Value == null : Comparer != null ? Comparer.Equals(x.Value, SelectedValue) : x.Value.Equals(SelectedValue));
            SelectedItems = SelectedValues == null ? null : items.Where(x => SelectedValues.Contains(x.Value, _comparer));
        }

        protected internal void UpdateSelectedValue()
        {
            if (!MultiSelection && SelectedItem == null)
            {
                SelectedValue = default(T);
                SelectedValues = null;
                return;
            }

            SelectedValue = SelectedItem == null ? default(T) : SelectedItem.Value;
            SelectedValues = SelectedItems.Select(x => x.Value).ToHashSet(_comparer);
        }

        private T _selectedValue;
        /// <summary>
        /// The current selected value.
        /// Note: Make the list Clickable or set MultiSelection true for item selection to work.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Selecting)]
        public T SelectedValue
        {
            get => _selectedValue;
            set
            {
                if (Converter.Set(_selectedValue) != Converter.Set(default(T)) && !_firstRendered)
                {
                    return;
                }
                if (!_centralCommanderResultRendered && _firstRendered)
                {
                    return;
                }
                if (ParentList != null)
                {
                    return;
                }
                if ((_selectedValue != null && value != null && _selectedValue.Equals(value)) || (_selectedValue == null && value == null))
                {
                    return;
                }

                _selectedValue = value;
                HandleCentralValueCommander(nameof(SelectedValue));
                SelectedValueChanged.InvokeAsync(_selectedValue).AndForget();
            }
        }

        private HashSet<T> _selectedValues;
        /// <summary>
        /// The current selected values. Holds single value (SelectedValue) if MultiSelection is false.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Selecting)]
        public IEnumerable<T> SelectedValues
        {
            get
            {
                return _selectedValues;
            }

            set
            {
                if (value == null && !_firstRendered)
                {
                    return;
                }
                if (!_centralCommanderResultRendered && _firstRendered)
                {
                    return;
                }
                if (ParentList != null)
                {
                    return;
                }
                //var set = value ?? new List<T>();
                if (value == null && _selectedValues == null)
                {
                    return;
                }

                if (value != null && _selectedValues != null && _selectedValues.SetEquals(value))
                {
                    return;
                }
                // This return condition(s) can be discussed. It is also important when we add experimental select, because commenting one more return condition causes infinite loops.
                //if (SelectedValues.Count() == set.Count() && _selectedValues != null && _selectedValues.All(x => set.Contains(x)))
                //{
                //    return;
                //}

                _selectedValues = value == null ? null : new HashSet<T>(value, _comparer);
                if (!_setParametersDone)
                {
                    return;
                }
                HandleCentralValueCommander(nameof(SelectedValues));
                SelectedValuesChanged.InvokeAsync(SelectedValues == null ? null : new HashSet<T>(SelectedValues, _comparer)).AndForget();
            }
        }

        private MudListItemExtended<T> _selectedItem;
        /// <summary>
        /// The current selected list item.
        /// Note: make the list Clickable or MultiSelection or both for item selection to work.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Selecting)]
        public MudListItemExtended<T> SelectedItem
        {
            get => _selectedItem;
            set
            {
                if (!_centralCommanderResultRendered && _firstRendered)
                {
                    return;
                }
                if (_selectedItem == value)
                    return;

                _selectedItem = value;
                if (!_setParametersDone)
                {
                    return;
                }
                HandleCentralValueCommander(nameof(SelectedItem));
                SelectedItemChanged.InvokeAsync(_selectedItem).AndForget();
            }
        }

        private HashSet<MudListItemExtended<T>> _selectedItems;
        /// <summary>
        /// The current selected listitems.
        /// Note: make the list Clickable for item selection to work.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Selecting)]
        public IEnumerable<MudListItemExtended<T>> SelectedItems
        {
            get => _selectedItems;
            set
            {
                if (!_centralCommanderResultRendered && _firstRendered)
                {
                    return;
                }

                if (value == null && _selectedItems == null)
                {
                    return;
                }

                if (value != null && _selectedItems != null && _selectedItems.SetEquals(value))
                    return;

                _selectedItems = value == null ? null : value.ToHashSet();
                if (!_setParametersDone)
                {
                    return;
                }
                HandleCentralValueCommander(nameof(SelectedItems));
                SelectedItemsChanged.InvokeAsync(_selectedItems).AndForget();
            }
        }

        /// <summary>
        /// Called whenever the selection changed. Can also be called even MultiSelection is true.
        /// </summary>
        [Parameter] public EventCallback<T> SelectedValueChanged { get; set; }

        /// <summary>
        /// Called whenever selected values changes. Can also be called even MultiSelection is false.
        /// </summary>
        [Parameter] public EventCallback<IEnumerable<T>> SelectedValuesChanged { get; set; }

        /// <summary>
        /// Called whenever the selected item changed. Can also be called even MultiSelection is true.
        /// </summary>
        [Parameter] public EventCallback<MudListItemExtended<T>> SelectedItemChanged { get; set; }

        /// <summary>
        /// Called whenever the selected items changed. Can also be called even MultiSelection is false.
        /// </summary>
        [Parameter] public EventCallback<IEnumerable<MudListItemExtended<T>>> SelectedItemsChanged { get; set; }

        /// <summary>
        /// Get all MudListItems in the list.
        /// </summary>
        public List<MudListItemExtended<T>> GetAllItems()
        {
            return CollectAllMudListItems();
        }

        /// <summary>
        /// Get all items that holds value.
        /// </summary>
        public List<MudListItemExtended<T>> GetItems()
        {
            return CollectAllMudListItems(true);
        }

        #endregion


        #region Lifecycle Methods, Dispose & Register

        bool _setParametersDone = false;
        public override Task SetParametersAsync(ParameterView parameters)
        {
            if (_centralCommanderIsProcessing)
            {
                return Task.CompletedTask;
            }

            if (MudSelectExtended != null || MudAutocomplete != null)
            {
                return Task.CompletedTask;
            }

            base.SetParametersAsync(parameters).AndForget();

            _setParametersDone = true;
            return Task.CompletedTask;
        }

        protected override void OnInitialized()
        {
            if (ParentList != null)
            {
                ParentList.Register(this);
            }
        }

        internal event Action ParametersChanged;
        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            ParametersChanged?.Invoke();
        }

        private IKeyInterceptor _keyInterceptor;
        private bool _firstRendered = false;
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
            if (firstRender)
            {
                _firstRendered = false;
                _keyInterceptor = KeyInterceptorFactory.Create();

                await _keyInterceptor.Connect(_elementId, new KeyInterceptorOptions()
                {
                    //EnableLogging = true,
                    TargetClass = "mud-list-item-extended",
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

                if (MudSelectExtended == null && MudAutocomplete == null)
                {
                    if (!MultiSelection && SelectedValue != null)
                    {
                        HandleCentralValueCommander(nameof(SelectedValue));
                    }
                    else if (MultiSelection && SelectedValues != null)
                    {
                        HandleCentralValueCommander(nameof(SelectedValues));
                    }
                }

                if (MudSelectExtended != null || MudAutocomplete != null)
                {
                    if (MultiSelection)
                    {
                        UpdateSelectAllState();
                        if (MudSelectExtended != null)
                        {
                            SelectedValues = MudSelectExtended.SelectedValues;
                        }
                        else if (MudAutocomplete != null)
                        {
                            // Uncomment on Autocomplete Phase. Currently autocomplete doesn't have "SelectedValues".
                            //SelectedValues = MudAutocomplete.SelectedValues;
                        }
                        HandleCentralValueCommander(nameof(SelectedValues));
                    }
                    else
                    {
                        // These updated style method cause to fail some tests after adding select phase. Can be discuss later.
                        //UpdateSelectedStyles();
                        UpdateLastActivatedItem(SelectedValue);
                    }
                }
                if (SelectedValues != null)
                {
                    UpdateLastActivatedItem(SelectedValues.LastOrDefault());
                }
                if (_lastActivatedItem != null && !(MultiSelection && _allSelected == true))
                {
                    await ScrollToMiddleAsync(_lastActivatedItem);
                }
                _firstRendered = true;
            }

            _centralCommanderResultRendered = true;
        }

        public void Dispose()
        {
            ParametersChanged = null;
            ParentList?.Unregister(this);
        }

        protected internal void Register(MudListItemExtended<T> item)
        {
            _items.Add(item);
            if (SelectedValue != null && object.Equals(item.Value, SelectedValue))
            {
                item.SetSelected(true);
                //TODO check if item is the selectable for a nested list, and deselect this.
                //SelectedItem = item;
                //SelectedItemChanged.InvokeAsync(item);
            }
        }

        protected internal void Unregister(MudListItemExtended<T> item)
        {
            _items.Remove(item);
        }

        protected internal void Register(MudListExtended<T> child)
        {
            _childLists.Add(child);
        }

        protected internal void Unregister(MudListExtended<T> child)
        {
            _childLists.Remove(child);
        }

        #endregion


        #region Events (Key, Focus)

        protected internal async Task SearchBoxHandleKeyDown(KeyboardEventArgs obj)
        {
            if (Disabled || (!Clickable && !MultiSelection))
                return;
            switch (obj.Key)
            {
                case " ":
                    _searchString = _searchString + " ";
                    await _searchField.BlurAsync();
                    await _searchField.FocusAsync();
                    StateHasChanged();
                    break;
            }
        }

        MudTextField<string> _searchField;
        protected internal async Task HandleKeyDown(KeyboardEventArgs obj)
        {
            if (Disabled || (!Clickable && !MultiSelection))
                return;
            if (ParentList != null)
            {
                return;
            }

            var key = obj.Key.ToLowerInvariant();
            if (key.Length == 1 && key != " " && !(obj.CtrlKey || obj.ShiftKey || obj.AltKey || obj.MetaKey))
            {
                await ActiveFirstItem(key);
                return;
            }
            switch (obj.Key)
            {
                case "Tab":
                    if (!MultiSelection && SelectValueOnTab)
                    {
                        SetSelectedValue(_lastActivatedItem);
                    }
                    break;
                case "ArrowUp":
                    await ActiveAdjacentItem(-1);
                    break;
                case "ArrowDown":
                    await ActiveAdjacentItem(1);
                    break;
                case "Home":
                    await ActiveFirstItem();
                    break;
                case "End":
                    await ActiveLastItem();
                    break;
                case "Enter":
                case "NumpadEnter":
                    if (_lastActivatedItem == null)
                    {
                        return;
                    }
                    SetSelectedValue(_lastActivatedItem);
                    break;
                case "a":
                case "A":
                    if (obj.CtrlKey)
                    {
                        if (MultiSelection)
                        {
                            SelectAllItems(_allSelected);
                        }
                    }
                    break;
                case "f":
                case "F":
                    if (obj.CtrlKey == true && obj.ShiftKey == true)
                    {
                        SearchBox = !SearchBox;
                        StateHasChanged();
                    }
                    break;
            }
            await OnKeyDown.InvokeAsync(obj);
        }

        protected async Task HandleOnFocusOut()
        {
            DeactiveAllItems();
            await OnFocusOut.InvokeAsync();
        }

        protected void HandleOnScroll()
        {
            if (Virtualize == true)
            {
                UpdateSelectedStyles();
            }
        }

        #endregion


        #region Select

        protected internal void SetSelectedValue(T value, bool force = false)
        {
            if ((!Clickable && !MultiSelection) && !force)
                return;

            //Make sure its the most parent one before continue method.
            if (ParentList != null)
            {
                ParentList?.SetSelectedValue(value);
                return;
            }

            if (!MultiSelection)
            {
                SelectedValue = value;
            }
            else
            {
                if (SelectedValues.Contains(value, _comparer))
                {
                    SelectedValues = SelectedValues?.Where(x => Comparer != null ? !Comparer.Equals(x, value) : !x.Equals(value)).ToHashSet(_comparer);
                }
                else
                {
                    SelectedValues = SelectedValues.Append(value).ToHashSet(_comparer);
                }
            }
            UpdateLastActivatedItem(value);
        }

        protected internal void SetSelectedValue(MudListItemExtended<T> item, bool force = false)
        {
            if (item == null)
            {
                return;
            }

            if ((!Clickable && !MultiSelection) && !force)
                return;

            //Make sure its the most parent one before continue method
            if (ParentList != null)
            {
                ParentList?.SetSelectedValue(item);
                return;
            }

            if (!MultiSelection)
            {
                SelectedValue = item.Value;
            }
            else
            {
                if (item.IsSelected)
                {
                    SelectedValues = SelectedValues?.Where(x => Comparer != null ? !Comparer.Equals(x, item.Value) : !x.Equals(item.Value));
                }
                else
                {
                    if (SelectedValues == null)
                    {
                        SelectedValues = new HashSet<T>(_comparer) { item.Value };
                    }
                    else
                    {
                        SelectedValues = SelectedValues.Append(item.Value).ToHashSet(_comparer);
                    }
                }
            }

            UpdateSelectAllState();
            _lastActivatedItem = item;
        }

        protected internal void UpdateSelectedStyles(bool deselectFirst = true, bool update = true)
        {
            var items = CollectAllMudListItems(true);
            if (deselectFirst)
            {
                DeselectAllItems(items);
            }

            if (!IsSelectable())
            {
                return;
            }

            if (!MultiSelection)
            {
                items.FirstOrDefault(x => SelectedValue == null ? x.Value == null : SelectedValue.Equals(x == null ? null : x.Value))?.SetSelected(true);
            }
            else if (SelectedValues != null)
            {
                items.Where(x => SelectedValues.Contains(x.Value, Comparer == null ? null : Comparer)).ToList().ForEach(x => x.SetSelected(true));
            }

            if (update)
            {
                StateHasChanged();
            }
        }

        protected bool IsSelectable()
        {
            if (Clickable || MultiSelection)
            {
                return true;
            }

            return false;
        }

        protected void DeselectAllItems(List<MudListItemExtended<T>> items)
        {
            foreach (var listItem in items)
                listItem?.SetSelected(false);
        }

        protected List<MudListItemExtended<T>> CollectAllMudListItems(bool exceptNestedAndExceptional = false)
        {
            var items = new List<MudListItemExtended<T>>();

            if (ParentList != null)
            {
                items.AddRange(ParentList._items);
                foreach (var list in ParentList._childLists)
                    items.AddRange(list._items);
            }
            else
            {
                items.AddRange(_items);
                foreach (var list in _childLists)
                    items.AddRange(list._items);
            }

            if (!exceptNestedAndExceptional)
            {
                return items;
            }
            else
            {
                return items.Where(x => x.NestedList == null && !x.IsFunctional).ToList();
            }
        }

        #endregion


        #region SelectAll

        protected internal void UpdateSelectAllState()
        {
            if (MultiSelection)
            {
                var oldState = _allSelected;
                if (_selectedValues == null || !_selectedValues.Any())
                {
                    _allSelected = false;
                }
                else if (CollectAllMudListItems(true).Count() == _selectedValues.Count)
                {
                    _allSelected = true;
                }
                else
                {
                    _allSelected = null;
                }
            }
        }

        protected string SelectAllCheckBoxIcon
        {
            get
            {
                return _allSelected.HasValue ? _allSelected.Value ? CheckedIcon : UncheckedIcon : IndeterminateIcon;
            }
        }

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

        protected void SelectAllItems(bool? deselect = false)
        {
            var items = CollectAllMudListItems(true);
            if (deselect == true)
            {
                foreach (var item in items)
                {
                    if (item.IsSelected)
                    {
                        item.SetSelected(false);
                    }
                }
                _allSelected = false;
            }
            else
            {
                foreach (var item in items)
                {
                    if (!item.IsSelected)
                    {
                        item.SetSelected(true);
                    }
                }
                _allSelected = true;
            }

            SelectedValues = items.Where(x => x.IsSelected).Select(y => y.Value).ToHashSet(_comparer);
            if (MudSelectExtended != null)
            {
                MudSelectExtended.BeginValidatePublic();
            }
        }

        #endregion


        #region Active (Hilight)

        protected int GetActiveItemIndex()
        {
            var items = CollectAllMudListItems(true);
            if (_lastActivatedItem == null)
            {
                var a = items.FindIndex(x => x.IsActive);
                return a;
            }
            else
            {
                var a = items.FindIndex(x => _lastActivatedItem.Value == null ? x.Value == null : Comparer != null ? Comparer.Equals(_lastActivatedItem.Value, x.Value) : _lastActivatedItem.Value.Equals(x.Value));
                return a;
            }
        }

        protected T GetActiveItemValue()
        {
            var items = CollectAllMudListItems(true);
            if (_lastActivatedItem == null)
            {
                return items.FirstOrDefault(x => x.IsActive).Value;
            }
            else
            {
                return _lastActivatedItem.Value;
            }
        }

        protected internal void UpdateLastActivatedItem(T value)
        {
            var items = CollectAllMudListItems(true);
            _lastActivatedItem = items.FirstOrDefault(x => value == null ? x.Value == null : Comparer != null ? Comparer.Equals(value, x.Value) : value.Equals(x.Value));
        }

        protected void DeactiveAllItems(List<MudListItemExtended<T>> items = null)
        {
            if (items == null)
            {
                items = CollectAllMudListItems(true);
            }

            foreach (var item in items)
            {
                item.SetActive(false);
            }
        }

#pragma warning disable BL0005
        public async Task ActiveFirstItem(string startChar = null)
        {
            var items = CollectAllMudListItems(true);
            if (items == null || items.Count == 0 || items[0].Disabled)
            {
                return;
            }
            DeactiveAllItems(items);

            if (string.IsNullOrWhiteSpace(startChar))
            {
                items[0].SetActive(true);
                _lastActivatedItem = items[0];
                if (items[0].ParentListItem != null && !items[0].ParentListItem.Expanded)
                {
                    items[0].ParentListItem.Expanded = true;
                }
                await ScrollToMiddleAsync(items[0]);
                return;
            }

            // find first item that starts with the letter
            var possibleItems = items.Where(x => (x.Text ?? Converter.Set(x.Value) ?? "").StartsWith(startChar, StringComparison.CurrentCultureIgnoreCase)).ToList();
            if (possibleItems == null || !possibleItems.Any())
            {
                if (_lastActivatedItem == null)
                {
                    return;
                }
                _lastActivatedItem.SetActive(true);
                if (_lastActivatedItem.ParentListItem != null && !_lastActivatedItem.ParentListItem.Expanded)
                {
                    _lastActivatedItem.ParentListItem.Expanded = true;
                }
                await ScrollToMiddleAsync(_lastActivatedItem);
                return;
            }

            var theItem = possibleItems.FirstOrDefault(x => x == _lastActivatedItem);
            if (theItem == null)
            {
                possibleItems[0].SetActive(true);
                _lastActivatedItem = possibleItems[0];
                if (_lastActivatedItem.ParentListItem != null && !_lastActivatedItem.ParentListItem.Expanded)
                {
                    _lastActivatedItem.ParentListItem.Expanded = true;
                }
                await ScrollToMiddleAsync(possibleItems[0]);
                return;
            }

            if (theItem == possibleItems.LastOrDefault())
            {
                possibleItems[0].SetActive(true);
                _lastActivatedItem = possibleItems[0];
                if (_lastActivatedItem.ParentListItem != null && !_lastActivatedItem.ParentListItem.Expanded)
                {
                    _lastActivatedItem.ParentListItem.Expanded = true;
                }
                await ScrollToMiddleAsync(possibleItems[0]);
            }
            else
            {
                var item = possibleItems[possibleItems.IndexOf(theItem) + 1];
                item.SetActive(true);
                _lastActivatedItem = item;
                if (_lastActivatedItem.ParentListItem != null && !_lastActivatedItem.ParentListItem.Expanded)
                {
                    _lastActivatedItem.ParentListItem.Expanded = true;
                }
                await ScrollToMiddleAsync(item);
            }
        }

        public async Task ActiveAdjacentItem(int changeCount)
        {
            var items = CollectAllMudListItems(true);
            if (items == null || items.Count == 0)
            {
                return;
            }
            int index = GetActiveItemIndex();
            if (index + changeCount >= items.Count || 0 > index + changeCount)
            {
                return;
            }
            if (items[index + changeCount].Disabled)
            {
                // Recursive
                await ActiveAdjacentItem(changeCount > 0 ? changeCount + 1 : changeCount - 1);
                return;
            }
            DeactiveAllItems(items);
            items[index + changeCount].SetActive(true);
            _lastActivatedItem = items[index + changeCount];

            if (items[index + changeCount].ParentListItem != null && !items[index + changeCount].ParentListItem.Expanded)
            {
                items[index + changeCount].ParentListItem.Expanded = true;
            }

            await ScrollToMiddleAsync(items[index + changeCount]);
        }

        public async Task ActivePreviousItem()
        {
            var items = CollectAllMudListItems(true);
            if (items == null || items.Count == 0)
            {
                return;
            }
            int index = GetActiveItemIndex();
            if (0 > index - 1)
            {
                return;
            }
            DeactiveAllItems(items);
            items[index - 1].SetActive(true);
            _lastActivatedItem = items[index - 1];

            if (items[index - 1].ParentListItem != null && !items[index - 1].ParentListItem.Expanded)
            {
                items[index - 1].ParentListItem.Expanded = true;
            }

            await ScrollToMiddleAsync(items[index - 1]);
        }

        public async Task ActiveLastItem()
        {
            var items = CollectAllMudListItems(true);
            if (items == null || items.Count == 0)
            {
                return;
            }
            var properLastIndex = items.Count - 1;
            DeactiveAllItems(items);
            for (int i = 0; i < items.Count; i++)
            {
                if (!items[properLastIndex - i].Disabled)
                {
                    properLastIndex -= i;
                    break;
                }
            }
            items[properLastIndex].SetActive(true);
            _lastActivatedItem = items[properLastIndex];

            if (items[properLastIndex].ParentListItem != null && !items[properLastIndex].ParentListItem.Expanded)
            {
                items[properLastIndex].ParentListItem.Expanded = true;
            }

            await ScrollToMiddleAsync(items[properLastIndex]);
        }
#pragma warning restore BL0005

        #endregion


        #region Others (Clear, Scroll, Search)

        /// <summary>
        /// Clears value(s) and item(s) and deactive all items.
        /// </summary>
        public void Clear()
        {
            var items = CollectAllMudListItems();
            if (!MultiSelection)
            {
                SelectedValue = default(T);
            }
            else
            {
                SelectedValues = null;
            }

            DeselectAllItems(items);
            DeactiveAllItems();
            UpdateSelectAllState();
        }

        protected internal ValueTask ScrollToMiddleAsync(MudListItemExtended<T> item)
            => ScrollManagerExtended.ScrollToMiddleAsync(_elementId, item.ItemId);

        protected ICollection<T> GetSearchedItems()
        {
            if (!SearchBox || ItemCollection == null || _searchString == null)
            {
                return ItemCollection;
            }

            if (SearchFunc != null)
            {
                return ItemCollection.Where(x => SearchFunc.Invoke(x, _searchString)).ToList();
            }

            return ItemCollection.Where(x => Converter.Set(x).Contains(_searchString, StringComparison.CurrentCultureIgnoreCase)).ToList();
        }

        public async Task ForceUpdate()
        {
            //if (!MultiSelection)
            //{
            //    SelectedValues = new HashSet<T>(_comparer) { SelectedValue };
            //}
            //else
            //{
            //    await SelectedValuesChanged.InvokeAsync(new HashSet<T>(SelectedValues, _comparer));
            //}
            await Task.Delay(1);
            UpdateSelectedStyles();
        }

        protected async Task OnDoubleClickHandler(MouseEventArgs args, T itemValue)
        {
            await OnDoubleClick.InvokeAsync(new ListItemClickEventArgs<T>() { MouseEventArgs = args, ItemValue = itemValue });
        }

        #endregion
    }

    public class ListItemClickEventArgs<T>
    {
        public MouseEventArgs MouseEventArgs { get; set; }
        public T ItemValue { get; set; }
    }
}
