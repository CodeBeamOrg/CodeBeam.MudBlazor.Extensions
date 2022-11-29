using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using System.Text.RegularExpressions;
using System.Text;
using MudBlazor.Utilities;

namespace MudExtensions
{
    //Default fields in your database
    public class MudFieldHeader
    {
        public string Name { get; set; } = "";
        public bool Required { get; set; } = false;
        public int FieldCount { get; set; } = 0;

        public MudFieldHeader(string name)
        {
            Name = name;
            Required = false;
        }

        public MudFieldHeader(string name, bool required = false)
        {
            Name = name;
            Required = required;
        }
    }

    //Header fields in your CSV File
    public class MudCSVHeader
    {
        public string Name { get; set; } = "";
        public string MappedField { get; set; } = "File";

        public MudCSVHeader(string name, string mappedField = "File")
        {
            Name = name;
            MappedField = mappedField;
        }
    }

    public partial class MudCSVFieldMapper : MudComponentBase
    {
        protected string Classname =>
           new CssBuilder("mud-input-input-control")
           .AddClass(Class)
           .Build();

        /// <summary>
        /// Choose Table Column Headers
        /// </summary>
        [Parameter]
        public List<MudFieldHeader> MudFieldHeaders { get; set; } = new();

        private bool _valid = false;

        [Parameter]
        public IBrowserFile CSVFile { get; set; } = null;

        [Parameter]
        public byte[] FileContentByte { get; set; }

        //if you want to see what was mapped use this dictionary
        [Parameter]
        public Dictionary<string, string> CSVMapping { get; set; } = new();

        [Parameter]
        public EventCallback<bool> OnUpload { get; set; }

        private static string DefaultDragClass = "relative rounded-lg border-2 border-dashed pa-4 mt-4 mud-width-full mud-height-full z-10";
        private string DragClass = DefaultDragClass;
        private MudDropContainer<MudCSVHeader> DropContainer;
        private List<string> FileNames = new List<string>();
        private string HeaderLine = "";
        List<MudCSVHeader> MudCSVHeaders = new();
        string FileContentStr;

        protected override void OnInitialized()
        {
            base.OnInitialized();
        }

        //TODO SearchFunc 
        /// <summary>
        /// The SearchFunc returns a list of items matching the typed text
        /// </summary>
        //[Parameter]
        //public Func<string, string> SearchFunc { get; set; }

        private async Task OnInputFileChanged(InputFileChangeEventArgs args)
        {
            ClearDragClass();
            var files = args.GetMultipleFiles();
            foreach (var file in files)
            {
                FileNames.Add(file.Name);
            }
            if (files.Count > 0)
            {

                long maxFileSize = 1024 * 1024 * 15;
                using var stream = new MemoryStream();
                var buffer = new byte[files[0].Size];

                using var newFileStream = files[0].OpenReadStream(maxFileSize);

                int bytesRead;
                double totalRead = 0;

                while ((bytesRead = await newFileStream.ReadAsync(buffer)) != 0)
                {
                    totalRead += bytesRead;
                    await stream.WriteAsync(buffer, 0, bytesRead);
                }

                FileContentByte = stream.GetBuffer();
                var reader = new StreamReader(new MemoryStream(FileContentByte), Encoding.Default);
                HeaderLine = reader.ReadLine();
                ReadCSVHeaders(HeaderLine);
                CSVFile = files[0];
                FileContentStr = reader.ReadToEnd();
            }
        }

        public async Task Upload()
        {
            string NewHeader = HeaderLine;
            for (int i = 0; i < MudCSVHeaders.Count; i++)
            {
                if (MudCSVHeaders[i].MappedField != "File")
                {
                    NewHeader = Regex.Replace(NewHeader, String.Format(@"\b{0}\b", MudCSVHeaders[i].Name), MudCSVHeaders[i].MappedField);
                    CSVMapping.Add(MudCSVHeaders[i].MappedField, MudCSVHeaders[i].Name);
                }
            }

            FileContentStr = NewHeader + "\r\n" + FileContentStr;
            FileContentByte = System.Text.Encoding.UTF8.GetBytes(FileContentStr);

            await OnUpload.InvokeAsync();
        }

        public void ReadCSVHeaders(string input)
        {
            Regex csvSplit = new Regex("(?:^|,)(\"(?:[^\"]+|\"\")*\"|[^,]*)", RegexOptions.Compiled);

            foreach (Match match in csvSplit.Matches(input))
            {
                string csvField = match.Value.TrimStart(',');
                bool matchedField = false;
                for (int i = 0; i < MudFieldHeaders.Count; i++)
                {
                    //Todo Create an optional Parent Method for Comparison so someone could use a fuzzy name matcher: https://github.com/JakeBayer/FuzzySharp
                    //if (FuzzySharp.Fuzz.Ratio(MudFieldHeaders[i].Name.ToLower(), csvField.ToLower()) > 90)
                    if (String.Compare(MudFieldHeaders[i].Name, csvField, StringComparison.CurrentCultureIgnoreCase) == 0)
                    {                    
                        if (MudFieldHeaders[i].FieldCount == 0) //only match if it hasn't already been matched
                        { 
                            MudCSVHeaders.Add(new MudCSVHeader(csvField, MudFieldHeaders[i].Name));
                            MudFieldHeaders[i].FieldCount++;
                            matchedField = true;
                            break;
                        }
                    }
                }

                if (matchedField) continue;
                MudCSVHeaders.Add(new MudCSVHeader(csvField));
            }

            IsValid();
        }

        private void SetDragClass()
        {
            DragClass = $"{DefaultDragClass} mud-border-primary";
        }

        private void ClearDragClass()
        {
            DragClass = DefaultDragClass;
        }

        /* handling board events */
        private void TaskUpdated(MudItemDropInfo<MudCSVHeader> mudCSVField)
        {
            string oldMappedField = mudCSVField.Item.MappedField;
            mudCSVField.Item.MappedField = mudCSVField.DropzoneIdentifier;

            for (int i = 0; i < MudFieldHeaders.Count; i++)
            {
                if (MudFieldHeaders[i].Name == oldMappedField)
                {
                    MudFieldHeaders[i].FieldCount--;
                }
            }

            for (int i = 0; i < MudFieldHeaders.Count; i++)
            {
                if (MudFieldHeaders[i].Name == mudCSVField.DropzoneIdentifier)
                {
                    MudFieldHeaders[i].FieldCount++;
                }
            }
            IsValid();
        }

        private void IsValid()
        {
            foreach (MudFieldHeader mudFieldHeader in MudFieldHeaders.Where(i => i.Required))
            {
                if (!MudCSVHeaders.Where(i => i.MappedField == mudFieldHeader.Name).Any())
                {
                    _valid = false;
                    return;
                }
            }
            _valid = true;
        }
    }
}
