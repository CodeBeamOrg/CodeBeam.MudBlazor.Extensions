using System.ComponentModel;

namespace MudExtensions.Enums
{
    public enum StepStatus
    {
        [Description("continued")]
        Continued,
        [Description("completed")]
        Completed,
        [Description("skipped")]
        Skipped,
    }
}
