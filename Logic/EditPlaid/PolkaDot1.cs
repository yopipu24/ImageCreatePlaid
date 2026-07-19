using SkiaSharp;

namespace ImageCreatePlaid
{
    public class PolkaDot1 : PlaidInterface
    {
        public SKBitmap EditImage(SKBitmap bmp, PlaidModel model)
        {
            int width = bmp.Width;
            int height = bmp.Height;

            var color1 = new SKColor(model.VerticalColorRed1, model.VerticalColorGreen1, model.VerticalColorBlue1, model.Alpha);
            var color2 = new SKColor(model.HorizontalColorRed2, model.HorizontalColorGreen2, model.HorizontalColorBlue2, model.Alpha);

            using var canvas = new SKCanvas(bmp);
            canvas.Clear(new SKColor(model.BaseColorRed, model.BaseColorGreen, model.BaseColorBlue, model.BaseAlpha));

            using var paint = new SKPaint
            {
                IsAntialias = true,
                Style = SKPaintStyle.Fill
            };

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
                            int heightRandomBlank =
                                BussinessLogic.GetRandomInt(model.HorizontalSize1) - model.HorizontalSize1 / 2;

                            paint.Color = colorFlg ? color1 : color2;

                            canvas.DrawOval(
                                new SKRect(
                                    i * model.HorizontalSize1 + widthBlank,
                                    j * model.VerticalSize1 + heightBlank + heightRandomBlank,
                                    i * model.HorizontalSize1 + widthBlank + model.HorizontalSize1,
                                    j * model.VerticalSize1 + heightBlank + heightRandomBlank + model.VerticalSize1),
                                paint);

                            colorFlg = !colorFlg;

                            i += widthBlank / model.HorizontalSize1 * 2 + 1;
                        }
                    }

                    j += heightBlank / model.VerticalSize1 * 2 + 1;
                }
            }

            return bmp;
        }
    }
}