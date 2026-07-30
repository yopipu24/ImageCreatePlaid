using SkiaSharp;

namespace ImageCreatePlaid
{
    public class RandomCheck : PlaidInterface
    {
        public SKBitmap EditImage(SKBitmap bmp, PlaidModel model)
        {
            int width = bmp.Width;
            int height = bmp.Height;

            int tileWidth = model.VerticalSize1;
            int tileHeight = model.HorizontalSize1;

            using SKBitmap tileBmp = BussinessLogic.NewCreateImage(tileWidth, tileHeight);
            using SKCanvas tileCanvas = new SKCanvas(tileBmp);

            SKColor color0 = new SKColor(model.BaseColorRed, model.BaseColorGreen, model.BaseColorBlue, model.BaseAlpha);
            SKColor color1v = new SKColor(model.VerticalColorRed1, model.VerticalColorGreen1, model.VerticalColorBlue1, model.Alpha);
            SKColor color2h = new SKColor(model.HorizontalColorRed2, model.HorizontalColorGreen2, model.HorizontalColorBlue2, model.Alpha);

            SKColor color1Cross = BussinessLogic.CalcColor2(color1v);

            tileCanvas.Clear(color0);

            bool[] line1X = CreateLinePattern(
                tileWidth,
                BussinessLogic.GetRandomInt(model.VerticalSize1 / 8) + model.VerticalSize1 / 10,
                BussinessLogic.GetRandomInt(model.VerticalSize1 / 3) + model.VerticalSize1 / 10,
                true);

            bool[] line1Y = CreateLinePattern(
                tileHeight,
                BussinessLogic.GetRandomInt(model.VerticalSize1 / 8) + model.VerticalSize1 / 10,
                BussinessLogic.GetRandomInt(model.VerticalSize1 / 3) + model.VerticalSize1 / 10,
                true);

            using var paint = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                IsAntialias = false
            };

            paint.Color = color1v;

            DrawVerticalRuns(tileCanvas, line1X, 0, tileHeight, paint);
            DrawHorizontalRuns(tileCanvas, line1Y, 0, tileWidth, paint);

            paint.Color = color1Cross;
            DrawCrossRuns(tileCanvas, line1X, line1Y, paint);

            bool drawSecondLine = BussinessLogic.GetRandomInt(50) > 40;

            bool[] line2X = CreateLinePattern2(
                tileWidth,
                BussinessLogic.GetRandomInt(model.VerticalSize1 / 10) + 5,
                BussinessLogic.GetRandomInt(model.VerticalSize1 / 5),
                drawSecondLine);

            bool[] line2Y = CreateLinePattern2(
                tileHeight,
                BussinessLogic.GetRandomInt(model.VerticalSize1 / 10) + 5,
                BussinessLogic.GetRandomInt(model.VerticalSize1 / 5),
                drawSecondLine);

            paint.Color = color2h;

            DrawVerticalRuns(tileCanvas, line2X, 0, tileHeight, paint);
            DrawHorizontalRuns(tileCanvas, line2Y, 0, tileWidth, paint);

            bmp = BussinessLogic.RepeatImage(width, height, tileBmp);

            return bmp;
        }

        private static bool[] CreateLinePattern(int length, int lineWidth, int startCount, bool repeatSecondLine)
        {
            bool[] pattern = new bool[length];

            int pos = Math.Min(startCount, length);

            FillPattern(pattern, pos, lineWidth, true);
            pos += lineWidth;

            pos += lineWidth / 2;

            if (repeatSecondLine)
            {
                FillPattern(pattern, pos, lineWidth, true);
            }

            return pattern;
        }

        private static bool[] CreateLinePattern2(int length, int lineWidth, int startCount, bool repeatSecondLine)
        {
            bool[] pattern = new bool[length];

            int pos = Math.Min(startCount, length);

            FillPattern(pattern, pos, lineWidth, true);
            pos += lineWidth;

            pos += lineWidth / 2;

            if (repeatSecondLine)
            {
                FillPattern(pattern, pos, lineWidth, true);
            }

            return pattern;
        }

        private static void FillPattern(bool[] pattern, int start, int width, bool value)
        {
            int end = Math.Min(start + width, pattern.Length);

            for (int i = start; i < end; i++)
            {
                pattern[i] = value;
            }
        }

        private static void DrawVerticalRuns(SKCanvas canvas, bool[] pattern, int top, int height, SKPaint paint)
        {
            for (int x = 0; x < pattern.Length; x++)
            {
                if (!pattern[x])
                {
                    continue;
                }

                int start = x;

                while (x < pattern.Length && pattern[x])
                {
                    x++;
                }

                canvas.DrawRect(start, top, x - start, height, paint);
            }
        }

        private static void DrawHorizontalRuns(SKCanvas canvas, bool[] pattern, int left, int width, SKPaint paint)
        {
            for (int y = 0; y < pattern.Length; y++)
            {
                if (!pattern[y])
                {
                    continue;
                }

                int start = y;

                while (y < pattern.Length && pattern[y])
                {
                    y++;
                }

                canvas.DrawRect(left, start, width, y - start, paint);
            }
        }

        private static void DrawCrossRuns(SKCanvas canvas, bool[] patternX, bool[] patternY, SKPaint paint)
        {
            for (int x = 0; x < patternX.Length; x++)
            {
                if (!patternX[x])
                {
                    continue;
                }

                int startX = x;

                while (x < patternX.Length && patternX[x])
                {
                    x++;
                }

                for (int y = 0; y < patternY.Length; y++)
                {
                    if (!patternY[y])
                    {
                        continue;
                    }

                    int startY = y;

                    while (y < patternY.Length && patternY[y])
                    {
                        y++;
                    }

                    canvas.DrawRect(startX, startY, x - startX, y - startY, paint);
                }
            }
        }
    }
}