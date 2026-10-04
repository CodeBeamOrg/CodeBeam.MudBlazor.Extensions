using System.Collections.Generic;
using System.Linq;

namespace MudExtensions
{
    public partial class MudSelectExtended<T>
    {
        /// <summary>
        /// Returns the values that must be materialized by the hidden list.
        /// Non-virtualized selects preserve the existing full item registry behavior;
        /// virtualized selects only materialize selected values that exist in the current item collection.
        /// </summary>
        protected ICollection<T?>? GetShadowItemCollection()
        {
            if (ItemCollection == null || !Virtualize)
            {
                return ItemCollection;
            }

            var selectedValues = SelectedValues?.ToHashSet(_comparer) ?? new HashSet<T?>(_comparer);
            return [.. ItemCollection.Where(selectedValues.Contains)];
        }
    }
}
