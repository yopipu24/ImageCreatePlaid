using SkiaSharp;

namespace ImageCreatePlaid
{
    public class Weave2 : PlaidInterface
    {
        public SKBitmap EditImage(SKBitmap bmp, PlaidModel model)
        {
            int width = bmp.Width;
            int height = bmp.Height;

            // --- 調整用パラメータ ---
            float wobbleRate = 0;    // 糸の揺らぎ(0で定規で引いたような直線。織物は控えめがおすすめ)
            float posVariation = 0;  // 糸の位置のばらつき(周期に対する割合)
            float tiltRate = 0;       // 糸の傾き
            float outlineDarken = 0.35f; // 輪郭線をどれだけ暗くするか(0で輪郭なし)
            float step = 3f;             // 輪郭を作る間隔(px)
            // -----------------------

            var rand = new Random();
            int balance = Math.Max(1, model.BaseBalance);

            float vPeriod = model.VerticalSize1;
            float vWidth = Math.Max(1f, vPeriod / balance);
            float hPeriod = model.HorizontalSize1;
            float hWidth = Math.Max(1f, hPeriod / balance);

            // 上下関係を表すため、糸は不透明で描く(model.Alpha は使わない)
            var vColor1 = new SKColor(model.VerticalColorRed1, model.VerticalColorGreen1, model.VerticalColorBlue1);
            var vColor2 = new SKColor(model.VerticalColorRed2, model.VerticalColorGreen2, model.VerticalColorBlue2);
            var hColor1 = new SKColor(model.HorizontalColorRed1, model.HorizontalColorGreen1, model.HorizontalColorBlue1);
            var hColor2 = new SKColor(model.HorizontalColorRed2, model.HorizontalColorGreen2, model.HorizontalColorBlue2);

            List<Strip> vStrips = CreateStrips(rand, width, height, vPeriod, vWidth, vColor1, vColor2,
                wobbleRate, posVariation, tiltRate, step);
            List<Strip> hStrips = CreateStrips(rand, height, width, hPeriod, hWidth, hColor1, hColor2,
                wobbleRate, posVariation, tiltRate, step);

            using var canvas = new SKCanvas(bmp);
            canvas.Clear(new SKColor(model.BaseColorRed, model.BaseColorGreen, model.BaseColorBlue, model.BaseAlpha));

            using var fill = new SKPaint { IsAntialias = true, Style = SKPaintStyle.Fill };
            using var line = new SKPaint { IsAntialias = true, Style = SKPaintStyle.Stroke, StrokeWidth = 1f };

            // 1. 横糸 → 縦糸の順に全体を描く(全交点で縦糸が上になる)
            foreach (Strip s in hStrips)
            {
                DrawStrip(canvas, fill, line, s, false, 0, s.MaxIndex, step, outlineDarken);
            }
            foreach (Strip s in vStrips)
            {
                DrawStrip(canvas, fill, line, s, true, 0, s.MaxIndex, step, outlineDarken);
            }

            // 2. 横糸が上になる交点だけ、横糸を重ねて描き直す
            for (int i = 0; i < vStrips.Count; i++)
            {
                for (int j = 0; j < hStrips.Count; j++)
                {
                    if ((i + j) % 2 == 0) continue; // 偶数は縦糸が上(描画済み)

                    Strip v = vStrips[i];
                    Strip h = hStrips[j];
                    FindCross(v, h, out float x, out float y);

                    // 横糸のうち、縦糸と重なる範囲(x方向)だけを描く
                    int k0 = (int)MathF.Floor((x - v.Reach) / step);
                    int k1 = (int)MathF.Ceiling((x + v.Reach) / step);
                    DrawStrip(canvas, fill, line, h, false, k0, k1, step, outlineDarken);
                }
            }

            return bmp;
        }

        private static List<Strip> CreateStrips(
            Random rand, float acrossLength, float alongLength,
            float period, float stripeWidth,
            SKColor color1, SKColor color2,
            float wobbleRate, float posVariation, float tiltRate, float step)
        {
            // 細い糸でも揺らぎが見えるように、太さと周期の大きいほうを基準にする
            float reference = Math.Max(stripeWidth, period * 0.25f);
            float swayAmp = reference * wobbleRate;
            float widthAmp = stripeWidth * wobbleRate * 0.5f;

            // 揺らぎの間隔は、輪郭の点の間隔の2倍以上にする
            float fineStep = Math.Max(period * 0.4f, step * 2f);
            float coarseStep = Math.Max(period * 1.5f, fineStep * 4f);

            // 糸の隙間の大きさ。交点の重ね描きがはみ出さないための上限に使う
            float gap = Math.Max(0f, period - stripeWidth);

            int count = (int)MathF.Ceiling(acrossLength / period) + 1;
            int maxIndex = (int)MathF.Ceiling(alongLength / step);
            float wobbleLength = (maxIndex + 1) * step;

            var strips = new List<Strip>(count);
            for (int n = 0; n < count; n++)
            {
                strips.Add(new Strip
                {
                    Center = n * period + stripeWidth / 2f + Random11(rand) * period * posVariation,
                    HalfWidth = stripeWidth / 2f,
                    SwayAmp = swayAmp,
                    WidthAmp = widthAmp,
                    Tilt = Random11(rand) * reference * tiltRate,
                    AlongLength = Math.Max(1f, alongLength),
                    Reach = stripeWidth / 2f + Math.Min(1.3f * widthAmp, gap),
                    MaxIndex = maxIndex,
                    Color = (n % 2 == 0) ? color1 : color2,
                    Sway = new Wobble(rand, wobbleLength, coarseStep, fineStep),
                    LeftWidth = new Wobble(rand, wobbleLength, coarseStep, fineStep),
                    RightWidth = new Wobble(rand, wobbleLength, coarseStep, fineStep)
                });
            }

            return strips;
        }

        // 縦糸と横糸の交点(おおよその中心)を求める
        private static void FindCross(Strip v, Strip h, out float x, out float y)
        {
            y = h.Center;
            x = v.CenterAt(y);
            for (int k = 0; k < 2; k++)
            {
                y = h.CenterAt(x);
                x = v.CenterAt(y);
            }
        }

        // 糸の k0〜k1 の範囲を描く(輪郭の点は step 刻みで揃えるので、一部だけ描いても全体と一致する)
        private static void DrawStrip(
            SKCanvas canvas, SKPaint fill, SKPaint line,
            Strip s, bool vertical, int k0, int k1, float step, float outlineDarken)
        {
            k0 = Math.Max(0, k0);
            k1 = Math.Min(s.MaxIndex, k1);
            if (k0 >= k1) return;

            using var body = new SKPath();
            for (int k = k0; k <= k1; k++)
            {
                float t = k * step;
                AddPoint(body, k == k0, vertical, s.Left(t), t);
            }
            for (int k = k1; k >= k0; k--)
            {
                float t = k * step;
                AddPoint(body, false, vertical, s.Right(t), t);
            }
            body.Close();

            fill.Color = s.Color;
            canvas.DrawPath(body, fill);

            if (outlineDarken <= 0f) return;

            // 左右の縁だけに輪郭線を引く(不透明なので、重ねて描いても濃くならない)
            using var edges = new SKPath();
            for (int k = k0; k <= k1; k++)
            {
                float t = k * step;
                AddPoint(edges, k == k0, vertical, s.Left(t), t);
            }
            for (int k = k0; k <= k1; k++)
            {
                float t = k * step;
                AddPoint(edges, k == k0, vertical, s.Right(t), t);
            }

            line.Color = Darken(s.Color, outlineDarken);
            canvas.DrawPath(edges, line);
        }

        private static void AddPoint(SKPath path, bool isFirst, bool vertical, float across, float along)
        {
            float x = vertical ? across : along;
            float y = vertical ? along : across;

            if (isFirst)
            {
                path.MoveTo(x, y);
            }
            else
            {
                path.LineTo(x, y);
            }
        }

        private static SKColor Darken(SKColor c, float amount)
        {
            float f = 1f - Math.Clamp(amount, 0f, 1f);
            return new SKColor((byte)(c.Red * f), (byte)(c.Green * f), (byte)(c.Blue * f));
        }

        // -1〜1の乱数
        private static float Random11(Random rand)
        {
            return (float)(rand.NextDouble() * 2 - 1);
        }

        /// <summary>
        /// 1本の糸。縞が並ぶ方向を across、糸が伸びる方向を along(t)として、
        /// 位置 t での左縁・右縁の座標を返す。
        /// </summary>
        private sealed class Strip
        {
            public float Center;
            public float HalfWidth;
            public float SwayAmp;
            public float WidthAmp;
            public float Tilt;
            public float AlongLength;
            public float Reach;      // 交差する相手の糸が占める、中心からの最大距離
            public int MaxIndex;
            public SKColor Color;
            public Wobble Sway = null!;
            public Wobble LeftWidth = null!;
            public Wobble RightWidth = null!;

            public float CenterAt(float t)
            {
                return Center + SwayAmp * Sway.At(t) + Tilt * (t / AlongLength);
            }

            public float Left(float t)
            {
                return CenterAt(t) - HalfWidth + WidthAmp * LeftWidth.At(t);
            }

            public float Right(float t)
            {
                return CenterAt(t) + HalfWidth + WidthAmp * RightWidth.At(t);
            }
        }

        /// <summary>
        /// 滑らかな1次元ノイズ。大きなうねり + 細かい手ぶれの2段重ね。戻り値はおおよそ -1.3〜1.3。
        /// </summary>
        private sealed class Wobble
        {
            private readonly float[] coarse;
            private readonly float[] fine;
            private readonly float coarseStep;
            private readonly float fineStep;

            public Wobble(Random rand, float length, float coarseStep, float fineStep)
            {
                this.coarseStep = coarseStep;
                this.fineStep = fineStep;
                coarse = CreateLattice(rand, length, coarseStep);
                fine = CreateLattice(rand, length, fineStep);
            }

            public float At(float t)
            {
                return Sample(coarse, coarseStep, t) + 0.3f * Sample(fine, fineStep, t);
            }

            private static float[] CreateLattice(Random rand, float length, float step)
            {
                int n = (int)MathF.Ceiling(length / step) + 3;
                var values = new float[n];
                for (int i = 0; i < n; i++)
                {
                    values[i] = (float)(rand.NextDouble() * 2 - 1);
                }
                return values;
            }

            private static float Sample(float[] values, float step, float t)
            {
                float f = Math.Max(0f, t) / step;
                int i = Math.Min((int)f, values.Length - 2);
                float u = f - i;
                u = u * u * (3f - 2f * u);
                return values[i] + (values[i + 1] - values[i]) * u;
            }
        }
    }
}