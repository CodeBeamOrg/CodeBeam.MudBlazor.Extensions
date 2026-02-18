using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using MudBlazor.State;
using MudBlazor.Utilities;

namespace MudExtensions;

/// <summary>
/// Represents a component that displays formatted code snippets with syntax highlighting.
/// </summary>
/// <remarks>Use this component to present code examples or source code within a user interface, typically
/// for documentation or demonstration purposes. The component supports syntax highlighting to improve readability.
/// </remarks>
public partial class MudCodeViewer : MudComponentBase
{
    private ElementReference _codeRef;
    private string? _copyIcon = Icons.Material.Filled.ContentCopy;

    private readonly ParameterState<string?> _code;
    private readonly ParameterState<bool> _showLineNumbers;
    private readonly ParameterState<bool> _wrap;
    private readonly ParameterState<CodeLanguage> _language;

    /// <summary>
    /// Initializes a new instance of the MudCodeViewer class.
    /// </summary>
    public MudCodeViewer()
    {
        using var registerScope = CreateRegisterScope();
        _code = registerScope.RegisterParameter<string?>(nameof(Code))
            .WithParameter(() => Code)
            .WithChangeHandler(ParameterChanged);
        _showLineNumbers = registerScope.RegisterParameter<bool>(nameof(ShowLineNumbers))
            .WithParameter(() => ShowLineNumbers)
            .WithChangeHandler(ParameterChanged);
        _wrap = registerScope.RegisterParameter<bool>(nameof(Wrap))
            .WithParameter(() => Wrap)
            .WithChangeHandler(ParameterChanged);
        _language = registerScope.RegisterParameter<CodeLanguage>(nameof(Language))
            .WithParameter(() => Language)
            .WithChangeHandler(ParameterChanged);
    }

    /// <summary>
    /// Gets the CSS class string for the code viewer component, including any additional classes specified by the user.
    /// </summary>
    protected string? Classname => new CssBuilder("mud-codeviewer")
        .AddClass("mud-codeviewer-bg", EnableDefaultBackground)
        .AddClass("wrap", _wrap.Value)
        .AddClass(Class)
        .Build();

    /// <summary>
    /// Gets the CSS class string applied to the header section of the code viewer.
    /// </summary>
    protected string? HeaderClassname => new CssBuilder("mud-codeviewer-header")
        .AddClass(HeaderClass)
        .Build();

    /// <summary>
    /// Gets or sets the code snippet to be displayed or processed by the component.
    /// </summary>
    [Parameter]
    public string? Code { get; set; }

    /// <summary>
    /// Gets or sets the programming language used for syntax highlighting in the code viewer.
    /// </summary>
    /// <remarks>Set this property to specify which language's syntax rules are applied when rendering code. The default is C#.
    /// </remarks>
    [Parameter]
    public CodeLanguage Language { get; set; } = CodeLanguage.CSharp;

    [Parameter]
    public bool ShowLineNumbers { get; set; }

    [Parameter]
    public bool ShowHeader { get; set; } = true;

    /// <summary>
    /// Gets or sets the CSS class string applied to the header section of the code viewer, allowing for custom styling of the header.
    /// </summary>
    [Parameter]
    public string? HeaderClass { get; set; }

    [Parameter]
    public bool ShowCopyButton { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether the code content should wrap to the next line when it exceeds the
    /// container width.
    /// </summary>
    [Parameter]
    public bool Wrap { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the default background should be enabled for the component.
    /// </summary>
    [Parameter]
    public bool EnableDefaultBackground { get; set; } = true;

    /// <summary>
    /// Gets or sets the content to be rendered inside this component.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }


    private string CodeClass => $"language-{_language.Value.ToDescriptionString()}";
    private string PreClass => new CssBuilder()
        .AddClass($"line-numbers language-{_language.Value.ToDescriptionString()}", _showLineNumbers.Value)
        .AddClass($"language-{_language.Value.ToDescriptionString()}", !_showLineNumbers.Value)
        .Build();

    /// <summary>
    /// Invoked after the component has rendered. Performs post-render logic, such as refreshing data, when the
    /// component is rendered for the first time.
    /// </summary>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await RefreshAsync();
        }
    }

    /// <summary>
    /// Asynchronously refreshes the code viewer by reapplying syntax highlighting to the displayed code.
    /// </summary>
    public async Task RefreshAsync()
    {
        if (_codeRef.Context is null)
            return;

        await JS.InvokeVoidAsync("MudCode.highlight", _codeRef);
    }

    /// <summary>
    /// Asynchronously copies the current code content to the user's clipboard using JavaScript interop.
    /// </summary>
    public async Task CopyAsync()
    {
        await JS.InvokeVoidAsync("MudCode.copy", Code);
    }

    protected async Task ParameterChanged()
    {
        await Task.Delay(1);
        await RefreshAsync();
    }

    protected async Task HandleCopyButtonClickAsync()
    {
        await CopyAsync();
        _copyIcon = Icons.Material.Filled.DoneAll;
        StateHasChanged();
        await Task.Delay(2000);
        _copyIcon = Icons.Material.Filled.ContentCopy;
        StateHasChanged();
    }
}
