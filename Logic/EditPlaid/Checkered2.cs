using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageCreatePlaid
{
    public class Checkered2 : PlaidInterface
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
                    bool hflg = false;
                    if ((x + model.VerticalSize1 / 2) % model.VerticalSize1 < model.VerticalSize1 / 2)
                    {
                        wflg = true;
                    }

                    if ((y + model.HorizontalSize1 / 2) % model.HorizontalSize1 < model.HorizontalSize1 / 2)
                    {
                        hflg = true;
                    }

                    Color color = Color.FromArgb(model.BaseAlpha, model.BaseColorRed, model.BaseColorGreen, model.BaseColorBlue);
                    if (wflg && hflg == false)
                    {
                        color = Color.FromArgb(model.Alpha, model.VerticalColorRed1, model.VerticalColorGreen1, model.VerticalColorBlue1);
                    }
                    else if (hflg && wflg == false)
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
