using Bunit;
using AwesomeAssertions;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using MudBlazor.Extensions;
using MudExtensions.UnitTests.TestComponents;

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

        [Test]
        public async Task ChipField_ParentFormDisabled_ShouldDisableChipsAndPreventRemoval()
        {
            var comp = Context.Render<ChipFieldFormStateTest>(opt =>
            {
                opt.Add(a => a.FormDisabled, true);
            });
            var chipField = comp.FindComponent<MudChipField<string>>();
            var chips = comp.FindComponents<MudChip<string>>();

            chips.Should().HaveCount(2);
            chips.Should().AllSatisfy(chip => chip.Instance.Disabled.Should().BeTrue());

            await comp.InvokeAsync(() => chipField.Instance.Closed(chips[0].Instance));
            chipField.Instance.GetState(x => x.Values).Should().BeEquivalentTo(new List<string> { "first", "second" });
        }

        [Test]
        public async Task ChipField_ParentFormReadOnly_ShouldPreventRemoval()
        {
            var comp = Context.Render<ChipFieldFormStateTest>(opt =>
            {
                opt.Add(a => a.FormReadOnly, true);
            });
            var chipField = comp.FindComponent<MudChipField<string>>();
            var chips = comp.FindComponents<MudChip<string>>();

            await comp.InvokeAsync(() => chipField.Instance.Closed(chips[0].Instance));
            chipField.Instance.GetState(x => x.Values).Should().BeEquivalentTo(new List<string> { "first", "second" });
        }
    }
}
