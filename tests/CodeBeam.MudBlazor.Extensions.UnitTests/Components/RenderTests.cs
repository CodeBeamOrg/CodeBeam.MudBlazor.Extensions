using AwesomeAssertions;
using Bunit;
using Microsoft.AspNetCore.Components;
using MudBlazor.Services;
using MudExtensions.Utilities;
using System.Reflection;

namespace MudExtensions.UnitTests.Components;

[TestFixture]
public class ComponentsRenderTests : BunitTest
{
    public static IEnumerable<Type> ComponentTypes()
    {
        var assembly = typeof(MudExtensions.MudColorProvider).Assembly;

        return assembly
            .GetTypes()
            .Where(t =>
                t.IsClass &&
                !t.IsAbstract &&
                !t.IsGenericTypeDefinition &&
                typeof(IComponent).IsAssignableFrom(t) &&
                t.IsPublic &&
                t.GetCustomAttribute<ExcludeFromSmokeTest>() == null);
    }

    [SetUp]
    public void Setup()
    {
        Context.Services.AddMudServices();
        Context.JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [TestCaseSource(nameof(ComponentTypes))]
    public void Component_Should_Render(Type componentType)
    {
        try
        {
            var cut = Context.Render(builder =>
            {
                builder.OpenComponent(0, componentType);
                builder.CloseComponent();
            });

            cut.Should().NotBeNull();
        }
        catch (Exception ex)
        {
            Assert.Fail(
                $"Component render FAILED: {componentType.FullName}\n" +
                $"Exception: {ex.GetType().Name}\n" +
                $"Message: {ex.Message}"
            );
        }
    }
}
