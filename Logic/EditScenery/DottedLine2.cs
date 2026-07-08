using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageCreatePlaid
{
    public class DottedLine2 : SceneryInterface
    {
        public Bitmap EditImage(Bitmap bmp, SceneryModel model)
        {
            int width = bmp.Width;
            int height = bmp.Height;
            decimal pointx = model.HorizontalSize1 / 2;
            decimal pointy = model.VerticalSize1 / 2;
            int px = (int)Math.Truncate(pointx);
            int py = (int)Math.Truncate(pointy);
            decimal pointx2 = model.HorizontalSize1 / 4;
            decimal pointy2 = model.VerticalSize1 / 4;
            int px2 = (int)Math.Truncate(pointx2);
            int py2 = (int)Math.Truncate(pointy2);
            Color basecolor = Color.FromArgb(model.BaseAlpha, model.BaseColorRed, model.BaseColorGreen, model.BaseColorBlue);
            Color h1color = Color.FromArgb(model.Alpha, model.HorizontalColorRed1, model.HorizontalColorGreen1, model.HorizontalColorBlue1);
            Color v1color = Color.FromArgb(model.Alpha, model.VerticalColorRed1, model.VerticalColorGreen1, model.VerticalColorBlue1); ;
            Color h2color = Color.FromArgb(model.Alpha, model.HorizontalColorRed2, model.HorizontalColorGreen2, model.HorizontalColorBlue2); ;
            Color v2color = Color.FromArgb(model.Alpha, model.VerticalColorRed2, model.VerticalColorGreen2, model.VerticalColorBlue2); ;

            int[] hlines = BussinessLogic.GetDotPattern(model.VerticalDottedPattern, model.HorizontalSize1);
            int[] vlines = BussinessLogic.GetDotPattern(model.HorizontalDottedPattern, model.VerticalSize1);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (x % model.HorizontalSize1 > px2 - model.HorizontalSize1 / 100 - model.HorizontalDottedWeight && x % model.HorizontalSize1 < px2 + model.HorizontalSize1 / 100 + model.HorizontalDottedWeight && vlines[y % model.VerticalSize1] == 1)
                    {
                        bmp.SetPixel(x, y, h2color);
                    }
                    else if (y % model.VerticalSize1 > py2 - model.VerticalSize1 / 100 - model.VerticalDottedWeight && y % model.VerticalSize1 < py2 + model.VerticalSize1 / 100 + model.VerticalDottedWeight && hlines[x % model.HorizontalSize1] == 1)
                    {
                        bmp.SetPixel(x, y, v2color);
                    }
                    else if (x % model.HorizontalSize1 > px - model.HorizontalSize1 / 100 - model.HorizontalDottedWeight && x % model.HorizontalSize1 < px + model.HorizontalSize1 / 100 + model.HorizontalDottedWeight && vlines[y % model.VerticalSize1] == 1)
                    {
                        bmp.SetPixel(x, y, h1color);
                    }
                    else if (y % model.VerticalSize1 > py - model.VerticalSize1 / 100 - model.VerticalDottedWeight && y % model.VerticalSize1 < py + model.VerticalSize1 / 100 + model.VerticalDottedWeight && hlines[x % model.HorizontalSize1] == 1)
                    {
                        bmp.SetPixel(x, y, v1color);
                    }
                    else
                    {
                        bmp.SetPixel(x, y, basecolor);
                    }
                }
            }

            return bmp;
        }
    }
}
