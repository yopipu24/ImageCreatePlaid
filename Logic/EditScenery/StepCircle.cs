using SkiaSharp;

namespace ImageCreatePlaid
{
    public class StepCircle : SceneryInterface
    {
        public SKBitmap EditImage(SKBitmap bmp, SceneryModel model)
        {
            int width = bmp.Width;
            int height = bmp.Height;

            var baseColor = new SKColor(
                model.BaseColorRed,
                model.BaseColorGreen,
                model.BaseColorBlue,
                model.BaseAlpha);

            int wStepCount = Math.Max(1, width / model.HorizontalSize1);
            int hStepCount = Math.Max(1, height / model.VerticalSize1);

            var h1Color = new SKColor(
                model.HorizontalColorRed1,
                model.HorizontalColorGreen1,
                model.HorizontalColorBlue1,
                model.BaseAlpha);

            var v1Color = new SKColor(
                model.VerticalColorRed1,
                model.VerticalColorGreen1,
                model.VerticalColorBlue1,
                model.BaseAlpha);

            var h2Color = new SKColor(
                model.HorizontalColorRed2,
                model.HorizontalColorGreen2,
                model.HorizontalColorBlue2,
                model.BaseAlpha);

            var v2Color = new SKColor(
                model.VerticalColorRed2,
                model.VerticalColorGreen2,
                model.VerticalColorBlue2,
                model.BaseAlpha);

            string pattern = model.StepPattern;
            SKColor currentColor = h1Color;
            bool useFirstColor = true;

            using var canvas = new SKCanvas(bmp);

            canvas.Clear(baseColor);

            using var paint = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                IsAntialias = true
            };

            for (int x = 0; x < wStepCount; x++)
            {
                for (int y = 0; y < hStepCount; y++)
                {
                    int circleSize;

                    if (pattern == "right")
                    {
                        circleSize = model.HorizontalSize1 * x / wStepCount;

                        if (model.StepColorPattern == "two")
                        {
                            currentColor = x % 2 == 0 ? h1Color : h2Color;
                        }
                    }
                    else if (pattern == "bottom")
                    {
                        circleSize = model.VerticalSize1 * y / hStepCount;

                        if (model.StepColorPattern == "two")
                        {
                            currentColor = y % 2 == 0 ? v1Color : v2Color;
                        }
                    }
                    else
                    {
                        circleSize = model.HorizontalSize1 * x / wStepCount * y / hStepCount;

                        if (model.StepColorPattern == "two")
                        {
                            currentColor = useFirstColor ? h1Color : h2Color;
                            useFirstColor = !useFirstColor;
                        }
                    }

                    if (model.StepColorPattern == "alt")
                    {
                        if ((x % 2 == 0 && y % 2 == 0) || (x % 2 == 1 && y % 2 == 1))
                        {
                            currentColor = h1Color;
                        }
                        else
                        {
                            currentColor = h2Color;
                        }
                    }

                    int offsetX = x * model.HorizontalSize1;
                    int offsetY = y * model.VerticalSize1;

                    paint.Color = currentColor;
                    canvas.DrawOval(offsetX, offsetY, circleSize / 2, circleSize / 2, paint);
                }
            }

            return bmp;
        }
    }
}