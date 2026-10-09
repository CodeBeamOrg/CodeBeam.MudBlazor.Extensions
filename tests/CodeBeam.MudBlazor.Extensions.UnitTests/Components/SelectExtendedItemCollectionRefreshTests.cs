using AwesomeAssertions;
using Bunit;
using MudExtensions.UnitTests.TestComponents;

namespace MudExtensions.UnitTests.Components
{
    [TestFixture]
    public class SelectExtendedItemCollectionRefreshTests : BunitTest
    {
        [Test]
        public async Task ItemCollectionMutation_WhileOpen_RendersNewItemsWithoutReopening()
        {
            var cut = Context.Render<SelectItemCollectionRefreshTest>();

            cut.Find("div.mud-input-control").Click();
            cut.WaitForAssertion(() =>
                cut.FindAll("div.mud-list-item-extended").Should().HaveCount(5));

            await cut.InvokeAsync(cut.Instance.AddMore);

            // The same ItemCollection instance was mutated while the popover remained open.
            // Hosted MudListExtended previously discarded this parameter update entirely (#645).
            cut.WaitForAssertion(() =>
                cut.FindAll("div.mud-list-item-extended").Should().HaveCount(10));
        }
    }
}
