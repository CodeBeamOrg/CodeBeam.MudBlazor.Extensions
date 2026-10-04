using BenchmarkDotNet.Attributes;
using Bunit;

namespace MudExtensions.Benchmarks;

[MemoryDiagnoser]
[BenchmarkCategory("MudSelectExtended", "InitialRender")]
public class SelectInitialRenderBenchmarks
{
    private List<int?> _items = null!;
    private int?[] _selectedValues = null!;
    private string _expectedText = null!;

    [Params(10, 100, 1_000, 4_000)]
    public int ItemCount { get; set; }

    [Params(false, true)]
    public bool Virtualize { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _items = Enumerable.Range(1, ItemCount).Select(static value => (int?)value).ToList();
        _selectedValues = [1, ItemCount];
        _expectedText = $"1, {ItemCount}";
    }

    [Benchmark]
    public async Task<int> RenderSelect()
    {
        await using var context = BenchmarkBunitContext.Create();
        var cut = context.Render<SelectBenchmarkHost>(parameters => parameters
            .Add(x => x.Items, _items)
            .Add(x => x.SelectedValues, _selectedValues)
            .Add(x => x.Virtualize, Virtualize));

        cut.WaitForAssertion(() =>
        {
            var value = cut.Find("input").GetAttribute("value");
            if (!string.Equals(value, _expectedText, StringComparison.Ordinal))
            {
                throw new InvalidOperationException($"Expected settled select text '{_expectedText}', but found '{value}'.");
            }
        });

        return cut.FindComponents<MudSelectItemExtended<int?>>().Count;
    }
}
