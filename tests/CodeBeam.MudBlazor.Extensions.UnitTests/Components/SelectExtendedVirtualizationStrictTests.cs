using AwesomeAssertions;
using Bunit;

namespace MudExtensions.UnitTests.Components
{
    [TestFixture]
    public class SelectExtendedVirtualizationStrictTests : BunitTest
    {
        [Test]
        public void StrictMode_ValidOffscreenInitialValue_RemainsRepresentable()
        {
            var items = Enumerable.Range(1, 4_000).Select(value => (int?)value).ToList();

            var cut = Context.Render<MudSelectExtended<int?>>(parameters => parameters
                .Add(x => x.ItemCollection, items)
                .Add(x => x.Virtualize, true)
                .Add(x => x.Strict, true)
                .Add(x => x.Value, 3_999));

            cut.WaitForAssertion(() =>
                cut.Find("input").Attributes["value"]?.Value.Should().Be("3999"));

            var shadowList = cut.Find("div[style='display: none']");
            shadowList.QuerySelectorAll("div.mud-list-item-extended").Count().Should().Be(1);
        }

        [Test]
        public void StrictMode_ValueOutsideItemCollection_IsNotAddedToShadowList()
        {
            var items = Enumerable.Range(1, 100).Select(value => (int?)value).ToList();

            var cut = Context.Render<MudSelectExtended<int?>>(parameters => parameters
                .Add(x => x.ItemCollection, items)
                .Add(x => x.Virtualize, true)
                .Add(x => x.Strict, true)
                .Add(x => x.Value, 999));

            var shadowList = cut.Find("div[style='display: none']");
            shadowList.QuerySelectorAll("div.mud-list-item-extended").Should().BeEmpty();
        }
    }
}
