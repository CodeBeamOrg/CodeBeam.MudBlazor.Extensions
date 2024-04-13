using System.ComponentModel;

namespace MudExtensions
{
#pragma warning disable CS1591
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
