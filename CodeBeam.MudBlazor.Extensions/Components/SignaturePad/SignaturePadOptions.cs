using MudBlazor.Utilities;

namespace MudExtensions;

public class SignaturePadOptions
{
    public LineCapTypes LineCapStyle { get; set; } = LineCapTypes.Round;
    public LineJoinTypes LineJoinStyle { get; set; } = LineJoinTypes.Round;
    public MudColor StrokeStyle { get; set; } = new MudColor("#000000");
    public decimal LineWidth { get; set; } = 4;
}