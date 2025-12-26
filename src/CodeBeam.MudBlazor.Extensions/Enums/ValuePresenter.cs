using System.ComponentModel;

namespace MudExtensions
{
#pragma warning disable CS1591
    public enum ValuePresenter
    {
        [Description("none")]
        None,
        [Description("text")]
        Text,
        [Description("chip")]
        Chip,
        [Description("itemcontent")]
        ItemContent,
    }
}
