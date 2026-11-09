#pragma warning disable CS1998 // async without await
#pragma warning disable IDE1006 // leading underscore
#pragma warning disable BL0005 // Set parameter outside component

using Bunit;
using MudExtensions.UnitTests.TestComponents;
using AwesomeAssertions;
using MudBlazor;
using MudExtensions.UnitTests.Extensions;

namespace MudExtensions.UnitTests.Components
{
    [TestFixture]
    public class ComboBoxItemTests : BunitTest
    {
        private static void OpenMenu(IRenderedComponent<ComboBoxItemTest> comp)
        {
            comp.FindComponent<MudComboBox<string>>().Find("div.mud-input-control").Click();
            comp.WaitForAssertion(() => comp.FindAll("div.mud-combobox-item").Count.Should().Be(4));
        }

        private static async Task FilterAsync(IRenderedComponent<ComboBoxItemTest> comp, string search)
        {
            var combobox = comp.FindComponent<MudComboBox<string>>();
            await comp.InvokeAsync(() => combobox.Instance.HandleInternalValueChanged(search));
            await comp.InvokeAsync(() => combobox.Instance.ForceRenderItems());
        }

        /// <summary>Every MudComboBoxItem registers itself with its parent combobox.</summary>
        [Test]
        public void ComboBoxItem_ShouldRegisterWithParent()
        {
            var comp = Context.Render<ComboBoxItemTest>();

            comp.FindComponent<MudComboBox<string>>().Instance.Items.Count.Should().Be(4);
        }

        /// <summary>The item Text is displayed when provided.</summary>
        [Test]
        public void ComboBoxItem_ShouldDisplayText()
        {
            var comp = Context.Render<ComboBoxItemTest>();

            OpenMenu(comp);

            comp.Find("div.item-apple").TextContent.Should().Contain("Apple");
        }

        /// <summary>The item Value is displayed when no Text is provided.</summary>
        [Test]
        public void ComboBoxItem_ShouldDisplayValueWhenTextIsMissing()
        {
            var comp = Context.Render<ComboBoxItemTest>();

            OpenMenu(comp);

            comp.Find("div.item-banana").TextContent.Should().Contain("banana");
        }

        /// <summary>Clicking an item invokes its OnClick callback.</summary>
        [Test]
        public void ComboBoxItem_ClickShouldInvokeOnClick()
        {
            var comp = Context.Render<ComboBoxItemTest>();
            OpenMenu(comp);

            comp.Find("div.item-apple").Click();

            comp.WaitForAssertion(() => comp.Instance.ClickCount.Should().Be(1));
        }

        /// <summary>Clicking an item sets the parent combobox value.</summary>
        [Test]
        public void ComboBoxItem_ClickShouldSetParentValue()
        {
            var comp = Context.Render<ComboBoxItemTest>();
            OpenMenu(comp);

            comp.Find("div.item-banana").Click();

            comp.WaitForAssertion(() => comp.FindComponent<MudComboBox<string>>().Instance.Value.Should().Be("banana"));
        }

        /// <summary>The Disabled
        [Test]
        public void ComboBoxItem_DisabledShouldHaveDisabledClass()
        {
            var comp = Context.Render<ComboBoxItemTest>();

            OpenMenu(comp);

            comp.Find("div.item-cherry").ClassList.Should().Contain("mud-combobox-item-disabled");
        }

        /// <summary>A disabled item has no ripple class.</summary>
        [Test]
        public void ComboBoxItem_DisabledShouldNotHaveRippleClass()
        {
            var comp = Context.Render<ComboBoxItemTest>();

            OpenMenu(comp);

            comp.Find("div.item-cherry").ClassList.Should().NotContain("mud-ripple");
        }

        /// <summary>An enabled item has the ripple class by default.</summary>
        [Test]
        public void ComboBoxItem_ShouldHaveRippleClassByDefault()
        {
            var comp = Context.Render<ComboBoxItemTest>();

            OpenMenu(comp);

            comp.Find("div.item-apple").ClassList.Should().Contain("mud-ripple");
        }

        /// <summary>Setting Ripple to false removes the ripple class.</summary>
        [Test]
        public void ComboBoxItem_RippleFalseShouldNotHaveRippleClass()
        {
            var comp = Context.Render<ComboBoxItemTest>();

            OpenMenu(comp);

            comp.Find("div.item-banana").ClassList.Should().NotContain("mud-ripple");
        }

        /// <summary>A user supplied Class is merged into the item classes.</summary>
        [Test]
        public void ComboBoxItem_ShouldApplyUserClass()
        {
            var comp = Context.Render<ComboBoxItemTest>();

            OpenMenu(comp);

            comp.Find("div.item-apple").ClassList.Should().Contain("mud-combobox-item");
        }

        /// <summary>The selected item gets the selected CSS class.</summary>
        [Test]
        public void ComboBoxItem_SelectedShouldHaveSelectedClass()
        {
            var comp = Context.Render<ComboBoxItemTest>();
            OpenMenu(comp);
            comp.Find("div.item-banana").Click();
            comp.WaitForAssertion(() => comp.FindComponent<MudComboBox<string>>().Instance.Value.Should().Be("banana"));

                        comp.FindComponent<MudComboBox<string>>().Find("div.mud-input-control").Click();

            comp.WaitForAssertion(() => comp.Find("div.item-banana").ClassList.Should().Contain("mud-selected-item"));
        }

        /// <summary>An unselected item does not get the selected CSS class.</summary>
        [Test]
        public void ComboBoxItem_UnselectedShouldNotHaveSelectedClass()
        {
            var comp = Context.Render<ComboBoxItemTest>();

            OpenMenu(comp);

            comp.Find("div.item-apple").ClassList.Should().NotContain("mud-selected-item");
        }

        /// <summary>SetActive(true) adds the highlight class.</summary>
        [Test]
        public async Task ComboBoxItem_SetActiveShouldAddHighlightClass()
        {
            var comp = Context.Render<ComboBoxItemTest>();
            OpenMenu(comp);
            var item = comp.FindComponents<MudComboBoxItem<string>>().First(x => x.Instance.Value == "apple");

            await comp.InvokeAsync(() => item.Instance.SetActive(true));
            await comp.InvokeAsync(() => item.Instance.ForceUpdate());

            comp.WaitForAssertion(() => comp.Find("div.item-apple").ClassList.Should().Contain("mud-combobox-item-hilight"));
        }

        /// <summary>SetActive(false) removes the highlight class.</summary>
        [Test]
        public async Task ComboBoxItem_SetInactiveShouldRemoveHighlightClass()
        {
            var comp = Context.Render<ComboBoxItemTest>();
            OpenMenu(comp);
            var item = comp.FindComponents<MudComboBoxItem<string>>().First(x => x.Instance.Value == "apple");
            await comp.InvokeAsync(() => item.Instance.SetActive(true));
            await comp.InvokeAsync(() => item.Instance.ForceUpdate());

            await comp.InvokeAsync(() => item.Instance.SetActive(false));
            await comp.InvokeAsync(() => item.Instance.ForceUpdate());

            comp.WaitForAssertion(() => comp.Find("div.item-apple").ClassList.Should().NotContain("mud-combobox-item-hilight"));
        }

        /// <summary>A disabled item does not get the highlight class when active.</summary>
        [Test]
        public async Task ComboBoxItem_DisabledActiveShouldNotHaveHighlightClass()
        {
            var comp = Context.Render<ComboBoxItemTest>();
            OpenMenu(comp);
            var item = comp.FindComponents<MudComboBoxItem<string>>().First(x => x.Instance.Value == "cherry");

            await comp.InvokeAsync(() => item.Instance.SetActive(true));
            await comp.InvokeAsync(() => item.Instance.ForceUpdate());

            comp.Find("div.item-cherry").ClassList.Should().NotContain("mud-combobox-item-hilight");
        }

        /// <summary>With Bordered enabled, an active item gets the bordered class.</summary>
        [Test]
        public async Task ComboBoxItem_BorderedActiveShouldHaveBorderedClass()
        {
            var comp = Context.Render<ComboBoxItemTest>(p => p.Add(x => x.Bordered, true));
            OpenMenu(comp);
            var item = comp.FindComponents<MudComboBoxItem<string>>().First(x => x.Instance.Value == "apple");

            await comp.InvokeAsync(() => item.Instance.SetActive(true));
            await comp.InvokeAsync(() => item.Instance.ForceUpdate());

            comp.WaitForAssertion(() => comp.Find("div.item-apple").ClassList.Should().Contain("mud-combobox-item-bordered"));
        }

        /// <summary>Without Bordered, an active item has no bordered class.</summary>
        [Test]
        public async Task ComboBoxItem_NotBorderedActiveShouldNotHaveBorderedClass()
        {
            var comp = Context.Render<ComboBoxItemTest>();
            OpenMenu(comp);
            var item = comp.FindComponents<MudComboBoxItem<string>>().First(x => x.Instance.Value == "apple");

            await comp.InvokeAsync(() => item.Instance.SetActive(true));
            await comp.InvokeAsync(() => item.Instance.ForceUpdate());

            comp.Find("div.item-apple").ClassList.Should().NotContain("mud-combobox-item-bordered");
        }

        /// <summary>The item is eligible by default when the combobox is not editable.</summary>
        [Test]
        public async Task ComboBoxItem_ShouldBeEligibleWhenNotEditable()
        {
            var comp = Context.Render<ComboBoxItemTest>();
            OpenMenu(comp);

            await FilterAsync(comp, "zzz");

            comp.FindComponents<MudComboBoxItem<string>>().All(x => x.Instance.Eligible).Should().BeTrue();
        }

        /// <summary>When editable, items not matching the search text are not eligible.</summary>
        [Test]
        public async Task ComboBoxItem_ShouldNotBeEligibleWhenSearchDoesNotMatch()
        {
            var comp = Context.Render<ComboBoxItemTest>(p => p.Add(x => x.Editable, true));
            OpenMenu(comp);

            await FilterAsync(comp, "zzz");

            comp.FindComponents<MudComboBoxItem<string>>().Any(x => x.Instance.Eligible).Should().BeFalse();
        }

        /// <summary>When editable, only items whose Text contains the search text are eligible.</summary>
        [Test]
        public async Task ComboBoxItem_ShouldMatchSearchAgainstText()
        {
            var comp = Context.Render<ComboBoxItemTest>(p => p.Add(x => x.Editable, true));
            OpenMenu(comp);

            await FilterAsync(comp, "app");

            comp.WaitForAssertion(() => comp.FindComponents<MudComboBoxItem<string>>().Where(x => x.Instance.Eligible).Select(x => x.Instance.Value)
                .Should().BeEquivalentTo(new[] { "apple" }));
        }

        /// <summary>The search is case insensitive.</summary>
        [Test]
        public async Task ComboBoxItem_SearchShouldBeCaseInsensitive()
        {
            var comp = Context.Render<ComboBoxItemTest>(p => p.Add(x => x.Editable, true));
            OpenMenu(comp);

            await FilterAsync(comp, "APP");

            comp.WaitForAssertion(() => comp.FindComponents<MudComboBoxItem<string>>().Where(x => x.Instance.Eligible).Select(x => x.Instance.Value)
                .Should().BeEquivalentTo(new[] { "apple" }));
        }

        /// <summary>An item without Text is matched by its converted Value.</summary>
        [Test]
        public async Task ComboBoxItem_ShouldMatchSearchAgainstValueWhenTextIsMissing()
        {
            var comp = Context.Render<ComboBoxItemTest>(p => p.Add(x => x.Editable, true));
            OpenMenu(comp);

            await FilterAsync(comp, "nan");

            comp.WaitForAssertion(() => comp.FindComponents<MudComboBoxItem<string>>().Where(x => x.Instance.Eligible).Select(x => x.Instance.Value)
                .Should().BeEquivalentTo(new[] { "banana" }));
        }

        /// <summary>A not eligible item gets the d-none class.</summary>
        [Test]
        public async Task ComboBoxItem_NotEligibleShouldHaveDNoneClass()
        {
            var comp = Context.Render<ComboBoxItemTest>(p => p.Add(x => x.Editable, true));
            OpenMenu(comp);

            await FilterAsync(comp, "app");

            comp.WaitForAssertion(() => comp.Find("div.item-banana").ClassList.Should().Contain("d-none"));
        }

        /// <summary>An eligible item has no d-none class.</summary>
        [Test]
        public async Task ComboBoxItem_EligibleShouldNotHaveDNoneClass()
        {
            var comp = Context.Render<ComboBoxItemTest>(p => p.Add(x => x.Editable, true));
            OpenMenu(comp);

            await FilterAsync(comp, "app");

            comp.WaitForAssertion(() => comp.Find("div.item-apple").ClassList.Should().NotContain("d-none"));
        }

        /// <summary>A custom SearchFunc overrides the default matching.</summary>
        [Test]
        public async Task ComboBoxItem_SearchFuncShouldOverrideDefaultMatching()
        {
            Func<string?, string?, string?, bool> searchFunc = (value, text, search) => value == "date";
            var comp = Context.Render<ComboBoxItemTest>(p =>
            {
                p.Add(x => x.Editable, true);
                p.Add(x => x.SearchFunc, searchFunc);
            });
            OpenMenu(comp);

            await FilterAsync(comp, "anything");

            comp.WaitForAssertion(() => comp.FindComponents<MudComboBoxItem<string>>().Where(x => x.Instance.Eligible).Select(x => x.Instance.Value)
                .Should().BeEquivalentTo(new[] { "date" }));
        }

        /// <summary>An empty search text keeps every item eligible.</summary>
        [Test]
        public async Task ComboBoxItem_EmptySearchShouldKeepAllItemsEligible()
        {
            var comp = Context.Render<ComboBoxItemTest>(p => p.Add(x => x.Editable, true));
            OpenMenu(comp);
            await FilterAsync(comp, "app");

            await FilterAsync(comp, "");

            comp.WaitForAssertion(() => comp.FindComponents<MudComboBoxItem<string>>().All(x => x.Instance.Eligible).Should().BeTrue());
        }

        /// <summary>A checkbox is rendered for each item in multi selection mode.</summary>
        [Test]
        public void ComboBoxItem_MultiSelectionShouldRenderCheckbox()
        {
            var comp = Context.Render<ComboBoxItemTest>(p => p.Add(x => x.MultiSelection, true));

            OpenMenu(comp);

            comp.FindAll("div.mud-combobox-item").All(x => x.QuerySelector(".mud-checkbox") is not null).Should().BeTrue();
        }

        /// <summary>No checkbox is rendered in single selection mode.</summary>
        [Test]
        public void ComboBoxItem_SingleSelectionShouldNotRenderCheckbox()
        {
            var comp = Context.Render<ComboBoxItemTest>();

            OpenMenu(comp);

            comp.FindAll("div.mud-combobox-item").Any(x => x.QuerySelector(".mud-checkbox") is not null).Should().BeFalse();
        }

        /// <summary>No checkbox is rendered in multi selection mode when ShowCheckbox is false.</summary>
        [Test]
        public void ComboBoxItem_ShowCheckboxFalseShouldNotRenderCheckbox()
        {
            var comp = Context.Render<ComboBoxItemTest>(p =>
            {
                p.Add(x => x.MultiSelection, true);
                p.Add(x => x.ShowCheckbox, false);
            });

            OpenMenu(comp);

            comp.FindAll("div.mud-combobox-item").Any(x => x.QuerySelector(".mud-checkbox") is not null).Should().BeFalse();
        }

        /// <summary>In multi selection, clicking several items selects all of them.</summary>
        [Test]
        public void ComboBoxItem_MultiSelectionClickShouldSelectMultipleItems()
        {
            var comp = Context.Render<ComboBoxItemTest>(p => p.Add(x => x.MultiSelection, true));
            OpenMenu(comp);

            comp.Find("div.item-apple").Click();
            comp.Find("div.item-banana").Click();

            comp.WaitForAssertion(() => comp.FindComponent<MudComboBox<string>>().Instance.SelectedValues.Should().BeEquivalentTo(new[] { "apple", "banana" }));
        }

        /// <summary>In multi selection, clicking a selected item deselects it.</summary>
        [Test]
        public void ComboBoxItem_MultiSelectionClickShouldToggleItem()
        {
            var comp = Context.Render<ComboBoxItemTest>(p => p.Add(x => x.MultiSelection, true));
            OpenMenu(comp);
            comp.Find("div.item-apple").Click();
            comp.WaitForAssertion(() => comp.FindComponent<MudComboBox<string>>().Instance.SelectedValues.Should().Contain("apple"));

            comp.Find("div.item-apple").Click();

            comp.WaitForAssertion(() => comp.FindComponent<MudComboBox<string>>().Instance.SelectedValues.Should().NotContain("apple"));
        }

        /// <summary>Child content is rendered inside the item when the item presenter allows it.</summary>
        [Test]
        public void ComboBoxItem_ChildContentShouldBeRendered()
        {
            var comp = Context.Render<ComboBoxItemTest>();

            OpenMenu(comp);

            comp.Find("div.item-date").TextContent.Should().Contain("date");
        }

        /// <summary>Disposing an item removes it from the parent combobox.</summary>
        [Test]
        public async Task ComboBoxItem_DisposeShouldRemoveItemFromParent()
        {
            var comp = Context.Render<ComboBoxItemTest>();
            var combobox = comp.FindComponent<MudComboBox<string>>();
            var item = comp.FindComponents<MudComboBoxItem<string>>().First(x => x.Instance.Value == "apple");

            await comp.InvokeAsync(() => item.Instance.Dispose());

            combobox.Instance.Items.Should().NotContain(item.Instance);
        }

        /// <summary>ForceRender re-evaluates eligibility against the current search text.</summary>
        [Test]
        public async Task ComboBoxItem_ForceRenderShouldReevaluateEligibility()
        {
            var comp = Context.Render<ComboBoxItemTest>(p => p.Add(x => x.Editable, true));
            OpenMenu(comp);
            var combobox = comp.FindComponent<MudComboBox<string>>();
            var item = comp.FindComponents<MudComboBoxItem<string>>().First(x => x.Instance.Value == "banana");
            await comp.InvokeAsync(() => combobox.Instance.HandleInternalValueChanged("app"));

            await comp.InvokeAsync(() => item.Instance.ForceRender());

            item.Instance.Eligible.Should().BeFalse();
        }
    }
}
