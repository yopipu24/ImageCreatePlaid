using SkiaSharp;

namespace ImageCreatePlaid
{
    public class StainedGlass : SceneryInterface
    {
        private static readonly Random Rand = new Random();

        public SKBitmap EditImage(SKBitmap bmp, SceneryModel model)
        {
            int width = bmp.Width;
            int height = bmp.Height;
            float saturation = model.SaturationValue;

            using var canvas = new SKCanvas(bmp);
            canvas.Clear(SKColors.White);

            int cellCount = Math.Max(1, (width * height) / 5000);

            List<SKPointI> points = new List<SKPointI>();
            for (int i = 0; i < cellCount; i++)
            {
                points.Add(new SKPointI(Rand.Next(width), Rand.Next(height)));
            }

            Dictionary<SKPointI, List<SKPointI>> regions = new Dictionary<SKPointI, List<SKPointI>>();

            foreach (SKPointI point in points)
            {
                regions[point] = new List<SKPointI>();
            }

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    SKPointI nearest = points[0];
                    double minDist = double.MaxValue;

                    foreach (SKPointI point in points)
                    {
                        double dist = (x - point.X) * (x - point.X) + (y - point.Y) * (y - point.Y);

                        if (dist < minDist)
                        {
                            minDist = dist;
                            nearest = point;
                        }
                    }

                    regions[nearest].Add(new SKPointI(x, y));
                }
            }

            foreach (List<SKPointI> region in regions.Values)
            {
                if (region.Count == 0)
                {
                    continue;
                }

                double hue = Rand.NextDouble() * 360;
                double baseValue = 0.6 + Rand.NextDouble() * 0.4;

                foreach (SKPointI point in region)
                {
                    double noise = (Rand.NextDouble() - 0.5) * 0.15;
                    double value = Math.Min(1.0, Math.Max(0.0, baseValue + noise));

                    double lightFactor = 1.0 - ((double)point.X / width + (double)point.Y / height) * 0.3;
                    value = Math.Min(1.0, value * (0.8 + lightFactor));

                    bmp.SetPixel(point.X, point.Y, FromHSV(hue, saturation, value));
                }
            }

            return bmp;
        }

        private static SKColor FromHSV(double hue, double saturation, double value)
        {
            int hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
            double f = hue / 60 - Math.Floor(hue / 60);

            value *= 255;

            byte v = ClampToByte(value);
            byte p = ClampToByte(value * (1 - saturation));
            byte q = ClampToByte(value * (1 - f * saturation));
            byte t = ClampToByte(value * (1 - (1 - f) * saturation));

            return hi switch
            {
                0 => new SKColor(v, t, p, 255),
                1 => new SKColor(q, v, p, 255),
                2 => new SKColor(p, v, t, 255),
                3 => new SKColor(p, q, v, 255),
                4 => new SKColor(t, p, v, 255),
                _ => new SKColor(v, p, q, 255),
            };
        }

        private static byte ClampToByte(double value)
        {
            return (byte)Math.Max(0, Math.Min(255, value));
        }
    }
}