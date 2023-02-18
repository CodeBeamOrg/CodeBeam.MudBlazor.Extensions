using System.ComponentModel;

namespace MudExtensions.Enums
{
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
