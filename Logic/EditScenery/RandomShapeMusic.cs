using SkiaSharp;

namespace ImageCreatePlaid
{
    public class RandomShapeMusic : SceneryInterface
    {
        private static readonly string[] Symbols =
        {
            "♪",
            "♫",
            "♬",
            "♩",
            "♭",
            "♯"
        };

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

            int symbolCount = model.ShapeCount;
            int minFontSize = model.ShapeMinSize;
            int maxFontSize = model.HorizontalSize1;
            int margin = 8;
            int maxRetry = 100;

            List<SKRect> usedAreas = new List<SKRect>();

            using var typeface = SKTypeface.FromFamilyName("Segoe UI Symbol", SKFontStyle.Bold);

            using var paint = new SKPaint
            {
                IsAntialias = true,
                Style = SKPaintStyle.Fill
            };

            for (int i = 0; i < symbolCount; i++)
            {
                bool found = false;

                for (int retry = 0; retry < maxRetry; retry++)
                {
                    string symbol = Symbols[random.Next(Symbols.Length)];
                    int fontSize = random.Next(minFontSize, maxFontSize + 1);

                    using var font = new SKFont(typeface, fontSize);

                    font.MeasureText(symbol, out SKRect textBounds);

                    float textWidth = textBounds.Width;
                    float textHeight = textBounds.Height;

                    if (textWidth >= bmp.Width || textHeight >= bmp.Height)
                    {
                        continue;
                    }

                    float x = random.Next(0, Math.Max(1, bmp.Width - (int)textWidth));
                    float y = random.Next(0, Math.Max(1, bmp.Height - (int)textHeight));

                    var rect = new SKRect(x, y, x + textWidth, y + textHeight);
                    var hitArea = Inflate(rect, margin);

                    if (IsOverlapped(hitArea, usedAreas))
                    {
                        continue;
                    }

                    paint.Color = random.Next(2) == 0 ? color1 : color2;

                    using var textBlob = SKTextBlob.Create(
                        symbol,
                        font,
                        new SKPoint(x - textBounds.Left, y - textBounds.Top));

                    canvas.DrawText(textBlob, 0, 0, paint);

                    usedAreas.Add(hitArea);
                    found = true;
                    break;
                }

                if (!found)
                {
                    continue;
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
                rect.Bottom + margin);
        }

        private static bool IsOverlapped(SKRect target, List<SKRect> usedAreas)
        {
            foreach (SKRect area in usedAreas)
            {
                if (Intersects(target, area))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool Intersects(SKRect a, SKRect b)
        {
            return a.Left < b.Right
                && a.Right > b.Left
                && a.Top < b.Bottom
                && a.Bottom > b.Top;
        }
    }
}