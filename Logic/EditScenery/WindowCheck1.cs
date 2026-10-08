using SkiaSharp;

namespace ImageCreatePlaid
{
    public class WindowCheck1 : PlaidInterface
    {
        internal const int ShapeTypeCount = 8;

        public SKBitmap EditImage(SKBitmap bmp, PlaidModel model)
        {
            int width = bmp.Width;
            int height = bmp.Height;

            // --- 調整用パラメータ ---
            int minWindows = 2;              // 窓の数(最小)
            int maxWindows = 4;              // 窓の数(最大)
            float minSizeRatio = 0.38f;      // 窓の大きさ(画面の短い辺に対する割合)の最小
            float maxSizeRatio = 0.62f;      // 同・最大
            float overlap = 0.9f;            // 窓どうしの重なり(1=ぎりぎり接する、0.8=少し重なる、1.1=離す)
            float windowRotate = 18f;        // 窓の傾き(±度)
            float patternRotateRate = 0.35f; // 中のチェック柄を斜めにする確率
            float scaleJitter = 0.3f;        // 中のチェック柄の大きさのばらつき(±割合。0で変えない)
            float borderPx = 0f;             // 窓のフチの太さ(px。0でなし)
            // -----------------------

            var rand = new Random();

            using var canvas = new SKCanvas(bmp);
            canvas.Clear(new SKColor(model.BaseColorRed, model.BaseColorGreen, model.BaseColorBlue, model.BaseAlpha));

            int count = rand.Next(minWindows, maxWindows + 1);
            List<Window> windows = PlaceWindows(rand, width, height, count, minSizeRatio, maxSizeRatio, overlap, windowRotate);
            List<string> patternNames = ChoosePatternNames(rand, model, windows.Count);

            for (int n = 0; n < windows.Count; n++)
            {
                DrawWindow(canvas, rand, model, windows[n], patternNames[n], patternRotateRate, scaleJitter, borderPx);
            }

            return bmp;
        }

        // ---- 窓の配置 ----

        private sealed class Window
        {
            public float Cx;
            public float Cy;
            public float W;
            public float H;
            public float Radius; // 重なり判定に使う、窓をおおまかに囲む円の半径
            public float Angle;
            public int ShapeType;
        }

        private static List<Window> PlaceWindows(
            Random rand, int width, int height, int count,
            float minSizeRatio, float maxSizeRatio, float overlap, float rotate)
        {
            var windows = new List<Window>();
            float minSide = Math.Min(width, height);

            // 形の種類をシャッフルして順に使う(同じ形が続きにくい)
            List<int> shapeOrder = Enumerable.Range(0, ShapeTypeCount).OrderBy(_ => rand.Next()).ToList();

            for (int n = 0; n < count; n++)
            {
                float size = minSide * Lerp(minSizeRatio, maxSizeRatio, rand.NextDouble());
                float aspect = Lerp(0.8f, 1.25f, rand.NextDouble());
                bool placed = false;

                // 置けなければ、少しずつ小さくして再挑戦する
                for (int shrink = 0; shrink < 6 && !placed; shrink++)
                {
                    float w = size * MathF.Sqrt(aspect);
                    float h = size / MathF.Sqrt(aspect);
                    float radius = Math.Max(w, h) * 0.5f * 0.95f;

                    for (int attempt = 0; attempt < 80; attempt++)
                    {
                        float cx = width * Lerp(0.08f, 0.92f, rand.NextDouble());
                        float cy = height * Lerp(0.08f, 0.92f, rand.NextDouble());

                        bool ok = true;
                        foreach (Window other in windows)
                        {
                            float dx = cx - other.Cx;
                            float dy = cy - other.Cy;
                            float min = (radius + other.Radius) * overlap;
                            if (dx * dx + dy * dy < min * min)
                            {
                                ok = false;
                                break;
                            }
                        }
                        if (!ok) continue;

                        windows.Add(new Window
                        {
                            Cx = cx,
                            Cy = cy,
                            W = w,
                            H = h,
                            Radius = radius,
                            Angle = Lerp(-rotate, rotate, rand.NextDouble()),
                            ShapeType = shapeOrder[n % shapeOrder.Count]
                        });
                        placed = true;
                        break;
                    }

                    size *= 0.88f;
                }
            }

            return windows;
        }

        // ---- 窓の中の柄の選択 ----

        private static List<string> ChoosePatternNames(Random rand, PlaidModel model, int count)
        {
            var names = new List<string>();

            // 指定されている場合は、すべての窓で同じ柄
            if (!model.WindowPatternRandom && PlaidRegistry.Contains(model.WindowPatternName))
            {
                for (int n = 0; n < count; n++)
                {
                    names.Add(model.WindowPatternName);
                }
                return names;
            }

            // ランダムの場合は、窓ごとに違う柄(使い切るまで重複させない)
            List<string> pool = PlaidRegistry.RandomPool.OrderBy(_ => rand.Next()).ToList();
            for (int n = 0; n < count; n++)
            {
                names.Add(pool[n % pool.Count]);
            }
            return names;
        }

        // ---- 窓1枚の描画 ----

        private static void DrawWindow(
            SKCanvas canvas, Random rand, PlaidModel model,
            Window win, string patternName,
            float patternRotateRate, float scaleJitter, float borderPx)
        {
            using SKPath shape = CreateShape(win.ShapeType, win.W, win.H, rand);

            // 柄の向き。窓の傾きとは別に、柄だけを斜めにすることがある
            float patternAngle = rand.NextDouble() < patternRotateRate
                ? Lerp(-45f, 45f, rand.NextDouble())
                : 0f;

            // 窓がどう回転しても覆えるよう、窓の対角線の長さの正方形に柄を描く
            int layerSize = (int)Math.Ceiling(Math.Sqrt(win.W * win.W + win.H * win.H)) + 4;

            // 窓ごとに柄の大きさを変える。PlaidModel のサイズを一時的に変えて実現する
            int v0 = model.VerticalSize1;
            int h0 = model.HorizontalSize1;
            float scale = 1f + Lerp(-scaleJitter, scaleJitter, rand.NextDouble());

            SKBitmap layer = BussinessLogic.NewCreateImage(layerSize, layerSize);
            SKBitmap pattern;
            try
            {
                model.VerticalSize1 = Math.Max(2, (int)Math.Round(v0 * scale));
                model.HorizontalSize1 = Math.Max(2, (int)Math.Round(h0 * scale));
                pattern = PlaidRegistry.Create(patternName).EditImage(layer, model);
            }
            finally
            {
                model.VerticalSize1 = v0;
                model.HorizontalSize1 = h0;
            }

            // 窓の形で切り抜いて、柄を描く
            canvas.Save();
            canvas.Translate(win.Cx, win.Cy);
            canvas.RotateDegrees(win.Angle);
            canvas.ClipPath(shape, SKClipOperation.Intersect, true);

            // 窓の傾きを打ち消してから、柄の角度をかける
            canvas.RotateDegrees(patternAngle - win.Angle);
            canvas.DrawBitmap(pattern, -pattern.Width / 2f, -pattern.Height / 2f);
            canvas.Restore();

            // フチ
            if (borderPx > 0f)
            {
                using var border = new SKPaint
                {
                    IsAntialias = true,
                    Style = SKPaintStyle.Stroke,
                    StrokeWidth = borderPx,
                    StrokeJoin = SKStrokeJoin.Round,
                    Color = SKColors.White
                };

                canvas.Save();
                canvas.Translate(win.Cx, win.Cy);
                canvas.RotateDegrees(win.Angle);
                canvas.DrawPath(shape, border);
                canvas.Restore();
            }

            if (!ReferenceEquals(pattern, layer))
            {
                pattern.Dispose();
            }
            layer.Dispose();
        }

        // ---- 窓の形(中心が原点、おおよそ w×h に収まる) ----

        internal static SKPath CreateShape(int type, float w, float h, Random rand)
        {
            float hw = w / 2f;
            float hh = h / 2f;
            float minSide = Math.Min(w, h);

            switch (type)
            {
                case 0: // 楕円
                    {
                        var p = new SKPath();
                        p.AddOval(new SKRect(-hw, -hh, hw, hh));
                        return p;
                    }
                case 1: // 角丸四角
                    {
                        var p = new SKPath();
                        float r = minSide * Lerp(0.15f, 0.32f, rand.NextDouble());
                        p.AddRoundRect(new SKRect(-hw, -hh, hw, hh), r, r);
                        return p;
                    }
                case 2: // ひし形(角を丸める)
                    {
                        var pts = new List<SKPoint> { new(0, -1), new(1, 0), new(0, 1), new(-1, 0) };
                        return RoundCorners(FitToBox(pts, w, h), minSide * 0.12f);
                    }
                case 3: // 六角形(角を丸める)
                    {
                        var pts = new List<SKPoint>();
                        for (int k = 0; k < 6; k++)
                        {
                            float a = MathF.PI / 3f * k;
                            pts.Add(new SKPoint(MathF.Cos(a), MathF.Sin(a)));
                        }
                        return RoundCorners(FitToBox(pts, w, h), minSide * 0.10f);
                    }
                case 4: // 星(角を丸める)
                    {
                        int points = rand.Next(5, 8); // 5〜7
                        float inner = Lerp(0.55f, 0.72f, rand.NextDouble());
                        var pts = new List<SKPoint>();
                        for (int k = 0; k < points * 2; k++)
                        {
                            float a = -MathF.PI / 2f + MathF.PI * k / points;
                            float r = (k % 2 == 0) ? 1f : inner;
                            pts.Add(new SKPoint(MathF.Cos(a) * r, MathF.Sin(a) * r));
                        }
                        return RoundCorners(FitToBox(pts, w, h), minSide * 0.06f);
                    }
                case 5: // 花形(ふちが波打つ円)
                    {
                        int petals = rand.Next(5, 9); // 5〜8
                        float amp = Lerp(0.10f, 0.18f, rand.NextDouble());
                        return FitToBox(Polar(180, a => 1f + amp * MathF.Cos(petals * a)), w, h);
                    }
                case 6: // ハート
                    {
                        var pts = new List<SKPoint>();
                        for (int k = 0; k < 160; k++)
                        {
                            double t = Math.PI * 2 * k / 160;
                            double x = 16 * Math.Pow(Math.Sin(t), 3);
                            double y = -(13 * Math.Cos(t) - 5 * Math.Cos(2 * t) - 2 * Math.Cos(3 * t) - Math.Cos(4 * t));
                            pts.Add(new SKPoint((float)x, (float)y));
                        }
                        return FitToBox(pts, w, h);
                    }
                default: // ブロブ(ゆるやかに歪んだ円)
                    {
                        float a2 = Lerp(0.04f, 0.12f, rand.NextDouble());
                        float a3 = Lerp(0.03f, 0.10f, rand.NextDouble());
                        float a4 = Lerp(0.02f, 0.06f, rand.NextDouble());
                        float p2 = Lerp(0f, MathF.PI * 2f, rand.NextDouble());
                        float p3 = Lerp(0f, MathF.PI * 2f, rand.NextDouble());
                        float p4 = Lerp(0f, MathF.PI * 2f, rand.NextDouble());
                        return FitToBox(
                            Polar(180, a => 1f + a2 * MathF.Cos(2 * a + p2) + a3 * MathF.Cos(3 * a + p3) + a4 * MathF.Cos(4 * a + p4)),
                            w, h);
                    }
            }
        }

        // 角度 a(0〜2π)に対する半径から、輪郭の点列を作る
        private static List<SKPoint> Polar(int count, Func<float, float> radius)
        {
            var pts = new List<SKPoint>(count);
            for (int k = 0; k < count; k++)
            {
                float a = MathF.PI * 2f * k / count;
                float r = radius(a);
                pts.Add(new SKPoint(MathF.Cos(a) * r, MathF.Sin(a) * r));
            }
            return pts;
        }

        // 点列を、中心が原点の w×h の箱にぴったり収めた閉じたパスにする
        private static SKPath FitToBox(List<SKPoint> pts, float w, float h)
        {
            float minX = pts.Min(p => p.X);
            float maxX = pts.Max(p => p.X);
            float minY = pts.Min(p => p.Y);
            float maxY = pts.Max(p => p.Y);

            float cx = (minX + maxX) / 2f;
            float cy = (minY + maxY) / 2f;
            float sx = w / Math.Max(1e-6f, maxX - minX);
            float sy = h / Math.Max(1e-6f, maxY - minY);

            var path = new SKPath();
            for (int i = 0; i < pts.Count; i++)
            {
                float x = (pts[i].X - cx) * sx;
                float y = (pts[i].Y - cy) * sy;

                if (i == 0)
                {
                    path.MoveTo(x, y);
                }
                else
                {
                    path.LineTo(x, y);
                }
            }
            path.Close();

            return path;
        }

        // 多角形の角を丸める(元のパスは、この中で破棄する)
        private static SKPath RoundCorners(SKPath poly, float radius)
        {
            using (poly)
            using (SKPathEffect effect = SKPathEffect.CreateCorner(radius))
            using (var paint = new SKPaint { PathEffect = effect, Style = SKPaintStyle.Fill })
            {
                return paint.GetFillPath(poly);
            }
        }

        private static float Lerp(float a, float b, double t)
        {
            return a + (b - a) * (float)t;
        }
    }
}