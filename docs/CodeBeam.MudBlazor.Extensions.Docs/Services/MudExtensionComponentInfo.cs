
using System.ComponentModel;

namespace MudExtensions.Docs.Services
{
    public class MudExtensionComponentInfo
    {
        public Type? Component { get; set; }
        public List<Type>? RelatedComponents { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public ComponentUsage Usage { get; set; }
        public bool IsUnique { get; set; }
        public bool IsMaterial3 { get; set; }
        public bool IsUtility { get; set; }
    }

    public enum ComponentUsage
    {
        [Description("Layout")]
        Layout,
        [Description("Button")]
        Button,
        [Description("Input")]
        Input,
        [Description("Utility")]
        Utility,
        [Description("Display")]
        Display,
    }
}
