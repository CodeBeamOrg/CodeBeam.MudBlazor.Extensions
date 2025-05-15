using MudExtensions.Docs.Examples;
using FluentAssertions;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace MudExtensions.UnitTests.Components
{
    [TestFixture]
    public class ChipFieldTests : BunitTest
    {
        [Test]
        public async Task ChipFieldBasicTest()
        {
            var comp = Context.RenderComponent<MudChipField<string>>(opt =>
            {
                opt.Add(a => a.Values, new List<string> { "asdf", "asd" });
            });
            var field = comp.FindComponent<MudTextFieldExtended<string>>();
            field.Find("input").Input(new ChangeEventArgs() { Value = "sdfg" });
            await comp.InvokeAsync(() => comp.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = " " }));
            comp.Instance.Values.Should().BeEquivalentTo(new List<string> { "asdf", "asd", "sdfg" });
            comp.Instance.Value.Should().BeEquivalentTo(null);
        }
    }
}
