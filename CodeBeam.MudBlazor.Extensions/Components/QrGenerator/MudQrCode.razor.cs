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
        public int Width { get; set; } = 100;

        [Parameter]
        public int Height { get; set; } = 100;

        protected byte[] GetQrCode()
        {
            if (string.IsNullOrEmpty(Value))
            {
                return null;
            }

            BarcodeWriter writer = new BarcodeWriter
            {
                Format = BarcodeFormat.QR_CODE,
                Options = new QrCodeEncodingOptions
                {
                    Width = Width,
                    Height = Height,
                }
            };

            var qrCodeImage = writer.Write(Value);

            using (var stream = new MemoryStream())
            {
                qrCodeImage.Encode(stream, SKEncodedImageFormat.Png, 100);
                return stream.ToArray();
            }
        }
    }
}
