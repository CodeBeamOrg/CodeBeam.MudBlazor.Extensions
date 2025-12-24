using NUglify;
using System.Text;

Console.WriteLine("Minifying MudExtensions.js...");

// Bulunduğumuz dizinden yukarı doğru çık
var current = Directory.GetCurrentDirectory();
string? solutionRoot = null;

while (current != null)
{
    if (Directory.Exists(Path.Combine(current, "CodeBeam.MudBlazor.Extensions")))
    {
        solutionRoot = current;
        break;
    }

    current = Directory.GetParent(current)?.FullName;
}

if (solutionRoot == null)
{
    Console.Error.WriteLine("Solution root not found.");
    Environment.Exit(1);
}

var projectRoot = Path.Combine(solutionRoot, "CodeBeam.MudBlazor.Extensions");
var input = Path.Combine(projectRoot, "TScripts", "MudExtensions.js");
var output = Path.Combine(projectRoot, "wwwroot", "MudExtensions.min.js");

Console.WriteLine($"IN : {input}");
Console.WriteLine($"OUT: {output}");

if (!File.Exists(input))
{
    Console.Error.WriteLine($"Input file not found: {input}");
    Environment.Exit(1);
}

var js = File.ReadAllText(input, Encoding.UTF8);
var result = Uglify.Js(js);

if (result.HasErrors)
{
    foreach (var error in result.Errors)
        Console.Error.WriteLine(error.ToString());

    Environment.Exit(1);
}

Directory.CreateDirectory(Path.GetDirectoryName(output)!);
File.WriteAllText(output, result.Code!, Encoding.UTF8);

Console.WriteLine("✔ MudExtensions.min.js generated successfully");
