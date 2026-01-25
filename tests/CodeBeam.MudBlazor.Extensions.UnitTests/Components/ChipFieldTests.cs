using Bunit;
using AwesomeAssertions;
using Microsoft.AspNetCore.Components;
using MudBlazor.Extensions;

namespace MudExtensions.UnitTests.Components
{
    [TestFixture]
    public class ChipFieldTests : BunitTest
    {
        [Test]
        public async Task ChipFieldBasicTest()
        {
            var comp = Context.Render<MudChipField<string>>(opt =>
            {
                opt.Add(a => a.Values, new List<string> { "asdf", "asd" });
            });
            var field = comp.FindComponent<MudTextFieldExtended<string>>();
            field.Find("input").Input(new ChangeEventArgs() { Value = "sdfg" });
            await comp.InvokeAsync(() => comp.Instance.HandleBeforeInput(new BeforeInputEventArgs() { Data = " ", InputType = "insert" }));
            comp.Instance.GetState(x => x.Values).Should().BeEquivalentTo(new List<string> { "asdf", "asd", "sdfg" });
            comp.Instance.GetState(x => x.Value).Should().BeEquivalentTo(null);
        }
    }
}
