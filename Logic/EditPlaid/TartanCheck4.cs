using SkiaSharp;

namespace ImageCreatePlaid
{
    public class TartanCheck4 : PlaidInterface
    {
        public SKBitmap EditImage(SKBitmap bmp, PlaidModel model)
        {
            int width = bmp.Width;
            int height = bmp.Height;
            int stripeSize1 = model.HorizontalSize1 / 5;
            int stripeSize2 = BussinessLogic.GetRandomInt(10) + 1;

            var randomColor = new SKColor(
                (byte)BussinessLogic.GetRandomInt(255),
                (byte)BussinessLogic.GetRandomInt(255),
                (byte)BussinessLogic.GetRandomInt(255),
                120);

            var randomColor2 = new SKColor(
                (byte)BussinessLogic.GetRandomInt(255),
                (byte)BussinessLogic.GetRandomInt(255),
                (byte)BussinessLogic.GetRandomInt(255),
                120);

            var randomColor3 = new SKColor(
                (byte)BussinessLogic.GetRandomInt(255),
                (byte)BussinessLogic.GetRandomInt(255),
                (byte)BussinessLogic.GetRandomInt(255),
                120);

            var offset = BussinessLogic.GetRandomInt(model.HorizontalSize1);
            var offset2 = BussinessLogic.GetRandomInt(model.HorizontalSize1);

            using var canvas = new SKCanvas(bmp);

            canvas.Clear(new SKColor(
                model.BaseColorRed,
                model.BaseColorGreen,
                model.BaseColorBlue,
                model.BaseAlpha));

            using var fillPaint = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                IsAntialias = false
            };

            for (int x = 0; x < width; x += stripeSize1 + stripeSize2 * 3)
            {
                fillPaint.Color = new SKColor(
                    model.HorizontalColorRed1,
                    model.HorizontalColorGreen1,
                    model.HorizontalColorBlue1,
                    180);

                canvas.DrawRect(x, 0, stripeSize1, height, fillPaint);

                fillPaint.Color = new SKColor(
                    model.HorizontalColorRed2,
                    model.HorizontalColorGreen2,
                    model.HorizontalColorBlue2,
                    150);

                canvas.DrawRect(x + stripeSize1, 0, stripeSize2, height, fillPaint);
            }

            for (int y = 0; y < height; y += stripeSize1 + stripeSize2 * 3)
            {
                fillPaint.Color = new SKColor(
                    model.VerticalColorRed1,
                    model.VerticalColorGreen1,
                    model.VerticalColorBlue1,
                    180);

                canvas.DrawRect(0, y, width, stripeSize1, fillPaint);

                fillPaint.Color = new SKColor(
                    model.HorizontalColorRed2,
                    model.HorizontalColorGreen2,
                    model.HorizontalColorBlue2,
                    150);

                canvas.DrawRect(0, y + stripeSize1, width, stripeSize2, fillPaint);
            }

            using var dashedPaint = new SKPaint
            {
                Color = randomColor3,
                Style = SKPaintStyle.Stroke,
                StrokeWidth = 3,
                IsAntialias = false,
                PathEffect = SKPathEffect.CreateDash(new float[] { 12, 6, 3, 6 }, 0)
            };

            for (int x = offset2; x < width + offset2; x += 50)
            {
                canvas.DrawLine(x, 0, x, height, dashedPaint);
            }

            for (int y = offset2; y < height + offset2; y += 50)
            {
                canvas.DrawLine(0, y, width, y, dashedPaint);
            }

            using var gridPaint = new SKPaint
            {
                Color = randomColor,
                Style = SKPaintStyle.Stroke,
                StrokeWidth = Math.Max(1, BussinessLogic.GetRandomInt(3)),
                IsAntialias = false
            };

            for (int x = offset; x < width + offset; x += model.HorizontalSize1)
            {
                canvas.DrawLine(x, 0, x, height, gridPaint);
            }

            for (int y = offset; y < height + offset; y += model.HorizontalSize1)
            {
                canvas.DrawLine(0, y, width, y, gridPaint);
            }

            using var dotPaint = new SKPaint
            {
                Color = randomColor2,
                Style = SKPaintStyle.Fill,
                IsAntialias = true
            };

            float dotRadiusX = stripeSize1 / 4f;
            float dotRadiusY = stripeSize1 / 4f;

            for (int y = offset; y <= height + offset; y += model.HorizontalSize1)
            {
                for (int x = offset; x <= width + offset; x += model.HorizontalSize1)
                {
                    canvas.DrawOval(
                        new SKRect(
                            x - dotRadiusX,
                            y - dotRadiusY,
                            x + dotRadiusX,
                            y + dotRadiusY),
                        dotPaint);
                }
            }

            return bmp;
        }
    }
}