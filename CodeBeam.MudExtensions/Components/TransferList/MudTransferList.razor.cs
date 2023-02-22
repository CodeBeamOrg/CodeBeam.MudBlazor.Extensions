﻿using Microsoft.AspNetCore.Components;
using MudBlazor;
using MudBlazor.Utilities;
using MudExtensions.Enums;

namespace MudExtensions
{
    public partial class MudTransferList<T> : MudComponentBase
    {
        MudListExtended<T> _startList;
        MudListExtended<T> _endList;

        protected string StartClassname => new CssBuilder()
            .AddClass(ClassListCommon)
            .AddClass(ClassStartList)
            .Build();

        protected string EndClassname => new CssBuilder()
            .AddClass(ClassListCommon)
            .AddClass(ClassEndList)
            .Build();

        protected string StartStylename => new StyleBuilder()
            .AddStyle(StyleListCommon)
            .AddStyle(StyleStartList)
            .Build();

        protected string EndStylename => new StyleBuilder()
            .AddStyle(StyleListCommon)
            .AddStyle(StyleEndList)
            .Build();

        /// <summary>
        /// The start list's collection.
        /// </summary>
        [Parameter]
        public ICollection<T> StartCollection { get; set; }

        /// <summary>
        /// Fires when start collection changed.
        /// </summary>
        [Parameter]
        public EventCallback<ICollection<T>> StartCollectionChanged { get; set; }

        /// <summary>
        /// The end list's collection.
        /// </summary>
        [Parameter]
        public ICollection<T> EndCollection { get; set; }

        /// <summary>
        /// Fires when end collection changed.
        /// </summary>
        [Parameter]
        public EventCallback<ICollection<T>> EndCollectionChanged { get; set; }

        /// <summary>
        /// Fires before transfer process start. Useful to backup items or prevent transfer.
        /// </summary>
        [Parameter]
        public EventCallback OnTransfer { get; set; }

        /// <summary>
        /// Fires when start collection changed. Takes a "StartToEnd" direction bool parameter.
        /// </summary>
        [Parameter]
        public Func<bool, bool> PreventTransfer { get; set; }

        [Parameter]
        public bool Vertical { get; set; }

        /// <summary>
        /// Allows the transfer multiple items at once.
        /// </summary>
        [Parameter]
        public bool MultiSelection { get; set; }

        [Parameter]
        public MultiSelectionComponent MultiSelectionComponent { get; set; } = MultiSelectionComponent.CheckBox;

        /// <summary>
        /// Select all types. If button is selected, 2 transfer all button appears. If Selectall item is selected, a list item appears.
        /// </summary>
        [Parameter]
        public SelectAllType SelectAllType { get; set; } = SelectAllType.Buttons;

        /// <summary>
        /// The color of lists and buttons. Default is primary.
        /// </summary>
        [Parameter]
        public Color Color { get; set; } = Color.Primary;

        /// <summary>
        /// The variant of buttons.
        /// </summary>
        [Parameter]
        public Variant ButtonVariant { get; set; } = Variant.Text;

        [Parameter]
        public string SelectAllText { get; set; } = "Select All";

        [Parameter]
        public int Spacing { get; set; } = 4;

        [Parameter]
        public int ButtonSpacing { get; set; } = 1;

        [Parameter]
        public int? MaxItems { get; set; }

        [Parameter]
        public string ClassStartList { get; set; }

        [Parameter]
        public string ClassEndList { get; set; }

        [Parameter]
        public string ClassListCommon { get; set; }

        [Parameter]
        public string StyleStartList { get; set; }

        [Parameter]
        public string StyleEndList { get; set; }

        [Parameter]
        public string StyleListCommon { get; set; }

        protected internal async Task Transfer(bool startToEnd = true)
        {
            await OnTransfer.InvokeAsync();
            if (PreventTransfer != null && PreventTransfer.Invoke(startToEnd) == true)
            {
                return;
            }
            if (startToEnd == true)
            {
                if (MultiSelection == false && _startList.SelectedValue != null)
                {
                    EndCollection.Add(_startList.SelectedValue);
                    StartCollection.Remove(_startList.SelectedValue);
                    await EndCollectionChanged.InvokeAsync(EndCollection);
                    await StartCollectionChanged.InvokeAsync(StartCollection);
                    _endList.SelectedValue = _startList.SelectedValue;
                    _startList.Clear();
                }
                else if (MultiSelection == true && _startList.SelectedValues != null)
                {
                    ICollection<T> transferredValues = new List<T>();
                    foreach (var item in _startList.SelectedValues)
                    {
                        // This is not a great fix, but changing multiselection true after transfering a single selection item causes a null item transfer.
                        if (item == null)
                        {
                            continue;
                        }
                        EndCollection.Add(item);
                        StartCollection.Remove(item);
                        transferredValues.Add(item);
                    }
                    _endList.SelectedValues = transferredValues;
                    await _endList.ForceUpdate();
                    _startList.Clear();
                    await EndCollectionChanged.InvokeAsync(EndCollection);
                    await StartCollectionChanged.InvokeAsync(StartCollection);
                }
                
            }
            else if (startToEnd == false)
            {
                if (MultiSelection == false && _endList.SelectedValue != null)
                {
                    StartCollection.Add(_endList.SelectedValue);
                    EndCollection.Remove(_endList.SelectedValue);
                    _startList.SelectedValue = _endList.SelectedValue;
                    _endList.Clear();
                    await StartCollectionChanged.InvokeAsync(StartCollection);
                    await EndCollectionChanged.InvokeAsync(EndCollection);
                }
                else if (MultiSelection == true && _endList.SelectedValues != null)
                {
                    ICollection<T> transferredValues = new List<T>();
                    foreach (var item in _endList.SelectedValues)
                    {
                        if (item == null)
                        {
                            continue;
                        }
                        StartCollection.Add(item);
                        EndCollection.Remove(item);
                        transferredValues.Add(item);
                    }
                    _startList.SelectedValues = transferredValues;
                    await _startList.ForceUpdate();
                    _endList.Clear();
                    await StartCollectionChanged.InvokeAsync(StartCollection);
                    await EndCollectionChanged.InvokeAsync(EndCollection);
                }

            }
        }

        protected internal async Task TransferAll(bool startToEnd = true)
        {
            await OnTransfer.InvokeAsync();
            if (PreventTransfer != null && PreventTransfer.Invoke(startToEnd) == true)
            {
                return;
            }
            if (startToEnd == true)
            {
                foreach (var item in StartCollection)
                {
                    EndCollection.Add(item);
                }
                StartCollection.Clear();
                _startList.Clear();
                await EndCollectionChanged.InvokeAsync(EndCollection);
                await StartCollectionChanged.InvokeAsync(StartCollection);
            }
            else if (startToEnd == false)
            {
                foreach (var item in EndCollection)
                {
                    StartCollection.Add(item);
                }
                EndCollection.Clear();
                _endList.Clear();
                await StartCollectionChanged.InvokeAsync(StartCollection);
                await EndCollectionChanged.InvokeAsync(EndCollection);
            }
        }

        public ICollection<T> GetStartListSelectedValues()
        {
            if (_startList == null)
            {
                return null;
            }

            if (MultiSelection == true)
            {
                return _startList.SelectedValues?.ToList();
            }
            else
            {
                return new List<T>() { _startList.SelectedValue };
            }
        }

        public ICollection<T> GetEndListSelectedValues()
        {
            if (_endList == null)
            {
                return null;
            }

            if (MultiSelection == true)
            {
                return _endList.SelectedValues?.ToList();
            }
            else
            {
                return new List<T>() { _endList.SelectedValue };
            }
        }

    }
}
