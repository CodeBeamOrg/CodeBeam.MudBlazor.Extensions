using MudBlazor.Utilities;

namespace MudExtensions
{
    /// <summary>
    /// Option class for SignaturePad.
    /// </summary>
    public class SignaturePadOptions
    {
#pragma warning disable CS1591
        public LineCapTypes LineCapStyle { get; set; } = LineCapTypes.Round;
        public LineJoinTypes LineJoinStyle { get; set; } = LineJoinTypes.Round;
        public MudColor StrokeStyle { get; set; } = new MudColor("#000000");
        public decimal LineWidth { get; set; } = 4;
    }
}

