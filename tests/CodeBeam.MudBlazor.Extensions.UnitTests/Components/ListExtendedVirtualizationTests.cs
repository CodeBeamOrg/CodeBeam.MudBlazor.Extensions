using AwesomeAssertions;
using Bunit;
using Microsoft.AspNetCore.Components;

namespace MudExtensions.UnitTests.Components
{
    [TestFixture]
    public class ListExtendedVirtualizationTests : BunitTest
    {
        [Test]
        public void ReusedListItem_ReappliesSelectionFromSelectedValues()
        {
            var selectedValues = new int?[] { 42 };
            var cut = RenderList(selectedValues, value: 1);

            var initialItem = cut.FindComponent<MudListItemExtended<int?>>();
            initialItem.Instance.IsSelected.Should().BeFalse();

            cut.Render(parameters => parameters
                .Add(x => x.Clickable, true)
                .Add(x => x.MultiSelection, true)
                .Add(x => x.SelectedValues, selectedValues)
                .Add(x => x.ChildContent, RenderItem(42)));

            var reusedItem = cut.FindComponent<MudListItemExtended<int?>>();
            ReferenceEquals(initialItem.Instance, reusedItem.Instance).Should().BeTrue();
            reusedItem.Instance.IsSelected.Should().BeTrue();
        }

        [Test]
        public void MaterializedListItem_UsesConfiguredComparerForSelection()
        {
            var cut = Context.Render<MudListExtended<int?>>(parameters => parameters
                .Add(x => x.Clickable, true)
                .Add(x => x.MultiSelection, true)
                .Add(x => x.Comparer, new AbsoluteValueComparer())
                .Add(x => x.SelectedValues, new int?[] { 42 })
                .Add(x => x.ChildContent, RenderItem(-42)));

            cut.FindComponent<MudListItemExtended<int?>>().Instance.IsSelected.Should().BeTrue();
        }

        private IRenderedComponent<MudListExtended<int?>> RenderList(IEnumerable<int?> selectedValues, int? value)
        {
            return Context.Render<MudListExtended<int?>>(parameters => parameters
                .Add(x => x.Clickable, true)
                .Add(x => x.MultiSelection, true)
                .Add(x => x.SelectedValues, selectedValues)
                .Add(x => x.ChildContent, RenderItem(value)));
        }

        private static RenderFragment RenderItem(int? value) => builder =>
        {
            builder.OpenComponent<MudListItemExtended<int?>>(0);
            builder.AddAttribute(1, nameof(MudListItemExtended<int?>.Value), value);
            builder.CloseComponent();
        };

        private sealed class AbsoluteValueComparer : IEqualityComparer<int?>
        {
            public bool Equals(int? x, int? y) => Math.Abs(x ?? 0) == Math.Abs(y ?? 0);

            public int GetHashCode(int? obj) => Math.Abs(obj ?? 0).GetHashCode();
        }
    }
}
