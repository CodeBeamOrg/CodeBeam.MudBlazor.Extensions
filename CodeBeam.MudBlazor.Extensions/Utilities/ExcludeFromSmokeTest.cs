
namespace MudExtensions.Utilities
{
    /// <summary>
    /// Indicates that a class should be excluded from automated test discovery and execution.
    /// </summary>
    /// <remarks>Apply this attribute to classes that should not be considered by test frameworks or test
    /// runners. This can be useful for utility, base, or helper classes that are not intended to be executed as
    /// tests.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class ExcludeFromSmokeTest : Attribute
    {

    }
}
