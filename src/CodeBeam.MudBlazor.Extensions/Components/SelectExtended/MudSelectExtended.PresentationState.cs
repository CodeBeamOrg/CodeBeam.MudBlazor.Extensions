using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Components;

namespace MudExtensions
{
    public partial class MudSelectExtended<T>
    {
        private bool ValuesEqual(T? left, T? right)
            => (_comparer ?? EqualityComparer<T?>.Default).Equals(left, right);

        internal bool PresentationValuesEqual(T? left, T? right)
            => ValuesEqual(left, right);

        private bool TryGetCollectionValue(T? value, out T? collectionValue)
        {
            if (ItemCollection != null)
            {
                foreach (var item in ItemCollection)
                {
                    if (ValuesEqual(item, value))
                    {
                        collectionValue = item;
                        return true;
                    }
                }
            }

            collectionValue = default;
            return false;
        }

        private MudSelectItemExtended<T?>? FindRegisteredItem(T? value)
            => Items?.FirstOrDefault(item => ValuesEqual(item.Value, value));

        internal bool IsSelectedPresentationValue(T? value)
            => MultiSelection
                ? SelectedValues?.Contains(value, _comparer) == true
                : ValuesEqual(ReadValue, value);

        private bool _declarativePresentationDirty;

        internal void MarkDeclarativePresentationDirty()
            => _declarativePresentationDirty = true;

        /// <summary>
        /// Resolves the selected option for ItemContent presentation without requiring that
        /// option to have a currently rendered list-item component.
        /// </summary>
        private MudListItemExtended<T?>? GetSelectedPresentationItem()
        {
            if (ItemCollection != null)
            {
                if (!TryGetCollectionValue(ReadValue, out var collectionValue))
                    return null;

                return CreatePresentationItem(
                    collectionValue,
                    ToStringFunc?.Invoke(collectionValue) ?? collectionValue?.ToString(),
                    childContent: null,
                    disabled: ItemDisabledFunc?.Invoke(collectionValue) == true);
            }

            var registeredItem = FindRegisteredItem(ReadValue);
            if (registeredItem != null)
            {
                return CreatePresentationItem(
                    registeredItem.Value,
                    registeredItem.Text,
                    registeredItem.ChildContent,
                    ItemDisabledFunc?.Invoke(registeredItem.Value) ?? registeredItem.Disabled,
                    registeredItem.IsFunctional,
                    registeredItem.Href,
                    registeredItem.Class,
                    registeredItem.Style);
            }

            return SelectedListItem != null && ValuesEqual(SelectedListItem.Value, ReadValue)
                ? SelectedListItem
                : null;
        }

#pragma warning disable BL0005 // Parameters are populated on an unrendered context object for ItemTemplate.
        private static MudListItemExtended<T?> CreatePresentationItem(
            T? value,
            string? text,
            RenderFragment? childContent,
            bool disabled,
            bool isFunctional = false,
            string? href = null,
            string? @class = null,
            string? style = null)
        {
            var item = new MudListItemExtended<T?>
            {
                Value = value,
                Text = text,
                ChildContent = childContent,
                Disabled = disabled,
                IsFunctional = isFunctional,
                Href = href,
                Class = @class,
                Style = style
            };

            item.SetSelected(true, forceRender: false);
            return item;
        }
#pragma warning restore BL0005
    }
}
