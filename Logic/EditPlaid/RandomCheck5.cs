using SkiaSharp;

namespace ImageCreatePlaid
{
    public class RandomCheck5 : PlaidInterface
    {
        public SKBitmap EditImage(SKBitmap bmp, PlaidModel model)
        {
            int width = bmp.Width;
            int height = bmp.Height;
            SKBitmap originalBmp = BussinessLogic.NewCreateImage(model.VerticalSize1, model.HorizontalSize1);
            int colorFlg = BussinessLogic.GetRandomInt(3);
            SKColor color0 = new SKColor(model.BaseColorRed, model.BaseColorGreen, model.BaseColorBlue, model.BaseAlpha);
            SKColor color1v = new SKColor(model.VerticalColorRed1, model.VerticalColorGreen1, model.VerticalColorBlue1,model.Alpha);
            SKColor color2v = new SKColor(model.VerticalColorRed2, model.VerticalColorGreen2, model.VerticalColorBlue2, model.Alpha);
            SKColor color1h = new SKColor(model.HorizontalColorRed1, model.HorizontalColorGreen1, model.HorizontalColorBlue1, model.Alpha);
            SKColor color2h = new SKColor(model.HorizontalColorRed2, model.HorizontalColorGreen2, model.HorizontalColorBlue2, model.Alpha);
            List<SKColor> colorLine = new List<SKColor>();
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

            bmp = BussinessLogic.RepeatImage(width, height, originalBmp);

            return bmp;
        }
    }
}
