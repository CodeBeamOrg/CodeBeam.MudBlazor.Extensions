using Microsoft.AspNetCore.Components;
using MudBlazor;
using ZXing;

namespace MudExtensions
{
    /// <summary>
    /// 
    /// </summary>
    public partial class MudBarcode : MudComponentBase
    {
        private static readonly Writer Encoder = new MultiFormatWriter();

        /// <summary>
        /// Determines which barcode format will shown. Default is QR Code.
        /// </summary>
        [Parameter]
        public BarcodeFormat BarcodeFormat { get; set; } = BarcodeFormat.QR_CODE;

        /// <summary>
        /// If true, it goes to specified href when click.
        /// </summary>
        [Parameter]
        public bool Clickable { get; set; }

        /// <summary>
        /// The error content.
        /// </summary>
        [Parameter]
        public string? ErrorText { get; set; }

        /// <summary>
        /// The width value as integer.
        /// </summary>
        [Parameter]
        public int Width { get; set; } = 200;

        /// <summary>
        /// The height value as integer.
        /// </summary>
        [Parameter]
        public int Height { get; set; } = 200;

        /// <summary>
        /// Use this value on not square sized barcode formats like UPC_A and UPC_E.
        /// </summary>
        [Parameter]
        public int ForceHeight { get; set; } = 1;

        /// <summary>
        /// Increase the stroke width if readers can not read the barcode easily.
        /// </summary>
        [Parameter]
        public double StrokeWidth { get; set; }

        /// <summary>
        /// Determines how user go to href content. Default is open in a new tab.
        /// </summary>
        [Parameter]
        public string? Target { get; set; } = "_blank";

        /// <summary>
        /// The value of the barcode format.
        /// </summary>
        [Parameter]
        public string? Value { get; set; }

        /// <summary>
        /// The color of the barcode as string. Accepts all kinds of CSS property values. (ex. #123456) Default is "black".
        /// </summary>
        [Parameter]
        public string? Color { get; set; } = "black";

        /// <summary>
        /// The background color of the barcode as string. Accepts all kinds of CSS property values. (ex. #123456) Default is "white".
        /// </summary>
        [Parameter]
        public string? BackgroundColor { get; set; } = "white";

        /// <summary>
        /// Fires when value changed.
        /// </summary>
        [Parameter]
        public EventCallback<string> ValueChanged { get; set; }

        /// <summary>
        /// Barcode process that returns BarcodeResult. Returns null if value is also null or empty.
        /// </summary>
        /// <returns></returns>
        protected BarcodeResult? GetCode()
        {
            if (string.IsNullOrEmpty(Value))
            {
                return null;
            }

            try
            {
                var matrix = Encoder.encode(Value, BarcodeFormat, 0, 0);

                var result = new BarcodeResult(matrix, 1, ForceHeight);
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