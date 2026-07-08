using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageCreatePlaid
{
    public class Check2 : PlaidInterface
    {
        public Bitmap EditImage(Bitmap bmp, PlaidModel model)
        {
            int width = bmp.Width;
            int height = bmp.Height;
            for (int x = 0; x < width; x++)
            {
                bool wflg1 = false;
                bool wflg2 = false;
                if (x % model.VerticalSize1 < model.VerticalSize1 / model.BaseBalance)
                {
                    if (x % model.VerticalSize1 == model.VerticalSize1 / model.BaseBalance / 2 - 1
                        || x % model.VerticalSize1 == model.VerticalSize1 / model.BaseBalance / 2)
                    {
                        wflg1 = true;
                    }
                    else
                    {
                        wflg2 = true;
                    }
                }

                for (int y = 0; y < height; y++)
                {
                    bool hflg1 = false;
                    bool hflg2 = false;
                    if (y % model.HorizontalSize1 < model.HorizontalSize1 / model.BaseBalance)
                    {

                        if (y % model.HorizontalSize1 == model.HorizontalSize1 / model.BaseBalance / 2 - 1
                            || y % model.HorizontalSize1 == model.HorizontalSize1 / model.BaseBalance / 2)
                        {
                            hflg1 = true;
                        }
                        else
                        {
                            hflg2 = true;
                        }
                    }

                    Color color = Color.FromArgb(model.BaseAlpha, model.BaseColorRed, model.BaseColorGreen, model.BaseColorBlue);
                    if (wflg1 && hflg1)
                    {
                        int rColor = BussinessLogic.CalcColor(model.VerticalColorRed1, model.HorizontalColorRed1);
                        int gColor = BussinessLogic.CalcColor(model.VerticalColorGreen1, model.HorizontalColorGreen1);
                        int bColor = BussinessLogic.CalcColor(model.VerticalColorBlue1, model.HorizontalColorBlue1);
                        color = Color.FromArgb(model.Alpha, rColor, gColor, bColor);
                    }
                    else if (wflg2 && hflg2)
                    {
                        int rColor = BussinessLogic.CalcColor(model.VerticalColorRed2, model.HorizontalColorRed2);
                        int gColor = BussinessLogic.CalcColor(model.VerticalColorGreen2, model.HorizontalColorGreen2);
                        int bColor = BussinessLogic.CalcColor(model.VerticalColorBlue2, model.HorizontalColorBlue2);
                        color = Color.FromArgb(model.Alpha, rColor, gColor, bColor);
                    }
                    else if (wflg1 && hflg2)
                    {
                        int rColor = BussinessLogic.CalcColor(model.VerticalColorRed1, model.HorizontalColorRed2);
                        int gColor = BussinessLogic.CalcColor(model.VerticalColorGreen1, model.HorizontalColorGreen2);
                        int bColor = BussinessLogic.CalcColor(model.VerticalColorBlue1, model.HorizontalColorBlue2);
                        color = Color.FromArgb(model.Alpha, rColor, gColor, bColor);
                    }
                    else if (wflg2 && hflg1)
                    {
                        int rColor = BussinessLogic.CalcColor(model.VerticalColorRed2, model.HorizontalColorRed1);
                        int gColor = BussinessLogic.CalcColor(model.VerticalColorGreen2, model.HorizontalColorGreen1);
                        int bColor = BussinessLogic.CalcColor(model.VerticalColorBlue2, model.HorizontalColorBlue1);
                        color = Color.FromArgb(model.Alpha, rColor, gColor, bColor);
                    }
                    else if (wflg1)
                    {
                        color = Color.FromArgb(model.Alpha, model.VerticalColorRed1, model.VerticalColorGreen1, model.VerticalColorBlue1);
                    }
                    else if (wflg2)
                    {
                        color = Color.FromArgb(model.Alpha, model.VerticalColorRed2, model.VerticalColorGreen2, model.VerticalColorBlue2);
                    }
                    else if (hflg1)
                    {
                        color = Color.FromArgb(model.Alpha, model.HorizontalColorRed1, model.HorizontalColorGreen1, model.HorizontalColorBlue1);
                    }
                    else if (hflg2)
                    {
                        color = Color.FromArgb(model.Alpha, model.HorizontalColorRed2, model.HorizontalColorGreen2, model.HorizontalColorBlue2);
                    }

                    bmp.SetPixel(x, y, color);
                }
            }
            return bmp;
        }
    }
}
