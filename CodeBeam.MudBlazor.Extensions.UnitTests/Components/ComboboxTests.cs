#pragma warning disable CS1998 // async without await
#pragma warning disable IDE1006 // leading underscore
#pragma warning disable BL0005 // Set parameter outside component

using Bunit;
using MudExtensions.UnitTests.TestComponents;
using FluentAssertions;
using Microsoft.AspNetCore.Components.Web;
using MudExtensions;
using NUnit.Framework;
using MudBlazor;
using MudExtensions.UnitTests.Extensions;

namespace MudExtensions.UnitTests.Components
{
    [TestFixture]
    public class ComboboxTests : BunitTest
    {


        // Note: MudSelect doesn't guaranteed the consequences of changing Value if MultiSelection is true for now.
        // When this feature will add, just uncomment the testcase to test it. No need to write new test.
        [Test]
        [TestCase(false)]
        //[TestCase(true)]
        public void Combobox_InitialValueTest(bool multiSelection)
        {
            var comp = Context.RenderComponent<ComboboxInitialValueTest>(x =>
            {
                x.Add(c => c.SelectedValue, "1");
                x.Add(c => c.MultiSelection, multiSelection);
            });
            var combobox = comp.FindComponent<MudComboBox<string>>();

            combobox.Instance.Value.Should().Be("1");
            combobox.Instance.SelectedValues.Should().BeEquivalentTo(new HashSet<string>() { "1" });
            combobox.Instance.Text.Should().Be("1");
        }

        // Note: MudSelect doesn't guaranteed the consequences of changing SelectedValues if MultiSelection is false for now.
        // When this feature will add, just uncomment the testcase to test it. No need to write new test.
        [Test]
        //[TestCase(false)]
        [TestCase(true)]
        public void Combobox_InitialValuesTest(bool multiSelection)
        {
            var comp = Context.RenderComponent<ComboboxInitialValueTest>(x =>
            {
                x.Add(c => c.SelectedValues, new HashSet<string>() { "1" });
                x.Add(c => c.MultiSelection, multiSelection);
            });
            var combobox = comp.FindComponent<MudComboBox<string>>();

            combobox.Instance.Value.Should().Be("1");
            combobox.Instance.SelectedValues.Should().BeEquivalentTo(new HashSet<string>() { "1" });
            combobox.Instance.Text.Should().Be("1");
        }

        [Test]
        public async Task Combobox_ValueBubblingTest()
        {
            var comp = Context.RenderComponent<ComboboxInitialValueTest>();
            var combobox = comp.FindComponent<MudComboBox<string>>();

            combobox.Instance.Value.Should().BeNull();
            combobox.Instance.Text.Should().BeNull();

            comp.SetParam("SelectedValue", "1");
            await comp.InvokeAsync(() => combobox.Instance.ForceUpdate());
            comp.WaitForAssertion(() => combobox.Instance.Value.Should().Be("1"));
            combobox.Instance.SelectedValues.Should().BeEquivalentTo(new HashSet<string>() { "1" });
            combobox.Instance.Text.Should().Be("1");

            comp.SetParam("SelectedValue", "2");
            comp.WaitForAssertion(() => combobox.Instance.Value.Should().Be("2"));
            combobox.Instance.SelectedValues.Should().BeEquivalentTo(new HashSet<string>() { "1" });
            await comp.InvokeAsync(() => combobox.Instance.ForceUpdate());
            combobox.Instance.SelectedValues.Should().BeEquivalentTo(new HashSet<string>() { "2" });
            combobox.Instance.Text.Should().Be("2");
        }

        [Test]
        public void Combobox_ValueBubblingTest_MultiSelection()
        {
            var comp = Context.RenderComponent<ComboboxInitialValueTest>(x =>
            {
                x.Add(c => c.MultiSelection, true);
            });
            var combobox = comp.FindComponent<MudComboBox<string>>();

            combobox.Instance.Value.Should().BeNull();
            combobox.Instance.Text.Should().BeNullOrEmpty();

            comp.SetParam("SelectedValues", new HashSet<string>() { "1" });
            combobox.Instance.Value.Should().Be(null);
            combobox.Instance.SelectedValues.Should().BeEquivalentTo(new HashSet<string>() { "1" });
            combobox.Instance.Text.Should().Be(null);

            comp.SetParam("SelectedValues", new HashSet<string>() { "2", "1" });
            combobox.Instance.Value.Should().Be(null);
            combobox.Instance.SelectedValues.Should().BeEquivalentTo(new HashSet<string>() { "2", "1" });
            combobox.Instance.Text.Should().Be(null);
        }

        [Test]
        public async Task Combobox_ValueChangeEventCountTest()
        {
            var comp = Context.RenderComponent<ComboboxEventCountTest>(x =>
            {
                x.Add(c => c.MultiSelection, false);
            });
            var combobox = comp.FindComponent<MudComboBox<string>>();
            var input = comp.Find("div.mud-input-control");

            comp.Instance.ValueChangeCount.Should().Be(0);
            comp.Instance.ValuesChangeCount.Should().Be(0);

            await comp.InvokeAsync(() => combobox.SetParam("Value", "1"));
            await comp.InvokeAsync(() => combobox.Instance.ForceUpdate());
            comp.WaitForAssertion(() => comp.Instance.ValueChangeCount.Should().Be(1));
            comp.Instance.ValuesChangeCount.Should().Be(1);
            combobox.Instance.Value.Should().Be("1");

            // Changing value programmatically without ForceUpdate should change value, but should not fire change events
            // Its by design, so this part can be change if design changes
            await comp.InvokeAsync(() => combobox.SetParam("Value", "2"));
            comp.WaitForAssertion(() => comp.Instance.ValueChangeCount.Should().Be(1));
            comp.Instance.ValuesChangeCount.Should().Be(1);
            combobox.Instance.Value.Should().Be("2");
        }

        [Test]
        public async Task Combobox_ValueChangeEventCountTest_MultiSelection()
        {
            var comp = Context.RenderComponent<ComboboxEventCountTest>(x =>
            {
                x.Add(c => c.MultiSelection, true);
            });
            var combobox = comp.FindComponent<MudComboBox<string>>();

            comp.Instance.ValueChangeCount.Should().Be(0);
            comp.Instance.ValuesChangeCount.Should().Be(0);

            await comp.InvokeAsync(() => combobox.SetParam("SelectedValues", new HashSet<string>() { "1" }));
            comp.WaitForAssertion(() => comp.Instance.ValueChangeCount.Should().Be(0));
            comp.WaitForAssertion(() => comp.Instance.ValuesChangeCount.Should().Be(1));


            // Setting same value should not fire events
            await comp.InvokeAsync(() => combobox.SetParam("SelectedValues", new HashSet<string>() { "1" }));
            comp.WaitForAssertion(() => comp.Instance.ValueChangeCount.Should().Be(0));
            comp.Instance.ValuesChangeCount.Should().Be(1);
        }

        /// <summary>
        /// Click should open the Menu and selecting a value should update the bindable value.
        /// </summary>
        [Test]
        public async Task ComboboxTest1()
        {
            var comp = Context.RenderComponent<ComboboxTest1>();
            // print the generated html
            //Console.WriteLine(comp.Markup);
            // select elements needed for the test
            var combobox = comp.FindComponent<MudComboBox<string>>();
            var menu = comp.Find("div.mud-popover");
            var input = comp.Find("div.mud-input-control");
            // check popover class
            menu.ClassList.Should().Contain("combobox-popover-class");
            // check initial state
            combobox.Instance.Value.Should().BeNullOrEmpty();
            comp.WaitForAssertion(() => comp.Find("div.mud-popover").ClassList.Should().Contain("d-none"));
            // click and check if it has toggled the menu
            input.MouseDown();
            menu.ClassList.Should().NotContain("d-none");
            // now click an item and see the value change
            comp.WaitForAssertion(() => comp.FindAll("div.mud-combobox-item").Count.Should().BeGreaterThan(0));
            var items = comp.FindAll("div.mud-combobox-item").ToArray();
            items[1].Click();
            // menu should be closed now
            comp.WaitForAssertion(() => menu.ClassList.Should().Contain("d-none"));
            combobox.Instance.Value.Should().Be("2");
            // now we cheat and click the list without opening the menu ;)

            input.MouseDown();
            comp.WaitForAssertion(() => comp.FindAll("div.mud-combobox-item").Count.Should().BeGreaterThan(0));
            items = comp.FindAll("div.mud-combobox-item").ToArray();

            items[0].Click();
            comp.WaitForAssertion(() => combobox.Instance.Value.Should().Be("1"));
            //Check user on blur implementation works
            var @switch = comp.FindComponent<MudSwitch<bool>>();
            @switch.Instance.Checked = true;
            await comp.InvokeAsync(() => combobox.Instance.HandleOnBlur(new FocusEventArgs()));
            comp.WaitForAssertion(() => @switch.Instance.Checked.Should().Be(false));
        }

        [Test]
        public async Task ComboboxClearableTest()
        {
            var comp = Context.RenderComponent<ComboboxClearableTest>();
            var combobox = comp.FindComponent<MudComboBox<string>>();
            var input = comp.Find("div.mud-input-control");

            // No button when initialized
            comp.FindAll("button").Should().BeEmpty();

            input.MouseDown();
            comp.WaitForAssertion(() => comp.FindAll("div.mud-combobox-item").Count.Should().BeGreaterThan(0));
            // Button shows after selecting item
            var items = comp.FindAll("div.mud-combobox-item").ToArray();
            items[1].Click();
            //comp.WaitForAssertion(() => comp.Find("div.mud-popover").ClassList.Should().NotContain("mud-popover-open"));
            comp.WaitForAssertion(() => combobox.Instance.Value.Should().Be("2"));
            comp.Find("button").Should().NotBeNull();
            // Selection cleared and button removed after clicking clear button
            comp.Find("button").MouseDown();
            comp.WaitForAssertion(() => combobox.Instance.Value.Should().BeNullOrEmpty());
            comp.FindAll("button").Should().BeEmpty();
            // Clear button click handler should have been invoked
            comp.Instance.ClearButtonClicked.Should().BeTrue();
        }

        [Test]
        public void MultiSelect_SelectAll()
        {
            var comp = Context.RenderComponent<ComboboxMultiSelectTest2>();
            // select element needed for the test
            var combobox = comp.FindComponent<MudComboBox<string>>();
            var menu = comp.Find("div.mud-popover");
            var input = combobox.Find("div.mud-input-control");
            // Open the menu
            input.MouseDown();
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
        /// Click should not close the menu and selecting multiple values should update the bindable value with a comma separated list.
        /// </summary>
        [Test]
        public async Task Combobox_MultiSelectTest1()
        {
            var comp = Context.RenderComponent<ComboboxMultiSelectTest1>();
            // print the generated html
            Console.WriteLine(comp.Markup);
            // select elements needed for the test
            var combobox = comp.FindComponent<MudComboBox<string>>();
            var menu = comp.Find("div.mud-popover");
            var input = combobox.Find("div.mud-input-control");
            // check initial state
            combobox.Instance.Value.Should().BeNullOrEmpty();
            comp.WaitForAssertion(() =>
                comp.Find("div.mud-popover").ClassList.Should().Contain("d-none"));
            // click and check if it has toggled the menu
            await comp.InvokeAsync(() => input.MouseDown());
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
            combobox.Instance.SelectedValues.Count().Should().Be(2);
            combobox.Instance.SelectedValues.Should().Contain("2");
            combobox.Instance.SelectedValues.Should().Contain("3");
            //Console.WriteLine(comp.Markup);
            const string @unchecked =
                "M19 5v14H5V5h14m0-2H5c-1.1 0-2 .9-2 2v14c0 1.1.9 2 2 2h14c1.1 0 2-.9 2-2V5c0-1.1-.9-2-2-2z";
            const string @checked =
                "M19 3H5c-1.11 0-2 .9-2 2v14c0 1.1.89 2 2 2h14c1.11 0 2-.9 2-2V5c0-1.1-.89-2-2-2zm-9 14l-5-5 1.41-1.41L10 14.17l7.59-7.59L19 8l-9 9z";
            // check that the correct items are checked
            comp.WaitForAssertion(() =>
                comp.FindAll("div.mud-combobox-item path")[1].Attributes["d"].Value.Should().Be(@unchecked));
            comp.FindAll("div.mud-combobox-item path")[3].Attributes["d"].Value.Should().Be(@checked);
            comp.FindAll("div.mud-combobox-item path")[5].Attributes["d"].Value.Should().Be(@checked);
            // now check how setting the SelectedValues makes items checked or unchecked
            // Note: If popover is open, selecting values programmatically doesn't work for now.
            await comp.InvokeAsync(() => combobox.Instance.CloseMenu());
            await comp.InvokeAsync(() =>
            {
                combobox.Instance.SelectedValues = new HashSet<string>() { "1", "2" };
            });
            await comp.InvokeAsync(() => input.MouseDown());
            comp.WaitForAssertion(() => comp.FindAll("div.mud-combobox-item path")[1].Attributes["d"].Value.Should().Be(@checked));
            comp.FindAll("div.mud-combobox-item path")[3].Attributes["d"].Value.Should().Be(@checked);
            combobox.Instance.SelectedValues.Should().NotContain("3");
            comp.WaitForAssertion(() => comp.FindAll("div.mud-combobox-item path")[5].Attributes["d"].Value.Should().Be(@unchecked));
            //Console.WriteLine(comp.Markup);
        }

        [Test]
        public async Task ComboboxTest_KeyboardNavigation_SingleSelect()
        {
            var comp = Context.RenderComponent<ComboboxTest1>();
            // print the generated html
            //Console.WriteLine(comp.Markup);
            // select elements needed for the test
            var combobox = comp.FindComponent<MudComboBox<string>>().Instance;

            await comp.InvokeAsync(() => combobox.HandleKeyDown(new KeyboardEventArgs() { Key = "Enter", Type = "keydown", }));
            comp.WaitForAssertion(() => comp.Find("div.mud-popover").ClassList.Should().NotContain("d-none"));

            await comp.InvokeAsync(() => combobox.HandleKeyDown(new KeyboardEventArgs() { Key = "Escape", Type = "keydown", }));
            comp.WaitForAssertion(() => comp.Find("div.mud-popover").ClassList.Should().Contain("d-none"));

            await comp.InvokeAsync(() => combobox.HandleKeyDown(new KeyboardEventArgs() { Key = " ", Type = "keydown", }));
            comp.WaitForAssertion(() => comp.Find("div.mud-popover").ClassList.Should().NotContain("d-none"));
            //If we didn't select an item with mouse or arrow keys yet, value should remains null.
            await comp.InvokeAsync(() => combobox.HandleKeyDown(new KeyboardEventArgs() { Key = " ", Type = "keydown", }));
            comp.WaitForAssertion(() => comp.Find("div.mud-popover").ClassList.Should().Contain("d-none"));
            comp.WaitForAssertion(() => combobox.Value.Should().Be(null));

            await comp.InvokeAsync(() => combobox.HandleKeyDown(new KeyboardEventArgs() { Key = "ArrowDown", AltKey = true, Type = "keydown", }));
            comp.WaitForAssertion(() => comp.Find("div.mud-popover").ClassList.Should().NotContain("d-none"));

            await comp.InvokeAsync(() => combobox.HandleKeyDown(new KeyboardEventArgs() { Key = "ArrowUp", AltKey = true, Type = "keydown", }));
            comp.WaitForAssertion(() => comp.Find("div.mud-popover").ClassList.Should().Contain("d-none"));
            //If dropdown is closed, arrow key should not set a value.
            await comp.InvokeAsync(() => combobox.HandleKeyDown(new KeyboardEventArgs() { Key = "ArrowDown", Type = "keydown", }));
            comp.WaitForAssertion(() => comp.Find("div.mud-popover").ClassList.Should().NotContain("d-none"));
            comp.WaitForAssertion(() => combobox.Value.Should().Be(null));
            // If no item is hiligted, enter should only close popover, not select any item and value
            await comp.InvokeAsync(() => combobox.HandleKeyDown(new KeyboardEventArgs() { Key = "Escape", Type = "keydown", }));
            comp.WaitForAssertion(() => comp.Find("div.mud-popover").ClassList.Should().Contain("d-none"));
            comp.WaitForAssertion(() => combobox.Value.Should().Be(null));

            await comp.InvokeAsync(() => combobox.HandleKeyDown(new KeyboardEventArgs() { Key = "Enter", Type = "keydown", }));
            comp.WaitForAssertion(() => comp.Find("div.mud-popover").ClassList.Should().NotContain("d-none"));

            await comp.InvokeAsync(() => combobox.HandleKeyDown(new KeyboardEventArgs() { Key = "ArrowDown", Type = "keydown", }));
            comp.WaitForAssertion(() => combobox.Value.Should().BeNull());

            await comp.InvokeAsync(() => combobox.HandleKeyDown(new KeyboardEventArgs() { Key = "ArrowDown", Type = "keydown", }));
            await comp.InvokeAsync(() => combobox.HandleKeyDown(new KeyboardEventArgs() { Key = "Enter", Type = "keydown", }));
            comp.WaitForAssertion(() => combobox.Value.Should().Be("2"));
            //End key should not select the last disabled item
            await comp.InvokeAsync(() => combobox.HandleKeyDown(new KeyboardEventArgs() { Key = "Enter", Type = "keydown", }));
            await comp.InvokeAsync(() => combobox.HandleKeyDown(new KeyboardEventArgs() { Key = "End", Type = "keydown", }));
            await comp.InvokeAsync(() => combobox.HandleKeyDown(new KeyboardEventArgs() { Key = "Enter", Type = "keydown", }));
            comp.WaitForAssertion(() =>  combobox.Value.Should().Be("3"));

            await comp.InvokeAsync(() => combobox.HandleKeyDown(new KeyboardEventArgs() { Key = "Enter", Type = "keydown", }));
            comp.WaitForAssertion(() => comp.Find("div.mud-popover").ClassList.Should().Contain("mud-popover-open"));
            await comp.InvokeAsync(() => combobox.HandleKeyDown(new KeyboardEventArgs() { Key = "ArrowDown", Type = "keydown", }));
            await comp.InvokeAsync(() => combobox.HandleKeyDown(new KeyboardEventArgs() { Key = "Enter", Type = "keydown", }));
            comp.WaitForAssertion(() => comp.Find("div.mud-popover").ClassList.Should().Contain("d-none"));
            comp.WaitForAssertion(() => combobox.Value.Should().Be("3"));

            await comp.InvokeAsync(() => combobox.HandleKeyDown(new KeyboardEventArgs() { Key = "Enter", Type = "keydown", }));
            await comp.InvokeAsync(() => combobox.HandleKeyDown(new KeyboardEventArgs() { Key = "ArrowDown", Type = "keydown", }));
            await comp.InvokeAsync(() => combobox.HandleKeyDown(new KeyboardEventArgs() { Key = "Enter", Type = "keydown", }));
            comp.WaitForAssertion(() => combobox.Value.Should().Be("3"));

            await comp.InvokeAsync(() => combobox.HandleKeyDown(new KeyboardEventArgs() { Key = "Enter", Type = "keydown", }));
            await comp.InvokeAsync(() => combobox.HandleKeyDown(new KeyboardEventArgs() { Key = "Home", Type = "keydown", }));
            await comp.InvokeAsync(() => combobox.HandleKeyDown(new KeyboardEventArgs() { Key = "Enter", Type = "keydown", }));
            comp.WaitForAssertion(() => combobox.Value.Should().Be("1"));
            //Arrow up should select still the first item
            await comp.InvokeAsync(() => combobox.HandleKeyDown(new KeyboardEventArgs() { Key = "Enter", Type = "keydown", }));
            await comp.InvokeAsync(() => combobox.HandleKeyDown(new KeyboardEventArgs() { Key = "ArrowUp", Type = "keydown", }));
            await comp.InvokeAsync(() => combobox.HandleKeyDown(new KeyboardEventArgs() { Key = "Enter", Type = "keydown", }));
            comp.WaitForAssertion(() => combobox.Value.Should().Be("1"));

            await comp.InvokeAsync(() => combobox.HandleKeyDown(new KeyboardEventArgs() { Key = "Enter", Type = "keydown", }));
            await comp.InvokeAsync(() => combobox.HandleKeyDown(new KeyboardEventArgs() { Key = "End", Type = "keydown", }));
            await comp.InvokeAsync(() => combobox.HandleKeyDown(new KeyboardEventArgs() { Key = "ArrowDown", Type = "keydown", }));
            await comp.InvokeAsync(() => combobox.HandleKeyDown(new KeyboardEventArgs() { Key = "Enter", Type = "keydown", }));
            comp.WaitForAssertion(() => combobox.Value.Should().Be("3"));

            await comp.InvokeAsync(() => combobox.HandleKeyDown(new KeyboardEventArgs() { Key = "Enter", Type = "keydown", }));
            await comp.InvokeAsync(() => combobox.HandleKeyDown(new KeyboardEventArgs() { Key = "2", Type = "keydown", }));
            comp.WaitForAssertion(() => combobox.Value.Should().Be("3"));

            await comp.InvokeAsync(() => combobox.HandleKeyDown(new KeyboardEventArgs() { Key = "Enter", Type = "keydown", }));
            comp.WaitForAssertion(() => combobox.Value.Should().Be("2"));
        }

        [Test]
        public async Task ComboboxTest_KeyboardNavigation_ToggleSelect()
        {
            var comp = Context.RenderComponent<ComboboxTest1>();
            var combobox = comp.FindComponent<MudComboBox<string>>().Instance;
            combobox.ToggleSelection = true;
            await comp.InvokeAsync(() => combobox.HandleKeyDown(new KeyboardEventArgs() { Key = "Enter", Type = "keydown", }));
            comp.WaitForAssertion(() => comp.Find("div.mud-popover").ClassList.Should().NotContain("d-none"));

            await comp.InvokeAsync(() => combobox.HandleKeyDown(new KeyboardEventArgs() { Key = "ArrowDown", Type = "keydown", }));
            comp.WaitForAssertion(() => combobox.Value.Should().BeNull());

            await comp.InvokeAsync(() => combobox.HandleKeyDown(new KeyboardEventArgs() { Key = "ArrowDown", Type = "keydown", }));
            await comp.InvokeAsync(() => combobox.HandleKeyDown(new KeyboardEventArgs() { Key = "Enter", Type = "keydown", }));
            comp.WaitForAssertion(() => combobox.Value.Should().Be("2"));
            //End key should not select the last disabled item
            await comp.InvokeAsync(() => combobox.HandleKeyDown(new KeyboardEventArgs() { Key = "Enter", Type = "keydown", }));
            await comp.InvokeAsync(() => combobox.HandleKeyDown(new KeyboardEventArgs() { Key = "Enter", Type = "keydown", }));
            comp.WaitForAssertion(() => combobox.Value.Should().BeNull());
        }

        [Test]
        public async Task ComboboxTest_KeyboardNavigation_MultiSelect()
        {
            var comp = Context.RenderComponent<ComboboxMultiSelectTest3>();
            var combobox = comp.FindComponent<MudComboBox<string>>();

            await comp.InvokeAsync(() => combobox.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = " ", Type = "keydown", }));
            comp.WaitForAssertion(() => comp.Find("div.mud-popover").ClassList.Should().NotContain("d-none"));

            await comp.InvokeAsync(() => combobox.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "a", CtrlKey = true, Type = "keydown", }));
            comp.WaitForAssertion(() => combobox.Instance.GetPresenterText().Should().Be("7 felines have been selected"));

            await comp.InvokeAsync(() => combobox.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "A", CtrlKey = true, Type = "keydown", }));
            comp.WaitForAssertion(() => combobox.Instance.GetPresenterText().Should().BeNull());

            await comp.InvokeAsync(() => combobox.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "ArrowDown", Type = "keydown", }));
            await comp.InvokeAsync(() => combobox.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "Enter", Type = "keydown", }));
            comp.WaitForAssertion(() => combobox.Instance.GetPresenterText().Should().Be("1 feline has been selected"));

            await comp.InvokeAsync(() => combobox.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "A", CtrlKey = true, Type = "keydown", }));
            comp.WaitForAssertion(() => combobox.Instance.GetPresenterText().Should().Be("7 felines have been selected"));

            await comp.InvokeAsync(() => combobox.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "Escape", Type = "keydown", }));
            comp.WaitForAssertion(() => comp.Find("div.mud-popover").ClassList.Should().Contain("d-none"));

            await comp.InvokeAsync(() => combobox.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "Enter", Type = "keydown", }));
            comp.WaitForAssertion(() => comp.Find("div.mud-popover").ClassList.Should().NotContain("d-none"));

            await comp.InvokeAsync(() => combobox.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "ArrowDown", Type = "keydown", }));
            await comp.InvokeAsync(() => combobox.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "Enter", Type = "keydown", }));
            comp.WaitForAssertion(() => combobox.Instance.SelectedValues.Should().NotContain("Tiger"));

            await comp.InvokeAsync(() => combobox.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "Home", Type = "keydown", }));
            await comp.InvokeAsync(() => combobox.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "NumpadEnter", Type = "keydown", }));
            comp.WaitForAssertion(() => combobox.Instance.SelectedValues.Should().NotContain("Jaguar"));

            await comp.InvokeAsync(() => combobox.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "ArrowDown", Type = "keydown", }));
            await comp.InvokeAsync(() => combobox.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "Enter", Type = "keydown", }));
            comp.WaitForAssertion(() => combobox.Instance.SelectedValues.Should().NotContain("Leopard"));

            await comp.InvokeAsync(() => combobox.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "End", Type = "keydown", }));
            await comp.InvokeAsync(() => combobox.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "Enter", Type = "keydown", }));
            comp.WaitForAssertion(() => combobox.Instance.SelectedValues.Should().Contain("Tiger"));

            combobox.SetParam("Disabled", true);
            await comp.InvokeAsync(() => combobox.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "Enter", Type = "keydown", }));
            comp.WaitForAssertion(() => combobox.Instance.SelectedValues.Should().Contain("Tiger"));

            combobox.SetParam("Disabled", false);
            //Test the keyup event
            await comp.InvokeAsync(() => combobox.Instance.HandleKeyUp(new KeyboardEventArgs() { Key = "Enter", Type = "keyup", }));
            comp.WaitForAssertion(() => combobox.Instance.SelectedValues.Should().Contain("Tiger"));

            await comp.InvokeAsync(() => combobox.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "Tab", Type = "keydown", }));
            await comp.InvokeAsync(() => combobox.Instance.OnKeyUp.InvokeAsync(new KeyboardEventArgs() { Key = "Tab" }));
            comp.Render(); // <-- this is necessary for reliable passing of the test
            comp.WaitForAssertion(() => comp.Find("div.mud-popover").ClassList.Should().Contain("d-none"));
        }


    }
}
