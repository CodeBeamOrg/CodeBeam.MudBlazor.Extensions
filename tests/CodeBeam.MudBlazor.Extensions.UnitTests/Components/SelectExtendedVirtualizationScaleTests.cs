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
        public void VirtualizedItemCollection_DoesNotMaterializeShadowList(int itemCount)
        {
            var items = Enumerable.Range(1, itemCount).Select(value => (int?)value).ToList();
            var selectedValues = new int?[] { 1, itemCount };

            var cut = Context.Render<MudSelectExtended<int?>>(parameters => parameters
                .Add(x => x.ItemCollection, items)
                .Add(x => x.Virtualize, true)
                .Add(x => x.MultiSelection, true)
                .Add(x => x.SelectedValues, selectedValues));

            cut.FindComponents<MudListExtended<int?>>().Should().BeEmpty();
            cut.Instance.Items.Should().BeEmpty();
            cut.Instance.SelectedValues.Should().BeEquivalentTo(selectedValues);

            cut.WaitForAssertion(() =>
                cut.Find("input").Attributes["value"]?.Value.Should().Be($"1, {itemCount}"));
        }

        [Test]
        public void NonVirtualizedItemCollection_IsAlsoValueDrivenWhileClosed()
        {
            var items = Enumerable.Range(1, 100).Select(value => (int?)value).ToList();
            var selectedValues = new int?[] { 1, 100 };

            var cut = Context.Render<MudSelectExtended<int?>>(parameters => parameters
                .Add(x => x.ItemCollection, items)
                .Add(x => x.Virtualize, false)
                .Add(x => x.MultiSelection, true)
                .Add(x => x.SelectedValues, selectedValues));

            cut.Instance.Items.Should().BeEmpty();
            cut.FindComponents<MudListExtended<int?>>().Should().BeEmpty();
            cut.WaitForAssertion(() =>
                cut.Find("input").Attributes["value"]?.Value.Should().Be("1, 100"));
        }
    }
}
