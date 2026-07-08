using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageCreatePlaid
{
    public class DottedLine : SceneryInterface
    {
        public Bitmap EditImage(Bitmap bmp, SceneryModel model)
        {
            int width = bmp.Width;
            int height = bmp.Height;
            decimal pointx = model.HorizontalSize1 / 2;
            decimal pointy = model.VerticalSize1 / 2;
            int px = (int)Math.Truncate(pointx);
            int py = (int)Math.Truncate(pointy);

            int[] hlines = BussinessLogic.GetDotPattern(model.VerticalDottedPattern, model.HorizontalSize1);
            int[] vlines = BussinessLogic.GetDotPattern(model.HorizontalDottedPattern, model.VerticalSize1);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Color color = Color.FromArgb(model.BaseAlpha, model.BaseColorRed, model.BaseColorGreen, model.BaseColorBlue);
                    if ((x % model.HorizontalSize1 > px - model.HorizontalSize1 / 100 - model.HorizontalDottedWeight && x % model.HorizontalSize1 < px + model.HorizontalSize1 / 100 + model.HorizontalDottedWeight) && vlines[y % model.VerticalSize1] == 1)
                    {
                        color = Color.FromArgb(model.Alpha, model.VerticalColorRed1, model.VerticalColorGreen1, model.VerticalColorBlue1);
                    }

                    if ((y % model.VerticalSize1 > py - model.VerticalSize1 / 100 - model.VerticalDottedWeight && y % model.VerticalSize1 < py + model.VerticalSize1 / 100 + model.VerticalDottedWeight) && hlines[x % model.HorizontalSize1] == 1)
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
