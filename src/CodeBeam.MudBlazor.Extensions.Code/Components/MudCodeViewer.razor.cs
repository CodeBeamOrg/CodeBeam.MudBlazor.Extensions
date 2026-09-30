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
    private ElementReference _textAreaRef;
    private string? _copyIcon = Icons.Material.Filled.ContentCopy;
    private bool _shouldHighlight;
    private bool _tabEnabled;

    private readonly ParameterState<string?> _code;
    private readonly ParameterState<bool> _showLineNumbers;
    private readonly ParameterState<bool> _wrap;
    private readonly ParameterState<bool> _editable;
    private readonly ParameterState<bool> _header;
    private readonly ParameterState<CodeLanguage> _language;

    /// <summary>
    /// Initializes a new instance of the MudCodeViewer class.
    /// </summary>
    public MudCodeViewer()
    {
        using var registerScope = CreateRegisterScope();
        _code = registerScope.RegisterParameter<string?>(nameof(Code))
            .WithParameter(() => Code)
            .WithChangeHandler(ParameterChanged)
            .WithEventCallback(() => CodeChanged);
        _showLineNumbers = registerScope.RegisterParameter<bool>(nameof(ShowLineNumbers))
            .WithParameter(() => ShowLineNumbers)
            .WithChangeHandler(ParameterChanged);
        _wrap = registerScope.RegisterParameter<bool>(nameof(Wrap))
            .WithParameter(() => Wrap)
            .WithChangeHandler(ParameterChanged);
        _editable = registerScope.RegisterParameter<bool>(nameof(Editable))
            .WithParameter(() => Editable)
            .WithChangeHandler(ParameterChanged);
        _header = registerScope.RegisterParameter<bool>(nameof(ShowHeader))
            .WithParameter(() => ShowHeader)
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
    /// Gets the CSS class name representing the current programming language for syntax highlighting.
    /// </summary>
    protected string CodeClass => $"language-{_language.Value.ToDescriptionString()}";

    /// <summary>
    /// Gets the CSS class string used for the pre element based on the selected language and line number display settings.
    /// </summary>
    private string PreClass => new CssBuilder()
        .AddClass($"line-numbers language-{_language.Value.ToDescriptionString()}", _showLineNumbers.Value)
        .AddClass($"language-{_language.Value.ToDescriptionString()}", !_showLineNumbers.Value)
        .Build();

    /// <summary>
    /// Gets the CSS class string used for the textarea element based on the line number display settings.
    /// </summary>
    private string TextAreaClass => new CssBuilder()
        .AddClass("mud-codeviewer-textarea")
        .AddClass("line-numbers", _showLineNumbers.Value)
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

    /// <summary>
    /// Gets or sets a value indicating whether line numbers are displayed in the code viewer.
    /// </summary>
    [Parameter]
    public bool ShowLineNumbers { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the header is displayed in the code viewer component.
    /// </summary>
    [Parameter]
    public bool ShowHeader { get; set; } = true;

    /// <summary>
    /// Gets or sets the CSS class string applied to the header section of the code viewer, allowing for custom styling of the header.
    /// </summary>
    [Parameter]
    public string? HeaderClass { get; set; }

    /// <summary>
    /// Gets or sets the content to be rendered in the header section of the component.
    /// </summary>
    [Parameter]
    public RenderFragment? HeaderContent { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the copy button is displayed in the code viewer component.
    /// </summary>
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

    /// <summary>
    /// Gets or sets a value indicating whether the content is editable by the user.
    /// </summary>
    [Parameter]
    public bool Editable { get; set; }

    /// <summary>
    /// Gets or sets the callback that is invoked when the code value changes.
    /// </summary>
    [Parameter]
    public EventCallback<string?> CodeChanged { get; set; }


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

        if (_editable.Value && !_tabEnabled)
        {
            await JS.InvokeVoidAsync("MudCode.enableTabIndent", _textAreaRef);
            _tabEnabled = true;
        }

        if (!_editable.Value)
        {
            _tabEnabled = false;
        }

        if (_shouldHighlight)
        {
            _shouldHighlight = false;
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

    /// <summary>
    /// Handles changes to component parameters asynchronously and refreshes the component state.
    /// </summary>
    protected async Task ParameterChanged()
    {
        await Task.Delay(1);
        await RefreshAsync();
    }

    /// <summary>
    /// Handles the copy button click event asynchronously, updates the copy icon to indicate success, and restores the
    /// original icon after a brief delay.
    /// </summary>
    protected async Task HandleCopyButtonClickAsync()
    {
        await CopyAsync();
        _copyIcon = Icons.Material.Filled.DoneAll;
        StateHasChanged();
        await Task.Delay(2000);
        _copyIcon = Icons.Material.Filled.ContentCopy;
        StateHasChanged();
    }

    private async Task OnInput(ChangeEventArgs e)
    {
        await _code.SetValueAsync(e.Value?.ToString());
        _shouldHighlight = true;
    }
}
