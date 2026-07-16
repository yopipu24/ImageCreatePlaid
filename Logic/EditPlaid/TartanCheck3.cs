using SkiaSharp;

namespace ImageCreatePlaid
{
    public class TartanCheck3 : PlaidInterface
    {
        public SKBitmap EditImage(SKBitmap bmp, PlaidModel model)
        {
            int width = bmp.Width;
            int height = bmp.Height;
            int stripeSize1 = 60;
            int stripeSize2 = 20;

            var randomColor = new SKColor(
                (byte)BussinessLogic.GetRandomInt(255),
                (byte)BussinessLogic.GetRandomInt(255),
                (byte)BussinessLogic.GetRandomInt(255),
                120);

            using var canvas = new SKCanvas(bmp);

            canvas.Clear(new SKColor(
                model.BaseColorRed,
                model.BaseColorGreen,
                model.BaseColorBlue,
                model.BaseAlpha));

            using var paint = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                IsAntialias = false
            };

            for (int x = 0; x < width; x += stripeSize1 + stripeSize2 * 2)
            {
                paint.Color = new SKColor(
                    model.HorizontalColorRed1,
                    model.HorizontalColorGreen1,
                    model.HorizontalColorBlue1,
                    180);

                canvas.DrawRect(x, 0, stripeSize1, height, paint);

                paint.Color = new SKColor(
                    model.HorizontalColorRed2,
                    model.HorizontalColorGreen2,
                    model.HorizontalColorBlue2,
                    150);

                canvas.DrawRect(x + stripeSize1, 0, stripeSize2, height, paint);

                paint.Color = randomColor;

                canvas.DrawRect(x + stripeSize1 + stripeSize2, 0, stripeSize2, height, paint);
            }

            for (int y = 0; y < height; y += stripeSize1 + stripeSize2 * 2)
            {
                paint.Color = new SKColor(
                    model.VerticalColorRed1,
                    model.VerticalColorGreen1,
                    model.VerticalColorBlue1,
                    180);

                canvas.DrawRect(0, y, width, stripeSize1, paint);

                paint.Color = new SKColor(
                    model.HorizontalColorRed2,
                    model.HorizontalColorGreen2,
                    model.HorizontalColorBlue2,
                    150);

                canvas.DrawRect(0, y + stripeSize1, width, stripeSize2, paint);

                paint.Color = randomColor;

                canvas.DrawRect(0, y + stripeSize1 + stripeSize2, width, stripeSize2, paint);
            }

            return bmp;
        }
    }
}