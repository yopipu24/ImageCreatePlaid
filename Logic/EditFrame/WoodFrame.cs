using System;
using SkiaSharp;

namespace ImageCreatePlaid
{
    public class WoodFrame : FrameInterface
    {
        public SKBitmap EditImage(SKBitmap bmp, FrameModel model)
        {
            int width = bmp.Width;
            int height = bmp.Height;

            // 内側の領域を確保するため、短辺の半分までに制限
            int frameSize = Math.Max(1, Math.Min(
                model.FrameSize,
                Math.Min(width, height) / 2));

            SKColor baseColor = new SKColor(
                model.BaseColorRed,
                model.BaseColorGreen,
                model.BaseColorBlue,
                model.BaseAlpha);

            SKColor color1 = new SKColor(
                model.ColorRed1,
                model.ColorGreen1,
                model.ColorBlue1,
                255);

            SKColor color2 = new SKColor(
                model.ColorRed2,
                model.ColorGreen2,
                model.ColorBlue2,
                255);

            // 同じ画像サイズなら同じ木目になるシード
            var random = new Random();
            //var random = new Random(width * 397 ^ height * 31 ^ frameSize);

            using var canvas = new SKCanvas(bmp);
            canvas.Clear(baseColor);

            // 額縁の基本色
            using (var basePaint = new SKPaint
            {
                Color = color1,
                IsAntialias = true
            })
            {
                canvas.DrawRect(0, 0, width, frameSize, basePaint);                         // 上
                canvas.DrawRect(0, height - frameSize, width, frameSize, basePaint);       // 下
                canvas.DrawRect(0, frameSize, frameSize, height - frameSize * 2, basePaint); // 左
                canvas.DrawRect(width - frameSize, frameSize, frameSize,
                    height - frameSize * 2, basePaint);                                    // 右
            }

            // 上下は横方向、左右は縦方向の木目
            DrawHorizontalWoodGrain(
                canvas, new SKRect(0, 0, width, frameSize),
                color1, color2, random);

            DrawHorizontalWoodGrain(
                canvas, new SKRect(0, height - frameSize, width, height),
                color1, color2, random);

            DrawVerticalWoodGrain(
                canvas, new SKRect(0, frameSize, frameSize, height - frameSize),
                color1, color2, random);

            DrawVerticalWoodGrain(
                canvas, new SKRect(width - frameSize, frameSize, width, height - frameSize),
                color1, color2, random);

            return bmp;
        }

        private static void DrawHorizontalWoodGrain(
            SKCanvas canvas,
            SKRect area,
            SKColor color1,
            SKColor color2,
            Random random)
        {
            canvas.Save();
            canvas.ClipRect(area);
            const int lineCount = 8; // frameSize が増えても本数は固定

            for (int i = 0; i < lineCount; i++)
            {
                float startY = area.Top + (i + 0.5f) * area.Height / lineCount;

                // 枠が太いほど木目の線も太くする
                float strokeWidth = Math.Max(
                    1f,
                    area.Height / lineCount * (0.35f + (float)random.NextDouble() * 0.45f));

                using var paint = new SKPaint
                {
                    Color = MixColor(
                        color1,
                        color2,
                        0.25f + (float)random.NextDouble() * 0.65f),
                    StrokeWidth = strokeWidth,
                    Style = SKPaintStyle.Stroke,
                    IsAntialias = true
                };

                using var path = new SKPath();
                path.MoveTo(area.Left, startY);

                float wave1 = 2f + (float)random.NextDouble() * 8f;
                float wave2 = 1f + (float)random.NextDouble() * 4f;
                float phase = (float)random.NextDouble() * MathF.PI * 2;

                for (float x = area.Left; x <= area.Right; x += 8f)
                {
                    float y = startY
                              + MathF.Sin(x * 0.025f + phase) * wave1
                              + MathF.Sin(x * 0.07f + phase) * wave2;

                    path.LineTo(x, y);
                }

                canvas.DrawPath(path, paint);
            }

            DrawKnots(canvas, area, color2, random, horizontal: true);
            canvas.Restore();
        }

        private static void DrawVerticalWoodGrain(
            SKCanvas canvas,
            SKRect area,
            SKColor color1,
            SKColor color2,
            Random random)
        {
            canvas.Save();
            canvas.ClipRect(area);
            const int lineCount = 8;

            for (int i = 0; i < lineCount; i++)
            {
                float startX = area.Left + (i + 0.5f) * area.Width / lineCount;

                float strokeWidth = Math.Max(
                    1f,
                    area.Width / lineCount * (0.35f + (float)random.NextDouble() * 0.45f));

                using var paint = new SKPaint
                {
                    Color = MixColor(color1, color2, 0.25f + (float)random.NextDouble() * 0.65f),
                    StrokeWidth = strokeWidth,
                    Style = SKPaintStyle.Stroke,
                    IsAntialias = true
                };

                using var path = new SKPath();
                path.MoveTo(startX, area.Top);

                float wave1 = 2f + (float)random.NextDouble() * 8f;
                float wave2 = 1f + (float)random.NextDouble() * 4f;
                float phase = (float)random.NextDouble() * MathF.PI * 2;

                for (float y = area.Top; y <= area.Bottom; y += 8f)
                {
                    float x = startX
                              + MathF.Sin(y * 0.025f + phase) * wave1
                              + MathF.Sin(y * 0.07f + phase) * wave2;

                    path.LineTo(x, y);
                }

                canvas.DrawPath(path, paint);
            }

            DrawKnots(canvas, area, color2, random, horizontal: false);
            canvas.Restore();
        }

        private static void DrawKnots(
            SKCanvas canvas,
            SKRect area,
            SKColor darkColor,
            Random random,
            bool horizontal)
        {
            int knotCount = Math.Max(1, (int)(horizontal ? area.Width : area.Height) / 250);

            using var paint = new SKPaint
            {
                Color = darkColor.WithAlpha(100),
                Style = SKPaintStyle.Stroke,
                StrokeWidth = 2f,
                IsAntialias = true
            };

            for (int i = 0; i < knotCount; i++)
            {
                float centerX = area.Left + (float)random.NextDouble() * area.Width;
                float centerY = area.Top + (float)random.NextDouble() * area.Height;

                float longRadius = 8f + (float)random.NextDouble() * 16f;
                float shortRadius = 3f + (float)random.NextDouble() * 7f;

                var rect = horizontal
                    ? new SKRect(centerX - longRadius, centerY - shortRadius,
                                 centerX + longRadius, centerY + shortRadius)
                    : new SKRect(centerX - shortRadius, centerY - longRadius,
                                 centerX + shortRadius, centerY + longRadius);

                canvas.DrawOval(rect, paint);
            }
        }

        private static SKColor MixColor(SKColor from, SKColor to, float amount)
        {
            amount = Math.Clamp(amount, 0f, 1f);

            return new SKColor(
                (byte)(from.Red + (to.Red - from.Red) * amount),
                (byte)(from.Green + (to.Green - from.Green) * amount),
                (byte)(from.Blue + (to.Blue - from.Blue) * amount),
                255);
        }
    }
}