using Microsoft.AspNetCore.Components;
using MudBlazor;
using SkiaSharp;
using ZXing;
using ZXing.QrCode;
using ZXing.SkiaSharp;

namespace MudExtensions
{
    public partial class MudQrCode : MudComponentBase
    {
        [Parameter]
        public string Value { get; set; }

        [Parameter]
        public EventCallback<string> ValueChanged { get; set; }

        [Parameter]
        public BarcodeFormat BarcodeFormat { get; set; } = BarcodeFormat.QR_CODE;

        [Parameter]
        public int Width { get; set; } = 200;

        [Parameter]
        public int Height { get; set; } = 200;

        /// <summary>
        /// If true, it goes to specified href when click.
        /// </summary>
        [Parameter]
        public bool Clickable { get; set; }

        [Parameter]
        public string Target { get; set; } = "_blank";

        [Parameter]
        public string ErrorText { get; set; }

        /// <summary>
        /// If true, no text show on barcode format.
        /// </summary>
        [Parameter]
        public bool PureBarcode { get; set; }

        protected byte[] GetQrCode()
        {
            if (string.IsNullOrEmpty(Value))
            {
                return null;
            }

            try
            {
                BarcodeWriter writer = new BarcodeWriter
                {
                    Format = BarcodeFormat,
                    Options = new QrCodeEncodingOptions
                    {
                        Width = Width,
                        Height = Height,
                        PureBarcode = PureBarcode,
                    }
                };

                var qrCodeImage = writer.Write(Value);

                using (var stream = new MemoryStream())
                {
                    qrCodeImage.Encode(stream, SKEncodedImageFormat.Png, 100);
                    ErrorText = null;
                    return stream.ToArray();
                }
            }
            catch (Exception ex)
            {
                ErrorText = ex.Message;
                return null;
            }
        }

    }
}
