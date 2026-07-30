using SkiaSharp;

namespace ImageCreatePlaid
{
    public class Check3 : PlaidInterface
    {
        public SKBitmap EditImage(SKBitmap bmp, PlaidModel model)
        {
            model.BaseBalance = 3;
            int width = bmp.Width;
            int height = bmp.Height;
            SKBitmap originalBmp = BussinessLogic.NewCreateImage(model.VerticalSize1 * model.BaseBalance, model.HorizontalSize1 * model.BaseBalance);
            for (int x = 0; x < originalBmp.Width; x++)
            {
                for (int y = 0; y < originalBmp.Height; y++)
                {
                    bool wflg1 = false;
                    bool wflg2 = false;
                    bool wflg3 = false;
                    if (x % model.VerticalSize1 < model.VerticalSize1 / model.BaseBalance)
                    {
                        if (x % model.VerticalSize1 < model.VerticalSize1 / model.BaseBalance / 3)
                        {
                            wflg1 = true;
                        }
                        else if (x % model.VerticalSize1 > model.VerticalSize1 / model.BaseBalance / 3 * 2)
                        {
                            wflg3 = true;
                        }
                        else
                        {
                            wflg2 = true;
                        }
                    }

                    bool hflg1 = false;
                    bool hflg2 = false;
                    bool hflg3 = false;
                    if (y % model.HorizontalSize1 < model.HorizontalSize1 / model.BaseBalance)
                    {

                        if (y % model.HorizontalSize1 < model.HorizontalSize1 / model.BaseBalance / 3)
                        {
                            hflg1 = true;
                        }
                        else if (y % model.HorizontalSize1 > model.HorizontalSize1 / model.BaseBalance / 3 * 2)
                        {
                            hflg3 = true;
                        }
                        else
                        {
                            hflg2 = true;
                        }
                    }

                    SKColor color = new SKColor(model.BaseColorRed, model.BaseColorGreen, model.BaseColorBlue, model.BaseAlpha);
                    if ((wflg1 && hflg1) || (wflg1 && hflg3) || (wflg3 && hflg1) || (wflg3 && hflg3) || (wflg2 && hflg2))
                    {
                        byte rColor = BussinessLogic.CalcColor(model.VerticalColorRed2, model.HorizontalColorRed2);
                        byte gColor = BussinessLogic.CalcColor(model.VerticalColorGreen2, model.HorizontalColorGreen2);
                        byte bColor = BussinessLogic.CalcColor(model.VerticalColorBlue2, model.HorizontalColorBlue2);
                        color = new SKColor(rColor, gColor, bColor, model.Alpha);
                    }
                    else if ((wflg2 && hflg1) || (wflg2 && hflg3) || (hflg2 && wflg1) || (hflg2 && wflg3))
                    {
                        color = new SKColor(model.VerticalColorRed1, model.VerticalColorGreen1, model.VerticalColorBlue1, model.Alpha);
                    }
                    else if (wflg2 && hflg2)
                    {
                        byte rColor = BussinessLogic.CalcColor(model.VerticalColorRed1, model.HorizontalColorRed2);
                        byte gColor = BussinessLogic.CalcColor(model.VerticalColorGreen1, model.HorizontalColorGreen2);
                        byte bColor = BussinessLogic.CalcColor(model.VerticalColorBlue1, model.HorizontalColorBlue2);
                        color = new SKColor(rColor, gColor, bColor, model.Alpha);
                    }
                    else if (wflg1 || wflg2 || wflg3)
                    {
                        color = new SKColor(model.VerticalColorRed2, model.VerticalColorGreen2, model.VerticalColorBlue2, model.Alpha);
                    }
                    else if (hflg1 || hflg2 || hflg3)
                    {
                        color = new SKColor(model.VerticalColorRed2, model.VerticalColorGreen2, model.VerticalColorBlue2, model.Alpha);
                    }

                    originalBmp.SetPixel(x, y, color);
                }
            }

            bmp = BussinessLogic.RepeatImage(width, height, originalBmp);

            return bmp;
        }
    }
}
