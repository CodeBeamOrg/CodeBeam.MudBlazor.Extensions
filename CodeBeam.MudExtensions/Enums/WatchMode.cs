using System.ComponentModel;

namespace MudExtensions.Enums
{
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
