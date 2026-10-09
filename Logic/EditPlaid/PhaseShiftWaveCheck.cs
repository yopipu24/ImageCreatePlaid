using SkiaSharp;
using System;

namespace ImageCreatePlaid
{
    public class PhaseShiftWaveCheck : PlaidInterface
    {
        private static readonly Random _rnd = new Random();

        public SKBitmap EditImage(SKBitmap bmp, PlaidModel model)
        {
            int width = bmp.Width;
            int height = bmp.Height;

            SKColor color0Base = new SKColor(model.BaseColorRed, model.BaseColorGreen, model.BaseColorBlue, model.BaseAlpha);
            SKColor color1V = new SKColor(model.VerticalColorRed1, model.VerticalColorGreen1, model.VerticalColorBlue1, model.Alpha);
            SKColor color2H = new SKColor(model.HorizontalColorRed2, model.HorizontalColorGreen2, model.HorizontalColorBlue2, model.Alpha);
            SKColor colorCross = BussinessLogic.CalcColor2(color1V);

            using SKCanvas canvas = new SKCanvas(bmp);
            canvas.Clear(color0Base);

            int baseGridSize = model.VerticalSize1 > 0 ? Math.Clamp(model.VerticalSize1, 20, 80) : 40;

            // 波動のパラメータ
            float waveAmplitude = baseGridSize * 0.4f; // 振幅
            float waveFrequency = 0.03f;               // 周波数

            float centerX = width / 2f;
            float centerY = height / 2f;

            using var strokePaint = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                StrokeWidth = baseGridSize * 0.4f,
                IsAntialias = true,
                StrokeCap = SKStrokeCap.Round
            };

            // --- 縦波の描画 (color1V) ---
            strokePaint.Color = color1V;
            for (float x = 0; x < width + baseGridSize; x += baseGridSize * 2)
            {
                using var path = new SKPath();
                bool first = true;

                for (float y = 0; y <= height; y += 10)
                {
                    // 中心からの距離に応じた位相シフト
                    float dist = SKPoint.Distance(new SKPoint(x, y), new SKPoint(centerX, centerY));
                    float phase = dist * 0.015f;

                    float offsetX = (float)Math.Sin(y * waveFrequency + phase) * waveAmplitude;

                    if (first) { path.MoveTo(x + offsetX, y); first = false; }
                    else { path.LineTo(x + offsetX, y); }
                }
                canvas.DrawPath(path, strokePaint);
            }

            // --- 横波の描画 (color2H) ---
            strokePaint.Color = color2H;
            for (float y = 0; y < height + baseGridSize; y += baseGridSize * 2)
            {
                using var path = new SKPath();
                bool first = true;

                for (float x = 0; x <= width; x += 10)
                {
                    float dist = SKPoint.Distance(new SKPoint(x, y), new SKPoint(centerX, centerY));
                    float phase = dist * 0.015f;

                    float offsetY = (float)Math.Sin(x * waveFrequency + phase) * waveAmplitude;

                    if (first) { path.MoveTo(x, y + offsetY); first = false; }
                    else { path.LineTo(x, y + offsetY); }
                }
                canvas.DrawPath(path, strokePaint);
            }

            // --- 波の交差点 (colorCross) を幾何学的ノイズで強調 ---
            using var fillPaint = new SKPaint { Style = SKPaintStyle.Fill, Color = colorCross, IsAntialias = true };
            for (float x = 0; x < width + baseGridSize; x += baseGridSize * 2)
            {
                for (float y = 0; y < height + baseGridSize; y += baseGridSize * 2)
                {
                    float dist = SKPoint.Distance(new SKPoint(x, y), new SKPoint(centerX, centerY));
                    float phase = dist * 0.015f;
                    float offsetX = (float)Math.Sin(y * waveFrequency + phase) * waveAmplitude;
                    float offsetY = (float)Math.Sin(x * waveFrequency + phase) * waveAmplitude;

                    canvas.DrawCircle(x + offsetX, y + offsetY, baseGridSize * 0.25f, fillPaint);
                }
            }

            return bmp;
        }
    }
}