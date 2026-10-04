using SkiaSharp;

namespace ImageCreatePlaid
{
    public class PolkaDot2 : PlaidInterface
    {
        public SKBitmap EditImage(SKBitmap bmp, PlaidModel model)
        {
            int width = bmp.Width;
            int height = bmp.Height;

            var color1 = new SKColor(model.VerticalColorRed1, model.VerticalColorGreen1, model.VerticalColorBlue1, model.Alpha);
            var color2 = new SKColor(model.HorizontalColorRed2, model.HorizontalColorGreen2, model.HorizontalColorBlue2, model.Alpha);

            // --- 調整用パラメータ(慣れたらPlaidModelに移すと便利) ---
            float jitter = 0.2f;       // 位置のゆらぎ(セルサイズに対する割合 0〜0.3くらい)
            float minScale = 0.5f;     // ドットの最小サイズ倍率
            float maxScale = 1.0f;     // ドットの最大サイズ倍率
            float skipRate = 0.1f;     // ドットを間引く確率(0で全セルに配置)
            float color1Rate = 0.5f;   // color1を使う確率(残りはcolor2)
            bool stagger = true;       // 1行おきに半セルずらす(千鳥配置)
            // -----------------------------------------------------

            var rand = new Random();

            float cellW = model.HorizontalSize1;
            float cellH = model.VerticalSize1;

            // ゆらぎを足しても、ほぼ重ならない最大半径
            float maxRx = cellW * (0.5f - jitter);
            float maxRy = cellH * (0.5f - jitter);

            using var canvas = new SKCanvas(bmp);
            canvas.Clear(new SKColor(model.BaseColorRed, model.BaseColorGreen, model.BaseColorBlue, model.BaseAlpha));

            using var paint = new SKPaint
            {
                IsAntialias = true,
                Style = SKPaintStyle.Fill
            };

            int cols = (int)Math.Ceiling(width / cellW) + 1;
            int rows = (int)Math.Ceiling(height / cellH) + 1;

            // -1 から始めて、端で切れるドットも描く
            for (int j = -1; j < rows; j++)
            {
                float rowOffset = (stagger && (j & 1) != 0) ? cellW / 2f : 0f;

                for (int i = -1; i < cols; i++)
                {
                    // 乱数は先にすべて引く(skipRateを変えても他のドットの配置が変わらないように)
                    double skip = rand.NextDouble();
                    float jx = (float)(rand.NextDouble() * 2 - 1) * jitter * cellW;
                    float jy = (float)(rand.NextDouble() * 2 - 1) * jitter * cellH;
                    float scale = minScale + (float)rand.NextDouble() * (maxScale - minScale);
                    bool useColor1 = rand.NextDouble() < color1Rate;

                    if (skip < skipRate) continue;

                    float cx = (i + 0.5f) * cellW + rowOffset + jx;
                    float cy = (j + 0.5f) * cellH + jy;

                    paint.Color = useColor1 ? color1 : color2;
                    canvas.DrawOval(cx, cy, maxRx * scale, maxRy * scale, paint);
                }
            }

            return bmp;
        }
    }
}