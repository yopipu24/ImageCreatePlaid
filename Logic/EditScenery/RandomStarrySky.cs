using SkiaSharp;

namespace ImageCreatePlaid
{
    public class RandomStarrySky : SceneryInterface
    {
        public SKBitmap EditImage(SKBitmap bmp, SceneryModel model)
        {
            using var canvas = new SKCanvas(bmp);

            var baseColor = new SKColor(
                model.BaseColorRed,
                model.BaseColorGreen,
                model.BaseColorBlue,
                model.BaseAlpha);

            var starColor1 = new SKColor(
                model.VerticalColorRed1,
                model.VerticalColorGreen1,
                model.VerticalColorBlue1,
                model.BaseAlpha);

            var starColor2 = new SKColor(
                model.VerticalColorRed2,
                model.VerticalColorGreen2,
                model.VerticalColorBlue2,
                model.BaseAlpha);

            canvas.Clear(baseColor);

            Random random = new Random();

            int smallStarCount = model.SmallStarCount;
            int largeStarCount = model.LargeStarCount;
            int sparkleCount = model.SparkleCount;

            DrawSmallStars(canvas, bmp.Width, bmp.Height, random, smallStarCount, starColor1, starColor2);
            DrawLargeStars(canvas, bmp.Width, bmp.Height, random, largeStarCount, starColor1, starColor2);
            DrawSparkles(canvas, bmp.Width, bmp.Height, random, sparkleCount, starColor1, starColor2);

            return bmp;
        }

        private static void DrawSmallStars(
            SKCanvas canvas,
            int width,
            int height,
            Random random,
            int count,
            SKColor color1,
            SKColor color2)
        {
            using var paint = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                IsAntialias = true
            };

            for (int i = 0; i < count; i++)
            {
                int size = random.Next(1, 4);
                int x = random.Next(0, width);
                int y = random.Next(0, height);

                SKColor color = random.Next(2) == 0 ? color1 : color2;

                byte alpha = (byte)random.Next(90, color.Alpha + 1);
                paint.Color = color.WithAlpha(alpha);

                canvas.DrawOval(x, y, size, size, paint);
            }
        }

        private static void DrawLargeStars(
            SKCanvas canvas,
            int width,
            int height,
            Random random,
            int count,
            SKColor color1,
            SKColor color2)
        {
            using var paint = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                IsAntialias = true
            };

            for (int i = 0; i < count; i++)
            {
                int size = random.Next(4, 9);
                int x = random.Next(0, Math.Max(1, width - size));
                int y = random.Next(0, Math.Max(1, height - size));

                SKColor color = random.Next(2) == 0 ? color1 : color2;

                byte alpha = (byte)random.Next(140, color.Alpha + 1);
                paint.Color = color.WithAlpha(alpha);

                canvas.DrawOval(x, y, size, size, paint);
            }
        }

        private static void DrawSparkles(
            SKCanvas canvas,
            int width,
            int height,
            Random random,
            int count,
            SKColor color1,
            SKColor color2)
        {
            using var paint = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                StrokeWidth = 1,
                IsAntialias = true
            };

            for (int i = 0; i < count; i++)
            {
                int size = random.Next(8, 18);
                int half = size / 2;

                int x = random.Next(half, Math.Max(half + 1, width - half));
                int y = random.Next(half, Math.Max(half + 1, height - half));

                SKColor color = random.Next(2) == 0 ? color1 : color2;

                byte alpha = (byte)random.Next(120, color.Alpha + 1);
                paint.Color = color.WithAlpha(alpha);

                canvas.DrawLine(x - half, y, x + half, y, paint);
                canvas.DrawLine(x, y - half, x, y + half, paint);

                if (random.Next(2) == 0)
                {
                    int diagonal = half / 2;
                    canvas.DrawLine(x - diagonal, y - diagonal, x + diagonal, y + diagonal, paint);
                    canvas.DrawLine(x + diagonal, y - diagonal, x - diagonal, y + diagonal, paint);
                }
            }
        }
    }
}