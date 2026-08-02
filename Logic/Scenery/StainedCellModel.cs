using SkiaSharp;

namespace ImageCreatePlaid
{
    public class StainedCellModel
    {
        public int X { get; set; }
        public int Y { get; set; }

        public double Hue { get; set; }
        public double BaseValue { get; set; }
        public double Radius { get; set; }

        public SKColor BaseColor { get; set; }
    }
}
