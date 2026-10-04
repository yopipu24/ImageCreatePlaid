using SkiaSharp;

namespace ImageCreatePlaid
{
    public class HandDrawn1 : PlaidInterface
    {
        public SKBitmap EditImage(SKBitmap bmp, PlaidModel model)
        {
            int width = bmp.Width;
            int height = bmp.Height;

            // --- 調整用パラメータ ---
            float wobbleRate = 0.3f;      // 縁の揺らぎの大きさ(縞の太さに対する割合)
            float posVariation = 0.12f;   // 縞の位置のばらつき(周期に対する割合)
            float tiltRate = 0.5f;        // 縞の傾き(縞全体で何本分の太さだけ流れるか)
            float alphaVariation = 0.15f; // 縞ごとの濃さのばらつき(0〜0.3くらい)
            float step = 3f;              // 輪郭を作る間隔(px)。小さいほど滑らか
            // -----------------------

            var rand = new Random();
            int balance = Math.Max(1, model.BaseBalance);

            using var canvas = new SKCanvas(bmp);
            canvas.Clear(new SKColor(model.BaseColorRed, model.BaseColorGreen, model.BaseColorBlue, model.BaseAlpha));

            using var paint = new SKPaint
            {
                IsAntialias = true,
                Style = SKPaintStyle.Fill
            };

            var vColor1 = new SKColor(model.VerticalColorRed1, model.VerticalColorGreen1, model.VerticalColorBlue1, model.Alpha);
            var vColor2 = new SKColor(model.VerticalColorRed2, model.VerticalColorGreen2, model.VerticalColorBlue2, model.Alpha);
            var hColor1 = new SKColor(model.HorizontalColorRed1, model.HorizontalColorGreen1, model.HorizontalColorBlue1, model.Alpha);
            var hColor2 = new SKColor(model.HorizontalColorRed2, model.HorizontalColorGreen2, model.HorizontalColorBlue2, model.Alpha);

            // 縦縞(x方向に並び、y方向に伸びる)
            DrawStripes(canvas, paint, rand, true,
                width, height,
                model.VerticalSize1, Math.Max(1f, (float)model.VerticalSize1 / balance),
                vColor1, vColor2,
                wobbleRate, posVariation, tiltRate, alphaVariation, step);

            // 横縞(y方向に並び、x方向に伸びる)
            DrawStripes(canvas, paint, rand, false,
                height, width,
                model.HorizontalSize1, Math.Max(1f, (float)model.HorizontalSize1 / balance),
                hColor1, hColor2,
                wobbleRate, posVariation, tiltRate, alphaVariation, step);

            return bmp;
        }

        /// <summary>
        /// 揺らぎのある縞を並べて描く。
        /// vertical=true: 縦縞、false: 横縞。
        /// acrossLength: 縞が並ぶ方向の長さ、alongLength: 縞が伸びる方向の長さ。
        /// </summary>
        private static void DrawStripes(
            SKCanvas canvas, SKPaint paint, Random rand, bool vertical,
            float acrossLength, float alongLength,
            float period, float stripeWidth,
            SKColor color1, SKColor color2,
            float wobbleRate, float posVariation, float tiltRate, float alphaVariation, float step)
        {
            float amp = stripeWidth * wobbleRate;
            float coarseStep = Math.Max(period * 1.5f, 8f);
            int count = (int)MathF.Ceiling(acrossLength / period) + 1;
            int points = (int)MathF.Ceiling(alongLength / step) + 1;
            float wobbleLength = points * step;

            for (int n = 0; n < count; n++)
            {
                float center = n * period + stripeWidth / 2f + Random11(rand) * period * posVariation;
                float tilt = Random11(rand) * stripeWidth * tiltRate;

                var left = new Wobble(rand, wobbleLength, coarseStep);
                var right = new Wobble(rand, wobbleLength, coarseStep);

                // 縞ごとに濃さを少し変える(インクの濃淡のイメージ)
                SKColor baseColor = (n % 2 == 0) ? color1 : color2;
                float alphaScale = 1f + Random11(rand) * alphaVariation;
                byte alpha = (byte)Math.Clamp(baseColor.Alpha * alphaScale, 0f, 255f);
                paint.Color = baseColor.WithAlpha(alpha);

                using var path = new SKPath();

                // 一方の縁を進む
                for (int p = 0; p <= points; p++)
                {
                    float t = p * step;
                    float a = center - stripeWidth / 2f + amp * left.At(t) + tilt * (t / alongLength);
                    AddPoint(path, p == 0, vertical, a, t);
                }

                // もう一方の縁を戻る
                for (int p = points; p >= 0; p--)
                {
                    float t = p * step;
                    float a = center + stripeWidth / 2f + amp * right.At(t) + tilt * (t / alongLength);
                    AddPoint(path, false, vertical, a, t);
                }

                path.Close();
                canvas.DrawPath(path, paint);
            }
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

        // -1〜1の乱数
        private static float Random11(Random rand)
        {
            return (float)(rand.NextDouble() * 2 - 1);
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

            public Wobble(Random rand, float length, float coarseStep)
            {
                this.coarseStep = coarseStep;
                this.fineStep = coarseStep / 4f;
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
                u = u * u * (3f - 2f * u); // なめらかに補間
                return values[i] + (values[i + 1] - values[i]) * u;
            }
        }
    }
}