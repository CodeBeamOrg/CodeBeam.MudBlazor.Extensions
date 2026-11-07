using AwesomeAssertions;
using Bunit;
using MudExtensions.UnitTests.TestComponents;

namespace MudExtensions.UnitTests.Components
{
    [TestFixture]
    public class SelectExtendedNestedSelectionTests : BunitTest
    {
        [TestCase(false)]
        [TestCase(true)]
        public void NestedMultiSelection_SelectingItem_PreservesExistingSelection(bool initiallyExpanded)
        {
            var cut = Context.Render<SelectNestedMultiSelectionTest>(parameters => parameters
                .Add(x => x.InitiallyExpanded, initiallyExpanded));

            cut.Find("div.mud-input-control").Click();
            if (!initiallyExpanded)
                cut.Find("[role='option'][aria-label='Station A']").Click();

            var callbacks = cut.Instance.SelectionChanges.Count;
            cut.Find("[role='option'][aria-label='Pressure']").Click();

            cut.WaitForAssertion(() =>
            {
                cut.Instance.SelectedValues.Should().BeEquivalentTo(new[] { "electric", "pressure" });
                cut.Instance.SelectionChanges.Should().HaveCount(callbacks + 1);
                cut.Find("[role='option'][aria-label='Electricity']").GetAttribute("aria-selected").Should().Be("true");
                cut.Find("[role='option'][aria-label='Pressure']").GetAttribute("aria-selected").Should().Be("true");
            });
        }

        [Test]
        public async Task NestedMultiSelection_ParentUpdates_ArePreservedWhenSelectingInAnotherGroup()
        {
            var cut = Context.Render<SelectNestedMultiSelectionTest>();
            cut.Find("div.mud-input-control").Click();

            await cut.InvokeAsync(() => cut.Instance.SetSelectedValues("flow", "pressure"));
            var callbacks = cut.Instance.SelectionChanges.Count;
            cut.Find("[role='option'][aria-label='Temperature']").Click();

            cut.WaitForAssertion(() =>
            {
                cut.Instance.SelectedValues.Should().BeEquivalentTo(new[] { "flow", "pressure", "temperature" });
                cut.Instance.SelectionChanges.Should().HaveCount(callbacks + 1);
                cut.Find("[role='option'][aria-label='Electricity']").GetAttribute("aria-selected").Should().Be("false");
                cut.Find("[role='option'][aria-label='Flow']").GetAttribute("aria-selected").Should().Be("true");
                cut.Find("[role='option'][aria-label='Pressure']").GetAttribute("aria-selected").Should().Be("true");
                cut.Find("[role='option'][aria-label='Temperature']").GetAttribute("aria-selected").Should().Be("true");
            });
        }

        [Test]
        public async Task NestedMultiSelection_DeselectingItem_PreservesOtherSelections()
        {
            var cut = Context.Render<SelectNestedMultiSelectionTest>();
            cut.Find("div.mud-input-control").Click();

            await cut.InvokeAsync(() => cut.Instance.SetSelectedValues("electric", "pressure", "temperature"));
            var callbacks = cut.Instance.SelectionChanges.Count;
            cut.Find("[role='option'][aria-label='Pressure']").Click();

            cut.WaitForAssertion(() =>
            {
                cut.Instance.SelectedValues.Should().BeEquivalentTo(new[] { "electric", "temperature" });
                cut.Instance.SelectionChanges.Should().HaveCount(callbacks + 1);
                cut.Find("[role='option'][aria-label='Pressure']").GetAttribute("aria-selected").Should().Be("false");
                cut.Find("[role='option'][aria-label='Temperature']").GetAttribute("aria-selected").Should().Be("true");
            });
        }

        [Test]
        public void NestedMultiSelection_ClickingSelectedItemAgain_DeselectsItOnce()
        {
            var cut = Context.Render<SelectNestedMultiSelectionTest>();
            cut.Find("div.mud-input-control").Click();
            cut.Find("[role='option'][aria-label='Pressure']").Click();
            var callbacks = cut.Instance.SelectionChanges.Count;

            cut.Find("[role='option'][aria-label='Pressure']").Click();

            cut.WaitForAssertion(() =>
            {
                cut.Instance.SelectedValues.Should().BeEquivalentTo(new[] { "electric" });
                cut.Instance.SelectionChanges.Should().HaveCount(callbacks + 1);
                cut.Find("[role='option'][aria-label='Pressure']").GetAttribute("aria-selected").Should().Be("false");
            });
        }

        [Test]
        public async Task NestedSingleSelection_ChildListsReadRootValueAfterChangesAndClear()
        {
            var cut = Context.Render<ListExperimentalSelectionInitialValueTest>();
            var root = cut.FindComponent<MudListExtended<int?>>();
            var lists = cut.FindComponents<MudListExtended<int?>>();
            lists.Should().HaveCount(3);

            await cut.InvokeAsync(() => cut.Instance.SetSelectedValue(4));
            cut.WaitForAssertion(() =>
            {
                root.Instance.SelectedValue.Should().Be(4);
                foreach (var list in lists)
                    list.Instance.SelectedValue.Should().Be(4);
            });

            await cut.InvokeAsync(() => cut.Instance.SetSelectedValue(null));
            cut.WaitForAssertion(() =>
            {
                root.Instance.SelectedValue.Should().BeNull();
                foreach (var list in lists)
                    list.Instance.SelectedValue.Should().BeNull();
            });
        }
    }
}
