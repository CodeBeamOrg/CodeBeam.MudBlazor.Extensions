using System.ComponentModel;

namespace MudExtensions
{
#pragma warning disable CS1591
    public enum StepStatus
    {
        [Description("not-started")]
        NotStarted,
        [Description("continued")]
        Continued,
        [Description("completed")]
        Completed,
        [Description("skipped")]
        Skipped,
        [Description("error")]
        Error,
    }
}
