using AwesomeAssertions;
using Bunit;

namespace MudExtensions.UnitTests.Components
{
    [TestFixture]
    public class SelectExtendedVirtualizationTests : BunitTest
    {
        [Test]
        public void VirtualizedItemCollection_ShadowListContainsOnlySelectedItems()
        {
            var items = Enumerable.Range(1, 4_000).Select(value => (int?)value).ToList();
            var selectedValues = new int?[] { 17, 3_999 };

            var cut = Context.Render<MudSelectExtended<int?>>(parameters => parameters
                .Add(x => x.ItemCollection, items)
                .Add(x => x.Virtualize, true)
                .Add(x => x.MultiSelection, true)
                .Add(x => x.SelectedValues, selectedValues));

            var shadowList = cut.Find("div[style='display: none']");
            shadowList.QuerySelectorAll("div.mud-list-item-extended").Count().Should().Be(2);
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
        public void VirtualizedItemCollection_ChangedSelectionReplacesShadowItems()
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

            // Assert each layer independently so failures identify whether parameters, hidden
            // components or the input presenter stopped following the selection.
            cut.WaitForAssertion(() =>
                cut.Instance.SelectedValues.Should().BeEquivalentTo(new int?[] { 3_999 }));

            cut.WaitForAssertion(() =>
            {
                var shadowList = cut.Find("div[style='display: none']");
                shadowList.QuerySelectorAll("div.mud-list-item-extended").Count().Should().Be(1);
                shadowList.TextContent.Should().Contain("3999");
                shadowList.TextContent.Should().NotContain("17");
            });

            cut.WaitForAssertion(() =>
                cut.Find("input").Attributes["value"]?.Value.Should().Be("3999"));
        }

        [Test]
        public void VirtualizedItemCollection_ShadowListRespectsComparer()
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

            var shadowList = cut.Find("div[style='display: none']");
            shadowList.QuerySelectorAll("div.mud-list-item-extended").Count().Should().Be(1);
            shadowList.TextContent.Should().Contain("Two");
        }

        private sealed record TestValue(int Id, string Name);

        private sealed class TestValueComparer : IEqualityComparer<TestValue?>
        {
            public bool Equals(TestValue? x, TestValue? y) => x?.Id == y?.Id;

            public int GetHashCode(TestValue? obj) => obj?.Id.GetHashCode() ?? 0;
        }
    }
}
