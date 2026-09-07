using System;
using SkiaSharp;
using ImageCreatePlaid.Models;

namespace ImageCreatePlaid
{
    public class GeometryFrame : FrameInterface
    {
        public SKBitmap EditImage(SKBitmap bmp, FrameModel model)
        {
            int width = bmp.Width;
            int height = bmp.Height;

            int frameSize = Math.Max(
                1,
                Math.Min(model.FrameSize, Math.Min(width, height) / 2));

            int tileSize = Math.Max(8, frameSize / 2);

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

            //var random = new Random();
            //EnumModel.FramePatternType pattern = (EnumModel.FramePatternType)random.Next(4);
            EnumModel.FramePatternType pattern = model.FramePatternType;

            using var canvas = new SKCanvas(bmp);
            canvas.Clear(baseColor);

            // 額縁のベース
            using (var basePaint = new SKPaint
            {
                Color = color1,
                IsAntialias = true
            })
            {
                canvas.DrawRect(0, 0, width, frameSize, basePaint);
                canvas.DrawRect(0, height - frameSize, width, frameSize, basePaint);
                canvas.DrawRect(0, frameSize, frameSize, height - frameSize * 2, basePaint);
                canvas.DrawRect(
                    width - frameSize,
                    frameSize,
                    frameSize,
                    height - frameSize * 2,
                    basePaint);
            }

            // 上下左右へ同じ模様を描画
            DrawPattern(canvas, new SKRect(0, 0, width, frameSize),
                width, height, tileSize, pattern, color1, color2);

            DrawPattern(canvas, new SKRect(0, height - frameSize, width, height),
                width, height, tileSize, pattern, color1, color2);

            DrawPattern(canvas, new SKRect(0, frameSize, frameSize, height - frameSize),
                width, height, tileSize, pattern, color1, color2);

            DrawPattern(canvas, new SKRect(width - frameSize, frameSize, width, height - frameSize),
                width, height, tileSize, pattern, color1, color2);

            return bmp;
        }

        private static void DrawPattern(
            SKCanvas canvas,
            SKRect clipArea,
            int imageWidth,
            int imageHeight,
            int tileSize,
            EnumModel.FramePatternType pattern,
            SKColor color1,
            SKColor color2)
        {
            canvas.Save();
            canvas.ClipRect(clipArea);

            using var paint1 = new SKPaint
            {
                Color = color1,
                IsAntialias = true
            };

            using var paint2 = new SKPaint
            {
                Color = color2,
                IsAntialias = true
            };

            for (int y = 0; y < imageHeight; y += tileSize)
            {
                for (int x = 0; x < imageWidth; x += tileSize)
                {
                    bool alternate = ((x / tileSize) + (y / tileSize)) % 2 == 0;
                    var rect = new SKRect(x, y, x + tileSize, y + tileSize);

                    switch (pattern)
                    {
                        case EnumModel.FramePatternType.Checker:
                            canvas.DrawRect(rect, alternate ? paint1 : paint2);
                            break;

                        case EnumModel.FramePatternType.Triangle:
                            DrawTriangle(canvas, rect, alternate ? paint1 : paint2, alternate);
                            break;

                        case EnumModel.FramePatternType.Diamond:
                            canvas.DrawRect(rect, paint1);
                            DrawDiamond(canvas, rect, paint2);
                            break;

                        case EnumModel.FramePatternType.Dot:
                            canvas.DrawRect(rect, paint1);

                            float radius = tileSize * 0.28f;
                            canvas.DrawCircle(
                                x + tileSize / 2f,
                                y + tileSize / 2f,
                                radius,
                                paint2);
                            break;
                    }
                }
            }

            canvas.Restore();
        }

        private static void DrawTriangle(
            SKCanvas canvas,
            SKRect rect,
            SKPaint paint,
            bool reverse)
        {
            using var path = new SKPath();

            if (reverse)
            {
                path.MoveTo(rect.Left, rect.Top);
                path.LineTo(rect.Right, rect.Top);
                path.LineTo(rect.Right, rect.Bottom);
            }
            else
            {
                path.MoveTo(rect.Left, rect.Top);
                path.LineTo(rect.Left, rect.Bottom);
                path.LineTo(rect.Right, rect.Bottom);
            }

            path.Close();
            canvas.DrawPath(path, paint);
        }

        private static void DrawDiamond(
            SKCanvas canvas,
            SKRect rect,
            SKPaint paint)
        {
            float centerX = rect.MidX;
            float centerY = rect.MidY;

            using var path = new SKPath();
            path.MoveTo(centerX, rect.Top);
            path.LineTo(rect.Right, centerY);
            path.LineTo(centerX, rect.Bottom);
            path.LineTo(rect.Left, centerY);
            path.Close();

            canvas.DrawPath(path, paint);
        }
    }
}