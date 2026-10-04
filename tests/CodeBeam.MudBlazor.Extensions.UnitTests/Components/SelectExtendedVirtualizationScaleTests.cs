using AwesomeAssertions;
using Bunit;

namespace MudExtensions.UnitTests.Components
{
    [TestFixture]
    public class SelectExtendedVirtualizationScaleTests : BunitTest
    {
        [TestCase(10)]
        [TestCase(100)]
        [TestCase(1_000)]
        [TestCase(4_000)]
        public void ShadowItemCount_IsBoundedBySelection_NotCollectionSize(int itemCount)
        {
            var items = Enumerable.Range(1, itemCount).Select(value => (int?)value).ToList();
            var selectedValues = new int?[] { 1, itemCount };

            var cut = Context.Render<MudSelectExtended<int?>>(parameters => parameters
                .Add(x => x.ItemCollection, items)
                .Add(x => x.Virtualize, true)
                .Add(x => x.MultiSelection, true)
                .Add(x => x.SelectedValues, selectedValues));

            var shadowList = cut.Find("div[style='display: none']");
            shadowList.QuerySelectorAll("div.mud-list-item-extended").Count().Should().Be(2);
        }

        [Test]
        public void NonVirtualizedItemCollection_PreservesFullRegisteredItemSet()
        {
            var items = Enumerable.Range(1, 100).Select(value => (int?)value).ToList();
            var selectedValues = new int?[] { 1, 100 };

            var cut = Context.Render<MudSelectExtended<int?>>(parameters => parameters
                .Add(x => x.ItemCollection, items)
                .Add(x => x.Virtualize, false)
                .Add(x => x.MultiSelection, true)
                .Add(x => x.SelectedValues, selectedValues));

            cut.Instance.Items.Should().HaveCount(items.Count);
        }
    }
}
