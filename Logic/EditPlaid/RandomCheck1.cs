using SkiaSharp;

namespace ImageCreatePlaid
{
    public class RandomCheck1 : PlaidInterface
    {
        public SKBitmap EditImage(SKBitmap bmp, PlaidModel model)
        {
            int width = bmp.Width;
            int height = bmp.Height;
            SKBitmap originalBmp = BussinessLogic.NewCreateImage(model.VerticalSize1, model.HorizontalSize1);
            int colorFlg = BussinessLogic.GetRandomInt(3);
            int countFlg = BussinessLogic.GetRandomInt(model.VerticalSize1 / 3) + 5;
            SKColor color0 = new SKColor(model.BaseColorRed, model.BaseColorGreen, model.BaseColorBlue, model.BaseAlpha);
            SKColor color1v = new SKColor(model.VerticalColorRed1, model.VerticalColorGreen1, model.VerticalColorBlue1, model.Alpha);
            SKColor color2v = new SKColor(model.VerticalColorRed2, model.VerticalColorGreen2, model.VerticalColorBlue2, model.Alpha);
            SKColor color1h = new SKColor(model.HorizontalColorRed1, model.HorizontalColorGreen1, model.HorizontalColorBlue1, model.Alpha);
            SKColor color2h = new SKColor(model.HorizontalColorRed2, model.HorizontalColorGreen2, model.HorizontalColorBlue2, model.Alpha);
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
                    SKColor tempColor = originalBmp.GetPixel(i, j);
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


            bmp = BussinessLogic.RepeatImage(width, height, originalBmp);

            return bmp;
        }
    }
}
