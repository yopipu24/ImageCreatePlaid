using SkiaSharp;

namespace ImageCreatePlaid
{
    public class GinghamCheck3 : PlaidInterface
    {
        public SKBitmap EditImage(SKBitmap bmp, PlaidModel model)
        {
            model.BaseColorRed = model.VerticalColorRed1;
            model.BaseColorGreen = model.VerticalColorGreen1;
            model.BaseColorBlue = model.VerticalColorBlue1;
            model.VerticalColorRed1 = BussinessLogic.CalcColor(model.VerticalColorRed1, 255);
            model.VerticalColorGreen1 = BussinessLogic.CalcColor(model.VerticalColorGreen1, 255);
            model.VerticalColorBlue1 = BussinessLogic.CalcColor(model.VerticalColorBlue1, 255);
            model.HorizontalColorRed1 = BussinessLogic.CalcColor(model.HorizontalColorRed1, 255);
            model.HorizontalColorGreen1 = BussinessLogic.CalcColor(model.HorizontalColorGreen1, 255);
            model.HorizontalColorBlue1 = BussinessLogic.CalcColor(model.HorizontalColorBlue1, 255);
            int width = bmp.Width;
            int height = bmp.Height;

            SKBitmap originalBmp = BussinessLogic.NewCreateImage(model.VerticalSize1, model.HorizontalSize1);
            for (int x = 0; x < originalBmp.Width; x++)
            {
                for (int y = 0; y < originalBmp.Height; y++)
                {
                    bool wflg = false;
                    bool hflg = false;
                    if ((x + model.VerticalSize1 / 2) % model.VerticalSize1 < model.VerticalSize1 / 3)
                    {
                        wflg = true;
                    }

                    if ((y + model.HorizontalSize1 / 2) % model.HorizontalSize1 < model.HorizontalSize1 / 3)
                    {
                        hflg = true;
                    }

                    SKColor color = new SKColor(model.BaseColorRed, model.BaseColorGreen, model.BaseColorBlue, model.BaseAlpha);
                    if (wflg && hflg)
                    {
                        byte rColor = BussinessLogic.CalcColor(model.VerticalColorRed1, model.HorizontalColorRed1);
                        byte gColor = BussinessLogic.CalcColor(model.VerticalColorGreen1, model.HorizontalColorGreen1);
                        byte bColor = BussinessLogic.CalcColor(model.VerticalColorBlue1, model.HorizontalColorBlue1);
                        color = new SKColor(rColor, gColor, bColor, model.Alpha);
                    }
                    else if (wflg)
                    {
                        color = new SKColor(model.VerticalColorRed1, model.VerticalColorGreen1, model.VerticalColorBlue1, model.Alpha);
                    }
                    else if (hflg)
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
