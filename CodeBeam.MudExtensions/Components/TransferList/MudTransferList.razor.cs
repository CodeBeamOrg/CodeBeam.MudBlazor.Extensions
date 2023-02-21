using Microsoft.AspNetCore.Components;
using MudBlazor;
using MudExtensions.Enums;

namespace MudExtensions
{
    public partial class MudTransferList<T> : MudComponentBase
    {
        MudListExtended<T> _startList;
        MudListExtended<T> _endList;

        [Parameter]
        public ICollection<T> StartCollection { get; set; }

        [Parameter]
        public EventCallback<ICollection<T>> StartCollectionChanged { get; set; }

        [Parameter]
        public ICollection<T> EndCollection { get; set; }

        [Parameter]
        public EventCallback<ICollection<T>> EndCollectionChanged { get; set; }

        [Parameter]
        public bool Vertical { get; set; }

        [Parameter]
        public bool MultiSelection { get; set; }

        [Parameter]
        public MultiSelectionComponent MultiSelectionComponent { get; set; } = MultiSelectionComponent.CheckBox;

        [Parameter]
        public int Spacing { get; set; } = 4;

        [Parameter]
        public int ButtonSpacing { get; set; } = 0;

        [Parameter]
        public int? MaxItems { get; set; }

        [Parameter]
        public string StyleList { get; set; }

        protected internal async Task Transfer(bool startToEnd = true)
        {
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

    }
}
