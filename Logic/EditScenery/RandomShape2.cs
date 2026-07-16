using SkiaSharp;

namespace ImageCreatePlaid
{
    public class RandomShape2 : SceneryInterface
    {
        public SKBitmap EditImage(SKBitmap bmp, SceneryModel model)
        {
            using var canvas = new SKCanvas(bmp);

            var baseColor = new SKColor(
                model.BaseColorRed,
                model.BaseColorGreen,
                model.BaseColorBlue,
                model.BaseAlpha);

            var color1 = new SKColor(
                model.VerticalColorRed1,
                model.VerticalColorGreen1,
                model.VerticalColorBlue1,
                model.BaseAlpha);

            var color2 = new SKColor(
                model.VerticalColorRed2,
                model.VerticalColorGreen2,
                model.VerticalColorBlue2,
                model.BaseAlpha);

            canvas.Clear(baseColor);

            Random random = new Random();

            int shapeCount = model.ShapeCount;
            int minSize = model.ShapeMinSize;
            int maxSize = model.HorizontalSize1;
            int margin = 5;
            int maxRetry = 200;

            List<SKRectI> usedAreas = new List<SKRectI>();

            for (int i = 0; i < shapeCount; i++)
            {
                bool found = false;

                for (int retry = 0; retry < maxRetry; retry++)
                {
                    int size = random.Next(minSize, maxSize + 1);

                    int x = random.Next(0, bmp.Width - size);
                    int y = random.Next(0, bmp.Height - size);

                    var rect = new SKRectI(x, y, x + size, y + size);
                    var hitArea = Inflate(rect, margin);

                    if (!IsOverlapped(hitArea, usedAreas))
                    {
                        SKColor color = random.Next(2) == 0 ? color1 : color2;
                        DrawRandomShape(canvas, random, rect, color);

                        usedAreas.Add(hitArea);
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    continue;
                }
            }

            return bmp;
        }

        private static SKRectI Inflate(SKRectI rect, int margin)
        {
            return new SKRectI(
                rect.Left - margin,
                rect.Top - margin,
                rect.Right + margin,
                rect.Bottom + margin);
        }

        private static bool IsOverlapped(SKRectI target, List<SKRectI> usedAreas)
        {
            foreach (SKRectI area in usedAreas)
            {
                if (Intersects(target, area))
                {
                    return true;
                }
            }

            return false;
        }
        private static bool Intersects(SKRectI a, SKRectI b)
        {
            return a.Left < b.Right
                && a.Right > b.Left
                && a.Top < b.Bottom
                && a.Bottom > b.Top;
        }

        private static void DrawRandomShape(
            SKCanvas canvas,
            Random random,
            SKRectI rect,
            SKColor color)
        {
            PatternShape shape = (PatternShape)random.Next(0, 4);

            using var fillPaint = new SKPaint
            {
                Color = color,
                Style = SKPaintStyle.Fill,
                IsAntialias = true
            };

            using var strokePaint = new SKPaint
            {
                Color = color,
                Style = SKPaintStyle.Stroke,
                StrokeWidth = Math.Max(2, rect.Width / 8),
                IsAntialias = true
            };

            var rectF = new SKRect(rect.Left, rect.Top, rect.Right, rect.Bottom);

            switch (shape)
            {
                case PatternShape.Circle:
                    canvas.DrawOval(rectF, fillPaint);
                    break;

                case PatternShape.Square:
                    canvas.DrawRect(rectF, fillPaint);
                    break;

                case PatternShape.Triangle:
                    var points = new[]
                    {
                        new SKPoint(rect.Left + rect.Width / 2f, rect.Top),
                        new SKPoint(rect.Left, rect.Bottom),
                        new SKPoint(rect.Right, rect.Bottom)
                    };

                    using (var vertices = SKVertices.CreateCopy(
                        vmode: SKVertexMode.Triangles,
                        positions: points,
                        texs: null,
                        colors: null,
                        indices: null))
                    {
                        canvas.DrawVertices(vertices, SKBlendMode.SrcOver, fillPaint);
                    }

                    break;

                case PatternShape.Cross:
                    canvas.DrawLine(rect.Left, rect.Top, rect.Right, rect.Bottom, strokePaint);
                    canvas.DrawLine(rect.Right, rect.Top, rect.Left, rect.Bottom, strokePaint);
                    break;
            }
        }
    }
}