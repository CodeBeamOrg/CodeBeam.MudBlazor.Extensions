using AwesomeAssertions;
using Bunit;
using MudExtensions.UnitTests.Extensions;

namespace MudExtensions.UnitTests.Components;

[TestFixture]
public class CodeViewerTests : BunitTest
{
    [Test]
    public void CodeViewer_DefaultRender_Test()
    {
        var comp = Context.Render<MudCodeViewer>(p => p
            .Add(x => x.Code, "public class Test {}")
            .Add(x => x.Language, CodeLanguage.CSharp)
        );

        var pre = comp.Find("pre");
        var code = comp.Find("code");

        code.ClassList.Should().Contain("language-csharp");
        pre.ClassList.Should().Contain("language-csharp");
    }

    [Test]
    public void CodeViewer_LineNumbers_Class_Test()
    {
        var comp = Context.Render<MudCodeViewer>(p => p
            .Add(x => x.Code, "test")
            .Add(x => x.ShowLineNumbers, true)
        );

        var pre = comp.Find("pre");
        pre.ClassList.Should().Contain("line-numbers");
    }

    [Test]
    public void CodeViewer_Header_Render_Test()
    {
        var comp = Context.Render<MudCodeViewer>(p => p
            .Add(x => x.Code, "test")
            .Add(x => x.ShowHeader, true)
        );

        comp.Find(".mud-codeviewer-header").Should().NotBeNull();
    }

    [Test]
    public void CodeViewer_Editable_Mode_Test()
    {
        var comp = Context.Render<MudCodeViewer>(p => p
            .Add(x => x.Code, "test")
            .Add(x => x.Editable, true)
        );

        comp.Find("textarea").Should().NotBeNull();
    }

    [Test]
    public void CodeViewer_Should_Call_Highlight_On_FirstRender()
    {
        var jsMock = Context.JSInterop.SetupVoid("MudCode.highlight", _ => true);

        var comp = Context.Render<MudCodeViewer>(p => p
            .Add(x => x.Code, "test")
        );

        jsMock.VerifyInvoke("MudCode.highlight");
    }

    [Test]
    public void CodeViewer_Language_Change_Should_Update_Class()
    {
        var comp = Context.Render<MudCodeViewer>(p => p
            .Add(x => x.Code, "test")
            .Add(x => x.Language, CodeLanguage.CSharp)
        );

        comp.SetParametersAndRenderAsync(p => p
            .Add(x => x.Language, CodeLanguage.JavaScript)
        );

        comp.Find("code").ClassList.Should().Contain("language-javascript");
    }

    [Test]
    public void CodeViewer_Copy_Should_Invoke_JS()
    {
        var jsMock = Context.JSInterop.SetupVoid("MudCode.copy", _ => true);

        var comp = Context.Render<MudCodeViewer>(p => p
            .Add(x => x.Code, "copy me")
            .Add(x => x.ShowCopyButton, true)
        );

        comp.Find("button").Click();

        jsMock.VerifyInvoke("MudCode.copy");
    }

    [Test]
    public void CodeViewer_EnableTabIndent_Only_When_Editable()
    {
        var jsMock = Context.JSInterop.SetupVoid("MudCode.enableTabIndent", _ => true);

        var comp = Context.Render<MudCodeViewer>(p => p
            .Add(x => x.Code, "test")
            .Add(x => x.Editable, true)
        );

        jsMock.VerifyInvoke("MudCode.enableTabIndent");
    }
}
