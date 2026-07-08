using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageCreatePlaid
{
    public class GinghamCheck3 : PlaidInterface
    {
        public Bitmap EditImage(Bitmap bmp, PlaidModel model)
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

                    Color color = Color.FromArgb(model.BaseAlpha, model.BaseColorRed, model.BaseColorGreen, model.BaseColorBlue);
                    if (wflg && hflg)
                    {
                        int rColor = BussinessLogic.CalcColor(model.VerticalColorRed1, model.HorizontalColorRed1);
                        int gColor = BussinessLogic.CalcColor(model.VerticalColorGreen1, model.HorizontalColorGreen1);
                        int bColor = BussinessLogic.CalcColor(model.VerticalColorBlue1, model.HorizontalColorBlue1);
                        color = Color.FromArgb(model.Alpha, rColor, gColor, bColor);
                    }
                    else if (wflg)
                    {
                        color = Color.FromArgb(model.Alpha, model.VerticalColorRed1, model.VerticalColorGreen1, model.VerticalColorBlue1);
                    }
                    else if (hflg)
                    {
                        color = Color.FromArgb(model.Alpha, model.HorizontalColorRed1, model.HorizontalColorGreen1, model.HorizontalColorBlue1);
                    }

                    bmp.SetPixel(x, y, color);
                }
            }
            return bmp;
        }
    }
}
