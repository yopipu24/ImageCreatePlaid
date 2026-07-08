using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageCreatePlaid
{
    public class TartanCheck2 : PlaidInterface
    {
        public Bitmap EditImage(Bitmap bmp, PlaidModel model)
        {
            int width = bmp.Width;
            int height = bmp.Height;
            int stripeSize1 = model.HorizontalSize1 / BussinessLogic.GetRandomInt(3) + 2;
            int stripeSize2 = model.HorizontalSize1 / BussinessLogic.GetRandomInt(6) + 4;
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.FromArgb(model.BaseAlpha, model.BaseColorRed, model.BaseColorGreen, model.BaseColorBlue));

                for (int x = 0; x < width; x += stripeSize1 * 2)
                {
                    using (Brush brush = new SolidBrush(Color.FromArgb(180, model.HorizontalColorRed1, model.HorizontalColorGreen1, model.HorizontalColorBlue1)))
                    {
                        g.FillRectangle(brush, x, 0, stripeSize1, height);
                    }

                    using (Brush brush = new SolidBrush(Color.FromArgb(150, model.HorizontalColorRed2, model.HorizontalColorGreen2, model.HorizontalColorBlue2)))
                    {
                        g.FillRectangle(brush, x + stripeSize1, 0, stripeSize2, height);
                    }
                }

                for (int y = 0; y < height; y += stripeSize1 * 2)
                {
                    using (Brush brush = new SolidBrush(Color.FromArgb(180, model.VerticalColorRed1, model.VerticalColorGreen1, model.VerticalColorBlue1)))
                    {
                        g.FillRectangle(brush, 0, y, width, stripeSize1);
                    }

                    using (Brush brush = new SolidBrush(Color.FromArgb(150, model.HorizontalColorRed2, model.HorizontalColorGreen2, model.HorizontalColorBlue2)))
                    {
                        g.FillRectangle(brush, 0, y + stripeSize1, width, stripeSize2);
                    }
                }
            }

            return bmp;
        }
    }
}
