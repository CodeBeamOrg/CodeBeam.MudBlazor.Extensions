using AwesomeAssertions;
using Bunit;
using Microsoft.AspNetCore.Components;
using MudExtensions.Docs.Pages;
using NUnit.Framework;
using System.Reflection;

namespace MudExtensions.UnitTests.Components;

[TestFixture]
public class DocsPagesRenderTests : BunitTest
{
    public static IEnumerable<Type> DocsPages()
    {
        return typeof(ApiPage).Assembly
            .GetTypes()
            .Where(t =>
                t.IsClass &&
                !t.IsAbstract &&
                typeof(IComponent).IsAssignableFrom(t) &&
                t.Namespace?.StartsWith("MudExtensions.Docs.Pages") == true &&
                t.GetCustomAttribute<RouteAttribute>() != null);
    }

    [TestCaseSource(nameof(DocsPages))]
    public void All_Docs_Pages_Should_Render(Type pageType)
    {
        var cut = Context.Render(builder =>
        {
            builder.OpenComponent(0, pageType);
            builder.CloseComponent();
        });

        cut.Markup.Should().NotBeNullOrWhiteSpace();
    }
}
