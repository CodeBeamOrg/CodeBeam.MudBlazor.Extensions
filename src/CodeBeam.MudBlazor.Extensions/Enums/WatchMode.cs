using System.ComponentModel;

namespace MudExtensions
{
#pragma warning disable CS1591
    public enum WatchMode
    {
        [Description("watch")]
        Watch,
        [Description("stopwatch")]
        StopWatch,
        [Description("countdown")]
        CountDown,
    }
}
