using BenchmarkDotNet.Attributes;
using Bunit;

namespace MudExtensions.Benchmarks;

[MemoryDiagnoser]
[BenchmarkCategory("MudListExtended", "InitialRender")]
public class ListExtendedInitialRenderBenchmarks
{
    private List<int?> _items = null!;
    private int?[] _selectedValues = null!;

    [Params(10, 100, 1_000, 4_000)]
    public int ItemCount { get; set; }

    [Params(false, true)]
    public bool Virtualize { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _items = Enumerable.Range(1, ItemCount).Select(static value => (int?)value).ToList();
        _selectedValues = [1, ItemCount];
    }

    [Benchmark]
    public async Task<int> RenderList()
    {
        await using var context = BenchmarkBunitContext.Create();
        var cut = context.Render<MudListExtended<int?>>(parameters => parameters
            .Add(x => x.ItemCollection, _items)
            .Add(x => x.Virtualize, Virtualize)
            .Add(x => x.MultiSelection, true)
            .Add(x => x.SelectedValues, _selectedValues));

        return cut.RenderCount;
    }
}
