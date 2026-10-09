using AwesomeAssertions;
using Bunit;

namespace MudExtensions.UnitTests.Components
{
    [TestFixture]
    public class SelectExtendedVirtualizationTests : BunitTest
    {
        [Test]
        public void VirtualizedItemCollection_SelectionDoesNotRequireHiddenItems()
        {
            var items = Enumerable.Range(1, 4_000).Select(value => (int?)value).ToList();
            var selectedValues = new int?[] { 17, 3_999 };

            var cut = Context.Render<MudSelectExtended<int?>>(parameters => parameters
                .Add(x => x.ItemCollection, items)
                .Add(x => x.Virtualize, true)
                .Add(x => x.MultiSelection, true)
                .Add(x => x.SelectedValues, selectedValues));

            cut.FindComponents<MudListExtended<int?>>().Should().BeEmpty();
            cut.Instance.Items.Should().BeEmpty();
            cut.Instance.SelectedValues.Should().BeEquivalentTo(selectedValues);
        }

        [Test]
        public void VirtualizedItemCollection_InitializedMultiSelectionStillProducesText()
        {
            var items = Enumerable.Range(1, 4_000).Select(value => (int?)value).ToList();
            var selectedValues = new int?[] { 17, 3_999 };

            var cut = Context.Render<MudSelectExtended<int?>>(parameters => parameters
                .Add(x => x.ItemCollection, items)
                .Add(x => x.Virtualize, true)
                .Add(x => x.MultiSelection, true)
                .Add(x => x.SelectedValues, selectedValues));

            cut.WaitForAssertion(() =>
                cut.Find("input").Attributes["value"]?.Value.Should().Be("17, 3999"));
        }

        [Test]
        public void VirtualizedItemCollection_ChangedSelectionUpdatesTextWithoutShadowItems()
        {
            var items = Enumerable.Range(1, 4_000).Select(value => (int?)value).ToList();

            var cut = Context.Render<MudSelectExtended<int?>>(parameters => parameters
                .Add(x => x.ItemCollection, items)
                .Add(x => x.Virtualize, true)
                .Add(x => x.MultiSelection, true)
                .Add(x => x.SelectedValues, new int?[] { 17 }));

            cut.WaitForAssertion(() =>
                cut.Find("input").Attributes["value"]?.Value.Should().Be("17"));

            cut.Render(parameters => parameters
                .Add(x => x.ItemCollection, items)
                .Add(x => x.Virtualize, true)
                .Add(x => x.MultiSelection, true)
                .Add(x => x.SelectedValues, new int?[] { 3_999 }));

            cut.WaitForAssertion(() =>
                cut.Instance.SelectedValues.Should().BeEquivalentTo(new int?[] { 3_999 }));

            cut.FindComponents<MudListExtended<int?>>().Should().BeEmpty();
            cut.Instance.Items.Should().BeEmpty();

            cut.WaitForAssertion(() =>
                cut.Find("input").Attributes["value"]?.Value.Should().Be("3999"));
        }

        [Test]
        public void VirtualizedItemCollection_ValuePresentationRespectsComparer()
        {
            var items = new List<TestValue?>
            {
                new(1, "One"),
                new(2, "Two"),
                new(3, "Three")
            };
            var selectedValues = new TestValue?[] { new(2, "Different instance") };

            var cut = Context.Render<MudSelectExtended<TestValue?>>(parameters => parameters
                .Add(x => x.ItemCollection, items)
                .Add(x => x.Virtualize, true)
                .Add(x => x.MultiSelection, true)
                .Add(x => x.SelectedValues, selectedValues)
                .Add(x => x.Comparer, new TestValueComparer())
                .Add(x => x.ToStringFunc, value => value?.Name));

            cut.FindComponents<MudListExtended<TestValue?>>().Should().BeEmpty();
            cut.Instance.Items.Should().BeEmpty();
            cut.WaitForAssertion(() =>
                cut.Find("input").Attributes["value"]?.Value.Should().Be("Two"));
        }

        private sealed record TestValue(int Id, string Name);

        private sealed class TestValueComparer : IEqualityComparer<TestValue?>
        {
            public bool Equals(TestValue? x, TestValue? y) => x?.Id == y?.Id;

            public int GetHashCode(TestValue? obj) => obj?.Id.GetHashCode() ?? 0;
        }
    }
}
