using Microsoft.AspNetCore.Components;
using MudBlazor;
using ZXing;
using ZXing.Common;

namespace MudExtensions
{
    public partial class MudQrCode : MudComponentBase
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

        [Parameter]
        public int Height { get; set; } = 200;

        [Parameter]
        public string Target { get; set; } = "_blank";

        [Parameter]
        public string Value { get; set; }

        [Parameter]
        public EventCallback<string> ValueChanged { get; set; }

        protected QRCodeResult GetCode()
        {
            if (string.IsNullOrEmpty(Value))
            {
                return null;
            }

            try
            {
                var width = Width;
                var height = Height;

                if (BarcodeFormat == BarcodeFormat.All_1D)
                {
                }
                else
                {
                    if (width > height)
                    {
                        width = height;
                    }
                    else
                    {
                        height = width;
                    }
                }

                var matrix = Encoder.encode(Value, BarcodeFormat, 0, 0);

                var moduleSizeX = width / matrix.Width;
                var moduleSizeY = height / matrix.Height;
                return new QRCodeResult(matrix, moduleSizeX, moduleSizeY);
            }
            catch (Exception ex)
            {
                ErrorText = ex.Message;
                return null;
            }
        }

    }
}