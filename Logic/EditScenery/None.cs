using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageCreatePlaid
{
    public class None : SceneryInterface
    {
        public Bitmap EditImage(Bitmap bmp, SceneryModel model)
        {
            int width = bmp.Width;
            int height = bmp.Height;
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Color color = Color.FromArgb(model.Alpha, model.BaseColorRed, model.BaseColorGreen, model.BaseColorBlue);
                    bmp.SetPixel(x, y, color);
                }
            }
            return bmp;
        }
    }
}
