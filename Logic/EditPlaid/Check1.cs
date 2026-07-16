using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageCreatePlaid
{
    public class Check1 : PlaidInterface
    {
        public SKBitmap EditImage(SKBitmap bmp, PlaidModel model)
        {
            int width = bmp.Width;
            int height = bmp.Height;
            int wCount = 0;
            bool wCountFlg = false;
            for (int x = 0; x < width; x++)
            {
                bool wflg1 = false;
                bool wflg2 = false;
                if (x % model.VerticalSize1 < model.VerticalSize1 / model.BaseBalance)
                {
                    if (wCount >= model.VerticalSize1 / model.BaseBalance)
                    {
                        wCount = 0;
                        if (wCountFlg)
                        {
                            wCountFlg = false;
                        }
                        else
                        {
                            wCountFlg = true;
                        }
                    }

                    if (wCountFlg)
                    {
                        wflg1 = true;
                    }
                    else
                    {
                        wflg2 = true;
                    }

                    wCount += 1;
                }

                int hCount = 0;
                bool hCountFlg = false;
                for (int y = 0; y < height; y++)
                {
                    bool hflg1 = false;
                    bool hflg2 = false;
                    if (y % model.HorizontalSize1 < model.HorizontalSize1 / model.BaseBalance)
                    {
                        if (hCount >= model.HorizontalSize1 / model.BaseBalance)
                        {
                            hCount = 0;
                            if (hCountFlg)
                            {
                                hCountFlg = false;
                            }
                            else
                            {
                                hCountFlg = true;
                            }
                        }

                        if (hCountFlg)
                        {
                            hflg1 = true;
                        }
                        else
                        {
                            hflg2 = true;
                        }

                        hCount += 1;
                    }

                    SKColor color = new SKColor(model.BaseColorRed, model.BaseColorGreen, model.BaseColorBlue, model.BaseAlpha);
                    if (wflg1 && hflg1)
                    {
                        byte rColor = BussinessLogic.CalcColor(model.VerticalColorRed1, model.HorizontalColorRed1);
                        byte gColor = BussinessLogic.CalcColor(model.VerticalColorGreen1, model.HorizontalColorGreen1);
                        byte bColor = BussinessLogic.CalcColor(model.VerticalColorBlue1, model.HorizontalColorBlue1);
                        color = new SKColor(rColor, gColor, bColor, model.BaseAlpha);
                    }
                    else if (wflg2 && hflg2)
                    {
                        byte rColor = BussinessLogic.CalcColor(model.VerticalColorRed2, model.HorizontalColorRed2);
                        byte gColor = BussinessLogic.CalcColor(model.VerticalColorGreen2, model.HorizontalColorGreen2);
                        byte bColor = BussinessLogic.CalcColor(model.VerticalColorBlue2, model.HorizontalColorBlue2);
                        color = new SKColor(rColor, gColor, bColor, model.BaseAlpha);
                    }
                    else if (wflg1 && hflg2)
                    {
                        byte rColor = BussinessLogic.CalcColor(model.VerticalColorRed1, model.HorizontalColorRed2);
                        byte gColor = BussinessLogic.CalcColor(model.VerticalColorGreen1, model.HorizontalColorGreen2);
                        byte bColor = BussinessLogic.CalcColor(model.VerticalColorBlue1, model.HorizontalColorBlue2);
                        color = new SKColor(rColor, gColor, bColor, model.BaseAlpha);
                    }
                    else if (wflg2 && hflg1)
                    {
                        byte rColor = BussinessLogic.CalcColor(model.VerticalColorRed2, model.HorizontalColorRed1);
                        byte gColor = BussinessLogic.CalcColor(model.VerticalColorGreen2, model.HorizontalColorGreen1);
                        byte bColor = BussinessLogic.CalcColor(model.VerticalColorBlue2, model.HorizontalColorBlue1);
                        color = new SKColor(rColor, gColor, bColor, model.BaseAlpha);
                    }
                    else if (wflg1)
                    {
                        color = new SKColor(model.VerticalColorRed1, model.VerticalColorGreen1, model.VerticalColorBlue1, model.BaseAlpha);
                    }
                    else if (wflg2)
                    {
                        color = new SKColor(model.VerticalColorRed2, model.VerticalColorGreen2, model.VerticalColorBlue2, model.BaseAlpha);
                    }
                    else if (hflg1)
                    {
                        color = new SKColor(model.HorizontalColorRed1, model.HorizontalColorGreen1, model.HorizontalColorBlue1, model.BaseAlpha);
                    }
                    else if (hflg2)
                    {
                        color = new SKColor(model.HorizontalColorRed2, model.HorizontalColorGreen2, model.HorizontalColorBlue2, model.BaseAlpha);
                    }

                    bmp.SetPixel(x, y, color);
                }
            }
            return bmp;
        }
    }
}
