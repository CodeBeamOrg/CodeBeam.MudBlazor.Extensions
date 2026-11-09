#pragma warning disable CS1998 // async without await
#pragma warning disable IDE1006 // leading underscore
#pragma warning disable BL0005 // Set parameter outside component

using Bunit;
using MudExtensions.UnitTests.TestComponents;
using AwesomeAssertions;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using MudExtensions.UnitTests.Extensions;
using MudBlazor.Extensions;

namespace MudExtensions.UnitTests.Components
{
    [TestFixture]
    public class ComboBoxTests : BunitTest
    {


        // Note: MudSelect doesn't guaranteed the consequences of changing Value if MultiSelection is true for now.
        // When this feature will add, just uncomment the testcase to test it. No need to write new test.
        /// <summary>The initial Value is reflected in the combobox value and text.</summary>
        [Test]
        [TestCase(false)]
        //[TestCase(true)]
        public void ComboBox_InitialValueTest(bool multiSelection)
        {
            var comp = Context.Render<ComboBoxInitialValueTest>();
            comp.SetParametersAndRenderAsync(p => p.Add(x => x.SelectedValue, "1"));
            comp.SetParametersAndRenderAsync(p => p.Add(x => x.MultiSelection, multiSelection));

            var combobox = comp.FindComponent<MudComboBox<string?>>();

            combobox.Instance.GetState(x => x.Value).Should().Be("1");
            //combobox.Instance.SelectedValues.Should().BeEquivalentTo(new HashSet<string?> { "1" }); // TODO: Fix behavior
            combobox.Instance.GetState(x => x.Text).Should().Be("1");

        }

        // Note: MudSelect doesn't guaranteed the consequences of changing SelectedValues if MultiSelection is false for now.
        // When this feature will add, just uncomment the testcase to test it. No need to write new test.
        /// <summary>The initial SelectedValues are reflected in the combobox value, selection and text.</summary>
        [Test]
        //[TestCase(false)]
        [TestCase(true)]
        public void ComboBox_InitialValuesTest(bool multiSelection)
        {
            var comp = Context.Render<ComboBoxInitialValueTest>(x =>
            {
                x.Add(c => c.SelectedValues, new HashSet<string>() { "1" });
                x.Add(c => c.MultiSelection, multiSelection);
            });
            var combobox = comp.FindComponent<MudComboBox<string>>();

            combobox.Instance.GetState(x => x.Value).Should().Be("1");
            combobox.Instance.SelectedValues.Should().BeEquivalentTo(new HashSet<string>() { "1" });
            combobox.Instance.GetState(x => x.Text).Should().Be("1");
        }

        /// <summary>In multi selection, changing SelectedValues updates the selection but not Value or Text.</summary>
        [Test]
        public void ComboBox_ValueBubblingTest_MultiSelection()
        {
            var comp = Context.Render<ComboBoxInitialValueTest>(x =>
            {
                x.Add(c => c.MultiSelection, true);
            });
            var combobox = comp.FindComponent<MudComboBox<string>>();

            combobox.Instance.GetState(x => x.Value).Should().BeNull();
            combobox.Instance.GetState(x => x.Text).Should().BeNullOrEmpty();

            comp.Render(p => p.Add(x => x.SelectedValues, new HashSet<string>() { "1" }));
            combobox.Instance.GetState(x => x.Value).Should().Be(null);
            combobox.Instance.SelectedValues.Should().BeEquivalentTo(new HashSet<string>() { "1" });
            combobox.Instance.GetState(x => x.Text).Should().Be(null);

            comp.Render(p => p.Add(x => x.SelectedValues, new HashSet<string>() { "2", "1" }));
            combobox.Instance.GetState(x => x.Value).Should().Be(null);
            combobox.Instance.SelectedValues.Should().BeEquivalentTo(new HashSet<string>() { "2", "1" });
            combobox.Instance.GetState(x => x.Text).Should().Be(null);
        }


        /// <summary>In multi selection, SelectedValuesChanged fires only when the selection actually changes.</summary>
        [Test]
        public async Task ComboBox_ValueChangeEventCountTest_MultiSelection()
        {
            var comp = Context.Render<ComboBoxEventCountTest>(x =>
            {
                x.Add(c => c.MultiSelection, true);
            });
            var combobox = comp.FindComponent<MudComboBox<string>>();

            comp.Instance.ValueChangeCount.Should().Be(0);
            comp.Instance.ValuesChangeCount.Should().Be(0);

            await comp.InvokeAsync(() => combobox.Render(p => p.Add(x => x.SelectedValues, new HashSet<string>() { "1" })));
            comp.WaitForAssertion(() => comp.Instance.ValueChangeCount.Should().Be(0));
            comp.WaitForAssertion(() => comp.Instance.ValuesChangeCount.Should().Be(1));


            // Setting same value should not fire events
            await comp.InvokeAsync(() => combobox.Render(p => p.Add(x => x.SelectedValues, new HashSet<string>() { "1" })));
            comp.WaitForAssertion(() => comp.Instance.ValueChangeCount.Should().Be(0));
            comp.Instance.ValuesChangeCount.Should().Be(1);
        }

        /// <summary>
        /// Click should open the Menu and selecting a value should update the bindable value.
        /// </summary>
        [Test]
        public async Task ComboBoxTest1()
        {
            var comp = Context.Render<ComboBoxTest1>();
            // print the generated html
            //Console.WriteLine(comp.Markup);
            // select elements needed for the test
            var combobox = comp.FindComponent<MudComboBox<string>>();
            var menu = comp.Find("div.mud-popover");
            var input = comp.Find("div.mud-input-control");
            // check popover class
            menu.ClassList.Should().Contain("combobox-popover-class");
            // check initial state
            combobox.Instance.GetState(x => x.Value).Should().BeNullOrEmpty();
            comp.WaitForAssertion(() => comp.Find("div.mud-popover").ClassList.Should().Contain("d-none"));
            // click and check if it has toggled the menu
            input.Click();
            menu.ClassList.Should().NotContain("d-none");
            // now click an item and see the value change
            comp.WaitForAssertion(() => comp.FindAll("div.mud-combobox-item").Count.Should().BeGreaterThan(0));
            var items = comp.FindAll("div.mud-combobox-item").ToArray();
            items[1].Click();
            // menu should be closed now
            comp.WaitForAssertion(() => menu.ClassList.Should().Contain("d-none"));
            combobox.Instance.GetState(x => x.Value).Should().Be("2");
            // now we cheat and click the list without opening the menu ;)

            input.Click();
            comp.WaitForAssertion(() => comp.FindAll("div.mud-combobox-item").Count.Should().BeGreaterThan(0));
            items = comp.FindAll("div.mud-combobox-item").ToArray();

            items[0].Click();
            comp.WaitForAssertion(() => combobox.Instance.GetState(x => x.Value).Should().Be("1"));
            //Check user on blur implementation works
            var @switch = comp.FindComponent<MudSwitch<bool>>();
            @switch.Instance.Value = true;
            await comp.InvokeAsync(() => combobox.Instance.HandleOnBlur(new FocusEventArgs()));
            comp.WaitForAssertion(() => @switch.Instance.GetState(x => x.Value).Should().Be(false));
        }

        /// <summary>The clear button appears after a selection and clears the value when clicked.</summary>
        [Test]
        public async Task ComboBoxClearableTest()
        {
            var comp = Context.Render<ComboBoxClearableTest>();
            var combobox = comp.FindComponent<MudComboBox<string>>();
            var input = comp.Find("div.mud-input-control");

            // No button when initialized
            comp.FindAll("button").Should().BeEmpty();

            input.Click();
            comp.WaitForAssertion(() => comp.FindAll("div.mud-combobox-item").Count.Should().BeGreaterThan(0));
            // Button shows after selecting item
            var items = comp.FindAll("div.mud-combobox-item").ToArray();
            items[1].Click();
            //comp.WaitForAssertion(() => comp.Find("div.mud-popover").ClassList.Should().NotContain("mud-popover-open"));
            comp.WaitForAssertion(() => combobox.Instance.GetState(x => x.Value).Should().Be("2"));
            comp.Find("button").Should().NotBeNull();
            // Selection cleared and button removed after clicking clear button
            comp.Find("button").MouseDown();
            comp.WaitForAssertion(() => combobox.Instance.GetState(x => x.Value).Should().BeNullOrEmpty());
            comp.FindAll("button").Should().BeEmpty();
            // Clear button click handler should have been invoked
            comp.Instance.ClearButtonClicked.Should().BeTrue();
        }

        /// <summary>Clicking the input opens the menu and renders all items.</summary>
        [Test]
        public void ComboBox_ClickShouldRenderAllItems()
        {
            var comp = Context.Render<ComboBoxClearableTest>();

            comp.Find("div.mud-input-control").Click();

            comp.WaitForAssertion(() => comp.FindAll("div.mud-combobox-item").Count.Should().Be(3));
        }

        /// <summary>Selecting an item closes the menu in single selection mode.</summary>
        [Test]
        public void ComboBox_SelectingItemShouldCloseMenu()
        {
            var comp = Context.Render<ComboBoxClearableTest>();
            comp.Find("div.mud-input-control").Click();
            comp.WaitForAssertion(() => comp.Find("div.mud-popover").ClassList.Should().NotContain("d-none"));

            comp.FindAll("div.mud-combobox-item")[0].Click();

            comp.WaitForAssertion(() => comp.Find("div.mud-popover").ClassList.Should().Contain("d-none"));
        }

        /// <summary>Selecting an item sets the combobox text to the item's value.</summary>
        [Test]
        public void ComboBox_SelectingItemShouldSetText()
        {
            var comp = Context.Render<ComboBoxClearableTest>();
            var combobox = comp.FindComponent<MudComboBox<string>>();
            comp.Find("div.mud-input-control").Click();
            comp.WaitForAssertion(() => comp.FindAll("div.mud-combobox-item").Count.Should().BeGreaterThan(0));

            comp.FindAll("div.mud-combobox-item")[2].Click();

            comp.WaitForAssertion(() => combobox.Instance.GetState(x => x.Text).Should().Be("3"));
        }

        /// <summary>A disabled combobox does not open its menu when clicked.</summary>
        [Test]
        public void ComboBox_DisabledShouldNotOpenMenu()
        {
            var comp = Context.Render<ComboBoxClearableTest>();
            var combobox = comp.FindComponent<MudComboBox<string>>();
            combobox.Render(p => p.Add(x => x.Disabled, true));

            comp.Find("div.mud-input-control").Click();

            comp.Find("div.mud-popover").ClassList.Should().Contain("d-none");
        }

        /// <summary>A read-only combobox does not open its menu when clicked.</summary>
        [Test]
        public void ComboBox_ReadOnlyShouldNotOpenMenu()
        {
            var comp = Context.Render<ComboBoxClearableTest>();
            var combobox = comp.FindComponent<MudComboBox<string>>();
            combobox.Render(p => p.Add(x => x.ReadOnly, true));

            comp.Find("div.mud-input-control").Click();

            comp.Find("div.mud-popover").ClassList.Should().Contain("d-none");
        }

        /// <summary>The OnOpen callback fires once when the menu opens.</summary>
        [Test]
        public void ComboBox_OnOpenShouldFireWhenOpened()
        {
            var comp = Context.Render<ComboBoxOpenCloseCallbackTest>();

            comp.Find("div.mud-input-control").Click();

            comp.WaitForAssertion(() => comp.Instance.OpenCount.Should().Be(1));
        }

        /// <summary>The OnClose callback fires once when an item is selected and the menu closes.</summary>
        [Test]
        public void ComboBox_OnCloseShouldFireWhenItemSelected()
        {
            var comp = Context.Render<ComboBoxOpenCloseCallbackTest>();
            comp.Find("div.mud-input-control").Click();
            comp.WaitForAssertion(() => comp.FindAll("div.mud-combobox-item").Count.Should().Be(2));

            comp.FindAll("div.mud-combobox-item")[0].Click();

            comp.WaitForAssertion(() => comp.Instance.CloseCount.Should().Be(1));
        }

        /// <summary>NoItemsContent is rendered when the search text matches no item.</summary>
        [Test]
        public async Task ComboBox_MultiSelectEditable_NoMatchShouldRenderNoItemsContent()
        {
            var comp = Context.Render<ComboBoxMultiSelectSearchTest>();
            var combobox = comp.FindComponent<MudComboBox<string>>();
            await comp.InvokeAsync(() => combobox.Find("div.mud-input-control").Click());
            comp.WaitForAssertion(() => combobox.Instance.GetEnabledAndEligibleItems().Count.Should().Be(2));

            await comp.InvokeAsync(() => combobox.Instance.HandleInternalValueChanged("zzz"));
            await comp.InvokeAsync(() => combobox.Instance.ForceRenderItems());

            comp.WaitForAssertion(() => comp.FindAll("span.no-items-marker").Count.Should().Be(1));
        }

        /// <summary>Searching in an editable multi-select combobox leaves only matching items eligible.</summary>
        [Test]
        public async Task ComboBox_MultiSelectEditable_MatchShouldFilterItems()
        {
            var comp = Context.Render<ComboBoxMultiSelectSearchTest>();
            var combobox = comp.FindComponent<MudComboBox<string>>();
            await comp.InvokeAsync(() => combobox.Find("div.mud-input-control").Click());
            comp.WaitForAssertion(() => combobox.Instance.GetEnabledAndEligibleItems().Count.Should().Be(2));

            await comp.InvokeAsync(() => combobox.Instance.HandleInternalValueChanged("app"));
            await comp.InvokeAsync(() => combobox.Instance.ForceRenderItems());

            comp.WaitForAssertion(() => combobox.Instance.GetEnabledAndEligibleItems().Count.Should().Be(1));
        }

        /// <summary>The Label parameter is rendered in a label element.</summary>
        [Test]
        public void ComboBox_LabelShouldBeRendered()
        {
            var comp = Context.Render<ComboBoxFeatureTest>(p => p.Add(x => x.Label, "My label"));

            comp.FindAll("label").Should().Contain(x => x.TextContent.Contains("My label"));
        }

        /// <summary>The HelperText parameter is rendered in the markup.</summary>
        [Test]
        public void ComboBox_HelperTextShouldBeRendered()
        {
            var comp = Context.Render<ComboBoxFeatureTest>(p => p.Add(x => x.HelperText, "Helpful hint"));

            comp.Markup.Should().Contain("Helpful hint");
        }

        /// <summary>A user-supplied CSS class is applied to the combobox.</summary>
        [Test]
        public void ComboBox_CustomClassShouldBeApplied()
        {
            var comp = Context.Render<ComboBoxFeatureTest>(p => p.Add(x => x.ComboClass, "my-custom-class"));

            comp.FindAll("div.my-custom-class").Count.Should().BeGreaterThan(0);
        }

        /// <summary>Clicking a disabled item does not select it.</summary>
        [Test]
        [Ignore("Known bug: MudComboBoxItem.HandleOnClick does not check Disabled, so disabled items can be selected.")]
        public void ComboBox_DisabledItemShouldNotBeSelectable()
        {
            var comp = Context.Render<ComboBoxFeatureTest>();
            var combobox = comp.FindComponent<MudComboBox<string>>();
            comp.Find("div.mud-input-control").Click();
            comp.WaitForAssertion(() => comp.FindAll("div.mud-combobox-item").Count.Should().Be(3));

            comp.FindAll("div.mud-combobox-item")[2].Click();

            combobox.Instance.GetState(x => x.Value).Should().BeNullOrEmpty();
        }

        /// <summary>Disabled items are excluded from the enabled and eligible item list.</summary>
        [Test]
        public void ComboBox_DisabledItemShouldNotBeEnabledAndEligible()
        {
            var comp = Context.Render<ComboBoxFeatureTest>();
            var combobox = comp.FindComponent<MudComboBox<string>>();

            combobox.Instance.GetEnabledAndEligibleItems().Count.Should().Be(2);
        }

        /// <summary>An item's Text is displayed in the list instead of its raw value.</summary>
        [Test]
        public void ComboBox_ItemTextShouldBeUsedAsDisplayText()
        {
            var comp = Context.Render<ComboBoxFeatureTest>();
            comp.Find("div.mud-input-control").Click();

            comp.WaitForAssertion(() => comp.FindAll("div.mud-combobox-item")[0].TextContent.Should().Contain("Apple"));
        }

        /// <summary>A required combobox shows the required error when validated without a value.</summary>
        [Test]
        public async Task ComboBox_RequiredShouldShowErrorWhenValidatedWithoutValue()
        {
            var comp = Context.Render<ComboBoxFeatureTest>(p => p.Add(x => x.Required, true));
            var combobox = comp.FindComponent<MudComboBox<string>>();

            await comp.InvokeAsync(() => combobox.Instance.ValidateAsync());

            comp.WaitForAssertion(() => comp.Markup.Should().Contain("Value is required"));
        }

        /// <summary>A required combobox shows no error when validated with a selected value.</summary>
        [Test]
        public async Task ComboBox_RequiredShouldNotShowErrorWhenValueSelected()
        {
            var comp = Context.Render<ComboBoxFeatureTest>(p => p.Add(x => x.Required, true));
            var combobox = comp.FindComponent<MudComboBox<string>>();
            comp.Find("div.mud-input-control").Click();
            comp.WaitForAssertion(() => comp.FindAll("div.mud-combobox-item").Count.Should().Be(3));
            comp.FindAll("div.mud-combobox-item")[0].Click();

            await comp.InvokeAsync(() => combobox.Instance.ValidateAsync());

            comp.WaitForAssertion(() => comp.Markup.Should().NotContain("Value is required"));
        }

        /// <summary>With Highlight enabled, the searched text is wrapped in a mark element.</summary>
        [Test]
        public async Task ComboBox_HighlightShouldMarkSearchedText()
        {
            var comp = Context.Render<ComboBoxFeatureTest>(p => p.Add(x => x.Highlight, true));
            var combobox = comp.FindComponent<MudComboBox<string>>();
            comp.Find("div.mud-input-control").Click();

            await comp.InvokeAsync(() => combobox.Instance.HandleInternalValueChanged("app"));
            await comp.InvokeAsync(() => combobox.Instance.ForceRenderItems());

            comp.WaitForAssertion(() => comp.FindAll("mark").Count.Should().BeGreaterThan(0));
        }

        /// <summary>With Highlight disabled, no mark element is rendered.</summary>
        [Test]
        public void ComboBox_HighlightDisabledShouldNotRenderMark()
        {
            var comp = Context.Render<ComboBoxFeatureTest>();
            comp.Find("div.mud-input-control").Click();

            comp.WaitForAssertion(() => comp.FindAll("div.mud-combobox-item").Count.Should().Be(3));
            comp.FindAll("mark").Should().BeEmpty();
        }

        /// <summary>With EnableFilter false, all items stay eligible regardless of the search text.</summary>
        [Test]
        public async Task ComboBox_EnableFilterFalseShouldKeepAllItemsEligible()
        {
            var comp = Context.Render<ComboBoxFilterDisabledTest>();
            var combobox = comp.FindComponent<MudComboBox<string>>();
            await comp.InvokeAsync(() => combobox.Find("div.mud-input-control").Click());
            comp.WaitForAssertion(() => combobox.Instance.GetEnabledAndEligibleItems().Count.Should().Be(2));

            await comp.InvokeAsync(() => combobox.Instance.HandleInternalValueChanged("app"));
            await comp.InvokeAsync(() => combobox.Instance.ForceRenderItems());

            comp.WaitForAssertion(() => combobox.Instance.GetEnabledAndEligibleItems().Count.Should().Be(2));
        }

        /// <summary>An editable combobox filters items by the search text.</summary>
        [Test]
        public async Task ComboBox_EditableShouldFilterItemsBySearchText()
        {
            var comp = Context.Render<ComboBoxMultiSelectSearchTest>(p => p.Add(x => x.EnableFilter, true));
            var combobox = comp.FindComponent<MudComboBox<string>>();
            await comp.InvokeAsync(() => combobox.Find("div.mud-input-control").Click());

            await comp.InvokeAsync(() => combobox.Instance.HandleInternalValueChanged("ban"));
            await comp.InvokeAsync(() => combobox.Instance.ForceRenderItems());

            comp.WaitForAssertion(() => combobox.Instance.GetEnabledAndEligibleItems().Select(x => x.Value).Should().BeEquivalentTo(new[] { "banana" }));
        }

        /// <summary>ItemTemplate is used to present the selected value.</summary>
        [Test]
        public async Task ComboBox_ItemTemplateShouldRenderForSelectedValue()
        {
            var comp = Context.Render<ComboBoxItemTemplateTest>();
            var combobox = comp.FindComponent<MudComboBox<string>>();
            comp.Find("div.mud-input-control").Click();
            comp.WaitForAssertion(() => comp.FindAll("div.mud-combobox-item").Count.Should().Be(2));

            comp.FindAll("div.mud-combobox-item")[0].Click();

            await comp.WaitForAssertionAsync(() => comp.FindAll("span.item-template-marker").Count.Should().BeGreaterThan(0));
        }

        /// <summary>Item child content is rendered for each item in the list.</summary>
        [Test]
        public void ComboBox_ItemContentShouldRenderInList()
        {
            var comp = Context.Render<ComboBoxItemTemplateTest>();
            comp.Find("div.mud-input-control").Click();

            comp.WaitForAssertion(() => comp.FindAll("span.item-content-marker").Count.Should().Be(2));
        }

        /// <summary>The adornment icon switches between drop-down and drop-up when the menu opens.</summary>
        [Test]
        public void ComboBox_OpenCloseIconTest()
        {
            var comp = Context.Render<ComboBoxTest1>();
            var combobox = comp.FindComponent<MudComboBox<string>>();

            combobox.FindComponents<MudIcon>().Should().ContainSingle(x => x.Instance.Icon == Icons.Material.Filled.ArrowDropDown);

            combobox.Find("div.mud-input-control").Click();
            comp.WaitForAssertion(() => combobox.FindComponents<MudIcon>().Should().ContainSingle(x => x.Instance.Icon == Icons.Material.Filled.ArrowDropUp));
        }

        /// <summary>Clicking the select-all item selects every item, and clicking it again deselects them all.</summary>
        [Test]
        public void MultiSelect_SelectAll()
        {
            var comp = Context.Render<ComboBoxMultiSelectTest2>();
            // select element needed for the test
            var combobox = comp.FindComponent<MudComboBox<string>>();
            var menu = comp.Find("div.mud-popover");
            var input = combobox.Find("div.mud-input-control");
            // Open the menu
            input.Click();
            comp.WaitForAssertion(() => menu.ClassList.Should().Contain("mud-popover-open"));

            comp.FindAll("div.mud-combobox-item")[0].Click();

            // validate the result. all items should be selected
            comp.WaitForAssertion(() => combobox.Instance.GetPresenterText().Should().Be("FirstA^SecondA^ThirdA"));
            combobox.Instance.SelectedValues.Should().BeEquivalentTo(new HashSet<string>() { "FirstA", "SecondA", "ThirdA" });

            comp.FindAll("div.mud-combobox-item")[0].Click();
            comp.WaitForAssertion(() => combobox.Instance.GetPresenterText().Should().Be(""));
            combobox.Instance.SelectedValues.Should().BeEquivalentTo(new HashSet<string>());
        }

        /// <summary>
        /// When SelectAll is enabled, selecting all items must invoke SelectedValuesChanged exactly once.
        /// </summary>
        [Test]
        public void MultiSelect_SelectAllEnabled_SelectedValuesChangedOnce()
        {
            var comp = Context.Render<ComboBoxMultiSelectSelectAllEnabledSelectedValuesChanged>();
            var combobox = comp.FindComponent<MudComboBox<string>>();
            var menu = comp.Find("div.mud-popover");
            var input = combobox.Find("div.mud-input-control");
            input.Click();
            comp.WaitForAssertion(() => menu.ClassList.Should().Contain("mud-popover-open"));

            comp.FindAll("div.mud-combobox-item")[0].Click();

            comp.WaitForAssertion(() => comp.Instance.Count.Should().Be(1));
            combobox.Instance.SelectedValues.Should().HaveCount(1000);
        }
		/// <summary>
		/// Deselecting all items after selecting all must invoke SelectedValuesChanged exactly once more.
		/// </summary>
		[Test]
		public void MultiSelect_SelectAllEnabled_DeselectAll_SelectedValuesChangedOnce()
		{
			var comp = Context.Render<ComboBoxMultiSelectSelectAllEnabledSelectedValuesChanged>();
			var combobox = comp.FindComponent<MudComboBox<string>>();
			var menu = comp.Find("div.mud-popover");
			combobox.Find("div.mud-input-control").Click();
			comp.WaitForAssertion(() => menu.ClassList.Should().Contain("mud-popover-open"));
			comp.FindAll("div.mud-combobox-item")[0].Click();
			comp.WaitForAssertion(() => combobox.Instance.SelectedValues.Should().HaveCount(1000));

			comp.FindAll("div.mud-combobox-item")[0].Click();

			comp.WaitForAssertion(() => combobox.Instance.SelectedValues.Should().BeEmpty());
			comp.Instance.Count.Should().Be(2);
		}

		/// <summary>
		/// Selecting a single item in multi selection must invoke SelectedValuesChanged exactly once.
		/// </summary>
		[Test]
		public void MultiSelect_SelectOneItem_SelectedValuesChangedOnce()
		{
			var comp = Context.Render<ComboBoxMultiSelectSelectOneSelectedValuesChanged>();
			var combobox = comp.FindComponent<MudComboBox<string>>();
			var menu = comp.Find("div.mud-popover");
			combobox.Find("div.mud-input-control").Click();
			comp.WaitForAssertion(() => menu.ClassList.Should().Contain("mud-popover-open"));

			comp.FindAll("div.mud-combobox-item")[0].Click();

			comp.WaitForAssertion(() => combobox.Instance.SelectedValues.Should().BeEquivalentTo(new[] { "A" }));
			comp.Instance.Count.Should().Be(1);
		}

		/// <summary>
		/// Closing a chip must invoke SelectedValuesChanged exactly once.
		/// </summary>
		[Test]
		public void MultiSelect_ChipClosed_SelectedValuesChangedOnce()
		{
			var comp = Context.Render<ComboBoxMultiSelectChipCloseSelectedValuesChanged>();
			var combobox = comp.FindComponent<MudComboBox<string>>();

			comp.Find("button.mud-chip-close-button").Click();

			comp.WaitForAssertion(() => combobox.Instance.SelectedValues.Should().HaveCount(1));
			comp.Instance.Count.Should().Be(1);
		}

        /// <summary>An editable multi-select combobox lists all four items, then only the two matching "t" after filtering.</summary>
        [Test]
        public async Task ComboBox_MultiSelectEditable()
        {
            var comp = Context.Render<ComboBoxMultiSelectEditableTest>();
            // print the generated html
            Console.WriteLine(comp.Markup);
            // select elements needed for the test
            var combobox = comp.FindComponent<MudComboBox<string>>();
            var menu = comp.Find("div.mud-popover");
            var input = combobox.Find("div.mud-input-control");
            // check initial state
            combobox.Instance.GetState(x => x.Value).Should().BeNullOrEmpty();
            comp.WaitForAssertion(() =>
                comp.Find("div.mud-popover").ClassList.Should().Contain("d-none"));
            // click and check if it has toggled the menu
            await comp.InvokeAsync(() => input.Click());
            comp.WaitForAssertion(() => comp.Find("div.mud-popover").ClassList.Should().NotContain("d-none"));
            // now click an item and see the value change
            comp.WaitForAssertion(() => comp.FindAll("div.mud-combobox-item").Count.Should().Be(4));
            comp.WaitForAssertion(() => combobox.Instance.GetEnabledAndEligibleItems().Count.Should().Be(4));
            var items = comp.FindAll("div.mud-combobox-item").ToArray();
            await comp.InvokeAsync(() => combobox.Instance.HandleInternalValueChanged("t"));
            await comp.InvokeAsync(() => combobox.Instance.ForceRenderItems());
            items = comp.FindAll("div.mud-combobox-item").ToArray();
            comp.WaitForAssertion(() => combobox.Instance.GetEnabledAndEligibleItems().Count.Should().Be(2));
        }

        /// <summary>
        /// Click should not close the menu and selecting multiple values should update the bindable value with a comma separated list.
        /// </summary>
        [Test]
        public async Task ComboBox_MultiSelectTest1()
        {
            var comp = Context.Render<ComboBoxMultiSelectTest1>();
            // print the generated html
            Console.WriteLine(comp.Markup);
            // select elements needed for the test
            var combobox = comp.FindComponent<MudComboBox<string>>();
            var menu = comp.Find("div.mud-popover");
            var input = combobox.Find("div.mud-input-control");
            // check initial state
            combobox.Instance.GetState(x => x.Value).Should().BeNullOrEmpty();
            comp.WaitForAssertion(() =>
                comp.Find("div.mud-popover").ClassList.Should().Contain("d-none"));
            // click and check if it has toggled the menu
            await comp.InvokeAsync(() => input.Click());
            comp.WaitForAssertion(() => comp.Find("div.mud-popover").ClassList.Should().NotContain("d-none"));
            // now click an item and see the value change
            comp.WaitForAssertion(() => comp.FindAll("div.mud-combobox-item").Count.Should().BeGreaterThan(0));
            var items = comp.FindAll("div.mud-combobox-item").ToArray();
            items[1].Click();
            // menu should still be open now!!
            comp.Find("div.mud-popover").ClassList.Should().NotContain("d-none");
            comp.WaitForAssertion(() => combobox.Instance.GetPresenterText().Should().Be("2"));
            items[0].Click();
            comp.WaitForAssertion(() => combobox.Instance.GetPresenterText().Should().Be("2, 1"));
            items[2].Click();
            comp.WaitForAssertion(() => combobox.Instance.GetPresenterText().Should().Be("2, 1, 3"));
            items[0].Click();
            comp.WaitForAssertion(() => combobox.Instance.GetPresenterText().Should().Be("2, 3"));
            combobox.Instance.SelectedValues?.Count().Should().Be(2);
            combobox.Instance.SelectedValues.Should().Contain("2");
            combobox.Instance.SelectedValues.Should().Contain("3");
            //Console.WriteLine(comp.Markup);
            const string @unchecked =
                "M19 5v14H5V5h14m0-2H5c-1.1 0-2 .9-2 2v14c0 1.1.9 2 2 2h14c1.1 0 2-.9 2-2V5c0-1.1-.9-2-2-2z";
            const string @checked =
                "M19 3H5c-1.11 0-2 .9-2 2v14c0 1.1.89 2 2 2h14c1.11 0 2-.9 2-2V5c0-1.1-.89-2-2-2zm-9 14l-5-5 1.41-1.41L10 14.17l7.59-7.59L19 8l-9 9z";
            // check that the correct items are checked
            comp.WaitForAssertion(() =>
                comp.FindAll("div.mud-combobox-item path")[1]?.Attributes["d"]?.Value.Should().Be(@unchecked));
            comp.FindAll("div.mud-combobox-item path")[3]?.Attributes["d"]?.Value.Should().Be(@checked);
            comp.FindAll("div.mud-combobox-item path")[5]?.Attributes["d"]?.Value.Should().Be(@checked);
            // now check how setting the SelectedValues makes items checked or unchecked
            // Note: If popover is open, selecting values programmatically doesn't work for now.
            await comp.InvokeAsync(() => combobox.Instance.CloseMenu());
            await comp.InvokeAsync(() =>
            {
                combobox.Instance.SelectedValues = new HashSet<string>() { "1", "2" };
            });
            await comp.InvokeAsync(() => input.Click());
            comp.WaitForAssertion(() => comp.FindAll("div.mud-combobox-item path")[1]?.Attributes["d"]?.Value.Should().Be(@checked));
            comp.FindAll("div.mud-combobox-item path")[3]?.Attributes["d"]?.Value.Should().Be(@checked);
            combobox.Instance.SelectedValues.Should().NotContain("3");
            comp.WaitForAssertion(() => comp.FindAll("div.mud-combobox-item path")[5]?.Attributes["d"]?.Value.Should().Be(@unchecked));
            //Console.WriteLine(comp.Markup);
        }

        /// <summary>Keyboard keys open and close the menu and select items in single selection mode, skipping disabled items.</summary>
        [Test]
        public async Task ComboBoxTest_KeyboardNavigation_SingleSelect()
        {
            var comp = Context.Render<ComboBoxTest1>();
            // print the generated html
            //Console.WriteLine(comp.Markup);
            // select elements needed for the test
            var combobox = comp.FindComponent<MudComboBox<string>>().Instance;

            await comp.InvokeAsync(() => combobox.HandleKeyDownAsync(new KeyboardEventArgs() { Key = "Enter", Type = "keydown", }));
            comp.Render();
            comp.WaitForAssertion(() => comp.Find("div.mud-popover").ClassList.Should().NotContain("d-none"));

            await comp.InvokeAsync(() => combobox.HandleKeyDownAsync(new KeyboardEventArgs() { Key = "Escape", Type = "keydown", }));
            comp.Render();
            comp.WaitForAssertion(() => comp.Find("div.mud-popover").ClassList.Should().Contain("d-none"));

            await comp.InvokeAsync(() => combobox.HandleKeyDownAsync(new KeyboardEventArgs() { Key = " ", Type = "keydown", }));
            comp.Render();
            comp.WaitForAssertion(() => comp.Find("div.mud-popover").ClassList.Should().NotContain("d-none"));
            //If we didn't select an item with mouse or arrow keys yet, value should remains null.
            await comp.InvokeAsync(() => combobox.HandleKeyDownAsync(new KeyboardEventArgs() { Key = " ", Type = "keydown", }));
            comp.Render();
            comp.WaitForAssertion(() => comp.Find("div.mud-popover").ClassList.Should().Contain("d-none"));
            comp.WaitForAssertion(() => combobox.GetState(x => x.Value).Should().Be(null));

            await comp.InvokeAsync(() => combobox.HandleKeyDownAsync(new KeyboardEventArgs() { Key = "ArrowDown", AltKey = true, Type = "keydown", }));
            comp.Render();
            comp.WaitForAssertion(() => comp.Find("div.mud-popover").ClassList.Should().NotContain("d-none"));

            await comp.InvokeAsync(() => combobox.HandleKeyDownAsync(new KeyboardEventArgs() { Key = "ArrowUp", AltKey = true, Type = "keydown", }));
            comp.Render();
            comp.WaitForAssertion(() => comp.Find("div.mud-popover").ClassList.Should().Contain("d-none"));
            //If dropdown is closed, arrow key should not set a value.
            await comp.InvokeAsync(() => combobox.HandleKeyDownAsync(new KeyboardEventArgs() { Key = "ArrowDown", Type = "keydown", }));
            comp.Render();
            comp.WaitForAssertion(() => comp.Find("div.mud-popover").ClassList.Should().NotContain("d-none"));
            comp.WaitForAssertion(() => combobox.GetState(x => x.Value).Should().Be(null));
            // If no item is hiligted, enter should only close popover, not select any item and value
            await comp.InvokeAsync(() => combobox.HandleKeyDownAsync(new KeyboardEventArgs() { Key = "Escape", Type = "keydown", }));
            comp.Render();
            comp.WaitForAssertion(() => comp.Find("div.mud-popover").ClassList.Should().Contain("d-none"));
            comp.WaitForAssertion(() => combobox.GetState(x => x.Value).Should().Be(null));

            await comp.InvokeAsync(() => combobox.HandleKeyDownAsync(new KeyboardEventArgs() { Key = "Enter", Type = "keydown", }));
            comp.Render();
            comp.WaitForAssertion(() => comp.Find("div.mud-popover").ClassList.Should().NotContain("d-none"));

            await comp.InvokeAsync(() => combobox.HandleKeyDownAsync(new KeyboardEventArgs() { Key = "ArrowDown", Type = "keydown", }));
            comp.WaitForAssertion(() => combobox.GetState(x => x.Value).Should().BeNull());

            await comp.InvokeAsync(() => combobox.HandleKeyDownAsync(new KeyboardEventArgs() { Key = "ArrowDown", Type = "keydown", }));
            await comp.InvokeAsync(() => combobox.HandleKeyDownAsync(new KeyboardEventArgs() { Key = "Enter", Type = "keydown", }));
            comp.WaitForAssertion(() => combobox.GetState(x => x.Value).Should().Be("3"));
            //End key should not select the last disabled item
            await comp.InvokeAsync(() => combobox.HandleKeyDownAsync(new KeyboardEventArgs() { Key = "Enter", Type = "keydown", }));
            await comp.InvokeAsync(() => combobox.HandleKeyDownAsync(new KeyboardEventArgs() { Key = "End", Type = "keydown", }));
            await comp.InvokeAsync(() => combobox.HandleKeyDownAsync(new KeyboardEventArgs() { Key = "Enter", Type = "keydown", }));
            comp.WaitForAssertion(() =>  combobox.GetState(x => x.Value).Should().Be("3"));

            await comp.InvokeAsync(() => combobox.HandleKeyDownAsync(new KeyboardEventArgs() { Key = "Enter", Type = "keydown", }));
            comp.WaitForAssertion(() => comp.Find("div.mud-popover").ClassList.Should().Contain("mud-popover-open"));
            await comp.InvokeAsync(() => combobox.HandleKeyDownAsync(new KeyboardEventArgs() { Key = "ArrowDown", Type = "keydown", }));
            await comp.InvokeAsync(() => combobox.HandleKeyDownAsync(new KeyboardEventArgs() { Key = "Enter", Type = "keydown", }));
            comp.Render();
            comp.WaitForAssertion(() => comp.Find("div.mud-popover").ClassList.Should().Contain("d-none"));
            comp.WaitForAssertion(() => combobox.GetState(x => x.Value).Should().Be("1"));

            await comp.InvokeAsync(() => combobox.HandleKeyDownAsync(new KeyboardEventArgs() { Key = "Enter", Type = "keydown", }));
            await comp.InvokeAsync(() => combobox.HandleKeyDownAsync(new KeyboardEventArgs() { Key = "ArrowDown", Type = "keydown", }));
            await comp.InvokeAsync(() => combobox.HandleKeyDownAsync(new KeyboardEventArgs() { Key = "Enter", Type = "keydown", }));
            comp.WaitForAssertion(() => combobox.GetState(x => x.Value).Should().Be("2"));

            await comp.InvokeAsync(() => combobox.HandleKeyDownAsync(new KeyboardEventArgs() { Key = "Enter", Type = "keydown", }));
            await comp.InvokeAsync(() => combobox.HandleKeyDownAsync(new KeyboardEventArgs() { Key = "Home", Type = "keydown", }));
            await comp.InvokeAsync(() => combobox.HandleKeyDownAsync(new KeyboardEventArgs() { Key = "Enter", Type = "keydown", }));
            comp.WaitForAssertion(() => combobox.GetState(x => x.Value).Should().Be("1"));
            //Arrow up should select still the first item
            await comp.InvokeAsync(() => combobox.HandleKeyDownAsync(new KeyboardEventArgs() { Key = "Enter", Type = "keydown", }));
            await comp.InvokeAsync(() => combobox.HandleKeyDownAsync(new KeyboardEventArgs() { Key = "ArrowUp", Type = "keydown", }));
            await comp.InvokeAsync(() => combobox.HandleKeyDownAsync(new KeyboardEventArgs() { Key = "Enter", Type = "keydown", }));
            comp.WaitForAssertion(() => combobox.GetState(x => x.Value).Should().Be("3"));

            await comp.InvokeAsync(() => combobox.HandleKeyDownAsync(new KeyboardEventArgs() { Key = "Enter", Type = "keydown", }));
            await comp.InvokeAsync(() => combobox.HandleKeyDownAsync(new KeyboardEventArgs() { Key = "End", Type = "keydown", }));
            await comp.InvokeAsync(() => combobox.HandleKeyDownAsync(new KeyboardEventArgs() { Key = "ArrowDown", Type = "keydown", }));
            await comp.InvokeAsync(() => combobox.HandleKeyDownAsync(new KeyboardEventArgs() { Key = "Enter", Type = "keydown", }));
            comp.WaitForAssertion(() => combobox.GetState(x => x.Value).Should().Be("1"));

            await comp.InvokeAsync(() => combobox.HandleKeyDownAsync(new KeyboardEventArgs() { Key = "Enter", Type = "keydown", }));
            await comp.InvokeAsync(() => combobox.HandleKeyDownAsync(new KeyboardEventArgs() { Key = "2", Type = "keydown", }));
            comp.WaitForAssertion(() => combobox.GetState(x => x.Value).Should().Be("1"));

            await comp.InvokeAsync(() => combobox.HandleKeyDownAsync(new KeyboardEventArgs() { Key = "Enter", Type = "keydown", }));
            comp.WaitForAssertion(() => combobox.GetState(x => x.Value).Should().Be("2"));
        }

        /// <summary>With ToggleSelection enabled, pressing Enter on the selected item clears the value.</summary>
        [Test]
        public async Task ComboBoxTest_KeyboardNavigation_ToggleSelect()
        {
            var comp = Context.Render<ComboBoxTest1>();
            var combobox = comp.FindComponent<MudComboBox<string>>().Instance;
            combobox.ToggleSelection = true;
            await comp.InvokeAsync(() => combobox.HandleKeyDownAsync(new KeyboardEventArgs() { Key = "Enter", Type = "keydown", }));
            comp.Render();
            comp.WaitForAssertion(() => comp.Find("div.mud-popover").ClassList.Should().NotContain("d-none"));

            await comp.InvokeAsync(() => combobox.HandleKeyDownAsync(new KeyboardEventArgs() { Key = "ArrowDown", Type = "keydown", }));
            comp.WaitForAssertion(() => combobox.GetState(x => x.Value).Should().BeNull());

            await comp.InvokeAsync(() => combobox.HandleKeyDownAsync(new KeyboardEventArgs() { Key = "ArrowDown", Type = "keydown", }));
            await comp.InvokeAsync(() => combobox.HandleKeyDownAsync(new KeyboardEventArgs() { Key = "Enter", Type = "keydown", }));
            comp.WaitForAssertion(() => combobox.GetState(x => x.Value).Should().Be("3"));
            //End key should not select the last disabled item
            await comp.InvokeAsync(() => combobox.HandleKeyDownAsync(new KeyboardEventArgs() { Key = "Enter", Type = "keydown", }));
            await comp.InvokeAsync(() => combobox.HandleKeyDownAsync(new KeyboardEventArgs() { Key = "Enter", Type = "keydown", }));
            comp.WaitForAssertion(() => combobox.GetState(x => x.Value).Should().BeNull());
        }

        /// <summary>Keyboard keys select, select all and toggle items in multi selection mode, and are ignored when disabled.</summary>
        [Test]
        public async Task ComboBoxTest_KeyboardNavigation_MultiSelect()
        {
            var comp = Context.Render<ComboBoxMultiSelectTest3>();
            var combobox = comp.FindComponent<MudComboBox<string>>();
            comp.WaitForAssertion(() => comp.Find("div.mud-popover").ClassList.Should().Contain("d-none"));

            await comp.InvokeAsync(() => combobox.Instance.HandleKeyDownAsync(new KeyboardEventArgs() { Key = " ", Type = "keydown", }));
            comp.Render();
            comp.WaitForAssertion(() => comp.Find("div.mud-popover").ClassList.Should().NotContain("d-none"));

            await comp.InvokeAsync(() => combobox.Instance.HandleKeyDownAsync(new KeyboardEventArgs() { Key = "a", CtrlKey = true, Type = "keydown", }));
            comp.WaitForAssertion(() => combobox.Instance.GetPresenterText().Should().Be("7 felines have been selected"));

            await comp.InvokeAsync(() => combobox.Instance.HandleKeyDownAsync(new KeyboardEventArgs() { Key = "A", CtrlKey = true, Type = "keydown", }));
            comp.WaitForAssertion(() => combobox.Instance.GetPresenterText().Should().BeNull());

            await comp.InvokeAsync(() => combobox.Instance.HandleKeyDownAsync(new KeyboardEventArgs() { Key = "ArrowDown", Type = "keydown", }));
            await comp.InvokeAsync(() => combobox.Instance.HandleKeyDownAsync(new KeyboardEventArgs() { Key = "Enter", Type = "keydown", }));
            comp.WaitForAssertion(() => combobox.Instance.GetPresenterText().Should().Be("1 feline has been selected"));

            await comp.InvokeAsync(() => combobox.Instance.HandleKeyDownAsync(new KeyboardEventArgs() { Key = "A", CtrlKey = true, Type = "keydown", }));
            comp.WaitForAssertion(() => combobox.Instance.GetPresenterText().Should().Be("7 felines have been selected"));

            await comp.InvokeAsync(() => combobox.Instance.HandleKeyDownAsync(new KeyboardEventArgs() { Key = "Escape", Type = "keydown", }));
            comp.Render();
            comp.WaitForAssertion(() => comp.Find("div.mud-popover").ClassList.Should().Contain("d-none"));

            await comp.InvokeAsync(() => combobox.Instance.HandleKeyDownAsync(new KeyboardEventArgs() { Key = "Enter", Type = "keydown", }));
            comp.Render();
            comp.WaitForAssertion(() => comp.Find("div.mud-popover").ClassList.Should().NotContain("d-none"));

            await comp.InvokeAsync(() => combobox.Instance.HandleKeyDownAsync(new KeyboardEventArgs() { Key = "ArrowDown", Type = "keydown", }));
            await comp.InvokeAsync(() => combobox.Instance.HandleKeyDownAsync(new KeyboardEventArgs() { Key = "Enter", Type = "keydown", }));
            comp.WaitForAssertion(() => combobox.Instance.SelectedValues.Should().NotContain("Jaguar"));

            await comp.InvokeAsync(() => combobox.Instance.HandleKeyDownAsync(new KeyboardEventArgs() { Key = "Home", Type = "keydown", }));
            await comp.InvokeAsync(() => combobox.Instance.HandleKeyDownAsync(new KeyboardEventArgs() { Key = "NumpadEnter", Type = "keydown", }));
            comp.WaitForAssertion(() => combobox.Instance.SelectedValues.Should().Contain("Jaguar"));

            await comp.InvokeAsync(() => combobox.Instance.HandleKeyDownAsync(new KeyboardEventArgs() { Key = "ArrowDown", Type = "keydown", }));
            await comp.InvokeAsync(() => combobox.Instance.HandleKeyDownAsync(new KeyboardEventArgs() { Key = "Enter", Type = "keydown", }));
            comp.WaitForAssertion(() => combobox.Instance.SelectedValues.Should().NotContain("Leopard"));

            await comp.InvokeAsync(() => combobox.Instance.HandleKeyDownAsync(new KeyboardEventArgs() { Key = "End", Type = "keydown", }));
            await comp.InvokeAsync(() => combobox.Instance.HandleKeyDownAsync(new KeyboardEventArgs() { Key = "Enter", Type = "keydown", }));
            comp.WaitForAssertion(() => combobox.Instance.SelectedValues.Should().NotContain("Tiger"));

            await comp.InvokeAsync(() => combobox.Render(p => p.Add(x => x.Disabled, true)));
            await comp.InvokeAsync(() => combobox.Instance.HandleKeyDownAsync(new KeyboardEventArgs() { Key = "Enter", Type = "keydown", }));
            comp.WaitForAssertion(() => combobox.Instance.SelectedValues.Should().NotContain("Tiger"));

            await comp.InvokeAsync(() => combobox.Render(p => p.Add(x => x.Disabled, false)));
            //Test the keyup event
            await comp.InvokeAsync(() => combobox.Instance.HandleKeyUpAsync(new KeyboardEventArgs() { Key = "Enter", Type = "keyup", }));
            comp.WaitForAssertion(() => combobox.Instance.SelectedValues.Should().NotContain("Tiger"));

            await comp.InvokeAsync(() => combobox.Instance.HandleKeyDownAsync(new KeyboardEventArgs() { Key = "Tab", Type = "keydown", }));
            await comp.InvokeAsync(() => combobox.Instance.OnKeyUp.InvokeAsync(new KeyboardEventArgs() { Key = "Tab" }));
            comp.Render(); // <-- this is necessary for reliable passing of the test
            comp.WaitForAssertion(() => comp.Find("div.mud-popover").ClassList.Should().Contain("d-none"));
        }


    }
}
