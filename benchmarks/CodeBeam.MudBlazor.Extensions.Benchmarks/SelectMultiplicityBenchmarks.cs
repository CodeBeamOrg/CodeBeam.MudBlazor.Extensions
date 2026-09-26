using BenchmarkDotNet.Attributes;
using Bunit;

namespace MudExtensions.Benchmarks;

[MemoryDiagnoser]
[BenchmarkCategory("MudSelectExtended", "Multiplicity")]
public class SelectMultiplicityBenchmarks
{
    private List<int?> _items = null!;
    private int?[] _selectedValues = null!;

    [Params(1, 5, 20)]
    public int SelectCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        const int itemCount = 4_000;
        _items = Enumerable.Range(1, itemCount).Select(static value => (int?)value).ToList();
        _selectedValues = [17, 3_999];
    }

    [Benchmark]
    public async Task<int> RenderVirtualizedSelects()
    {
        await using var context = BenchmarkBunitContext.Create();
        var cut = context.Render<SelectBenchmarkHost>(parameters => parameters
            .Add(x => x.Items, _items)
            .Add(x => x.SelectedValues, _selectedValues)
            .Add(x => x.SelectCount, SelectCount)
            .Add(x => x.Virtualize, true));

        return cut.RenderCount;
    }
}
