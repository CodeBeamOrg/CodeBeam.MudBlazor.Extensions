
namespace MudExtensions.Utilities
{
    /// <summary>
    /// Holds value for MudWatch.
    /// </summary>
    public class LapRecord
    {
        /// <summary>
        /// 
        /// </summary>
        public TimeSpan TotalTime { get; set; }

        /// <summary>
        /// Difference between previous timespan.
        /// </summary>
        public TimeSpan Gap { get; set; }
    }
}
