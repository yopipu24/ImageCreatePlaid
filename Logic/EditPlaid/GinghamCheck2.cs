using SkiaSharp;

namespace ImageCreatePlaid
{
    public class GinghamCheck2 : PlaidInterface
    {
        public SKBitmap EditImage(SKBitmap bmp, PlaidModel model)
        {
            int width = bmp.Width;
            int height = bmp.Height;
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
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

                    bmp.SetPixel(x, y, color);
                }
            }
            return bmp;
        }
    }
}
