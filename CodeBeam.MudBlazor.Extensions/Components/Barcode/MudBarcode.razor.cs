using Microsoft.AspNetCore.Components;
using MudBlazor;
using ZXing;
using ZXing.Common;

namespace MudExtensions
{
    public partial class MudBarcode : MudComponentBase
    {
        private static readonly Writer Encoder = new MultiFormatWriter();

        [Parameter]
        public BarcodeFormat BarcodeFormat { get; set; } = BarcodeFormat.QR_CODE;

        /// <summary>
        /// If true, it goes to specified href when click.
        /// </summary>
        [Parameter]
        public bool Clickable { get; set; }

        [Parameter]
        public string ErrorText { get; set; }

        [Parameter]
        public int Width { get; set; } = 200;

        /// <summary>
        /// Increase the stroke width if readers can not read the barcode easily.
        /// </summary>
        [Parameter]
        public double StrokeWidth { get; set; }

        [Parameter]
        public int Height { get; set; } = 200;

        [Parameter]
        public string Target { get; set; } = "_blank";

        [Parameter]
        public string Value { get; set; }

        [Parameter]
        public string Color { get; set; } = "black";

        [Parameter]
        public string BackgroundColor { get; set; } = "white";

        [Parameter]
        public EventCallback<string> ValueChanged { get; set; }

        protected BarcodeResult GetCode()
        {
            if (string.IsNullOrEmpty(Value))
            {
                return null;
            }

            try
            {
                var matrix = Encoder.encode(Value, BarcodeFormat, 0, 0);

                var result = new BarcodeResult(matrix, 1, 1);
                ErrorText = null;
                return result;
            }
            catch (Exception ex)
            {
                ErrorText = ex.Message;
                return null;
            }
        }

    }
}