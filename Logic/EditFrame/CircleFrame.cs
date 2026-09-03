using SkiaSharp;

namespace ImageCreatePlaid
{
    public class CircleFrame : FrameInterface
    {
        public SKBitmap EditImage(SKBitmap bmp, FrameModel model)
        {
            int width = bmp.Width;
            int height = bmp.Height;

            int frameSize = model.FrameSize;

            SKColor baseColor = new SKColor(model.BaseColorRed, model.BaseColorGreen, model.BaseColorBlue, model.BaseAlpha);
            SKColor color1 = new SKColor(model.ColorRed1, model.ColorGreen1, model.ColorBlue1, 255);
            SKColor color2 = new SKColor(model.ColorRed2, model.ColorGreen2, model.ColorBlue2, 255);
            SKPaint paint1 = new SKPaint { Color = color1, IsAntialias = true };
            SKPaint paint2 = new SKPaint { Color = color2, IsAntialias = true };

            using (SKCanvas canvas = new SKCanvas(bmp))
            {
                canvas.Clear(baseColor);
                DrawAlternatingFrame(canvas, width, height, frameSize, paint1, paint2);
            }

            return bmp;
        }

        private static void DrawAlternatingFrame(
            SKCanvas canvas,
            int width,
            int height,
            int frameSize,
            SKPaint paint1,
            SKPaint paint2)
        {
            int index = 0;

            for (int x = 0; x < width; x += frameSize)
            {
                SKPaint paint = index % 2 == 0 ? paint1 : paint2;
                float segmentWidth = Math.Min(frameSize, width - x);

                // 上下の四角い枠
                canvas.DrawRect(x, 0, segmentWidth, frameSize, paint);
                canvas.DrawRect(x, height - frameSize, segmentWidth, frameSize, paint);

                // 内側へ向かう半円
                DrawScallop(
                    canvas,
                    x,
                    segmentWidth,
                    0,
                    frameSize,
                    isTop: true,
                    paint);

                DrawScallop(
                    canvas,
                    x,
                    segmentWidth,
                    height,
                    frameSize,
                    isTop: false,
                    paint);

                index++;
            }
        }

        private static void DrawScallop(
            SKCanvas canvas,
            float x,
            float segmentWidth,
            float y,
            float frameSize,
            bool isTop,
            SKPaint paint)
        {
            float radius = frameSize / 2f;

            // segmentWidth ではなく frameSize 基準にする
            float centerX = x + radius;

            float centerY = isTop
                ? y + frameSize
                : y - frameSize;

            canvas.DrawCircle(centerX, centerY, radius, paint);
        }
    }
}
