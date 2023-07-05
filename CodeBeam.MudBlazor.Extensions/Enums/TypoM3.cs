using System.ComponentModel;

namespace MudExtensions.Enums
{
    public enum TypeStyle
    {
        [Description("Display")]
        Display,
        [Description("Headline")]
        Headline,
        [Description("Title")]
        Title,
        [Description("Body")]
        Body,
        [Description("Label")]
        Label
    }

    public enum TypeSize
    {
        [Description("Large")]
        Large,
        [Description("Medium")]
        Medium,
        [Description("Small")]
        Small
    }
}
