using SkiaSharp;

namespace ImageCreatePlaid
{
    public enum PatternShape
    {
        Circle,
        Cross,
        Square,
        Triangle
    }

    public class RandomShape : SceneryInterface
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
                model.Alpha);

            var color2 = new SKColor(
                model.VerticalColorRed2,
                model.VerticalColorGreen2,
                model.VerticalColorBlue2,
                model.Alpha);

            canvas.Clear(baseColor);

            int cellWidth = model.HorizontalSize1;
            int cellHeight = model.VerticalSize1;

            Random random = new Random();

            using var fillPaint = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                IsAntialias = true
            };

            using var strokePaint = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                StrokeWidth = Math.Max(2, Math.Min(cellWidth, cellHeight) / 10),
                IsAntialias = true
            };

            for (int x = 0; x < bmp.Width; x += cellWidth)
            {
                for (int y = 0; y < bmp.Height; y += cellHeight)
                {
                    PatternShape shape = GetRandomShape(random);
                    SKColor color = random.Next(2) == 0 ? color1 : color2;

                    fillPaint.Color = color;
                    strokePaint.Color = color;

                    SKRect rect = CreateShapeRect(x, y, cellWidth, cellHeight);

                    DrawShape(canvas, shape, rect, fillPaint, strokePaint);
                }
            }

            return bmp;
        }

        private static PatternShape GetRandomShape(Random random)
        {
            PatternShape[] shapes =
            {
                PatternShape.Circle,
                PatternShape.Cross,
                PatternShape.Square,
                PatternShape.Triangle
            };

            return shapes[random.Next(shapes.Length)];
        }

        private static SKRect CreateShapeRect(int x, int y, int cellWidth, int cellHeight)
        {
            int size = Math.Min(cellWidth, cellHeight) / 2;
            int left = x + (cellWidth - size) / 2;
            int top = y + (cellHeight - size) / 2;

            return new SKRect(left, top, left + size, top + size);
        }

        private static void DrawShape(
            SKCanvas canvas,
            PatternShape shape,
            SKRect rect,
            SKPaint fillPaint,
            SKPaint strokePaint)
        {
            switch (shape)
            {
                case PatternShape.Circle:
                    canvas.DrawOval(rect, fillPaint);
                    break;

                case PatternShape.Square:
                    canvas.DrawRect(rect, fillPaint);
                    break;

                case PatternShape.Triangle:
                    var points = new[]
                    {
                        new SKPoint(rect.Left + rect.Width / 2, rect.Top),
                        new SKPoint(rect.Left, rect.Bottom),
                        new SKPoint(rect.Right, rect.Bottom)
                    };

                    using (var vertices = SKVertices.CreateCopy(
                        SKVertexMode.Triangles,
                        points,
                        null,
                        null,
                        null))
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