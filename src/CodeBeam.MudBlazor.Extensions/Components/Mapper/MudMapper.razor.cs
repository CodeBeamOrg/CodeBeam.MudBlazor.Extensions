using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using MudBlazor.Utilities;
using MudExtensions.Utilities;

namespace MudExtensions
{
    /// <summary>
    /// Represents a confirmed default value for a target header when no source item is mapped.
    /// </summary>
    public class ConfirmedDefaultValue
    {
        /// <summary>
        /// The default value to use.
        /// </summary>
        public string? DefaultValue { get; set; }

        /// <summary>
        /// Whether the default value has been confirmed by the user.
        /// </summary>
        public bool Confirmed { get; set; }
    }

    /// <summary>
    /// Represents a target field that source items can be mapped onto.
    /// </summary>
    public class MudExpectedHeader
    {
        /// <summary>
        /// CSS applied to the drop zone when this required header has no match.
        /// </summary>
        public readonly string? RequiredCss = "border-color: var(--mud-palette-error); color: var(--mud-palette-error);";

        /// <summary>
        /// The name of the target header.
        /// </summary>
        public string? Name { get; set; } = "";

        /// <summary>
        /// Aliases for the header. If any alias matches a source item name it is treated as a match.
        /// </summary>
        public IEnumerable<string>? Aliases { get; set; } = null;

        /// <summary>
        /// Whether this header must be mapped before the user can confirm.
        /// </summary>
        public bool Required { get; set; }

        /// <summary>
        /// Whether the user may supply a default value instead of mapping a source item.
        /// </summary>
        public bool AllowDefaultValue { get; set; }

        /// <summary>
        /// Internal UI state: whether the default-value entry form is expanded.
        /// </summary>
        public bool CreatingDefaultValue { get; set; }

        /// <summary>
        /// Number of source items currently mapped to this header.
        /// </summary>
        public int MatchedFieldCount { get; set; } = 0;

        /// <summary>Initializes a new instance.</summary>
        public MudExpectedHeader() { }

        /// <summary>Initializes a new instance with the given name.</summary>
        public MudExpectedHeader(string? name)
        {
            Name = name;
            Required = false;
        }

        /// <summary>Initializes a new instance with the given name and required flag.</summary>
        public MudExpectedHeader(string? name, bool required = false)
        {
            Name = name;
            Required = required;
        }

        /// <summary>Initializes a new instance with name, required and allowDefaultValue flags.</summary>
        public MudExpectedHeader(string? name, bool required = false, bool allowDefaultValue = false)
        {
            Name = name;
            Required = required;
            AllowDefaultValue = allowDefaultValue;
        }

        /// <summary>Initializes a new instance with all properties.</summary>
        public MudExpectedHeader(string? name, bool required = false, bool allowDefaultValue = false, IEnumerable<string>? aliases = null)
        {
            Name = name;
            Required = required;
            AllowDefaultValue = allowDefaultValue;
            Aliases = aliases;
        }
    }

    /// <summary>
    /// Represents a source item that can be dragged onto a target header zone.
    /// </summary>
    public class MudMapperItem
    {
        /// <summary>
        /// The display name of the source item.
        /// </summary>
        public string Name { get; set; } = "";

        /// <summary>
        /// The identifier of the drop zone this item currently occupies.
        /// Defaults to <c>"Source"</c> (the unassigned pool).
        /// </summary>
        public string MappedZone { get; set; } = "Source";

        /// <summary>Initializes a new instance.</summary>
        /// <param name="name">Display name.</param>
        /// <param name="mappedZone">Initial zone identifier; defaults to <c>"Source"</c>.</param>
        public MudMapperItem(string? name, string? mappedZone = "Source")
        {
            Name = name ?? "";
            MappedZone = mappedZone ?? "Source";
        }
    }

    /// <summary>
    /// A standalone drag-and-drop field mapper component.
    /// Allows users to map source items onto target headers without any file or CSV dependency.
    /// </summary>
    public partial class MudMapper : MudComponentBase
    {
        /// <summary>
        /// The identifier for the drop zone that contains unassigned source items.
        /// </summary>
        public const string SourcePoolZoneIdentifier = "__mud_mapper_source_pool__";

        /// <summary>
        /// CSS class for the root element.
        /// </summary>
        protected string? Classname =>
            new CssBuilder("mud-mapper")
            .AddClass(Class)
            .Build();

        /// <summary>
        /// Localized display strings for the component.
        /// </summary>
        [Parameter]
        public MudMapperLocalizedStrings LocalizedStrings { get; set; } = new();

        /// <summary>
        /// The list of target headers that source items can be mapped onto.
        /// </summary>
        [Parameter]
        public List<MudExpectedHeader> TargetHeaders { get; set; } = new();

        /// <summary>
        /// The source items available for mapping. Mutated in-place as the user drags items.
        /// </summary>
        [Parameter]
        public List<MudMapperItem> SourceItems { get; set; } = new();

        /// <summary>
        /// Whether the user may create new target headers at runtime.
        /// </summary>
        [Parameter]
        public bool AllowCreateTargetHeaders { get; set; }

        /// <summary>
        /// Whether to show the "Include unmapped data" toggle.
        /// </summary>
        [Parameter]
        public bool ShowIncludeUnmappedData { get; set; }

        /// <summary>
        /// Label for the confirm action button. Defaults to the value in <see cref="LocalizedStrings"/>.
        /// </summary>
        [Parameter]
        public string? ConfirmLabel { get; set; }

        /// <summary>
        /// Icon for the confirm action button.
        /// </summary>
        [Parameter]
        public string ConfirmIcon { get; set; } = Icons.Material.Filled.Check;

        /// <summary>
        /// Fires when the user clicks the confirm button and the mapping is valid.
        /// </summary>
        [Parameter]
        public EventCallback OnConfirmed { get; set; }

        /// <summary>
        /// Fires when the user clicks the reset button.
        /// </summary>
        [Parameter]
        public EventCallback OnReset { get; set; }

        /// <summary>
        /// Whether the user has already confirmed the mapping.
        /// Controls whether the confirm button or the reset button is displayed.
        /// </summary>
        public bool IsConfirmed { get; private set; }

        /// <summary>
        /// Whether the current mapping satisfies all required target headers.
        /// </summary>
        public bool IsValid => _valid;

        /// <summary>
        /// Whether source items that are not mapped to any target should be included in the output.
        /// Readable after the user confirms.
        /// </summary>
        public bool IncludeUnmappedData { get; private set; }

        /// <summary>
        /// The confirmed default values keyed by target header name.
        /// Readable after the user confirms.
        /// </summary>
        public IReadOnlyDictionary<string, ConfirmedDefaultValue>? DefaultValues => _defaultValueHeaders;

        [Inject] private NavigationManager? _navigationManager { get; set; }

        private bool _valid = false;
        private readonly string _requiredDefaultValueMessage = "Default value is required if no header is mapped";
        private readonly string _expectedHeaderDropZoneWidth = "width: 180px;";
        private MudExpectedHeader _model { get; set; } = new();
        private bool _addSectionOpen;
        private Dictionary<string, ConfirmedDefaultValue>? _defaultValueHeaders;

        /// <inheritdoc />
        protected override void OnInitialized()
        {
            base.OnInitialized();
            EnsureDefaultValueHeaders();
        }

        /// <inheritdoc />
        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            EnsureDefaultValueHeaders();
            SyncMappedZonesToHeaders();
        }

        private static bool ItemSelector(MudMapperItem item, string? identifier)
        {
            return item.MappedZone == identifier;
        }

        /// <summary>
        /// Determines whether a given source item is currently in the unassigned source pool.
        /// </summary>
        /// <param name="item">The source item to check.</param>
        /// <returns>True if the item is in the unassigned source pool; otherwise, false.</returns>
        public static bool IsSourcePoolItem(MudMapperItem item)
        {
            return string.Equals(item.MappedZone, SourcePoolZoneIdentifier, StringComparison.OrdinalIgnoreCase);
        }

        private void EnsureDefaultValueHeaders()
        {
            var defaults = new Dictionary<string, ConfirmedDefaultValue>(StringComparer.OrdinalIgnoreCase);
            foreach (var header in TargetHeaders.Where(x => x.AllowDefaultValue))
            {
                var key = header.Name ?? string.Empty;
                if (_defaultValueHeaders?.TryGetValue(key, out var existing) == true)
                {
                    defaults[key] = new ConfirmedDefaultValue
                    {
                        Confirmed = existing.Confirmed,
                        DefaultValue = existing.DefaultValue
                    };
                }
                else
                {
                    defaults[key] = new ConfirmedDefaultValue { Confirmed = false, DefaultValue = "" };
                }
            }

            _defaultValueHeaders = defaults;
        }

        private void SyncMappedZonesToHeaders()
        {
            TargetHeaders.ForEach(h => h.MatchedFieldCount = 0);

            foreach (var item in SourceItems)
            {
                if (item == null) continue;
                if (string.IsNullOrWhiteSpace(item.MappedZone))
                {
                    item.MappedZone = SourcePoolZoneIdentifier;
                    continue;
                }

                if (string.Equals(item.MappedZone, SourcePoolZoneIdentifier, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                var header = TargetHeaders.FirstOrDefault(h => string.Equals(h.Name, item.MappedZone, StringComparison.OrdinalIgnoreCase));
                if (header == null)
                {
                    item.MappedZone = SourcePoolZoneIdentifier;
                    continue;
                }

                header.MatchedFieldCount++;
            }

            CheckValid();
        }

        private void OnDrop(MudItemDropInfo<MudMapperItem> dropInfo)
        {
            if (dropInfo.Item == null || dropInfo.DropzoneIdentifier == null)
            {
                return;
            }

            dropInfo.Item.MappedZone = dropInfo.DropzoneIdentifier;
            SyncMappedZonesToHeaders();
        }

        private void CheckValid()
        {
            foreach (var requiredHeader in TargetHeaders.Where(h => h.Required))
            {
                if (SourceItems.Any(i => i.MappedZone == requiredHeader.Name)) continue;
                if (_defaultValueHeaders?.Any(x => x.Key == requiredHeader.Name && x.Value.Confirmed) == true) continue;
                _valid = false;
                return;
            }
            _valid = true;
        }

        private void OpenAddSection() => _addSectionOpen = true;

        private void SubmitDefaultValue(string? name)
        {
            if (_defaultValueHeaders == null) return;
            var key = name ?? "";
            if (!string.IsNullOrWhiteSpace(_defaultValueHeaders[key].DefaultValue))
            {
                _defaultValueHeaders[key].Confirmed = !_defaultValueHeaders[key].Confirmed;
                CheckValid();
            }
        }

        private void OnSubmit(EditContext context)
        {
            if (string.IsNullOrWhiteSpace(_model.Name)) return;
            TargetHeaders.Add(_model);
            if (_model.AllowDefaultValue)
            {
                _defaultValueHeaders?.Add(_model.Name, new ConfirmedDefaultValue { Confirmed = false, DefaultValue = "" });
            }
            _model = new();
            _addSectionOpen = false;
        }

        private async Task HandleConfirm()
        {
            IsConfirmed = true;
            await OnConfirmed.InvokeAsync();
        }

        private async Task HandleReset()
        {
            IsConfirmed = false;
            await OnReset.InvokeAsync();
        }

        private void OnIncludeUnmappedDataChanged(bool value)
        {
            IncludeUnmappedData = value;
        }

        /// <summary>
        /// Resets mapping state so the component can be reused for a new set of source items.
        /// </summary>
        public void ResetMapping()
        {
            IsConfirmed = false;
            IncludeUnmappedData = false;
            foreach (var item in SourceItems)
            {
                item.MappedZone = SourcePoolZoneIdentifier;
            }
            _defaultValueHeaders = TargetHeaders
                .Where(x => x.AllowDefaultValue)
                .ToDictionary(
                    key => key.Name ?? string.Empty,
                    _ => new ConfirmedDefaultValue { Confirmed = false, DefaultValue = "" },
                    StringComparer.OrdinalIgnoreCase);
            SyncMappedZonesToHeaders();
            _valid = false;
        }
    }
}
