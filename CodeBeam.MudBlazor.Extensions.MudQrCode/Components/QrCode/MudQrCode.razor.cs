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
        public int Height { get; set; } = 200;

        /// <summary>
        /// If true, no text show on barcode format.
        /// </summary>
        [Parameter]
        public bool PureBarcode { get; set; }

        [Parameter]
        public string Target { get; set; } = "_blank";

        [Parameter]
        public string Value { get; set; }

        [Parameter]
        public EventCallback<string> ValueChanged { get; set; }

        [Parameter]
        public int Width { get; set; } = 200;

        protected CodeResult GetCode()
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
                return new CodeResult(matrix, moduleSizeX, moduleSizeY);
            }
            catch (Exception ex)
            {
                ErrorText = ex.Message;
                return null;
            }
        }

        protected class CodeResult
        {
            private readonly BitMatrix bitMatrix;

            public CodeResult(BitMatrix bitMatrix, int moduleSizeX, int moduleSizeY)
            {
                this.bitMatrix = bitMatrix ?? throw new ArgumentNullException(nameof(bitMatrix));
                ModuleSizeX = moduleSizeX;
                ModuleSizeY = moduleSizeY;
            }

            public int Columns => bitMatrix.Width;
            public int ModuleSizeX { get; }
            public int ModuleSizeY { get; }
            public int Rows => bitMatrix.Height;

            public bool this[int x, int y] => bitMatrix[x, y];
        }
    }
}