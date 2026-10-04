using SkiaSharp;

namespace ImageCreatePlaid
{
    public class RoundPatch2 : PlaidInterface
    {
        public SKBitmap EditImage(SKBitmap bmp, PlaidModel model)
        {
            int width = bmp.Width;
            int height = bmp.Height;

            // --- 調整用パラメータ ---
            float randomness = 1f;     // ばらつき全体の強さ(0=なし、1=標準、2=強め)
            float gapRatio = 0.12f;    // マス同士の隙間(マスの短い辺に対する割合。0〜0.3くらい)
            float cornerRatio = 0.28f; // 角の丸み(パッチの短い辺に対する割合。0=角ばる、0.5=円)

            // 以下は randomness=1 のときのばらつき幅
            float posJitter = 0.04f;    // 位置のずれ(マスの短い辺に対する割合)
            float sizeJitter = 0.08f;   // 大きさのばらつき(±割合。縦横別々)
            float rotateJitter = 2.5f;  // 傾き(±度)
            float cornerJitter = 0.15f; // 角の丸みのばらつき(±)
            float toneJitter = 0.06f;   // 色の濃淡のばらつき(±割合)
            float swapRate = 0.08f;     // マスの色が、市松と逆になる確率
            float skipRate = 0f;        // パッチが欠ける確率(0〜0.2くらい。ベース色の穴になる)
            // -----------------------

            var rand = new Random();

            var color1 = new SKColor(model.VerticalColorRed1, model.VerticalColorGreen1, model.VerticalColorBlue1, model.Alpha);
            var color2 = new SKColor(model.HorizontalColorRed2, model.HorizontalColorGreen2, model.HorizontalColorBlue2, model.Alpha);
            var gapColor = new SKColor(model.BaseColorRed, model.BaseColorGreen, model.BaseColorBlue, model.BaseAlpha);

            // マスの幅は VerticalSize1、高さは HorizontalSize1
            int cellW = Math.Max(4, model.VerticalSize1);
            int cellH = Math.Max(4, model.HorizontalSize1);
            float minCell = Math.Min(cellW, cellH);

            float gap = minCell * Math.Clamp(gapRatio, 0f, 0.4f);
            float baseW = cellW - gap;
            float baseH = cellH - gap;

            using var canvas = new SKCanvas(bmp);
            canvas.Clear(gapColor);

            // 下地に合成せず、そのまま置き換えて描く(Alphaが低くても色が濁らない)
            using var paint = new SKPaint
            {
                IsAntialias = true,
                Style = SKPaintStyle.Fill,
                BlendMode = SKBlendMode.Src
            };

            int cols = (int)Math.Ceiling(width / (float)cellW);
            int rows = (int)Math.Ceiling(height / (float)cellH);

            for (int j = 0; j < rows; j++)
            {
                for (int i = 0; i < cols; i++)
                {
                    // パッチが欠ける
                    if (rand.NextDouble() < skipRate) continue;

                    // 市松の色。たまに逆の色になる
                    bool useColor1 = ((i + j) & 1) == 0;
                    if (rand.NextDouble() < swapRate * randomness)
                    {
                        useColor1 = !useColor1;
                    }

                    paint.Color = Tone(useColor1 ? color1 : color2, Random11(rand) * toneJitter * randomness);

                    float w = baseW * (1f + Random11(rand) * sizeJitter * randomness);
                    float h = baseH * (1f + Random11(rand) * sizeJitter * randomness);
                    float cx = (i + 0.5f) * cellW + Random11(rand) * posJitter * minCell * randomness;
                    float cy = (j + 0.5f) * cellH + Random11(rand) * posJitter * minCell * randomness;
                    float angle = Random11(rand) * rotateJitter * randomness;
                    float corner = Math.Clamp(cornerRatio + Random11(rand) * cornerJitter * randomness, 0f, 0.5f);
                    float radius = Math.Min(w, h) * corner;

                    // パッチの中心を原点にして、傾けてから描く
                    canvas.Save();
                    canvas.Translate(cx, cy);
                    canvas.RotateDegrees(angle);
                    canvas.DrawRoundRect(-w / 2f, -h / 2f, w, h, radius, radius, paint);
                    canvas.Restore();
                }
            }

            return bmp;
        }

        // 色の濃淡を変える。t が正なら白に近づけ、負なら黒に近づける(-1〜1)
        private static SKColor Tone(SKColor c, float t)
        {
            t = Math.Clamp(t, -1f, 1f);

            if (t >= 0f)
            {
                return new SKColor(
                    (byte)(c.Red + (255 - c.Red) * t),
                    (byte)(c.Green + (255 - c.Green) * t),
                    (byte)(c.Blue + (255 - c.Blue) * t),
                    c.Alpha);
            }

            float f = 1f + t;
            return new SKColor((byte)(c.Red * f), (byte)(c.Green * f), (byte)(c.Blue * f), c.Alpha);
        }

        // -1〜1の乱数
        private static float Random11(Random rand)
        {
            return (float)(rand.NextDouble() * 2 - 1);
        }
    }
}