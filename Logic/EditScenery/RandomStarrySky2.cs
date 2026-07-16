using SkiaSharp;

namespace ImageCreatePlaid
{
    public class RandomStarrySky2 : SceneryInterface
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

            int bigStarCount = model.LargeStarCount;
            int smallStarCount = model.SmallStarCount;
            int sparkleCount = model.SparkleCount;

            DrawAnimeStars(canvas, bmp.Width, bmp.Height, random, bigStarCount, 24, 46, starColor1, starColor2);
            DrawAnimeStars(canvas, bmp.Width, bmp.Height, random, smallStarCount, 10, 22, starColor1, starColor2);
            DrawSparkles(canvas, bmp.Width, bmp.Height, random, sparkleCount, starColor1, starColor2);

            return bmp;
        }

        private static void DrawAnimeStars(
            SKCanvas canvas,
            int width,
            int height,
            Random random,
            int count,
            int minSize,
            int maxSize,
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
                int size = random.Next(minSize, maxSize + 1);
                int radius = size / 2;

                int x = random.Next(radius, Math.Max(radius + 1, width - radius));
                int y = random.Next(radius, Math.Max(radius + 1, height - radius));

                paint.Color = random.Next(2) == 0 ? color1 : color2;
                float rotation = random.Next(0, 360);

                SKPoint[] star = CreateStarPoints(
                    x,
                    y,
                    radius,
                    radius * 0.45f,
                    5,
                    rotation);

                DrawFilledStar(canvas, star, paint);

                if (size >= 24)
                {
                    DrawHighlight(canvas, x, y, radius);
                }
            }
        }

        private static SKPoint[] CreateStarPoints(
            float centerX,
            float centerY,
            float outerRadius,
            float innerRadius,
            int points,
            float rotationDegrees)
        {
            SKPoint[] result = new SKPoint[points * 2];

            double rotation = Math.PI / 180 * rotationDegrees;
            double step = Math.PI / points;

            for (int i = 0; i < result.Length; i++)
            {
                double angle = rotation - Math.PI / 2 + step * i;
                float radius = i % 2 == 0 ? outerRadius : innerRadius;

                result[i] = new SKPoint(
                    centerX + (float)Math.Cos(angle) * radius,
                    centerY + (float)Math.Sin(angle) * radius);
            }

            return result;
        }

        private static void DrawFilledStar(SKCanvas canvas, SKPoint[] star, SKPaint paint)
        {
            float centerX = 0;
            float centerY = 0;

            foreach (SKPoint point in star)
            {
                centerX += point.X;
                centerY += point.Y;
            }

            var center = new SKPoint(centerX / star.Length, centerY / star.Length);

            for (int i = 0; i < star.Length; i++)
            {
                SKPoint p1 = star[i];
                SKPoint p2 = star[(i + 1) % star.Length];

                var points = new[]
                {
                    center,
                    p1,
                    p2
                };

                using var vertices = SKVertices.CreateCopy(
                    vmode: SKVertexMode.Triangles,
                    positions: points,
                    texs: null,
                    colors: null,
                    indices: null);

                canvas.DrawVertices(vertices, SKBlendMode.SrcOver, paint);
            }
        }

        private static void DrawHighlight(SKCanvas canvas, int x, int y, int radius)
        {
            int highlightSize = Math.Max(3, radius / 4);

            using var paint = new SKPaint
            {
                Color = new SKColor(255, 255, 255, 180),
                Style = SKPaintStyle.Fill,
                IsAntialias = true
            };

            canvas.DrawOval(
                x - radius / 4,
                y - radius / 3,
                highlightSize,
                highlightSize,
                paint);
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
                StrokeWidth = 2,
                IsAntialias = true
            };

            for (int i = 0; i < count; i++)
            {
                int size = random.Next(8, 18);
                int half = size / 2;

                int x = random.Next(half, Math.Max(half + 1, width - half));
                int y = random.Next(half, Math.Max(half + 1, height - half));

                paint.Color = random.Next(2) == 0 ? color1 : color2;

                canvas.DrawLine(x - half, y, x + half, y, paint);
                canvas.DrawLine(x, y - half, x, y + half, paint);
            }
        }
    }
}