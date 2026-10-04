using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using MudBlazor.Utilities;
using MudExtensions.Utilities;
using System.Globalization;
using System.Text;

namespace MudExtensions
{
    /// <summary>
    /// Backward-compatibility alias for <see cref="MudMapperItem"/>.
    /// </summary>
    [Obsolete("MudCsvHeader has been renamed to MudMapperItem. Please update your code to use MudMapperItem and its MappedZone property instead of MappedField.")]
    public class MudCsvHeader : MudMapperItem
    {
        /// <inheritdoc/>
        public MudCsvHeader(string? name, string? mappedField = "Source")
            : base(name, mappedField) { }
    }

    /// <summary>
    /// A component that combines a CSV file upload with a <see cref="MudMapper"/> to let users
    /// map CSV columns onto expected target headers and produce a re-mapped CSV output.
    /// </summary>
    public partial class MudCsvMapper : MudComponentBase
    {
        /// <summary>CSS class for the root element.</summary>
        protected string? Classname =>
           new CssBuilder("mud-csv-mapper")
           .AddClass(Class)
           .Build();

        /// <summary>
        /// A class to provide all localized strings at once.
        /// </summary>
        [Parameter]
        public CsvMapperLocalizedStrings LocalizedStrings { get; set; } = new();

        /// <summary>
        /// The expected target headers that CSV columns should be mapped onto.
        /// </summary>
        [Parameter]
        public List<MudExpectedHeader> ExpectedHeaders { get; set; } = new();

        /// <summary>
        /// The uploaded CSV file as a browser file reference.
        /// </summary>
        [Parameter]
        public IBrowserFile? CsvFile { get; set; } = null;

        /// <summary>
        /// The raw bytes of the (re-mapped) CSV file after import.
        /// </summary>
        [Parameter]
        public byte[]? FileContentByte { get; set; }

        /// <summary>
        /// A dictionary of the mappings that were applied: key = target header name, value = original CSV column name.
        /// </summary>
        [Parameter]
        public Dictionary<string, string> CsvMapping { get; set; } = new();

        /// <summary>
        /// Fires when the CSV has been successfully imported and re-mapped.
        /// </summary>
        [Parameter]
        public EventCallback<bool> OnImported { get; set; }

        /// <summary>
        /// Whether to show the "Include unmapped data" toggle inside the mapper.
        /// </summary>
        [Parameter]
        public bool ShowIncludeUnmappedData { get; set; }

        /// <summary>
        /// Whether the user may create new target headers at runtime.
        /// </summary>
        [Parameter]
        public bool AllowCreateExpectedHeaders { get; set; }

        /// <summary>
        /// When <c>true</c>, header names are normalised (lowercased, spaces and quotes stripped)
        /// before being written to the output CSV.
        /// </summary>
        [Parameter]
        public bool NormalizeHeaders { get; set; }

        /// <summary>
        /// The column delimiter used when reading and writing the CSV file. Defaults to <c>","</c>.
        /// </summary>
        [Parameter]
        public string Delimiter { get; set; } = ",";

        [Inject] private NavigationManager? _navigationManager { get; set; }

        private MudMapper? _mapper;

        private MudMapperLocalizedStrings _mapperLocalizedStrings => new()
        {
            SourceItems = "CSV File Headers",
            TargetHeaders = LocalizedStrings.ExpectedHeaders,
            DragHere = LocalizedStrings.DragHere,
            DefineHeaders = LocalizedStrings.DefineHeaders
        };

        private string DragClass = DefaultDragClass;
        private static readonly string DefaultDragClass = "relative rounded-lg border-2 border-dashed pa-4 mt-4 mud-width-full mud-height-full z-10";

        private List<string> FileNames = new();
        private List<MudMapperItem> _sourceItems = new();
        private List<IDictionary<string, object?>>? CsvContent;

        private async Task OnInputFileChanged(InputFileChangeEventArgs args)
        {
            ResetMapping();
            ClearDragClass();
            var files = args.GetMultipleFiles();
            foreach (var file in files)
            {
                FileNames.Add(file.Name);
            }
            if (files.Count > 0)
            {
                CsvFile = files[0];
                await ReadFile(files[0]);
                CreateCsvContent();
                MatchSourceItemsWithExpectedHeaders();
            }
        }

        private void ResetMapping()
        {
            _sourceItems = new();
            CsvMapping.Clear();
            CsvContent = null;
            FileContentByte = null;
            _mapper?.ResetMapping();
            foreach (var header in ExpectedHeaders)
            {
                header.MatchedFieldCount = 0;
            }
        }

        private async Task ReadFile(IBrowserFile file)
        {
            long maxFileSize = 1024 * 1024 * 15;
            await using var stream = new MemoryStream();
            var buffer = new byte[file.Size];

            await using var newFileStream = file.OpenReadStream(maxFileSize);

            int bytesRead;
            while ((bytesRead = await newFileStream.ReadAsync(buffer)) != 0)
            {
                await stream.WriteAsync(buffer, 0, bytesRead);
            }
            FileContentByte = stream.GetBuffer();
        }

        private void CreateCsvContent()
        {
            using var reader = new StreamReader(new MemoryStream(FileContentByte ?? Array.Empty<byte>()), Encoding.Default);
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = Delimiter,
                IgnoreBlankLines = true,
                HasHeaderRecord = true
            };

            using var csv = new CsvReader(reader, config);
            CsvContent = csv.GetRecords<dynamic>().Select(x => (IDictionary<string, object?>)x).ToList();
        }

        /// <summary>
        /// Matches CSV column names against the expected headers (exact match first, then aliases).
        /// Unmatched columns are left in the source pool.
        /// </summary>
        private void MatchSourceItemsWithExpectedHeaders()
        {
            _sourceItems = new List<MudMapperItem>();
            var csvFields = CsvContent?.FirstOrDefault()?.Keys;
            foreach (var csvField in csvFields ?? new List<string>())
            {
                if (TryExactMatch(csvField)) continue;
                if (TryAliasMatch(csvField)) continue;
                _sourceItems.Add(new MudMapperItem(csvField, MudMapper.SourcePoolZoneIdentifier));
            }
        }

        private bool TryExactMatch(string csvField)
        {
            foreach (var expectedField in ExpectedHeaders)
            {
                if (string.Compare(expectedField.Name, csvField, StringComparison.CurrentCultureIgnoreCase) != 0) continue;
                if (expectedField.MatchedFieldCount != 0) continue;

                _sourceItems.Add(new MudMapperItem(csvField, expectedField.Name));
                expectedField.MatchedFieldCount++;
                return true;
            }
            return false;
        }

        private bool TryAliasMatch(string csvField)
        {
            foreach (var expectedField in ExpectedHeaders)
            {
                if (expectedField.Aliases == null) continue;
                if (!expectedField.Aliases.Any(alias => string.Compare(alias, csvField, StringComparison.CurrentCultureIgnoreCase) == 0)) continue;
                if (expectedField.MatchedFieldCount != 0) continue;

                _sourceItems.Add(new MudMapperItem(csvField, expectedField.Name));
                expectedField.MatchedFieldCount++;
                return true;
            }
            return false;
        }

        private async Task OnImport()
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                PrepareHeaderForMatch = header => header.Header,
                Delimiter = Delimiter
            };

            UpdateHeadersWithMappedFields();

            bool includeUnmapped = _mapper?.IncludeUnmappedData ?? false;
            if (!includeUnmapped) RemoveUnmappedData();

            AddDefaultValues();

            await using (var writer = new StringWriter())
            await using (var csv = new CsvWriter(writer, config))
            {
                var dynamicContent = CsvContent?.Cast<dynamic>();
                await csv.WriteRecordsAsync(dynamicContent ?? Enumerable.Empty<dynamic>());

                var str = writer.ToString();
                FileContentByte = Encoding.UTF8.GetBytes(str);
            }

            await OnImported.InvokeAsync();
        }

        private void UpdateHeadersWithMappedFields()
        {
            var mappedItems = _sourceItems.Where(x => !MudMapper.IsSourcePoolItem(x));
            foreach (var map in mappedItems)
            {
                var normalizedTarget = Normalize(map.MappedZone);
                foreach (var row in CsvContent ?? new List<IDictionary<string, object?>>())
                {
                    var temp = row[map.Name];
                    row.Remove(map.Name);
                    row[normalizedTarget] = temp;
                }
                CsvMapping[map.MappedZone] = map.Name;
            }
        }

        private void AddDefaultValues()
        {
            AddDefaultValues(_mapper?.DefaultValues);
        }

        internal void AddDefaultValues(IReadOnlyDictionary<string, ConfirmedDefaultValue>? defaultValues)
        {
            if (defaultValues == null) return;

            foreach (var record in CsvContent ?? new List<IDictionary<string, object?>>())
            {
                foreach (var header in defaultValues.Where(h => h.Value.Confirmed))
                {
                    var normalizedKey = Normalize(header.Key);
                    if (record.Keys.Contains(header.Key))
                        throw new Exception("Shouldn't happen");
                    record[normalizedKey] = header.Value.DefaultValue;
                }
            }
        }

        private void RemoveUnmappedData()
        {
            var unmappedNames = _sourceItems.Where(x => MudMapper.IsSourcePoolItem(x)).Select(x => x.Name);
            foreach (var record in CsvContent ?? new List<IDictionary<string, object?>>())
            {
                foreach (var name in unmappedNames)
                {
                    record.Remove(name);
                }
            }
        }

        private string Normalize(string str)
        {
            return NormalizeHeaders ? str.Replace(" ", "").Replace("\"", "").ToLower() : str;
        }

        private void SetDragClass()
        {
            DragClass = $"{DefaultDragClass} mud-border-primary";
        }

        private void ClearDragClass()
        {
            DragClass = DefaultDragClass;
        }

        private void ReloadPage()
        {
            _navigationManager?.NavigateTo(_navigationManager.Uri, forceLoad: true);
        }
    }
}
