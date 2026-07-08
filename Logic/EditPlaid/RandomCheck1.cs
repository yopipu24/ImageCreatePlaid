using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageCreatePlaid
{
    public class RandomCheck1 : PlaidInterface
    {
        public Bitmap EditImage(Bitmap bmp, PlaidModel model)
        {
            int width = bmp.Width;
            int height = bmp.Height;
            Bitmap originalBmp = BussinessLogic.NewCreateImage(model.VerticalSize1, model.HorizontalSize1);
            int colorFlg = BussinessLogic.GetRandomInt(3);
            int countFlg = BussinessLogic.GetRandomInt(model.VerticalSize1 / 3) + 5;
            Color color0 = Color.FromArgb(model.BaseAlpha, model.BaseColorRed, model.BaseColorGreen, model.BaseColorBlue);
            Color color1v = Color.FromArgb(model.Alpha, model.VerticalColorRed1, model.VerticalColorGreen1, model.VerticalColorBlue1);
            Color color2v = Color.FromArgb(model.Alpha, model.VerticalColorRed2, model.VerticalColorGreen2, model.VerticalColorBlue2);
            Color color1h = Color.FromArgb(model.Alpha, model.HorizontalColorRed1, model.HorizontalColorGreen1, model.HorizontalColorBlue1);
            Color color2h = Color.FromArgb(model.Alpha, model.HorizontalColorRed2, model.HorizontalColorGreen2, model.HorizontalColorBlue2);
            for (int i = 0; i < model.VerticalSize1; i++)
            {
                for (int j = 0; j < model.HorizontalSize1; j++)
                {
                    if (colorFlg == 0)
                    {
                        originalBmp.SetPixel(i, j, color0);
                    }
                    else if (colorFlg == 1)
                    {
                        originalBmp.SetPixel(i, j, color1v);
                    }
                    else if (colorFlg == 2)
                    {
                        originalBmp.SetPixel(i, j, color2v);
                    }
                }

                if (countFlg == i)
                {
                    countFlg += BussinessLogic.GetRandomInt(model.VerticalSize1 / 3 + 5);
                    colorFlg = BussinessLogic.GetRandomInt(3);
                }
            }

            colorFlg = 0;
            countFlg = BussinessLogic.GetRandomInt(model.VerticalSize1 / 3 + 5);
            int[] colorHList = new int[model.HorizontalSize1];
            for (int j = 0; j < model.HorizontalSize1; j++)
            {
                for (int i = 0; i < model.VerticalSize1; i++)
                {
                    Color tempColor = originalBmp.GetPixel(i, j);
                    if (colorFlg == 0)
                    {
                        tempColor = BussinessLogic.GetMixColor(tempColor, color0);
                    }
                    else if (colorFlg == 1)
                    {
                        tempColor = BussinessLogic.GetMixColor(tempColor, color1h);
                    }
                    else if (colorFlg == 2)
                    {
                        tempColor = BussinessLogic.GetMixColor(tempColor, color2h);
                    }

                    originalBmp.SetPixel(i, j, tempColor);
                }

                if (countFlg == j)
                {
                    countFlg += BussinessLogic.GetRandomInt(model.HorizontalSize1 / 3 + 5);
                    colorFlg = BussinessLogic.GetRandomInt(3);
                }
            }
            
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Color originalColor = originalBmp.GetPixel(x % model.VerticalSize1, y % model.HorizontalSize1);
                    bmp.SetPixel(x, y, originalColor);
                }
            }
            return bmp;
        }
    }
}
