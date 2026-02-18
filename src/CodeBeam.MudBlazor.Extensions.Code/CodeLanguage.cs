using System.ComponentModel;

namespace MudExtensions;

#pragma warning disable CS1591
public enum CodeLanguage
{
    [Description("csharp")]
    CSharp,
    [Description("razor")]
    Razor,
    [Description("javascript")]
    JavaScript,
    [Description("json")]
    Json,
    [Description("css")]
    Css,
    [Description("sql")]
    Sql,
    [Description("yaml")]
    Yaml,
    [Description("powershell")]
    PowerShell,
    [Description("markup")]
    Markup
}
