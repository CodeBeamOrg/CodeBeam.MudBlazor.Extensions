using AwesomeAssertions;
using Bunit;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace MudExtensions.UnitTests.Components
{
    [TestFixture]
    public class SelectExtendedVirtualizationSingleSelectionTests : BunitTest
    {
        [Test]
        public void VirtualizedItemCollection_InitializedSingleSelectionUsesOneShadowItem()
        {
            var items = Enumerable.Range(1, 4_000).Select(value => (int?)value).ToList();

            var cut = Context.Render<MudSelectExtended<int?>>(parameters => parameters
                .Add(x => x.ItemCollection, items)
                .Add(x => x.Virtualize, true)
                .Add(x => x.Value, 3_999));

            cut.WaitForAssertion(() =>
                cut.Find("input").Attributes["value"]?.Value.Should().Be("3999"));

            var shadowList = cut.Find("div[style='display: none']");
            shadowList.QuerySelectorAll("div.mud-list-item-extended").Count().Should().Be(1);
        }

        [Test]
        public void VirtualizedItemCollection_OffscreenValueSupportsItemContentPresenter()
        {
            var items = Enumerable.Range(1, 4_000).Select(value => (int?)value).ToList();
            RenderFragment<MudListItemExtended<int?>> template = item => builder =>
                builder.AddContent(0, $"Selected item {item.Value}");

            var cut = Context.Render<MudSelectExtended<int?>>(parameters => parameters
                .Add(x => x.ItemCollection, items)
                .Add(x => x.Virtualize, true)
                .Add(x => x.ValuePresenter, ValuePresenter.ItemContent)
                .Add(x => x.ItemTemplate, template)
                .Add(x => x.Value, 3_999));

            cut.WaitForAssertion(() =>
                cut.Markup.Should().Contain("Selected item 3999"));

            var shadowList = cut.Find("div[style='display: none']");
            shadowList.QuerySelectorAll("div.mud-list-item-extended").Count().Should().Be(1);
        }
    }
}
