using System.Collections.Generic;
using System.Linq;

namespace MudExtensions
{
    public partial class MudListItemExtended<T>
    {
        /// <summary>
        /// Re-applies selection state whenever a list item receives parameters.
        /// Virtualized lists can reuse an existing component instance for a different value,
        /// so selection cannot rely only on registration/scroll timing.
        /// </summary>
        protected override void OnParametersSet()
        {
            base.OnParametersSet();

            if (MudListExtended == null)
            {
                return;
            }

            var comparer = MudListExtended.Comparer ?? EqualityComparer<T?>.Default;

            _selected = MudListExtended.MultiSelection
                ? MudListExtended.SelectedValues?.Contains(Value, comparer) == true
                : comparer.Equals(MudListExtended.SelectedValue, Value);
        }
    }
}
