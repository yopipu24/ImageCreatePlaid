using SkiaSharp;
using System;
using static ImageCreatePlaid.Models.EnumModel;

namespace ImageCreatePlaid
{
    public class RandomFrame : FrameInterface
    {
        public SKBitmap EditImage(SKBitmap bmp, FrameModel model)
        {
            int width = bmp.Width;
            int height = bmp.Height;
            int frameSize = Math.Max(1, Math.Min(
                model.FrameSize,
                Math.Min(width, height) / 2));

            SKColor baseColor = new SKColor(
                model.BaseColorRed, model.BaseColorGreen,
                model.BaseColorBlue, model.BaseAlpha);

            SKColor color1 = new SKColor(
                model.ColorRed1, model.ColorGreen1, model.ColorBlue1, 255);

            SKColor color2 = new SKColor(
                model.ColorRed2, model.ColorGreen2, model.ColorBlue2, 255);

            // 毎回異なる柄にする
            var random = new Random();
            RandomFramePattern pattern = model.RandomFramePatternValue;

            using var canvas = new SKCanvas(bmp);
            canvas.Clear(baseColor);

            // 額縁の下地
            using (var backgroundPaint = new SKPaint { Color = color1 })
            {
                canvas.DrawRect(0, 0, width, frameSize, backgroundPaint);
                canvas.DrawRect(0, height - frameSize, width, frameSize, backgroundPaint);
                canvas.DrawRect(0, frameSize, frameSize, height - frameSize * 2, backgroundPaint);
                canvas.DrawRect(
                    width - frameSize, frameSize,
                    frameSize, height - frameSize * 2,
                    backgroundPaint);
            }

            // 模様のサイズ。frameSize が大きくなると柄も大きくなる
            float unit = Math.Max(8f, frameSize * 0.35f);

            DrawPattern(canvas, new SKRect(0, 0, width, frameSize),
                pattern, unit, color2, random);

            DrawPattern(canvas, new SKRect(0, height - frameSize, width, height),
                pattern, unit, color2, random);

            DrawPattern(canvas, new SKRect(0, frameSize, frameSize, height - frameSize),
                pattern, unit, color2, random);

            DrawPattern(canvas, new SKRect(width - frameSize, frameSize, width, height - frameSize),
                pattern, unit, color2, random);

            return bmp;
        }

        private static void DrawPattern(
            SKCanvas canvas,
            SKRect area,
            RandomFramePattern pattern,
            float unit,
            SKColor color,
            Random random)
        {
            canvas.Save();
            canvas.ClipRect(area);

            using var paint = new SKPaint
            {
                Color = color,
                IsAntialias = true,
                StrokeCap = SKStrokeCap.Round
            };

            switch (pattern)
            {
                case RandomFramePattern.PolkaDot:
                    DrawDots(canvas, area, unit, paint, random);
                    break;

                case RandomFramePattern.Stitch:
                    DrawStitches(canvas, area, unit, paint);
                    break;

                case RandomFramePattern.Confetti:
                    DrawConfetti(canvas, area, unit, paint, random);
                    break;

                case RandomFramePattern.ZigZag:
                    DrawZigZag(canvas, area, unit, paint);
                    break;

                case RandomFramePattern.DiagonalLine:
                    DrawDiagonalLines(canvas, area, unit, paint);
                    break;
            }

            canvas.Restore();
        }

        private static void DrawDots(
            SKCanvas canvas, SKRect area, float unit, SKPaint paint, Random random)
        {
            int count = Math.Max(4, (int)(area.Width * area.Height / (unit * unit * 2)));

            for (int i = 0; i < count; i++)
            {
                float x = area.Left + (float)random.NextDouble() * area.Width;
                float y = area.Top + (float)random.NextDouble() * area.Height;
                float radius = unit * (0.15f + (float)random.NextDouble() * 0.25f);

                canvas.DrawCircle(x, y, radius, paint);
            }
        }

        private static void DrawStitches(
            SKCanvas canvas, SKRect area, float unit, SKPaint paint)
        {
            paint.Style = SKPaintStyle.Stroke;
            paint.StrokeWidth = Math.Max(2f, unit * 0.12f);

            bool horizontal = area.Width >= area.Height;
            float center = horizontal ? area.MidY : area.MidX;

            if (horizontal)
            {
                for (float x = area.Left; x < area.Right; x += unit)
                {
                    canvas.DrawLine(x, center, x + unit * 0.55f, center, paint);
                }
            }
            else
            {
                for (float y = area.Top; y < area.Bottom; y += unit)
                {
                    canvas.DrawLine(center, y, center, y + unit * 0.55f, paint);
                }
            }
        }

        private static void DrawConfetti(
            SKCanvas canvas, SKRect area, float unit, SKPaint paint, Random random)
        {
            int count = Math.Max(6, (int)(area.Width * area.Height / (unit * unit)));

            for (int i = 0; i < count; i++)
            {
                float x = area.Left + (float)random.NextDouble() * area.Width;
                float y = area.Top + (float)random.NextDouble() * area.Height;
                float size = unit * (0.2f + (float)random.NextDouble() * 0.35f);

                canvas.Save();
                canvas.RotateDegrees(random.Next(0, 360), x, y);
                canvas.DrawRect(x - size, y - size / 3f, x + size, y + size / 3f, paint);
                canvas.Restore();
            }
        }

        private static void DrawZigZag(
            SKCanvas canvas, SKRect area, float unit, SKPaint paint)
        {
            paint.Style = SKPaintStyle.Stroke;
            paint.StrokeWidth = Math.Max(2f, unit * 0.1f);

            bool horizontal = area.Width >= area.Height;

            using var path = new SKPath();

            if (horizontal)
            {
                float y = area.MidY;
                path.MoveTo(area.Left, y);

                for (float x = area.Left; x <= area.Right + unit; x += unit)
                {
                    path.LineTo(x + unit / 2f, y - unit / 3f);
                    path.LineTo(x + unit, y + unit / 3f);
                }
            }
            else
            {
                float x = area.MidX;
                path.MoveTo(x, area.Top);

                for (float y = area.Top; y <= area.Bottom + unit; y += unit)
                {
                    path.LineTo(x - unit / 3f, y + unit / 2f);
                    path.LineTo(x + unit / 3f, y + unit);
                }
            }

            canvas.DrawPath(path, paint);
        }

        private static void DrawDiagonalLines(
            SKCanvas canvas, SKRect area, float unit, SKPaint paint)
        {
            paint.Style = SKPaintStyle.Stroke;
            paint.StrokeWidth = Math.Max(2f, unit * 0.12f);

            for (float x = area.Left - area.Height; x < area.Right; x += unit)
            {
                canvas.DrawLine(x, area.Bottom, x + area.Height, area.Top, paint);
            }
        }
    }
}