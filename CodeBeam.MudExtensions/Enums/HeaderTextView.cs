using System.ComponentModel;

namespace MudExtensions.Enums
{
    public enum HeaderTextView
    {
        [Description("none")]
        None,
        [Description("only-active-text")]
        OnlyActiveText,
        [Description("new-line")]
        NewLine,
        [Description("all")]
        All,
    }
}
