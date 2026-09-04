using SkiaSharp;

namespace ImageCreatePlaid
{
    public class RandomSnowCrystal : SceneryInterface
    {
        public SKBitmap EditImage(SKBitmap bmp, SceneryModel model)
        {
            using SKCanvas canvas = new SKCanvas(bmp);

            SKColor baseColor = new SKColor(
                (byte)model.BaseColorRed,
                (byte)model.BaseColorGreen,
                (byte)model.BaseColorBlue,
                (byte)model.BaseAlpha
            );

            SKColor crystalColor1 = new SKColor(
                (byte)model.VerticalColorRed1,
                (byte)model.VerticalColorGreen1,
                (byte)model.VerticalColorBlue1,
                (byte)model.Alpha
            );

            SKColor crystalColor2 = new SKColor(
                (byte)model.VerticalColorRed2,
                (byte)model.VerticalColorGreen2,
                (byte)model.VerticalColorBlue2,
                (byte)model.Alpha
            );

            canvas.Clear(baseColor);

            Random random = new Random();

            int crystalCount = model.CrystalCount;
            int minSize = model.SnowMinSize;
            int maxSize = model.SnowMaxSize;
            float margin = model.SnowMargin;
            int maxRetry = 100;

            List<SKRect> usedAreas = new List<SKRect>();

            for (int i = 0; i < crystalCount; i++)
            {
                for (int retry = 0; retry < maxRetry; retry++)
                {
                    int size = random.Next(minSize, maxSize + 1);
                    float radius = size / 2f;

                    float x = random.Next((int)radius, Math.Max((int)radius + 1, bmp.Width - (int)radius));
                    float y = random.Next((int)radius, Math.Max((int)radius + 1, bmp.Height - (int)radius));

                    SKRect area = new SKRect(
                        x - radius,
                        y - radius,
                        x + radius,
                        y + radius
                    );

                    SKRect hitArea = Inflate(area, margin);

                    if (IsOverlapped(hitArea, usedAreas))
                    {
                        continue;
                    }

                    SKColor color = random.Next(2) == 0 ? crystalColor1 : crystalColor2;
                    byte alpha = (byte)random.Next(120, color.Alpha + 1);

                    SKColor drawColor = color.WithAlpha(alpha);

                    using SKPaint paint = new SKPaint
                    {
                        Color = drawColor,
                        IsAntialias = true,
                        StrokeWidth = Math.Max(1.2f, size / 18f),
                        Style = SKPaintStyle.Stroke,
                        StrokeCap = SKStrokeCap.Round
                    };

                    DrawSnowCrystal(canvas, x, y, radius, paint, random);

                    usedAreas.Add(hitArea);
                    break;
                }
            }

            return bmp;
        }

        private static SKRect Inflate(SKRect rect, float margin)
        {
            return new SKRect(
                rect.Left - margin,
                rect.Top - margin,
                rect.Right + margin,
                rect.Bottom + margin
            );
        }

        private static bool IsOverlapped(SKRect target, List<SKRect> usedAreas)
        {
            foreach (SKRect area in usedAreas)
            {
                if (target.IntersectsWith(area))
                {
                    return true;
                }
            }

            return false;
        }

        private static void DrawSnowCrystal(
            SKCanvas canvas,
            float centerX,
            float centerY,
            float radius,
            SKPaint paint,
            Random random)
        {
            int branchCount = 6;

            // 結晶ごとに全体の向きをランダムに回転させる
            double baseRotation = random.NextDouble() * Math.PI * 2;

            for (int i = 0; i < branchCount; i++)
            {
                double angle = baseRotation + Math.PI * 2 / branchCount * i;

                DrawBranch(canvas, centerX, centerY, radius, angle, paint, random);
            }

            using SKPaint centerPaint = new SKPaint
            {
                Color = paint.Color,
                IsAntialias = true,
                Style = SKPaintStyle.Fill
            };

            float centerSize = Math.Max(2f, radius / 8f);
            canvas.DrawCircle(centerX, centerY, centerSize, centerPaint);
        }

        private static void DrawBranch(
            SKCanvas canvas,
            float centerX,
            float centerY,
            float radius,
            double angle,
            SKPaint paint,
            Random random)
        {
            float endX = centerX + (float)Math.Cos(angle) * radius;
            float endY = centerY + (float)Math.Sin(angle) * radius;

            canvas.DrawLine(centerX, centerY, endX, endY, paint);

            int twigCount = random.Next(2, 4);

            for (int i = 1; i <= twigCount; i++)
            {
                float position = radius * (0.35f + i * 0.18f);
                float twigLength = radius * random.Next(22, 36) / 100f;

                float baseX = centerX + (float)Math.Cos(angle) * position;
                float baseY = centerY + (float)Math.Sin(angle) * position;

                DrawTwig(canvas, baseX, baseY, twigLength, angle + Math.PI / 4, paint);
                DrawTwig(canvas, baseX, baseY, twigLength, angle - Math.PI / 4, paint);
            }
        }

        private static void DrawTwig(
            SKCanvas canvas,
            float startX,
            float startY,
            float length,
            double angle,
            SKPaint paint)
        {
            float endX = startX + (float)Math.Cos(angle) * length;
            float endY = startY + (float)Math.Sin(angle) * length;

            canvas.DrawLine(startX, startY, endX, endY, paint);
        }
    }
}