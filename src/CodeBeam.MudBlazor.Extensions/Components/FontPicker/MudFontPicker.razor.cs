using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using MudExtensions.Utilities;

namespace MudExtensions;

/// <summary>
/// A font picker based on MudAutocomplete.
/// </summary>
public partial class MudFontPicker : MudComponentBase
{
    /// <summary>
    /// The selected font.
    /// </summary>
    [Parameter]
    public string? Font { get; set; }

    /// <summary>
    /// Occurs when the selected font changes.
    /// </summary>
    [Parameter]
    public EventCallback<string?> FontChanged { get; set; }

    /// <summary>
    /// The available fonts.
    /// </summary>
    [Parameter]
    public List<string> FontCollection { get; set; } = GoogleFonts.FontListLatin;

    /// <summary>
    /// The root URL used to import fonts.
    /// </summary>
    [Parameter]
    public string? RootImportUrl { get; set; }

    /// <summary>
    /// Uses the theme font for the input text.
    /// </summary>
    [Parameter]
    public bool StaticInputText { get; set; }

    /// <summary>
    /// The label displayed for the input.
    /// </summary>
    [Parameter] public string? Label { get; set; }

    /// <summary>
    /// The placeholder text displayed when the input is empty.
    /// </summary>
    [Parameter] public string? Placeholder { get; set; }

    /// <summary>
    /// Additional CSS classes applied to the autocomplete popover.
    /// </summary>
    [Parameter] public string? PopoverClass { get; set; }

    /// <summary>
    /// The visual variant of the input.
    /// </summary>
    [Parameter] public Variant Variant { get; set; } = Variant.Text;

    /// <summary>
    /// The spacing around the input.
    /// </summary>
    [Parameter] public Margin Margin { get; set; } = Margin.None;

    /// <summary>
    /// Reduces the vertical spacing of the input when enabled.
    /// </summary>
    [Parameter] public bool Dense { get; set; }

    /// <summary>
    /// Requires a value to be selected.
    /// </summary>
    [Parameter] public bool Required { get; set; }

    /// <summary>
    /// The validation message displayed when a required value is missing.
    /// </summary>
    [Parameter] public string RequiredError { get; set; } = "Required";

    /// <summary>
    /// Prevents user interaction when enabled.
    /// </summary>
    [Parameter] public bool Disabled { get; set; }

    /// <summary>
    /// Prevents the user from modifying the selected value.
    /// </summary>
    [Parameter] public bool ReadOnly { get; set; }

    /// <summary>
    /// Expands the input to fill the available width.
    /// </summary>
    [Parameter] public bool FullWidth { get; set; }

    /// <summary>
    /// Automatically focuses the input after its initial render.
    /// </summary>
    [Parameter] public bool AutoFocus { get; set; }

    /// <summary>
    /// Updates the input value immediately as the user types.
    /// </summary>
    [Parameter] public bool Immediate { get; set; }

    /// <summary>
    /// Displays the input underline when enabled.
    /// </summary>
    [Parameter] public bool Underline { get; set; } = true;

    /// <summary>
    /// Helper text displayed below the input.
    /// </summary>
    [Parameter] public string? HelperText { get; set; }

    /// <summary>
    /// Displays helper text only while the input has focus.
    /// </summary>
    [Parameter] public bool HelperTextOnFocus { get; set; }

    /// <summary>
    /// Displays a character counter when specified.
    /// </summary>
    [Parameter] public int? Counter { get; set; }

    /// <summary>
    /// The maximum number of characters allowed in the input.
    /// </summary>
    [Parameter] public int MaxLength { get; set; } = 524288;

    /// <summary>
    /// The maximum number of suggestions displayed in the dropdown.
    /// </summary>
    [Parameter] public int MaxItems { get; set; } = 10;

    /// <summary>
    /// The minimum number of characters required before searching.
    /// </summary>
    [Parameter] public int MinCharacters { get; set; }

    /// <summary>
    /// The delay, in milliseconds, before searching after the input changes.
    /// </summary>
    [Parameter] public int DebounceInterval { get; set; } = 100;

    /// <summary>
    /// The position of the input adornment.
    /// </summary>
    [Parameter] public Adornment Adornment { get; set; } = Adornment.None;

    /// <summary>
    /// The accessible label for the input adornment.
    /// </summary>
    [Parameter] public string? AdornmentAriaLabel { get; set; }

    /// <summary>
    /// The color of the input adornment.
    /// </summary>
    [Parameter] public Color AdornmentColor { get; set; } = Color.Default;

    /// <summary>
    /// The icon displayed in the input adornment.
    /// </summary>
    [Parameter] public string? AdornmentIcon { get; set; }

    /// <summary>
    /// The text displayed in the input adornment.
    /// </summary>
    [Parameter] public string? AdornmentText { get; set; }

    /// <summary>
    /// The size of the adornment icon.
    /// </summary>
    [Parameter] public Size IconSize { get; set; } = Size.Medium;

    /// <summary>
    /// Occurs when the input adornment is clicked.
    /// </summary>
    [Parameter] public EventCallback<MouseEventArgs> OnAdornmentClick { get; set; }

    /// <summary>
    /// Selects the current input text when the input is activated.
    /// </summary>
    [Parameter] public bool SelectOnActivation { get; set; } = true;

    /// <summary>
    /// Resets the selected value when the input text is cleared.
    /// </summary>
    [Parameter] public bool ResetValueOnEmptyText { get; set; }

    /// <summary>
    /// Displays a button for clearing the selected value.
    /// </summary>
    [Parameter] public bool Clearable { get; set; }

    /// <summary>
    /// Selects the highlighted suggestion when the Tab key is pressed.
    /// </summary>
    [Parameter] public bool SelectValueOnTab { get; set; }

    /// <summary>
    /// Performs validation only after the input value has been modified.
    /// </summary>
    [Parameter] public bool OnlyValidateIfDirty { get; set; }

    /// <summary>
    /// Indicates whether the input is in an error state.
    /// </summary>
    [Parameter] public bool Error { get; set; }

    /// <summary>
    /// The ID of the element containing the validation error message.
    /// </summary>
    [Parameter] public string? ErrorId { get; set; }

    /// <summary>
    /// The validation error message displayed below the input.
    /// </summary>
    [Parameter] public string? ErrorText { get; set; }

    /// <summary>
    /// The origin point used to anchor the autocomplete popover.
    /// </summary>
    [Parameter] public Origin AnchorOrigin { get; set; } = Origin.BottomCenter;

    /// <summary>
    /// The origin point used to position the autocomplete popover relative to its anchor.
    /// </summary>
    [Parameter] public Origin TransformOrigin { get; set; } = Origin.TopCenter;

    private string? InputStyle =>
        StaticInputText || string.IsNullOrWhiteSpace(Font)
            ? Style
            : $"{Style};font-family:\"{Font}\" !important";

    /// <summary>
    /// Returns the CSS import declaration for the specified font.
    /// </summary>
    public string ImportFontStyleText(string? font)
    {
        var fontName = font?.Replace(" ", "+");

        var url = string.IsNullOrEmpty(RootImportUrl)
            ? $"https://fonts.googleapis.com/css2?family={fontName}"
            : $"{RootImportUrl}{fontName}";

        return $"@import url('{url}')";
    }

    /// <summary>
    /// Clears the selected font.
    /// </summary>
    public Task ClearFont()
    {
        return SetFontValue(null);
    }

    private async Task SetFontValue(string? font)
    {
        if (Font == font)
            return;

        Font = font;
        await FontChanged.InvokeAsync(font);
    }

    private Task<IEnumerable<string>?> SearchFonts(
        string? searchText,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        IEnumerable<string> fonts = FontCollection ?? [];

        if (!string.IsNullOrWhiteSpace(searchText))
        {
            fonts = fonts.Where(font =>
                font.StartsWith(
                    searchText.Trim(),
                    StringComparison.OrdinalIgnoreCase));
        }

        return Task.FromResult<IEnumerable<string>?>(fonts);
    }
}
