using BenchmarkDotNet.Attributes;
using Bunit;

namespace MudExtensions.Benchmarks;

[MemoryDiagnoser]
[BenchmarkCategory("MudSelectExtended", "SelectionDensity")]
public class SelectSelectionDensityBenchmarks
{
    private const int ItemCount = 4_000;
    private List<int?> _items = null!;
    private int?[] _selectedValues = null!;

    [Params(1, 2, 10, 30, 100)]
    public int SelectedCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _items = Enumerable.Range(1, ItemCount).Select(static value => (int?)value).ToList();
        _selectedValues = Enumerable.Range(1, SelectedCount)
            .Select(index => (int?)(1 + ((index - 1) * (ItemCount - 1) / Math.Max(1, SelectedCount - 1))))
            .Distinct()
            .ToArray();
    }

    [Benchmark]
    public async Task<int> RenderVirtualizedSelect()
    {
        await using var context = BenchmarkBunitContext.Create();
        var cut = context.Render<SelectBenchmarkHost>(parameters => parameters
            .Add(x => x.Items, _items)
            .Add(x => x.SelectedValues, _selectedValues)
            .Add(x => x.Virtualize, true));

        return cut.RenderCount;
    }
}
