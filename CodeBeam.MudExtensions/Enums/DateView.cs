using System.ComponentModel;

namespace MudExtensions
{
    public enum DateView
    {
        [Description("date")]
        Date,
        [Description("time")]
        Time,
        [Description("both")]
        Both,
    }
}
