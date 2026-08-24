using SkiaSharp;

namespace ImageCreatePlaid
{
    public class None : SceneryInterface
    {
        public SKBitmap EditImage(SKBitmap bmp, SceneryModel model)
        {
            int width = bmp.Width;
            int height = bmp.Height;
            
            SKColor color = new SKColor(model.BaseColorRed, model.BaseColorGreen, model.BaseColorBlue, model.BaseAlpha);
            using (SKCanvas canvas = new SKCanvas(bmp))
            {
                canvas.Clear(color);
            }

            return bmp;
        }
    }
}
