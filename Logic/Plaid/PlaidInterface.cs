using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageCreatePlaid
{
    interface PlaidInterface
    {
        SKBitmap EditImage(SKBitmap bmp, PlaidModel model);
    }
}
