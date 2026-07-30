using SkiaSharp;

namespace ImageCreatePlaid
{
    public class HorizontalLine1 : PlaidInterface
    {
        public SKBitmap EditImage(SKBitmap bmp, PlaidModel model)
        {
            int width = bmp.Width;
            int height = bmp.Height;

            SKBitmap originalBmp = BussinessLogic.NewCreateImage(model.VerticalSize1, model.HorizontalSize1);
            for (int x = 0; x < originalBmp.Width; x++)
            {
                for (int y = 0; y < originalBmp.Height; y++)
                {
                    bool hflg = false;
                    if ((y + model.HorizontalSize1 / 2) % model.HorizontalSize1 < model.HorizontalSize1 / 2)
                    {
                        hflg = true;
                    }
                    SKColor color = new SKColor(model.BaseColorRed, model.BaseColorGreen, model.BaseColorBlue, model.BaseAlpha);
                    if (hflg)
                    {
                        color = new SKColor(model.HorizontalColorRed1, model.HorizontalColorGreen1, model.HorizontalColorBlue1, model.Alpha);
                    }

                    originalBmp.SetPixel(x, y, color);
                }
            }

            bmp = BussinessLogic.RepeatImage(width, height, originalBmp);

            return bmp;
        }
    }
}
