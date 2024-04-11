using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZXing.Common;

namespace MudExtensions
{
    /// <summary>
    /// The result that used in MudBarcode component to show one of the barcode formats.
    /// </summary>
    public class BarcodeResult
    {
        private readonly BitMatrix bitMatrix;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bitMatrix"></param>
        /// <param name="moduleSizeX"></param>
        /// <param name="moduleSizeY"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public BarcodeResult(BitMatrix bitMatrix, int moduleSizeX, int moduleSizeY)
        {
            this.bitMatrix = bitMatrix ?? throw new ArgumentNullException(nameof(bitMatrix));
            ModuleSizeX = moduleSizeX;
            ModuleSizeY = moduleSizeY;
        }

        /// <summary>
        /// 
        /// </summary>
        public int Columns => bitMatrix.Width;

        /// <summary>
        /// 
        /// </summary>
        public int ModuleSizeX { get; }

        /// <summary>
        /// 
        /// </summary>
        public int ModuleSizeY { get; }

        /// <summary>
        /// 
        /// </summary>
        public int Rows => bitMatrix.Height;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public bool this[int x, int y] => bitMatrix[x, y];
    }
}
