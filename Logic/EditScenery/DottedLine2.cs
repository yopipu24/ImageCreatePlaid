using SkiaSharp;

namespace ImageCreatePlaid
{
    public class DottedLine2 : SceneryInterface
    {
        public SKBitmap EditImage(SKBitmap bmp, SceneryModel model)
        {
            int width = bmp.Width;
            int height = bmp.Height;
            decimal pointx = model.HorizontalSize1 / 2;
            decimal pointy = model.VerticalSize1 / 2;
            int px = (int)Math.Truncate(pointx);
            int py = (int)Math.Truncate(pointy);
            decimal pointx2 = model.HorizontalSize1 / 4;
            decimal pointy2 = model.VerticalSize1 / 4;
            int px2 = (int)Math.Truncate(pointx2);
            int py2 = (int)Math.Truncate(pointy2);
            SKColor basecolor = new SKColor(model.BaseColorRed, model.BaseColorGreen, model.BaseColorBlue, model.BaseAlpha);
            SKColor h1color = new SKColor(model.HorizontalColorRed1, model.HorizontalColorGreen1, model.HorizontalColorBlue1, model.Alpha);
            SKColor v1color = new SKColor(model.VerticalColorRed1, model.VerticalColorGreen1, model.VerticalColorBlue1, model.Alpha);
            SKColor h2color = new SKColor(model.HorizontalColorRed2, model.HorizontalColorGreen2, model.HorizontalColorBlue2, model.Alpha);
            SKColor v2color = new SKColor(model.VerticalColorRed2, model.VerticalColorGreen2, model.VerticalColorBlue2, model.Alpha);

            int[] hlines = BussinessLogic.GetDotPattern(model.VerticalDottedPattern, model.HorizontalSize1);
            int[] vlines = BussinessLogic.GetDotPattern(model.HorizontalDottedPattern, model.VerticalSize1);

            SKBitmap originalBmp = BussinessLogic.NewCreateImage(model.VerticalSize1 * model.BaseBalance, model.HorizontalSize1 * model.BaseBalance);

            for (int x = 0; x < originalBmp.Width; x++)
            {
                for (int y = 0; y < originalBmp.Height; y++)
                {
                    if (x % model.HorizontalSize1 > px2 - model.HorizontalSize1 / 100 - model.HorizontalDottedWeight && x % model.HorizontalSize1 < px2 + model.HorizontalSize1 / 100 + model.HorizontalDottedWeight && vlines[y % model.VerticalSize1] == 1)
                    {
                        originalBmp.SetPixel(x, y, h2color);
                    }
                    else if (y % model.VerticalSize1 > py2 - model.VerticalSize1 / 100 - model.VerticalDottedWeight && y % model.VerticalSize1 < py2 + model.VerticalSize1 / 100 + model.VerticalDottedWeight && hlines[x % model.HorizontalSize1] == 1)
                    {
                        originalBmp.SetPixel(x, y, v2color);
                    }
                    else if (x % model.HorizontalSize1 > px - model.HorizontalSize1 / 100 - model.HorizontalDottedWeight && x % model.HorizontalSize1 < px + model.HorizontalSize1 / 100 + model.HorizontalDottedWeight && vlines[y % model.VerticalSize1] == 1)
                    {
                        originalBmp.SetPixel(x, y, h1color);
                    }
                    else if (y % model.VerticalSize1 > py - model.VerticalSize1 / 100 - model.VerticalDottedWeight && y % model.VerticalSize1 < py + model.VerticalSize1 / 100 + model.VerticalDottedWeight && hlines[x % model.HorizontalSize1] == 1)
                    {
                        originalBmp.SetPixel(x, y, v1color);
                    }
                    else
                    {
                        originalBmp.SetPixel(x, y, basecolor);
                    }
                }
            }

            bmp = BussinessLogic.RepeatImage(width, height, originalBmp);

            return bmp;
        }
    }
}
