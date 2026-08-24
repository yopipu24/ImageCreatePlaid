using SkiaSharp;

namespace ImageCreatePlaid
{
    public class Puzzle : SceneryInterface
    {
        private readonly Random Rand = new();

        public SKBitmap EditImage(SKBitmap bmp, SceneryModel model)
        {
            int width = bmp.Width;
            int height = bmp.Height;

            int pieceWidth = model.HorizontalSize1;
            int pieceHeight = model.VerticalSize1;

            // 背景色
            SKColor backgroundColor = new SKColor(
                model.BaseColorRed,
                model.BaseColorGreen,
                model.BaseColorBlue,
                model.BaseAlpha
            );

            // 境界線
            SKColor lineColor = new SKColor(255, 255, 255, 180);
            SKColor vColor = new SKColor(model.VerticalColorRed1, model.VerticalColorGreen1, model.VerticalColorBlue1, 180);
            SKColor hColor = new SKColor(model.HorizontalColorRed2, model.HorizontalColorGreen2, model.HorizontalColorBlue2, 180);

            using SKCanvas canvas = new SKCanvas(bmp);

            // 背景を塗る
            canvas.Clear(backgroundColor);

            using SKPaint paint = new SKPaint
            {
                Color = lineColor,
                Style = SKPaintStyle.Stroke,
                StrokeWidth = 2.0f,
                IsAntialias = true
            };

            int columns = (width + pieceWidth - 1) / pieceWidth;
            int rows = (height + pieceHeight - 1) / pieceHeight;

            using SKPaint vPaint = new SKPaint
            {
                Color = vColor,
                Style = SKPaintStyle.Stroke,
                StrokeWidth = 2.0f,
                IsAntialias = true
            };

            // --------------------------------------------------
            // 縦方向の境界
            // --------------------------------------------------
            for (int col = 1; col < columns; col++)
            {
                float x = col * pieceWidth;

                for (int row = 0; row < rows; row++)
                {
                    float y = row * pieceHeight;
                    float bottom = Math.Min(y + pieceHeight, height);

                    int direction = Rand.Next(2) == 0 ? -1 : 1;

                    using SKPath path = CreateVerticalLine(
                        x,
                        y,
                        bottom,
                        direction
                    );

                    canvas.DrawPath(path, vPaint);
                }
            }

            using SKPaint hPaint = new SKPaint
            {
                Color = hColor,
                Style = SKPaintStyle.Stroke,
                StrokeWidth = 2.0f,
                IsAntialias = true
            };


            // --------------------------------------------------
            // 横方向の境界
            // --------------------------------------------------
            for (int row = 1; row < rows; row++)
            {
                float y = row * pieceHeight;

                for (int col = 0; col < columns; col++)
                {
                    float x = col * pieceWidth;
                    float right = Math.Min(x + pieceWidth, width);

                    int direction = Rand.Next(2) == 0 ? -1 : 1;

                    using SKPath path = CreateHorizontalLine(
                        x,
                        right,
                        y,
                        direction
                    );

                    canvas.DrawPath(path, hPaint);
                }
            }

            return bmp;
        }

        /// <summary>
        /// 縦方向のジグソー境界
        /// </summary>
        private SKPath CreateVerticalLine(
            float x,
            float top,
            float bottom,
            int direction)
        {
            float length = bottom - top;

            float center = top + length * 0.5f;
            float tabSize = Math.Min(20, length * 0.18f);

            float start = center - tabSize;
            float end = center + tabSize;

            // 凸方向
            float bulge = 18 * direction;

            SKPath path = new SKPath();

            path.MoveTo(x, top);

            path.LineTo(x, start);

            // ジグソーの凸凹部分
            path.CubicTo(
                x + bulge, start + tabSize * 0.1f,
                x + bulge, end - tabSize * 0.1f,
                x, end
            );

            path.LineTo(x, bottom);

            return path;
        }

        /// <summary>
        /// 横方向のジグソー境界
        /// </summary>
        private SKPath CreateHorizontalLine(
            float left,
            float right,
            float y,
            int direction)
        {
            float length = right - left;

            float center = left + length * 0.5f;
            float tabSize = Math.Min(20, length * 0.18f);

            float start = center - tabSize;
            float end = center + tabSize;

            // 凸方向
            float bulge = 18 * direction;

            SKPath path = new SKPath();

            path.MoveTo(left, y);

            path.LineTo(start, y);

            // ジグソーの凸凹部分
            path.CubicTo(
                start + tabSize * 0.1f,
                y + bulge,
                end - tabSize * 0.1f,
                y + bulge,
                end,
                y
            );

            path.LineTo(right, y);

            return path;
        }
    }
}