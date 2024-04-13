using System.ComponentModel;

namespace MudExtensions
{
#pragma warning disable CS1591
    public enum SelectAllPosition
    {
        [Description("Upper line")]
        BeforeSearchBox,
        [Description("Start of the searchbox in the same line")]
        NextToSearchBox,
        [Description("Below line")]
        AfterSearchBox
    }
}
