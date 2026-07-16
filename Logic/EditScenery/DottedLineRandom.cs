using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageCreatePlaid
{
    public class DottedLineRandom : SceneryInterface
    {
        public SKBitmap EditImage(SKBitmap bmp, SceneryModel model)
        {
            int width = bmp.Width;
            int height = bmp.Height;
            int px = BussinessLogic.GetRandomInt(model.HorizontalSize1);
            int py = BussinessLogic.GetRandomInt(model.VerticalSize1);
            int px2 = BussinessLogic.GetRandomInt(model.HorizontalSize1);
            int py2 = BussinessLogic.GetRandomInt(model.VerticalSize1);
            SKColor basecolor = new SKColor(model.BaseColorRed, model.BaseColorGreen, model.BaseColorBlue, model.BaseAlpha);
            SKColor h1color = new SKColor(model.HorizontalColorRed1, model.HorizontalColorGreen1, model.HorizontalColorBlue1, model.BaseAlpha);
            SKColor v1color = new SKColor(model.VerticalColorRed1, model.VerticalColorGreen1, model.VerticalColorBlue1, model.BaseAlpha);
            SKColor h2color = new SKColor(model.HorizontalColorRed2, model.HorizontalColorGreen2, model.HorizontalColorBlue2, model.BaseAlpha);
            SKColor v2color = new SKColor(model.VerticalColorRed2, model.VerticalColorGreen2, model.VerticalColorBlue2, model.BaseAlpha);

            int[] hlines = BussinessLogic.GetDotPattern(model.VerticalDottedPattern, model.HorizontalSize1);
            int[] vlines = BussinessLogic.GetDotPattern(model.HorizontalDottedPattern, model.VerticalSize1);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (x % model.HorizontalSize1 > px2 - model.HorizontalSize1 / 100 - model.HorizontalDottedWeight && x % model.HorizontalSize1 < px2 + model.HorizontalSize1 / 100 + model.HorizontalDottedWeight && vlines[y % model.VerticalSize1] == 1)
                    {
                        bmp.SetPixel(x, y, h2color);
                    }
                    else if (y % model.VerticalSize1 > py2 - model.VerticalSize1 / 100 - model.VerticalDottedWeight && y % model.VerticalSize1 < py2 + model.VerticalSize1 / 100 + model.VerticalDottedWeight && hlines[x % model.HorizontalSize1] == 1)
                    {
                        bmp.SetPixel(x, y, v2color);
                    }
                    else if (x % model.HorizontalSize1 > px - model.HorizontalSize1 / 100 - model.HorizontalDottedWeight && x % model.HorizontalSize1 < px + model.HorizontalSize1 / 100 + model.HorizontalDottedWeight && vlines[y % model.VerticalSize1] == 1)
                    {
                        bmp.SetPixel(x, y, h1color);
                    }
                    else if (y % model.VerticalSize1 > py - model.VerticalSize1 / 100 - model.VerticalDottedWeight && y % model.VerticalSize1 < py + model.VerticalSize1 / 100 + model.VerticalDottedWeight && hlines[x % model.HorizontalSize1] == 1)
                    {
                        bmp.SetPixel(x, y, v1color);
                    }
                    else
                    {
                        bmp.SetPixel(x, y, basecolor);
                    }
                }
            }

            return bmp;
        }
    }
}
