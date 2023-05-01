#pragma warning disable CS1998 // async without await
#pragma warning disable IDE1006 // leading underscore
#pragma warning disable BL0005 // Set parameter outside component

using Bunit;
using MudExtensions.UnitTests.TestComponents;
using FluentAssertions;
using Microsoft.AspNetCore.Components.Web;
using MudExtensions;
using NUnit.Framework;
using static MudExtensions.UnitTests.TestComponents.SelectWithEnumTest;
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
            var combobox = comp.FindComponent<MudCombobox<string>>();

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
            var combobox = comp.FindComponent<MudCombobox<string>>();

            combobox.Instance.Value.Should().Be("1");
            combobox.Instance.SelectedValues.Should().BeEquivalentTo(new HashSet<string>() { "1" });
            combobox.Instance.Text.Should().Be("1");
        }

        [Test]
        public async Task Combobox_ValueBubblingTest()
        {
            var comp = Context.RenderComponent<ComboboxInitialValueTest>();
            var combobox = comp.FindComponent<MudCombobox<string>>();

            combobox.Instance.Value.Should().BeNull();
            combobox.Instance.Text.Should().BeNull();

            comp.SetParam("SelectedValue", "1");
            await comp.InvokeAsync(() => combobox.Instance.ForceUpdate());
            comp.WaitForAssertion(() => combobox.Instance.Value.Should().Be("1"));
            combobox.Instance.SelectedValues.Should().BeEquivalentTo(new HashSet<string>() { "1" });
            combobox.Instance.Text.Should().Be("1");

            comp.SetParam("SelectedValue", "2");
            await comp.InvokeAsync(() => combobox.Instance.ForceUpdate());
            comp.WaitForAssertion(() => combobox.Instance.Value.Should().Be("2"));
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
            var combobox = comp.FindComponent<MudCombobox<string>>();

            combobox.Instance.Value.Should().BeNull();
            combobox.Instance.Text.Should().BeNullOrEmpty();

            comp.SetParam("SelectedValues", new HashSet<string>() { "1" });
            combobox.Instance.Value.Should().Be(null);
            combobox.Instance.SelectedValues.Should().BeEquivalentTo(new HashSet<string>() { "1" });
            combobox.Instance.Text.Should().Be("1");

            comp.SetParam("SelectedValues", new HashSet<string>() { "2", "1" });
            combobox.Instance.Value.Should().Be(null);
            combobox.Instance.SelectedValues.Should().BeEquivalentTo(new HashSet<string>() { "2", "1" });
            combobox.Instance.Text.Should().Be("2, 1");
        }

        [Test]
        public async Task Combobox_ValueChangeEventCountTest()
        {
            var comp = Context.RenderComponent<ComboboxEventCountTest>(x =>
            {
                x.Add(c => c.MultiSelection, false);
            });
            var combobox = comp.FindComponent<MudCombobox<string>>();
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
            var combobox = comp.FindComponent<MudCombobox<string>>();

            comp.Instance.ValueChangeCount.Should().Be(0);
            comp.Instance.ValuesChangeCount.Should().Be(1);

            await comp.InvokeAsync(() => combobox.SetParam("SelectedValues", new HashSet<string>() { "1" }));
            comp.WaitForAssertion(() => comp.Instance.ValueChangeCount.Should().Be(0));
            comp.WaitForAssertion(() => comp.Instance.ValuesChangeCount.Should().Be(2));


            // Setting same value should not fire events
            await comp.InvokeAsync(() => combobox.SetParam("SelectedValues", new HashSet<string>() { "1" }));
            comp.WaitForAssertion(() => comp.Instance.ValueChangeCount.Should().Be(0));
            comp.Instance.ValuesChangeCount.Should().Be(2);
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
            var combobox = comp.FindComponent<MudCombobox<string>>();
            var menu = comp.Find("div.mud-popover");
            var input = comp.Find("div.mud-input-control");
            // check popover class
            menu.ClassList.Should().Contain("combobox-popover-class");
            // check initial state
            combobox.Instance.Value.Should().BeNullOrEmpty();
            comp.WaitForAssertion(() => comp.Find("div.mud-popover").ClassList.Should().Contain("d-none"));
            // click and check if it has toggled the menu
            input.Click();
            menu.ClassList.Should().NotContain("d-none");
            // now click an item and see the value change
            comp.WaitForAssertion(() => comp.FindAll("div.mud-list-item-extended").Count.Should().BeGreaterThan(0));
            var items = comp.FindAll("div.mud-list-item-extended").ToArray();
            items[1].Click();
            // menu should be closed now
            comp.WaitForAssertion(() => menu.ClassList.Should().Contain("d-none"));
            combobox.Instance.Value.Should().Be("2");
            // now we cheat and click the list without opening the menu ;)

            input.Click();
            comp.WaitForAssertion(() => comp.FindAll("div.mud-list-item-extended").Count.Should().BeGreaterThan(0));
            items = comp.FindAll("div.mud-list-item-extended").ToArray();

            items[0].Click();
            comp.WaitForAssertion(() => combobox.Instance.Value.Should().Be("1"));
            //Check user on blur implementation works
            var @switch = comp.FindComponent<MudSwitch<bool>>();
            @switch.Instance.Checked = true;
            await comp.InvokeAsync(() => combobox.Instance.OnLostFocus(new FocusEventArgs()));
            comp.WaitForAssertion(() => @switch.Instance.Checked.Should().Be(false));
        }

        /// <summary>
        /// Click should not close the menu and selecting multiple values should update the bindable value with a comma separated list.
        /// </summary>
        //[Test]
        //public async Task Combobox_MultiSelectTest1()
        //{
        //    var comp = Context.RenderComponent<ComboboxMultiSelectTest1>();
        //    // print the generated html
        //    Console.WriteLine(comp.Markup);
        //    // select elements needed for the test
        //    var combobox = comp.FindComponent<MudCombobox<string>>();
        //    var menu = comp.Find("div.mud-popover");
        //    var input = comp.Find("div.mud-input-control");
        //    // check initial state
        //    combobox.Instance.Value.Should().BeNullOrEmpty();
        //    comp.WaitForAssertion(() =>
        //        comp.Find("div.mud-popover").ClassList.Should().Contain("d-none"));
        //    // click and check if it has toggled the menu
        //    await comp.InvokeAsync(() => input.Click());
        //    comp.WaitForAssertion(() => comp.Find("div.mud-popover").ClassList.Should().NotContain("d-none"));
        //    // now click an item and see the value change
        //    comp.WaitForAssertion(() => comp.FindAll("div.mud-list-item-extended").Count.Should().BeGreaterThan(0));
        //    var items = comp.FindAll("div.mud-list-item-extended").ToArray();
        //    items[1].Click();
        //    // menu should still be open now!!
        //    comp.Find("div.mud-popover").ClassList.Should().NotContain("d-none");
        //    comp.WaitForAssertion(() => combobox.Instance.Text.Should().Be("2"));
        //    items[0].Click();
        //    comp.WaitForAssertion(() => combobox.Instance.Text.Should().Be("2, 1"));
        //    items[2].Click();
        //    comp.WaitForAssertion(() => combobox.Instance.Text.Should().Be("2, 1, 3"));
        //    items[0].Click();
        //    comp.WaitForAssertion(() => combobox.Instance.Text.Should().Be("2, 3"));
        //    combobox.Instance.SelectedValues.Count().Should().Be(2);
        //    combobox.Instance.SelectedValues.Should().Contain("2");
        //    combobox.Instance.SelectedValues.Should().Contain("3");
        //    //Console.WriteLine(comp.Markup);
        //    const string @unchecked =
        //        "M19 5v14H5V5h14m0-2H5c-1.1 0-2 .9-2 2v14c0 1.1.9 2 2 2h14c1.1 0 2-.9 2-2V5c0-1.1-.9-2-2-2z";
        //    const string @checked =
        //        "M19 3H5c-1.11 0-2 .9-2 2v14c0 1.1.89 2 2 2h14c1.11 0 2-.9 2-2V5c0-1.1-.89-2-2-2zm-9 14l-5-5 1.41-1.41L10 14.17l7.59-7.59L19 8l-9 9z";
        //    // check that the correct items are checked
        //    comp.WaitForAssertion(() =>
        //        comp.FindAll("div.mud-list-item-extended path")[1].Attributes["d"].Value.Should().Be(@unchecked));
        //    comp.FindAll("div.mud-list-item-extended path")[3].Attributes["d"].Value.Should().Be(@checked);
        //    comp.FindAll("div.mud-list-item-extended path")[5].Attributes["d"].Value.Should().Be(@checked);
        //    // now check how setting the SelectedValues makes items checked or unchecked
        //    // Note: If popover is open, selecting values programmatically doesn't work for now.
        //    await comp.InvokeAsync(() => combobox.Instance.CloseMenu());
        //    await comp.InvokeAsync(() =>
        //    {
        //        combobox.Instance.SelectedValues = new HashSet<string>() { "1", "2" };
        //    });
        //    await comp.InvokeAsync(() => input.Click());
        //    comp.WaitForAssertion(() => comp.FindAll("div.mud-list-item-extended path")[1].Attributes["d"].Value.Should().Be(@checked));
        //    comp.FindAll("div.mud-list-item-extended path")[3].Attributes["d"].Value.Should().Be(@checked);
        //    combobox.Instance.SelectedValues.Should().NotContain("3");
        //    comp.WaitForAssertion(() => comp.FindAll("div.mud-list-item-extended path")[5].Attributes["d"].Value.Should().Be(@unchecked));
        //    //Console.WriteLine(comp.Markup);
        //}

        
    }
}
