using SkiaSharp;

namespace ImageCreatePlaid
{
    public class Check2Line : PlaidInterface
    {
        public SKBitmap EditImage(SKBitmap bmp, PlaidModel model)
        {
            int width = bmp.Width;
            int height = bmp.Height;
            SKBitmap originalBmp = BussinessLogic.NewCreateImage(model.VerticalSize1, model.HorizontalSize1);
            int colorFlg = BussinessLogic.GetRandomInt(3);
            SKColor color0 = new SKColor(model.BaseColorRed, model.BaseColorGreen, model.BaseColorBlue, model.BaseAlpha);
            SKColor color1v = new SKColor(model.VerticalColorRed1, model.VerticalColorGreen1, model.VerticalColorBlue1, model.Alpha);
            SKColor color2v = new SKColor(model.VerticalColorRed2, model.VerticalColorGreen2, model.VerticalColorBlue2, model.Alpha);
            SKColor color1h = new SKColor(model.HorizontalColorRed1, model.HorizontalColorGreen1, model.HorizontalColorBlue1, model.Alpha);
            SKColor color2h = new SKColor(model.HorizontalColorRed2, model.HorizontalColorGreen2, model.HorizontalColorBlue2, model.Alpha);
            List<SKColor> colorLine = new List<SKColor>();
            int lineWidth = BussinessLogic.GetRandomInt(model.VerticalSize1 / 8) + model.VerticalSize1 / 10;
            int countFlg = BussinessLogic.GetRandomInt(model.VerticalSize1 / 3) + model.VerticalSize1 / 10;
            for (int i = 0; i < countFlg; i++) colorLine.Add(color0);
            countFlg += lineWidth;
            for (int i = colorLine.Count; i < countFlg && i < model.VerticalSize1; i++) colorLine.Add(color1v);
            countFlg += lineWidth / 2;
            for (int i = colorLine.Count; i < countFlg && i < model.VerticalSize1; i++) colorLine.Add(color0);
            countFlg += lineWidth;
            for (int i = colorLine.Count; i < countFlg && i < model.VerticalSize1; i++) colorLine.Add(color1v);
            for (int i = colorLine.Count; i < model.VerticalSize1; i++) colorLine.Add(color0);
            var colorArray = colorLine.ToArray();
            for (int i = 0; i < model.VerticalSize1; i++)
            {
                for (int j = 0; j < model.HorizontalSize1; j++)
                {
                    if (colorArray[i] == color1v && colorArray[j] == color1v)
                    {
                        originalBmp.SetPixel(i, j, BussinessLogic.CalcColor2(color1v));
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
                        
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    SKColor originalColor = originalBmp.GetPixel(x % model.VerticalSize1, y % model.HorizontalSize1);
                    bmp.SetPixel(x, y, originalColor);
                }
            }
            return bmp;
        }
    }
}
