using System.Diagnostics;
using System.Globalization;
using Bunit;

namespace MudExtensions.Benchmarks;

internal static class ScaleProbe
{
    public static async Task<int> RunAsync(string[] args)
    {
        var outputPath = GetOption(args, "--output");
        var variant = Environment.GetEnvironmentVariable("BENCHMARK_VARIANT") ?? "current";

        await WarmUpAsync();

        var results = new List<ProbeResult>();
        foreach (var scenario in Scenarios())
        {
            results.Add(await MeasureAsync(variant, scenario));
        }

        WriteTable(results);

        if (!string.IsNullOrWhiteSpace(outputPath))
        {
            var fullPath = Path.GetFullPath(outputPath);
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);
            await File.WriteAllTextAsync(fullPath, ToCsv(results));
            Console.WriteLine($"\nWrote {fullPath}");
        }

        return 0;
    }

    private static async Task<ProbeResult> MeasureAsync(string variant, ProbeScenario scenario)
    {
        var items = Enumerable.Range(1, scenario.ItemCount).Select(static value => (int?)value).ToList();
        var selectedValues = CreateSelectedValues(scenario.ItemCount, scenario.SelectedCount);

        await using var context = BenchmarkBunitContext.Create();

        GC.Collect(2, GCCollectionMode.Forced, blocking: true, compacting: true);
        GC.WaitForPendingFinalizers();
        var retainedBefore = GC.GetTotalMemory(forceFullCollection: true);
        var allocatedBefore = GC.GetTotalAllocatedBytes(precise: true);

        var stopwatch = Stopwatch.StartNew();
        var cut = context.Render<SelectBenchmarkHost>(parameters => parameters
            .Add(x => x.Items, items)
            .Add(x => x.SelectedValues, selectedValues)
            .Add(x => x.SelectCount, scenario.SelectCount)
            .Add(x => x.Virtualize, scenario.Virtualize));
        stopwatch.Stop();

        var allocatedAfter = GC.GetTotalAllocatedBytes(precise: true);
        var retainedAfter = GC.GetTotalMemory(forceFullCollection: true);

        var shadowItemCount = cut.FindAll("div[style='display: none'] div.mud-list-item-extended").Count;
        var selectItemComponentCount = cut.FindComponents<MudSelectItemExtended<int?>>().Count;
        var markupLength = cut.Markup.Length;

        return new(
            variant,
            scenario.ItemCount,
            scenario.SelectCount,
            selectedValues.Length,
            scenario.Virtualize,
            stopwatch.Elapsed.TotalMilliseconds,
            allocatedAfter - allocatedBefore,
            retainedAfter - retainedBefore,
            cut.RenderCount,
            shadowItemCount,
            selectItemComponentCount,
            markupLength);
    }

    private static int?[] CreateSelectedValues(int itemCount, int selectedCount)
    {
        if (selectedCount <= 0)
        {
            return [];
        }

        if (selectedCount == 1)
        {
            return [itemCount];
        }

        return Enumerable.Range(0, Math.Min(itemCount, selectedCount))
            .Select(index => (int?)(1 + (index * (itemCount - 1) / (Math.Min(itemCount, selectedCount) - 1))))
            .Distinct()
            .ToArray();
    }

    private static IEnumerable<ProbeScenario> Scenarios()
    {
        foreach (var itemCount in new[] { 10, 100, 1_000, 4_000 })
        {
            yield return new(itemCount, 1, 2, true);
        }

        yield return new(4_000, 5, 2, true);
        yield return new(4_000, 20, 2, true);

        foreach (var itemCount in new[] { 10, 100, 1_000, 4_000 })
        {
            yield return new(itemCount, 1, 2, false);
        }

        foreach (var selectedCount in new[] { 1, 10, 30, 100 })
        {
            yield return new(4_000, 1, selectedCount, true);
        }
    }

    private static async Task WarmUpAsync()
    {
        var items = Enumerable.Range(1, 10).Select(static value => (int?)value).ToList();
        await using var context = BenchmarkBunitContext.Create();
        context.Render<SelectBenchmarkHost>(parameters => parameters
            .Add(x => x.Items, items)
            .Add(x => x.SelectedValues, new int?[] { 1, 10 })
            .Add(x => x.Virtualize, true));
    }

    private static void WriteTable(IEnumerable<ProbeResult> results)
    {
        Console.WriteLine("Variant   Items Selects Selected Virt  Time ms   Allocated    Retained*  Renders ShadowItems Components Markup");
        foreach (var result in results)
        {
            Console.WriteLine(
                $"{result.Variant,-9} {result.ItemCount,5} {result.SelectCount,7} {result.SelectedCount,8} {result.Virtualize,4} " +
                $"{result.ElapsedMilliseconds,8:F2} {FormatBytes(result.AllocatedBytes),11} {FormatBytes(result.ApproxRetainedBytes),11} " +
                $"{result.RenderCount,8} {result.ShadowItemCount,11} {result.SelectItemComponentCount,10} {result.MarkupLength,6}");
        }

        Console.WriteLine("\n* Retained is an approximate full-GC delta while the rendered component remains alive; use BenchmarkDotNet allocation/GC results for statistical comparisons.");
    }

    private static string ToCsv(IEnumerable<ProbeResult> results)
    {
        var lines = new List<string>
        {
            "Variant,ItemCount,SelectCount,SelectedCount,Virtualize,ElapsedMilliseconds,AllocatedBytes,ApproxRetainedBytes,RenderCount,ShadowItemCount,SelectItemComponentCount,MarkupLength"
        };

        lines.AddRange(results.Select(result => string.Join(',',
            Csv(result.Variant),
            result.ItemCount.ToString(CultureInfo.InvariantCulture),
            result.SelectCount.ToString(CultureInfo.InvariantCulture),
            result.SelectedCount.ToString(CultureInfo.InvariantCulture),
            result.Virtualize.ToString(CultureInfo.InvariantCulture),
            result.ElapsedMilliseconds.ToString("F4", CultureInfo.InvariantCulture),
            result.AllocatedBytes.ToString(CultureInfo.InvariantCulture),
            result.ApproxRetainedBytes.ToString(CultureInfo.InvariantCulture),
            result.RenderCount.ToString(CultureInfo.InvariantCulture),
            result.ShadowItemCount.ToString(CultureInfo.InvariantCulture),
            result.SelectItemComponentCount.ToString(CultureInfo.InvariantCulture),
            result.MarkupLength.ToString(CultureInfo.InvariantCulture))));

        return string.Join(Environment.NewLine, lines) + Environment.NewLine;
    }

    private static string Csv(string value) => $"\"{value.Replace("\"", "\"\"")}\"";

    private static string FormatBytes(long bytes)
    {
        var sign = bytes < 0 ? "-" : string.Empty;
        var absolute = Math.Abs((double)bytes);
        return absolute switch
        {
            >= 1024 * 1024 => $"{sign}{absolute / (1024 * 1024):F1} MB",
            >= 1024 => $"{sign}{absolute / 1024:F1} KB",
            _ => $"{bytes} B"
        };
    }

    private static string? GetOption(string[] args, string name)
    {
        for (var index = 0; index < args.Length - 1; index++)
        {
            if (string.Equals(args[index], name, StringComparison.OrdinalIgnoreCase))
            {
                return args[index + 1];
            }
        }

        return null;
    }

    private sealed record ProbeScenario(int ItemCount, int SelectCount, int SelectedCount, bool Virtualize);

    private sealed record ProbeResult(
        string Variant,
        int ItemCount,
        int SelectCount,
        int SelectedCount,
        bool Virtualize,
        double ElapsedMilliseconds,
        long AllocatedBytes,
        long ApproxRetainedBytes,
        int RenderCount,
        int ShadowItemCount,
        int SelectItemComponentCount,
        int MarkupLength);
}
