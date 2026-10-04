using SkiaSharp;

namespace ImageCreatePlaid
{
    public class PolkaDot3 : PlaidInterface
    {
        public SKBitmap EditImage(SKBitmap bmp, PlaidModel model)
        {
            int width = bmp.Width;
            int height = bmp.Height;

            var color1 = new SKColor(model.VerticalColorRed1, model.VerticalColorGreen1, model.VerticalColorBlue1, model.Alpha);
            var color2 = new SKColor(model.HorizontalColorRed2, model.HorizontalColorGreen2, model.HorizontalColorBlue2, model.Alpha);

            // --- 調整用パラメータ ---
            float spacing = 1.1f;      // 点同士の最小距離(ドットの大きさに対する倍率。1.0以上で重ならない)
            float minScale = 0.5f;     // ドットの最小サイズ倍率
            float maxScale = 1.0f;     // ドットの最大サイズ倍率
            float color1Rate = 0.5f;   // color1を使う確率(残りはcolor2)
            // -----------------------

            var rand = new Random();

            float sizeW = model.HorizontalSize1;
            float sizeH = model.VerticalSize1;

            // 最大のドット直径より少し大きい距離を最小距離にすると、ドット同士が重ならない
            float minDist = Math.Max(sizeW, sizeH) * spacing;

            List<SKPoint> points = PoissonDisk(width, height, minDist, rand);

            using var canvas = new SKCanvas(bmp);
            canvas.Clear(new SKColor(model.BaseColorRed, model.BaseColorGreen, model.BaseColorBlue, model.BaseAlpha));

            using var paint = new SKPaint
            {
                IsAntialias = true,
                Style = SKPaintStyle.Fill
            };

            foreach (SKPoint p in points)
            {
                float scale = minScale + (float)rand.NextDouble() * (maxScale - minScale);
                paint.Color = rand.NextDouble() < color1Rate ? color1 : color2;
                canvas.DrawOval(p.X, p.Y, sizeW / 2f * scale, sizeH / 2f * scale, paint);
            }

            return bmp;
        }

        /// <summary>
        /// ポアソンディスクサンプリング(Bridson法)。
        /// 点同士が minDist 以上離れた、自然な散らばりの点群を返す。
        /// </summary>
        private static List<SKPoint> PoissonDisk(int width, int height, float minDist, Random rand, int k = 30)
        {
            // 端で切れるドットも描けるように、画面の外側に少し広げた領域で生成する
            float margin = minDist;
            float w = width + margin * 2;
            float h = height + margin * 2;

            // 1セルに最大1点しか入らない大きさのグリッドで、近傍探索を高速化
            float cellSize = minDist / MathF.Sqrt(2f);
            int gridW = (int)MathF.Ceiling(w / cellSize);
            int gridH = (int)MathF.Ceiling(h / cellSize);
            int[] grid = new int[gridW * gridH];
            Array.Fill(grid, -1);

            var points = new List<SKPoint>();
            var active = new List<int>();

            void AddPoint(SKPoint p)
            {
                int index = points.Count;
                points.Add(p);
                active.Add(index);
                grid[(int)(p.Y / cellSize) * gridW + (int)(p.X / cellSize)] = index;
            }

            AddPoint(new SKPoint((float)rand.NextDouble() * w, (float)rand.NextDouble() * h));

            while (active.Count > 0)
            {
                int activeIndex = rand.Next(active.Count);
                SKPoint center = points[active[activeIndex]];
                bool found = false;

                for (int n = 0; n < k; n++)
                {
                    // 中心点から minDist〜2*minDist の範囲に候補点を作る
                    double angle = rand.NextDouble() * Math.PI * 2;
                    double dist = minDist * (1 + rand.NextDouble());
                    var p = new SKPoint(
                        center.X + (float)(Math.Cos(angle) * dist),
                        center.Y + (float)(Math.Sin(angle) * dist));

                    if (p.X < 0 || p.X >= w || p.Y < 0 || p.Y >= h) continue;

                    int gx = (int)(p.X / cellSize);
                    int gy = (int)(p.Y / cellSize);
                    bool ok = true;

                    for (int yy = Math.Max(0, gy - 2); yy <= Math.Min(gridH - 1, gy + 2) && ok; yy++)
                    {
                        for (int xx = Math.Max(0, gx - 2); xx <= Math.Min(gridW - 1, gx + 2); xx++)
                        {
                            int index = grid[yy * gridW + xx];
                            if (index < 0) continue;

                            float dx = points[index].X - p.X;
                            float dy = points[index].Y - p.Y;
                            if (dx * dx + dy * dy < minDist * minDist)
                            {
                                ok = false;
                                break;
                            }
                        }
                    }

                    if (ok)
                    {
                        AddPoint(p);
                        found = true;
                        break;
                    }
                }

                // k回試しても置けなければ、その点の周りは埋まったので候補から外す
                if (!found)
                {
                    active[activeIndex] = active[active.Count - 1];
                    active.RemoveAt(active.Count - 1);
                }
            }

            // 外側に広げた分を元の座標に戻す
            for (int i = 0; i < points.Count; i++)
            {
                points[i] = new SKPoint(points[i].X - margin, points[i].Y - margin);
            }

            return points;
        }
    }
}