using SkiaSharp;

namespace ImageCreatePlaid
{
    public class NormalFrame : FrameInterface
    {
        public SKBitmap EditImage(SKBitmap bmp, FrameModel model)
        {
            int width = bmp.Width;
            int height = bmp.Height;

            string frameType = model.FrameType.ToString();
            int frameSize = model.FrameSize;

            SKColor baseColor = new SKColor(model.BaseColorRed, model.BaseColorGreen, model.BaseColorBlue, model.BaseAlpha);
            SKColor color1 = new SKColor(model.ColorRed1, model.ColorGreen1, model.ColorBlue1, 255);

            using (SKCanvas canvas = new SKCanvas(bmp))
            {
                canvas.Clear(baseColor); 
                using (SKPaint paint = new SKPaint { Color = color1 })
                {
                    if (frameType == "topbottom")
                    {
                        // 上下の枠
                        canvas.DrawRect(0, 0, width, frameSize, paint);
                        canvas.DrawRect(0, height - frameSize, width, frameSize, paint);
                    }
                    else if (frameType == "all")
                    {
                        canvas.DrawRect(0, 0, width, frameSize, paint);
                        canvas.DrawRect(0, height - frameSize, width, frameSize, paint);
                        canvas.DrawRect(0, frameSize, frameSize, height - frameSize * 2, paint);
                        canvas.DrawRect(width - frameSize, frameSize, frameSize, height - frameSize * 2, paint);
                    }
                }
            }

            return bmp;
        }
    }
}
