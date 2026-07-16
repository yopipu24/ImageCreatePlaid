using SkiaSharp;

namespace ImageCreatePlaid
{
    public class TartanCheck5 : PlaidInterface
    {
        public SKBitmap EditImage(SKBitmap bmp, PlaidModel model)
        {
            int width = bmp.Width;
            int height = bmp.Height;
            int stripeSize1 = model.HorizontalSize1 / 2;
            int stripeSize2 = BussinessLogic.GetRandomInt(model.HorizontalSize1 / 3) + 10;
            int stripeSize3 = BussinessLogic.GetRandomInt(model.HorizontalSize1 / 4) + 15;

            var hColor1 = new SKColor(
                model.HorizontalColorRed1,
                model.HorizontalColorGreen1,
                model.HorizontalColorBlue1,
                180);

            var hColor2 = new SKColor(
                model.HorizontalColorRed2,
                model.HorizontalColorGreen2,
                model.HorizontalColorBlue2,
                180);

            using var canvas = new SKCanvas(bmp);

            canvas.Clear(new SKColor(
                model.BaseColorRed,
                model.BaseColorGreen,
                model.BaseColorBlue,
                model.BaseAlpha));

            DrawTartan(canvas, width, height, stripeSize1, stripeSize2, hColor1, hColor2);
            DrawDiagonalMesh(canvas, width, height, stripeSize3);

            return bmp;
        }

        private static void DrawTartan(
            SKCanvas canvas,
            int width,
            int height,
            int s1,
            int s2,
            SKColor c1,
            SKColor c2)
        {
            var baseSize = BussinessLogic.GetRandomInt(s1);

            using var paint = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                IsAntialias = false
            };

            for (int x = 0; x < width; x += s1 + s2 + baseSize)
            {
                paint.Color = c1;
                canvas.DrawRect(x, 0, s1, height, paint);

                paint.Color = c2;
                canvas.DrawRect(x + s1, 0, s2, height, paint);
            }

            for (int y = 0; y < height; y += s1 + s2 + baseSize)
            {
                paint.Color = c1;
                canvas.DrawRect(0, y, width, s1, paint);

                paint.Color = c2;
                canvas.DrawRect(0, y + s1, width, s2, paint);
            }
        }

        private static void DrawDiagonalMesh(SKCanvas canvas, int width, int height, int spacing)
        {
            var randomColor = new SKColor(
                (byte)BussinessLogic.GetRandomInt(255),
                (byte)BussinessLogic.GetRandomInt(255),
                (byte)BussinessLogic.GetRandomInt(255),
                120);

            using var meshPaint = new SKPaint
            {
                Color = randomColor,
                Style = SKPaintStyle.Stroke,
                StrokeWidth = Math.Max(1, BussinessLogic.GetRandomInt(3)),
                IsAntialias = false,
                PathEffect = SKPathEffect.CreateDash(new float[] { 2, 6 }, 0)
            };

            for (int i = -1 * width; i < width; i += spacing)
            {
                canvas.DrawLine(i, 0, i + width, height, meshPaint);
            }

            for (int i = 0; i < width * 2; i += spacing)
            {
                canvas.DrawLine(i, 0, i - width, height, meshPaint);
            }
        }
    }
}