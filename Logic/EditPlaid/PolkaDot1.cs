using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageCreatePlaid
{
    public class PolkaDot1 : PlaidInterface
    {
        public Bitmap EditImage(Bitmap bmp, PlaidModel model)
        {
            int width = bmp.Width;
            int height = bmp.Height;
            Color color1 = Color.FromArgb(model.Alpha, model.VerticalColorRed1, model.VerticalColorGreen1, model.VerticalColorBlue1);
            Color color2 = Color.FromArgb(model.Alpha, model.HorizontalColorRed2, model.HorizontalColorGreen2, model.HorizontalColorBlue2);
            
            Graphics g = Graphics.FromImage(bmp);
            Brush b = new SolidBrush(color1);

            bool colorFlg = false;
            for (int j = 0; j < height / model.VerticalSize1; j++)
            {
                int heightBlank = BussinessLogic.GetRandomInt(model.VerticalSize1 * 2);
                if (j * model.VerticalSize1 + heightBlank <= height - heightBlank)
                {
                    for (int i = 0; i < width / model.HorizontalSize1; i++)
                    {
                        int widthBlank = BussinessLogic.GetRandomInt(model.HorizontalSize1 * 2);
                        if (i * model.HorizontalSize1 + widthBlank <= width - widthBlank)
                        {
                            int heightRandomBlank = BussinessLogic.GetRandomInt(model.HorizontalSize1) - model.HorizontalSize1 / 2;
                            if (colorFlg)
                            {
                                b = new SolidBrush(color1);
                                g.FillEllipse(b, i * model.HorizontalSize1 + widthBlank, j * model.VerticalSize1 + heightBlank + heightRandomBlank, model.HorizontalSize1, model.VerticalSize1);
                                colorFlg = false;
                            }
                            else
                            {
                                b = new SolidBrush(color2);
                                g.FillEllipse(b, i * model.HorizontalSize1 + widthBlank, j * model.VerticalSize1 + heightBlank + heightRandomBlank, model.HorizontalSize1, model.VerticalSize1);
                                colorFlg = true;
                            }

                            i += widthBlank / model.HorizontalSize1 * 2 + 1;
                        }
                    }

                    j += heightBlank / model.VerticalSize1 * 2 + 1;
                }
            }

            g.Dispose();
            return bmp;
        }
    }
}
