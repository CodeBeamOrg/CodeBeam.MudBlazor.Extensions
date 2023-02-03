using System.ComponentModel;

namespace MudExtensions.Enums
{
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
