using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZXing.Common;

namespace MudExtensions
{
    public class BarcodeResult
    {
        private readonly BitMatrix bitMatrix;

        public BarcodeResult(BitMatrix bitMatrix, int moduleSizeX, int moduleSizeY)
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
