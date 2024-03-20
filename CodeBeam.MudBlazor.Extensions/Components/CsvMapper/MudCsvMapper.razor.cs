using System.ComponentModel;
using CsvHelper;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using MudBlazor.Utilities;
using MudExtensions.Utilities;
using System.Globalization;
using System.Text;
using CsvHelper.Configuration;

namespace MudExtensions
{
    internal class ConfirmedDefaultValue
    {
        public string DefaultValue { get; set; }
        public bool Confirmed { get; set; }
    }

    //Default fields in your database
    public class MudExpectedHeader
    {
        public readonly string RequiredCss = "border-color: var(--mud-palette-error); color: var(--mud-palette-error);";
        public string Name { get; set; } = "";
        public bool Required { get; set; }
        public bool AllowDefaultValue { get; set; }
        public bool CreatingDefaultValue { get; set; }
        public int MatchedFieldCount { get; set; } = 0;

        public MudExpectedHeader()
        {
        }
        public MudExpectedHeader(string name)
        {
            Name = name;
            Required = false;
        }
        public MudExpectedHeader(string name, bool required = false)
        {
            Name = name;
            Required = required;
        }
        public MudExpectedHeader(string name, bool required = false, bool allowDefaultValue = false)
        {
            Name = name;
            Required = required;
            AllowDefaultValue = allowDefaultValue;
        }
    }

    //Header fields in your CSV File
    public class MudCsvHeader
    {
        public string Name { get; set; } = "";
        public string MappedField { get; set; } = "File";

        public MudCsvHeader(string name, string mappedField = "File")
        {
            Name = name;
            MappedField = mappedField;
        }
    }

    public partial class MudCsvMapper : MudComponentBase
    {
        protected string Classname =>
           new CssBuilder("mud-csv-mapper")
           .AddClass(Class)
           .Build();

        /// <summary>
        /// A class for provide all local strings at once.
        /// </summary>
        [Parameter]
        public CsvMapperLocalizedStrings LocalizedStrings { get; set; } = new();

        /// <summary>
        /// Choose Table Column Headers
        /// </summary>
        [Parameter]
        public List<MudExpectedHeader> ExpectedHeaders { get; set; } = new();

        private bool _valid = false;

        [Parameter]
        public IBrowserFile CsvFile { get; set; } = null;

        [Parameter]
        public byte[] FileContentByte { get; set; }

        //if you want to see what was mapped use this dictionary
        [Parameter]
        public Dictionary<string, string> CsvMapping { get; set; } = new();

        [Parameter]
        public EventCallback<bool> OnImported { get; set; }

        [Parameter]
        public bool ShowIncludeUnmappedData { get; set; }

        [Parameter]
        public bool AllowCreateExpectedHeaders { get; set; }

        [Parameter]
        public bool NormalizeHeaders { get; set; }

        [Parameter]
        public string Delimter { get; set; } = ",";

        [Inject] private IDialogService _dialogService { get; set; }
        [Inject] private NavigationManager _navigationManager { get; set; }

        private string DragClass = DefaultDragClass;
        private static readonly string DefaultDragClass = "relative rounded-lg border-2 border-dashed pa-4 mt-4 mud-width-full mud-height-full z-10";
        private readonly string _requiredDefaultValueMessage = "Default value is required if no header is mapped";
        private readonly string _expectedHeaderDropZoneWidth = "width: 180px;";
        private List<string> FileNames = new ();
        private List<MudCsvHeader> MudCsvHeaders = new();
        private List<IDictionary<string,object>> CsvContent;
        private bool _includeUnmappedData;
        private bool _importedComplete;

        private MudExpectedHeader _model { get; set; } = new();
        private bool _addSectionOpen;
        private Dictionary<string, ConfirmedDefaultValue> _defaultValueHeaders { get; set; }
        protected override void OnInitialized()
        {
            _defaultValueHeaders = ExpectedHeaders.Where(x =>x.AllowDefaultValue).ToDictionary(key => key.Name, val => new ConfirmedDefaultValue()
            {
                Confirmed = false,
                DefaultValue = ""
            });
        }
        private async Task OnInputFileChanged(InputFileChangeEventArgs args)
        {
            Reset();
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
                MatchCsvHeadersWithExpectedHeaders();
            }
        }
        private void Reset()
        {
            MudCsvHeaders = new();
            ExpectedHeaders.ForEach(x => x.MatchedFieldCount = 0);
        }
        private async Task ReadFile(IBrowserFile file)
        {
            long maxFileSize = 1024 * 1024 * 15;
            await using var stream = new MemoryStream();
            var buffer = new byte[file.Size];

            await using var newFileStream = file.OpenReadStream(maxFileSize);

            int bytesRead;
            double totalRead = 0;
            while ((bytesRead = await newFileStream.ReadAsync(buffer)) != 0)
            {
                totalRead += bytesRead;
                await stream.WriteAsync(buffer, 0, bytesRead);
            }
            FileContentByte = stream.GetBuffer();
        }
        private void CreateCsvContent()
        {
            using var reader = new StreamReader(new MemoryStream(FileContentByte), Encoding.Default);
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = Delimter,
                IgnoreBlankLines = true,
                HasHeaderRecord = true
            };

            using var csv = new CsvReader(reader, config);
            CsvContent = csv.GetRecords<dynamic>().Select(x => (IDictionary<string, object>)x).ToList();
        }
        private void MatchCsvHeadersWithExpectedHeaders()
        {
            var csvFields = CsvContent.FirstOrDefault().Keys;
            foreach (var csvField in csvFields)
            {
                // You can add other matching try as FuzzySharp here
                bool isMatched = TryExactMatch(csvField);

                if (isMatched) continue;
                MudCsvHeaders.Add(new MudCsvHeader(csvField));
            }

            IsValid();

        }
        private bool TryExactMatch(string csvField)
        {
            foreach (var expectedField in ExpectedHeaders)
            {
                if (string.Compare(expectedField.Name, csvField, StringComparison.CurrentCultureIgnoreCase) != 0) continue;
                if (expectedField.MatchedFieldCount != 0) continue;

                MudCsvHeaders.Add(new MudCsvHeader(csvField, expectedField.Name));
                expectedField.MatchedFieldCount++;
                return true;
            }
            return false;
        }
        private async Task OnImport()
        {
            var config = new CsvHelper.Configuration.CsvConfiguration(CultureInfo.InvariantCulture)
            {
                PrepareHeaderForMatch = (header) => header.Header,
                Delimiter = Delimter
            };
            UpdateHeaderLineWithMatchedFields();
            if(!_includeUnmappedData) RemoveUnmappedData();
            AddDefaultValues();
            await using (var writer = new StringWriter())
            await using (var csv = new CsvWriter(writer, config))
            {
                var dynamicContent = CsvContent.Cast<dynamic>();
                await csv.WriteRecordsAsync(dynamicContent);

                var str = writer.ToString();
                FileContentByte = Encoding.UTF8.GetBytes(str);
            }
            await OnImported.InvokeAsync();
            await Task.Delay(100);
            _importedComplete = true;
        }
        private void UpdateHeaderLineWithMatchedFields()
        {
            foreach (var map in MudCsvHeaders)
            {
                if (map.MappedField == "File") continue;
                var normalizedMappedField = Normalize(map.MappedField);
                foreach (var row in CsvContent)
                {
                    var temp = row[map.Name];
                    row.Remove(map.Name);
                    row[normalizedMappedField] = temp;
                }
                CsvMapping.Add(map.MappedField, map.Name);
            }
        }
        private void AddDefaultValues()
        {
            foreach (var record in CsvContent)
            {
                foreach (var header in _defaultValueHeaders.Where(header => header.Value.Confirmed))
                {
                    var normalizedDefaultHeader = Normalize(header.Key);
                    if (record.Keys.Contains(header.Key))
                        throw new Exception("Shouldn't happen");
                    record[normalizedDefaultHeader] = header.Value.DefaultValue;
                }
            }
        }
        private void RemoveUnmappedData()
        {
            var unMappedHeaders = MudCsvHeaders.Where(x => x.MappedField == "File").Select(x => x.Name);
            foreach (var record in CsvContent)
            {
                foreach (var unMappedHeader in unMappedHeaders)
                {
                    record.Remove(unMappedHeader);
                }
            }
        }

        private string Normalize(string str)
        {
            return NormalizeHeaders ? str.Replace(" ", "").Replace("\"", "").ToLower() : str;
        }
        /* handling board events */
        private void OnDrop(MudItemDropInfo<MudCsvHeader> mudCSVField)
        {
            string oldMappedField = mudCSVField.Item.MappedField;
            mudCSVField.Item.MappedField = mudCSVField.DropzoneIdentifier;
            DecrementOldMatchedFieldCount(oldMappedField);
            IncrementNewMatchedFieldCount(mudCSVField.DropzoneIdentifier);
            IsValid();
        }
        private void DecrementOldMatchedFieldCount(string fieldName)
        {
            foreach (var expectedHeader in ExpectedHeaders.Where(expectedHeader => expectedHeader.Name == fieldName))
            {
                expectedHeader.MatchedFieldCount--;
            }
        }
        private void IncrementNewMatchedFieldCount(string fieldName)
        {
            foreach (var expectedHeader in ExpectedHeaders)
            {
                if (expectedHeader.Name == fieldName)
                {
                    expectedHeader.MatchedFieldCount++;
                }
            }
        }
        private void IsValid()
        {
            foreach (MudExpectedHeader requiredHeader in ExpectedHeaders.Where(i => i.Required))
            {
                if (MudCsvHeaders.Any(i => i.MappedField == requiredHeader.Name)) continue;
                if (_defaultValueHeaders.Any(x =>
                        x.Key == requiredHeader.Name && x.Value.Confirmed))
                {
                    continue;
                }
                _valid = false;
                return;
            }
            _valid = true;
        }
        private void SetDragClass()
        {
            DragClass = $"{DefaultDragClass} mud-border-primary";
        }
        private void ClearDragClass()
        {
            DragClass = DefaultDragClass;
        }
        private static bool ItemSelector(MudCsvHeader item, string identifier)
        {
            return item.MappedField == identifier;
        }
        private void OpenAddSection()
        {
            _addSectionOpen = true;
        }
        private void SubmitDefaultValue(string name)
        {
            if (!string.IsNullOrWhiteSpace(_defaultValueHeaders[name].DefaultValue))
            {
                _defaultValueHeaders[name].Confirmed = !_defaultValueHeaders[name].Confirmed;
                IsValid();
            }
        }
        private void OnSubmit(EditContext context)
        {
            if (string.IsNullOrWhiteSpace(_model.Name)) return;
            ExpectedHeaders.Add(_model);
            if (_model.AllowDefaultValue)
            {
                _defaultValueHeaders.Add(_model.Name, new ConfirmedDefaultValue()
                {
                    Confirmed = false,
                    DefaultValue = ""
                });
            }
            _model = new();
            _addSectionOpen = false;
        }
        private void ReloadPage()
        {
            _navigationManager.NavigateTo(_navigationManager.Uri, forceLoad:true);
        }
    }
}
