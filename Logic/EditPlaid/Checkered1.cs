using SkiaSharp;

namespace ImageCreatePlaid
{
    public class Checkered1 : PlaidInterface
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
                    bool wflg = false;
                    bool hflg = false;
                    if ((x + model.VerticalSize1 / 2) % model.VerticalSize1 < model.VerticalSize1 / 2)
                    {
                        wflg = true;
                    }

                    if ((y + model.HorizontalSize1 / 2) % model.HorizontalSize1 < model.HorizontalSize1 / 2)
                    {
                        hflg = true;
                    }

                    SKColor color = new SKColor(model.BaseColorRed, model.BaseColorGreen, model.BaseColorBlue, model.BaseAlpha);
                    if (wflg && hflg == false)
                    {
                        color = new SKColor(model.VerticalColorRed1, model.VerticalColorGreen1, model.VerticalColorBlue1, model.Alpha);
                    }
                    else if (hflg && wflg == false)
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
