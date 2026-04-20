using AwesomeAssertions;
using Bunit;
using Microsoft.AspNetCore.Components;
using MudExtensions.UnitTests.Extensions;
using MudExtensions.UnitTests.TestComponents;

namespace MudExtensions.UnitTests.Components;

[TestFixture]
public class DebouncedInputExtendedTests : BunitTest
{
    /// <summary>
    /// If Debounce Interval is null or 0, Value should change immediately
    /// </summary>
    [Test]
    public async Task WithNoDebounceIntervalValueShouldChangeImmediately()
    {
        //no interval passed, so, by default is 0
        // We pass the Immediate parameter set to true, in order to bind to oninput
        var comp = Context.Render<MudTextFieldExtended<string>>(parameters => parameters.Add(p => p.Immediate, true));
        var textField = comp.Instance;
        var input = comp.Find("input");

        //Act
        await input.InputAsync(new ChangeEventArgs() { Value = "Some Value" });

        //Assert
        //input value has changed, DebounceInterval is 0, so Value should change in TextField immediately
        textField.ReadValue.Should().Be("Some Value");
    }

    /// <summary>
    /// Value should not change immediately. Should respect the Debounce Interval
    /// </summary>
    [Test]
    public async Task ShouldRespectDebounceIntervalPropertyInTextField()
    {
        var comp = Context.Render<MudTextFieldExtended<string>>(parameters => parameters.Add(p => p.DebounceInterval, 200d));
        var textField = comp.Instance;
        var input = comp.Find("input");

        //Act
        await input.InputAsync(new ChangeEventArgs() { Value = "Some Value" });

        //Assert
        //if DebounceInterval is set, Immediate should be true by default
        textField.Immediate.Should().BeTrue();

        //input value has changed, but elapsed time is 0, so Value should not change in TextField
        textField.ReadValue.Should().BeNull();

        //DebounceInterval is 200 ms, so at 100 ms Value should not change in TextField
        await Task.Delay(100);
        textField.ReadValue.Should().BeNull();

        //More than 200 ms had elapsed, so Value should be updated
        await comp.WaitForAssertionAsync(() => textField.ReadValue.Should().Be("Some Value"));
    }

    /// <summary>
    /// DebounceInterval updates with epsilon-equivalent values should not break debouncing
    /// </summary>
    [Test]
    public async Task DebounceInterval_EpsilonEquivalentValues_PreservesDebounce()
    {
        // Arrange
        var comp = Context.Render<MudTextFieldExtended<string>>(parameters => parameters.Add(p => p.DebounceInterval, 200.0));
        var textField = comp.Instance;
        var input = comp.Find("input");

        // Act - Input a value
        await input.InputAsync(new ChangeEventArgs() { Value = "Test Value" });

        // Change DebounceInterval to an epsilon-equivalent value (should not reset debouncer)
        await comp.SetParametersAndRenderAsync(parameters => parameters.Add(p => p.DebounceInterval, 200.0000001));

        // Assert - Value should still be null (debounce still pending)
        textField.ReadValue.Should().BeNull();

        // Wait for the debounce to complete
        await comp.WaitForAssertionAsync(() => textField.ReadValue.Should().Be("Test Value"));
    }

    [Test]
    public async Task DebouncedTextField_ShouldStayInSyncWithBoundValueAfterAsyncInitialization()
    {
        var comp = Context.Render<DebouncedTextFieldAsyncInitializationSyncTest>();

        await comp.WaitForAssertionAsync(() =>
        {
            var inputs = comp.FindAll("input");
            inputs[0].GetAttribute("value").Should().Be("init value");
            inputs[1].GetAttribute("value").Should().Be("init value");
        });

        var immediateInput = comp.FindAll("input")[1];
        await immediateInput.ChangeAsync(new ChangeEventArgs { Value = "changed value" });

        await comp.WaitForAssertionAsync(() =>
        {
            var inputs = comp.FindAll("input");
            inputs[0].GetAttribute("value").Should().Be("changed value");
            inputs[1].GetAttribute("value").Should().Be("changed value");
        }, TimeSpan.FromSeconds(1));
    }
}
