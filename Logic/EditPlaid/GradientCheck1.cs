using SkiaSharp;

namespace ImageCreatePlaid
{
    public class GradientCheck1 : PlaidInterface
    {
        public SKBitmap EditImage(SKBitmap bmp, PlaidModel model)
        {
            int width = bmp.Width;
            int height = bmp.Height;

            // --- 調整用パラメータ ---
            int mode = 3;            // 0=左→右, 1=上→下, 2=左上→右下, 3=中心→外側 に向かって変化
            float endRatio = 0.25f;  // 終端のマスの大きさ(開始時を1とした倍率)。1より大きいと逆に大きくなる
            float curve = 1f;        // 変化の速さ(1=一定、2=前半ゆっくり後半急に、0.5=前半急に後半ゆっくり)
            float fadeRate = 0f;     // 色の変化量(0=なし、1=終端で1色目が2色目に溶け込む)
            int minCellPx = 2;       // マスの最小サイズ(px)
            // -----------------------

            var color1 = new SKColor(model.VerticalColorRed1, model.VerticalColorGreen1, model.VerticalColorBlue1, model.Alpha);
            var color2 = new SKColor(model.HorizontalColorRed2, model.HorizontalColorGreen2, model.HorizontalColorBlue2, model.Alpha);

            bool gradX = mode == 0 || mode == 2 || mode == 3; // 横方向にマスの大きさが変わる
            bool gradY = mode == 1 || mode == 2 || mode == 3; // 縦方向にマスの大きさが変わる
            bool fromCenter = mode == 3;
            int axes = (gradX ? 1 : 0) + (gradY ? 1 : 0);

            // マスの幅は VerticalSize1、高さは HorizontalSize1 を開始時の大きさにする
            List<int> xs = BuildEdges(width, model.VerticalSize1, gradX, fromCenter, endRatio, curve, minCellPx);
            List<int> ys = BuildEdges(height, model.HorizontalSize1, gradY, fromCenter, endRatio, curve, minCellPx);

            using var canvas = new SKCanvas(bmp);
            canvas.Clear(color2);

            // 1色目のマスは、下地(2色目)の上に合成せず、そのまま置き換えて描く(Alphaが低くても色が濁らない)
            using var paint = new SKPaint
            {
                IsAntialias = false,
                Style = SKPaintStyle.Fill,
                BlendMode = SKBlendMode.Src
            };

            for (int j = 0; j < ys.Count - 1; j++)
            {
                if (ys[j] >= height) break;
                if (ys[j + 1] <= 0) continue;

                float py = gradY ? AxisProgress((ys[j] + ys[j + 1]) / 2f, height, fromCenter) : 0f;

                for (int i = 0; i < xs.Count - 1; i++)
                {
                    if (xs[i] >= width) break;
                    if (xs[i + 1] <= 0) continue;

                    // 市松: マスの番号の偶奇で塗り分ける(偶数マスだけ1色目)
                    if (((i + j) & 1) != 0) continue;

                    if (fadeRate > 0f)
                    {
                        float px = gradX ? AxisProgress((xs[i] + xs[i + 1]) / 2f, width, fromCenter) : 0f;
                        float p = (px + py) / axes;
                        paint.Color = Mix(color1, color2, Math.Clamp(fadeRate * p, 0f, 1f));
                    }
                    else
                    {
                        paint.Color = color1;
                    }

                    canvas.DrawRect(xs[i], ys[j], xs[i + 1] - xs[i], ys[j + 1] - ys[j], paint);
                }
            }

            return bmp;
        }

        /// <summary>
        /// マスの境界の座標(昇順)を作る。隣り合う境界の間が1マス。
        /// gradient=false: 一定の大きさ / fromCenter=false: 0から順に / fromCenter=true: 中心から左右(上下)へ
        /// </summary>
        private static List<int> BuildEdges(
            int length, float startSize, bool gradient, bool fromCenter,
            float endRatio, float curve, int minPx)
        {
            var edges = new List<int>();
            float baseSize = Math.Max(minPx, startSize);

            if (!gradient)
            {
                int step = Math.Max(minPx, (int)MathF.Round(baseSize));
                for (int pos = 0; pos < length + step; pos += step)
                {
                    edges.Add(pos);
                }
                return edges;
            }

            if (!fromCenter)
            {
                int pos = 0;
                edges.Add(pos);
                while (pos < length)
                {
                    float p = (float)pos / length;
                    pos += SizeAt(baseSize, endRatio, curve, p, minPx);
                    edges.Add(pos);
                }
                return edges;
            }

            // 中心から外側へ: 中心を境界にして、右(下)側と左(上)側を別々に作って左右対称にする
            int center = length / 2;

            var right = new List<int> { center };
            int r = center;
            while (r < length)
            {
                float p = (float)(r - center) / Math.Max(1, length - center);
                r += SizeAt(baseSize, endRatio, curve, p, minPx);
                right.Add(r);
            }

            var left = new List<int>();
            int l = center;
            while (l > 0)
            {
                float p = (float)(center - l) / Math.Max(1, center);
                l -= SizeAt(baseSize, endRatio, curve, p, minPx);
                left.Add(l);
            }
            left.Reverse();

            edges.AddRange(left);
            edges.AddRange(right);
            return edges;
        }

        // 位置 p(0〜1)でのマスの大きさ(px)
        private static int SizeAt(float baseSize, float endRatio, float curve, float p, int minPx)
        {
            float t = MathF.Pow(Math.Clamp(p, 0f, 1f), curve);
            float size = baseSize * (1f + (endRatio - 1f) * t);
            return Math.Max(minPx, (int)MathF.Round(size));
        }

        // 位置が、変化の始点(0)から終点(1)のどのあたりか
        private static float AxisProgress(float pos, int length, bool fromCenter)
        {
            if (fromCenter)
            {
                float c = length / 2f;
                return Math.Clamp(Math.Abs(pos - c) / c, 0f, 1f);
            }
            return Math.Clamp(pos / length, 0f, 1f);
        }

        private static SKColor Mix(SKColor a, SKColor b, float t)
        {
            return new SKColor(
                (byte)Math.Round(a.Red + (b.Red - a.Red) * t),
                (byte)Math.Round(a.Green + (b.Green - a.Green) * t),
                (byte)Math.Round(a.Blue + (b.Blue - a.Blue) * t),
                a.Alpha);
        }
    }
}