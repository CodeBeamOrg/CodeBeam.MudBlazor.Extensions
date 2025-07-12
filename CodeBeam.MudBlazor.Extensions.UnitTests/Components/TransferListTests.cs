using Bunit;
using FluentAssertions;
using MudExtensions.UnitTests.TestComponents;

namespace MudExtensions.UnitTests.Components
{
    [TestFixture]
    public class TransferListTests : BunitTest
    {
        [Test]
        public async Task TransferListTransferAllTest()
        {
            var comp = Context.RenderComponent<TransferListTest>();
            var transferList = comp.FindComponent<MudTransferList<string>>();
            transferList.Instance.StartCollection.Should().Contain("Turkey");
            transferList.Instance.StartCollection.Should().NotContain("China");
            transferList?.Instance?.StartCollection?.Count.Should().Be(5);
            transferList?.Instance?.EndCollection?.Count.Should().Be(5);
            await comp.InvokeAsync(() => transferList.Instance.TransferAll(true));
            transferList.Instance.StartCollection.Should().NotContain("Turkey");
            transferList.Instance.EndCollection.Should().Contain("Turkey");
            await comp.InvokeAsync(() => transferList.Instance.TransferAll(false));
            transferList.Instance.StartCollection.Should().Contain("Turkey");
            transferList.Instance.EndCollection.Should().NotContain("Turkey");
            transferList?.Instance?.StartCollection?.Count.Should().Be(10);
            transferList?.Instance?.EndCollection?.Count.Should().Be(0);
        }

    }
}
