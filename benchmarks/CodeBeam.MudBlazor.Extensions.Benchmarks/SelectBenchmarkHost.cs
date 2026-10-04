using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace MudExtensions.Benchmarks;

internal sealed class SelectBenchmarkHost : ComponentBase
{
    [Parameter, EditorRequired]
    public ICollection<int?> Items { get; set; } = [];

    [Parameter, EditorRequired]
    public IEnumerable<int?> SelectedValues { get; set; } = [];

    [Parameter]
    public int SelectCount { get; set; } = 1;

    [Parameter]
    public bool Virtualize { get; set; } = true;

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        for (var index = 0; index < SelectCount; index++)
        {
            builder.OpenComponent<MudSelectExtended<int?>>(0);
            builder.SetKey(index);
            builder.AddAttribute(1, nameof(MudSelectExtended<int?>.ItemCollection), Items);
            builder.AddAttribute(2, nameof(MudSelectExtended<int?>.Virtualize), Virtualize);
            builder.AddAttribute(3, nameof(MudSelectExtended<int?>.MultiSelection), true);
            builder.AddAttribute(4, nameof(MudSelectExtended<int?>.SelectedValues), SelectedValues);
            builder.CloseComponent();
        }
    }
}
