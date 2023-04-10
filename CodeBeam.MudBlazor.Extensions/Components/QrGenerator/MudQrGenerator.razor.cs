using Microsoft.AspNetCore.Components;
using MudBlazor;
using SkiaSharp;
using ZXing;
using ZXing.QrCode;
using ZXing.SkiaSharp;

namespace MudExtensions
{
    public partial class MudQrGenerator : MudComponentBase
    {
        string _content = "";

        public byte[] Value { get; set; }

        public void Refresh(string value)
        {
            _content = value;
            Value = CreateQrCode(value);
            StateHasChanged();
        }

        public byte[] CreateQrCode(string content)
        {
            BarcodeWriter writer = new BarcodeWriter
            {
                Format = BarcodeFormat.QR_CODE,
                Options = new QrCodeEncodingOptions
                {
                    Width = 100,
                    Height = 100,
                }
            };

            var qrCodeImage = writer.Write(content);

            using (var stream = new MemoryStream())
            {
                qrCodeImage.Encode(stream, SKEncodedImageFormat.Png, 100);
                return stream.ToArray();
            }
        }
    }
}
