using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageCreatePlaid
{
    public class VerticalLine1 : PlaidInterface
    {
        public Bitmap EditImage(Bitmap bmp, PlaidModel model)
        {
            int width = bmp.Width;
            int height = bmp.Height;
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    bool wflg = false;
                    if ((x + model.VerticalSize1 / 2) % model.VerticalSize1 < model.VerticalSize1 / 2)
                    {
                        wflg = true;
                    }
                    Color color = Color.FromArgb(model.BaseAlpha, model.BaseColorRed, model.BaseColorGreen, model.BaseColorBlue);
                    if (wflg)
                    {
                        color = Color.FromArgb(model.Alpha, model.VerticalColorRed1, model.VerticalColorGreen1, model.VerticalColorBlue1);
                    }

                    bmp.SetPixel(x, y, color);
                }
            }
            return bmp;
        }
    }
}
