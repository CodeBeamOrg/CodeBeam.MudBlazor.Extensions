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
    public partial class MudFontPicker : MudComponentBase
    {
        [Parameter]
        public string Font { get; set; } = null;

        public bool IsValid
        {
            get
            {
                return !string.IsNullOrEmpty(Font);
            }
        }


        [Parameter]
        public bool Required { get; set; } = false;

        [Parameter]
        public string Label { get; set; } = "Font";

        [Parameter]
        public Variant Variant { get; set; }

        [Parameter]
        public Margin Margin { get; set; }

        [Parameter]
        public bool Dense { get; set; }

        [Parameter]
        public string PlaceHolder { get; set; }

        [Parameter]
        public EventCallback<String> FontChanged { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await OnFontValueChanged(Font);
            await base.OnInitializedAsync();
        }

        private async Task ClearFontValue()
        {
            Font = "";
            await OnFontValueChanged(null);
        }

        private async Task OnFontValueChanged(string font)
        {
            if (font == null)
            {
                await FontChanged.InvokeAsync(null);
                return;
            }

            Font = font;
            await FontChanged.InvokeAsync(font);
            StateHasChanged();
        }


        private async Task<IEnumerable<String>> SearchFonts(string searchText)
        {
            await Task.Delay(1);
            if (String.IsNullOrEmpty(searchText)) return FontList;
            return FontList.Where(c => c.ToLower().Trim().Contains(searchText.ToLower().Trim())).ToList();
        }

        List<string> FontList = new List<string>{
        "Akshar",
        "Albert Sans",
        "Alegreya",
        "Alumni Sans",
        "Andada Pro",
        "Anek Bangla",
        "Anek Devanagari",
        "Anek Gujarati",
        "Anek Gurmukhi",
        "Anek Kannada",
        "Anek Latin",
        "Anek Malayalam",
        "Anek Odia",
        "Anek Tamil",
        "Anek Telugu",
        "Antonio",
        "Anybody",
        "Archivo",
        "Archivo Narrow",
        "Arima",
        "Arimo",
        "Asap",
        "Assistant",
        "Azeret Mono",
        "Ballet",
        "Baloo 2",
        "Baloo Bhai 2",
        "Baloo Bhaijaan 2",
        "Baloo Bhaina 2",
        "Baloo Chettan 2",
        "Baloo Da 2",
        "Baloo Paaji 2",
        "Baloo Tamma 2",
        "Baloo Tammudu 2",
        "Baloo Thambi 2",
        "Besley",
        "Big Shoulders Display",
        "Big Shoulders Inline Display",
        "Big Shoulders Inline Text",
        "Big Shoulders Stencil Display",
        "Big Shoulders Stencil Text",
        "Big Shoulders Text",
        "Bitter",
        "Bodoni Moda",
        "Brygada 1918",
        "Cabin",
        "Cairo",
        "Cairo Play",
        "Catamaran",
        "Caveat",
        "Changa",
        "Cinzel",
        "Comfortaa",
        "Commissioner",
        "Cormorant",
        "Crimson Pro",
        "Cuprum",
        "Dancing Script",
        "Domine",
        "Dosis",
        "DynaPuff",
        "EB Garamond",
        "Eczar",
        "Edu NSW ACT Foundation",
        "Edu QLD Beginner",
        "Edu SA Beginner",
        "Edu TAS Beginner",
        "Edu VIC WA NT Beginner",
        "El Messiri",
        "Encode Sans",
        "Encode Sans SC",
        "Epilogue",
        "Exo",
        "Exo 2",
        "Expletus Sans",
        "Familjen Grotesk",
        "Faustina",
        "Figtree",
        "Finlandica",
        "Fira Code",
        "Fraunces",
        "Fredoka",
        "Gantari",
        "Gemunu Libre",
        "Genos",
        "Georama",
        "Glory",
        "Gluten",
        "Grandstander",
        "Grenze Gotisch",
        "Hahmlet",
        "Heebo",
        "Hepta Slab",
        "Ibarra Real Nova",
        "Imbue",
        "Inconsolata",
        "Inter",
        "Inter Tight",
        "JetBrains Mono",
        "Josefin Sans",
        "Josefin Slab",
        "Jost",
        "Jura",
        "Kantumruy Pro",
        "Karla",
        "Kreon",
        "Kufam",
        "Kumbh Sans",
        "League Gothic",
        "League Spartan",
        "Lemonada",
        "Lexend",
        "Lexend Deca",
        "Lexend Exa",
        "Lexend Giga",
        "Lexend Mega",
        "Lexend Peta",
        "Lexend Tera",
        "Lexend Zetta",
        "Libre Bodoni",
        "Libre Franklin",
        "Literata",
        "Lora",
        "M PLUS 1",
        "M PLUS 1 Code",
        "M PLUS 2",
        "M PLUS Code Latin",
        "Manrope",
        "Manuale",
        "Markazi Text",
        "Maven Pro",
        "Merriweather Sans",
        "Mohave",
        "Montagu Slab",
        "Montserrat",
        "Mulish",
        "Murecho",
        "MuseoModerno",
        "Nabla",
        "Newsreader",
        "Noto Emoji",
        "Noto Kufi Arabic",
        "Noto Naskh Arabic",
        "Noto Nastaliq Urdu",
        "Noto Rashi Hebrew",
        "Noto Sans Adlam",
        "Noto Sans Adlam Unjoined",
        "Noto Sans Arabic",
        "Noto Sans Armenian",
        "Noto Sans Balinese",
        "Noto Sans Bamum",
        "Noto Sans Bengali",
        "Noto Sans Canadian Aboriginal",
        "Noto Sans Cham",
        "Noto Sans Cherokee",
        "Noto Sans Devanagari",
        "Noto Sans Display",
        "Noto Sans Ethiopic",
        "Noto Sans Georgian",
        "Noto Sans Gujarati",
        "Noto Sans Gurmukhi",
        "Noto Sans Hanifi Rohingya",
        "Noto Sans Hebrew",
        "Noto Sans Javanese",
        "Noto Sans Kannada",
        "Noto Sans Kayah Li",
        "Noto Sans Khmer",
        "Noto Sans Lao",
        "Noto Sans Lisu",
        "Noto Sans Malayalam",
        "Noto Sans Medefaidrin",
        "Noto Sans Meetei Mayek",
        "Noto Sans Mono",
        "Noto Sans Ol Chiki",
        "Noto Sans Oriya",
        "Noto Sans Sinhala",
        "Noto Sans Sora Sompeng",
        "Noto Sans Sundanese",
        "Noto Sans Symbols",
        "Noto Sans Tai Tham",
        "Noto Sans Tamil",
        "Noto Sans Telugu",
        "Noto Sans Thaana",
        "Noto Sans Thai",
        "Noto Serif Armenian",
        "Noto Serif Bengali",
        "Noto Serif Devanagari",
        "Noto Serif Display",
        "Noto Serif Ethiopic",
        "Noto Serif Georgian",
        "Noto Serif Gujarati",
        "Noto Serif Gurmukhi",
        "Noto Serif HK",
        "Noto Serif Hebrew",
        "Noto Serif Kannada",
        "Noto Serif Khmer",
        "Noto Serif Lao",
        "Noto Serif Malayalam",
        "Noto Serif Nyiakeng Puachue Hmong",
        "Noto Serif Sinhala",
        "Noto Serif Tamil",
        "Noto Serif Telugu",
        "Noto Serif Thai",
        "Noto Serif Tibetan",
        "Noto Serif Yezidi",
        "Nunito",
        "Open Sans",
        "Orbitron",
        "Oswald",
        "Outfit",
        "Overpass",
        "Overpass Mono",
        "Oxanium",
        "Petrona",
        "Piazzolla",
        "Playfair Display",
        "Plus Jakarta Sans",
        "Podkova",
        "Public Sans",
        "Quicksand",
        "Radio Canada",
        "Raleway",
        "Rasa",
        "Readex Pro",
        "Recursive",
        "Red Hat Display",
        "Red Hat Mono",
        "Red Hat Text",
        "Red Rose",
        "Reem Kufi",
        "Reem Kufi Fun",
        "Roboto Flex",
        "Roboto Mono",
        "Roboto Serif",
        "Roboto Slab",
        "Rokkitt",
        "Rosario",
        "Rubik",
        "Ruda",
        "STIX Two Text",
        "Saira",
        "Sansita Swashed",
        "Signika",
        "Signika Negative",
        "Smooch Sans",
        "Sora",
        "Source Code Pro",
        "Source Sans 3",
        "Source Serif 4",
        "Space Grotesk",
        "Spline Sans",
        "Spline Sans Mono",
        "Stick No Bills",
        "Syne",
        "Texturina",
        "Tourney",
        "Trispace",
        "Truculenta",
        "Urbanist",
        "Varta",
        "Vazirmatn",
        "Vollkorn",
        "Work Sans",
        "Yaldevi",
        "Yanone Kaffeesatz",
        "Yrsa"
    };
    }
}
