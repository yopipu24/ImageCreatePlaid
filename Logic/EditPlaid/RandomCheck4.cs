using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageCreatePlaid
{
    public class RandomCheck4 : PlaidInterface
    {
        public Bitmap EditImage(Bitmap bmp, PlaidModel model)
        {
            int width = bmp.Width;
            int height = bmp.Height;
            Bitmap originalBmp = BussinessLogic.NewCreateImage(model.VerticalSize1, model.HorizontalSize1);
            int colorFlg = BussinessLogic.GetRandomInt(3);
            Color color0 = Color.FromArgb(model.BaseAlpha, model.BaseColorRed, model.BaseColorGreen, model.BaseColorBlue);
            Color color1v = Color.FromArgb(model.Alpha, model.VerticalColorRed1, model.VerticalColorGreen1, model.VerticalColorBlue1);
            Color color2v = Color.FromArgb(model.Alpha, model.VerticalColorRed2, model.VerticalColorGreen2, model.VerticalColorBlue2);
            Color color1h = Color.FromArgb(model.Alpha, model.HorizontalColorRed1, model.HorizontalColorGreen1, model.HorizontalColorBlue1);
            Color color2h = Color.FromArgb(model.Alpha, model.HorizontalColorRed2, model.HorizontalColorGreen2, model.HorizontalColorBlue2);
            List<Color> colorLine = new List<Color>();
            int countFlg = BussinessLogic.GetRandomInt(model.VerticalSize1 / 2) + 5;
            for (int i = 0; i < countFlg; i++) colorLine.Add(color0);
            countFlg += BussinessLogic.GetRandomInt(model.VerticalSize1 / 2);
            for (int i = colorLine.Count; i < countFlg && i < model.VerticalSize1; i++) colorLine.Add(color1v);
            countFlg += BussinessLogic.GetRandomInt(model.VerticalSize1 / 2) + 5;
            for (int i = colorLine.Count; i < countFlg && i < model.VerticalSize1; i++) colorLine.Add(color0);
            countFlg += BussinessLogic.GetRandomInt(model.VerticalSize1 / 2);
            //for (int i = colorLine.Count; i < countFlg && i < model.VerticalSize1; i++) colorLine.Add(color1v);
            for (int i = colorLine.Count; i < model.VerticalSize1; i++) colorLine.Add(color0);
            var colorArray = colorLine.ToArray();
            for (int i = 0; i < model.VerticalSize1; i++)
            {
                for (int j = 0; j < model.HorizontalSize1; j++)
                {
                    if (colorArray[i] == color1v && colorArray[j] == color1v)
                    {
                        originalBmp.SetPixel(i, j, BussinessLogic.CalcColor(color1v));
                    }
                    else if (colorArray[i] == color1v || colorArray[j] == color1v)
                    {
                        originalBmp.SetPixel(i, j, color1v);
                    }
                    else
                    {
                        originalBmp.SetPixel(i, j, color0);
                    }
                }
            }

            colorLine.Clear();
            countFlg = BussinessLogic.GetRandomInt(model.HorizontalSize1 / 3) + 5;
            for (int i = 0; i < countFlg; i++) colorLine.Add(color0);
            countFlg += BussinessLogic.GetRandomInt(model.HorizontalSize1 / 5);
            for (int i = colorLine.Count; i < countFlg && i < model.HorizontalSize1; i++) colorLine.Add(color2h);
            countFlg += BussinessLogic.GetRandomInt(model.HorizontalSize1 / 3) + 5;
            for (int i = colorLine.Count; i < countFlg && i < model.HorizontalSize1; i++) colorLine.Add(color0);
            countFlg += BussinessLogic.GetRandomInt(model.HorizontalSize1 / 5);
            //for (int i = colorLine.Count; i < countFlg && i < model.HorizontalSize1; i++) colorLine.Add(color2h);
            for (int i = colorLine.Count; i < model.HorizontalSize1; i++) colorLine.Add(color0);
            colorArray = colorLine.ToArray();
            for (int j = 0; j < model.HorizontalSize1; j++)
            {
                for (int i = 0; i < model.VerticalSize1; i++)
                {
                    if (colorArray[i] == color2h && colorArray[j] == color2h)
                    {
                        originalBmp.SetPixel(i, j, color2h);
                    }
                    else if (colorArray[i] == color2h || colorArray[j] == color2h)
                    {
                        originalBmp.SetPixel(i, j, color2h);
                    }
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
