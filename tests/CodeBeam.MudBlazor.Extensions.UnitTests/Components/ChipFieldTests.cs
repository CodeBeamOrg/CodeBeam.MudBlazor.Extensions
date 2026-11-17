using Bunit;
using AwesomeAssertions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
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

        [Test]
        public async Task ChipField_Required_WithChips_ShouldBeValid()
        {
            var comp = Context.Render<ChipFieldRequiredTest>(opt =>
            {
                opt.Add(a => a.Values, new List<string> { "first" });
            });

            await comp.InvokeAsync(() => comp.Instance.Form!.Validate());

            comp.Instance.Form!.IsValid.Should().BeTrue();
            comp.Instance.Form.Errors.Should().BeEmpty();
            comp.FindAll(".mud-input-error").Should().BeEmpty();
            comp.Markup.Should().NotContain("Add at least one chip");
        }

        [Test]
        public async Task ChipField_Required_WithoutChips_ShouldBeInvalid()
        {
            var comp = Context.Render<ChipFieldRequiredTest>(opt =>
            {
                opt.Add(a => a.Values, new List<string>());
            });

            await comp.InvokeAsync(() => comp.Instance.Form!.Validate());

            comp.Instance.Form!.IsValid.Should().BeFalse();
            comp.Instance.Form.Errors.Should().ContainSingle().Which.Should().Be("Add at least one chip");
            comp.Markup.Should().Contain("Add at least one chip");
        }

        [Test]
        public async Task ChipField_Required_WithTextButNoChips_ShouldBeInvalid()
        {
            var comp = Context.Render<ChipFieldRequiredTest>(opt =>
            {
                opt.Add(a => a.Values, new List<string>());
            });

            comp.Find("input").Input(new ChangeEventArgs { Value = "not a chip yet" });
            await comp.InvokeAsync(() => comp.Instance.Form!.Validate());

            comp.Instance.Form!.IsValid.Should().BeFalse();
        }

        [Test]
        public async Task ChipField_Required_TypingAfterError_ShouldKeepShowingError()
        {
            var comp = Context.Render<ChipFieldRequiredTest>(opt =>
            {
                opt.Add(a => a.Values, new List<string>());
            });
            await comp.InvokeAsync(() => comp.Instance.Form!.Validate());
            comp.Markup.Should().Contain("Add at least one chip");

            comp.Find("input").Input(new ChangeEventArgs { Value = "not a chip yet" });
            comp.Markup.Should().Contain("Add at least one chip");

            await comp.InvokeAsync(() => comp.Instance.Form!.Validate());

            comp.Instance.Form!.IsValid.Should().BeFalse();
            comp.Markup.Should().Contain("Add at least one chip");
            comp.Find("input").GetAttribute("aria-invalid").Should().Be("true");
        }

        [Test]
        public async Task ChipField_Required_AddingChipAfterError_ShouldBecomeValid()
        {
            var comp = Context.Render<ChipFieldRequiredTest>(opt =>
            {
                opt.Add(a => a.Values, new List<string>());
            });
            await comp.InvokeAsync(() => comp.Instance.Form!.Validate());
            comp.Instance.Form!.IsValid.Should().BeFalse();

            var chipField = comp.FindComponent<MudChipField<string>>();
            comp.Find("input").Input(new ChangeEventArgs { Value = "first" });
            await comp.InvokeAsync(() => chipField.Instance.HandleBeforeInput(new BeforeInputEventArgs { Data = " ", InputType = "insert" }));
            chipField.Instance.GetState(x => x.Values).Should().BeEquivalentTo(new List<string> { "first" });

            await comp.InvokeAsync(() => comp.Instance.Form!.Validate());

            comp.Instance.Form!.IsValid.Should().BeTrue();
            comp.FindAll(".mud-input-error").Should().BeEmpty();
            comp.Markup.Should().NotContain("Add at least one chip");
        }

        [Test]
        public async Task ChipField_Required_BlurWithChips_ShouldNotShowError()
        {
            var comp = Context.Render<ChipFieldRequiredTest>(opt =>
            {
                opt.Add(a => a.Values, new List<string> { "first" });
            });

            await comp.Find("input").BlurAsync(new FocusEventArgs());

            comp.FindAll(".mud-input-error").Should().BeEmpty();
            comp.Instance.Form!.Errors.Should().BeEmpty();
        }

        [Test]
        public void ChipField_Required_ShouldKeepRequiredMarker()
        {
            var comp = Context.Render<ChipFieldRequiredTest>(opt =>
            {
                opt.Add(a => a.Values, new List<string> { "first" });
            });

            comp.FindAll(".mud-input-control.mud-input-required").Should().NotBeEmpty();
        }

        [Test]
        public async Task ChipField_Required_WithNullValues_ShouldBeInvalid()
        {
            var comp = Context.Render<ChipFieldRequiredTest>();

            await comp.InvokeAsync(() => comp.Instance.Form!.Validate());

            comp.Instance.Form!.IsValid.Should().BeFalse();
            comp.Markup.Should().Contain("Add at least one chip");
        }

        [Test]
        public async Task ChipFieldTextField_WithoutHasChips_ShouldValidateRequiredAgainstText()
        {
            var comp = Context.Render<ChipFieldTextField<string>>(opt =>
            {
                opt.Add(a => a.Required, true);
            });

            await comp.InvokeAsync(() => comp.Instance.ValidateAsync());
            comp.Instance.HasErrors.Should().BeTrue();

            comp.Find("input").Change("text");
            await comp.InvokeAsync(() => comp.Instance.ValidateAsync());
            comp.Instance.HasErrors.Should().BeFalse();
        }
    }
}
