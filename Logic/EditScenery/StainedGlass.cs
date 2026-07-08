using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageCreatePlaid
{
    public class StainedGlass : SceneryInterface
    {
        private static Random rand = new Random();
        public Bitmap EditImage(Bitmap bmp, SceneryModel model)
        {
            int width = bmp.Width;
            int height = bmp.Height;
            float saturation = model.SaturationValue;
            Graphics g = Graphics.FromImage(bmp);
            g.Clear(Color.White);

            // ランダムな点を生成（セルの中心）
            int cellCount = (width * height) / 5000; // サイズに応じてセル数調整
            List<Point> points = new List<Point>();
            for (int i = 0; i < cellCount; i++)
            {
                points.Add(new Point(rand.Next(width), rand.Next(height)));
            }
            // 各ピクセルを最近傍点に割り当て
            Dictionary<Point, List<Point>> regions = new Dictionary<Point, List<Point>>();
            foreach (var p in points) regions[p] = new List<Point>();

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Point nearest = points[0];
                    double minDist = double.MaxValue;

                    foreach (var p in points)
                    {
                        double dist = (x - p.X) * (x - p.X) + (y - p.Y) * (y - p.Y);
                        if (dist < minDist)
                        {
                            minDist = dist;
                            nearest = p;
                        }
                    }
                    regions[nearest].Add(new Point(x, y));
                }
            }

            // 各セルを塗る
            foreach (var region in regions.Values)
            {
                if (region.Count == 0) continue;

                // HSV で色決定
                double hue = rand.NextDouble() * 360;
                double baseValue = 0.6 + rand.NextDouble() * 0.4; // 黒を避ける
                Color baseColor = FromHSV(hue, saturation, baseValue);

                // セル全体を塗る（色ムラ & 光沢）
                foreach (var p in region)
                {
                    // セル内でのランダム揺らぎ
                    double noise = (rand.NextDouble() - 0.5) * 0.15;
                    double val = Math.Min(1.0, Math.Max(0.0, baseValue + noise));

                    // 左上が少し明るく → 光沢っぽい
                    double lightFactor = 1.0 - ((double)p.X / width + (double)p.Y / height) * 0.3;
                    val = Math.Min(1.0, val * (0.8 + lightFactor));

                    bmp.SetPixel(p.X, p.Y, FromHSV(hue, saturation, val));
                }
            }

            // 枠線を描画（軽く影をつける）
            using (Pen pen = new Pen(Color.FromArgb(180, 50, 50, 50), 1.2f))
            {
                foreach (var center in points)
                {
                    g.DrawEllipse(pen, center.X - 1, center.Y - 1, 2, 2);
                }
            }

            g.Dispose();
            return bmp;
        }

        // HSV → RGB 変換
        private static Color FromHSV(double hue, double saturation, double value)
        {
            int hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
            double f = hue / 60 - Math.Floor(hue / 60);

            value = value * 255;
            int v = (int)value;
            int p = (int)(value * (1 - saturation));
            int q = (int)(value * (1 - f * saturation));
            int t = (int)(value * (1 - (1 - f) * saturation));

            return hi switch
            {
                0 => Color.FromArgb(255, v, t, p),
                1 => Color.FromArgb(255, q, v, p),
                2 => Color.FromArgb(255, p, v, t),
                3 => Color.FromArgb(255, p, q, v),
                4 => Color.FromArgb(255, t, p, v),
                _ => Color.FromArgb(255, v, p, q),
            };
        }
    }
}
