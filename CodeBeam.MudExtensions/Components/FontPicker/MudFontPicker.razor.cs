using MudExtensions.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using MudBlazor.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MudExtensions
{
    public partial class MudFontPicker : MudAutocomplete<string>
    {
        [Parameter]
        public string Font { get; set; }

        [Parameter]
        public List<string> FontCollection { get; set; } = GoogleFonts.FontListLatin;

        /// <summary>
        /// The css import url for adding custom fonts. Default is google font url. Ex. 'https://font.something.com'
        /// </summary>
        [Parameter]
        public string RootImportUrl { get; set; }

        /// <summary>
        /// If true, the input's label and text always use the theme font instead of font picker's value.
        /// </summary>
        [Parameter]
        public bool StaticInputText { get; set; }

        [Parameter]
        public EventCallback<string> FontChanged { get; set; }

        public string ImportFontStyleText(string font)
        {
            if (!string.IsNullOrEmpty(RootImportUrl))
            {
                return $"@import url('{RootImportUrl}{font.Replace(" ", "+")}')";
            }
            else
            {
                return $"@import url('https://fonts.googleapis.com/css2?family={font.Replace(" ", "+")}')";
            }
        }

        public async Task ClearFont()
        {
            Font = null;
            await SetFontValue(null);
        }

        protected async Task SetFontValue(string font)
        {
            if (font == Font)
            {
                return;
            }

            Font = font;
            await FontChanged.InvokeAsync(font);
            StateHasChanged();
        }

        private async Task<IEnumerable<String>> SearchFonts(string searchText)
        {
            if (FontCollection == null)
            {
                return null;
            }
            await Task.Delay(1);
            if (String.IsNullOrEmpty(searchText)) return FontCollection;
            return FontCollection.Where(c => c.ToLowerInvariant().Trim().StartsWith(searchText.ToLower().Trim())).ToList();
        }
    }
}
