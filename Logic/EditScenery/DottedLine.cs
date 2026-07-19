using SkiaSharp;

namespace ImageCreatePlaid
{
    public class DottedLine : SceneryInterface
    {
        public SKBitmap EditImage(SKBitmap bmp, SceneryModel model)
        {
            int width = bmp.Width;
            int height = bmp.Height;
            decimal pointx = model.HorizontalSize1 / 2;
            decimal pointy = model.VerticalSize1 / 2;
            int px = (int)Math.Truncate(pointx);
            int py = (int)Math.Truncate(pointy);

            int[] hlines = BussinessLogic.GetDotPattern(model.VerticalDottedPattern, model.HorizontalSize1);
            int[] vlines = BussinessLogic.GetDotPattern(model.HorizontalDottedPattern, model.VerticalSize1);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    SKColor color = new SKColor(model.BaseColorRed, model.BaseColorGreen, model.BaseColorBlue, model.BaseAlpha);
                    if ((x % model.HorizontalSize1 > px - model.HorizontalSize1 / 100 - model.HorizontalDottedWeight && x % model.HorizontalSize1 < px + model.HorizontalSize1 / 100 + model.HorizontalDottedWeight) && vlines[y % model.VerticalSize1] == 1)
                    {
                        color = new SKColor(model.VerticalColorRed1, model.VerticalColorGreen1, model.VerticalColorBlue1, model.Alpha);
                    }

                    if ((y % model.VerticalSize1 > py - model.VerticalSize1 / 100 - model.VerticalDottedWeight && y % model.VerticalSize1 < py + model.VerticalSize1 / 100 + model.VerticalDottedWeight) && hlines[x % model.HorizontalSize1] == 1)
                    {
                        color = new SKColor(model.VerticalColorRed1, model.VerticalColorGreen1, model.VerticalColorBlue1, model.Alpha);
                    }

                    bmp.SetPixel(x, y, color);
                }
            }

            return bmp;
        }
    }
}
