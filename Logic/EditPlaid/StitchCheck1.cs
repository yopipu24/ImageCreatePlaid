using SkiaSharp;

namespace ImageCreatePlaid
{
    public class StitchCheck1 : PlaidInterface
    {
        public SKBitmap EditImage(SKBitmap bmp, PlaidModel model)
        {
            int width = bmp.Width;
            int height = bmp.Height;

            // --- 調整用パラメータ ---
            float insetRatio = 0.14f;     // 縫い目の位置(マスの端からの距離。短い辺に対する割合)
            float cornerRatio = 0.12f;    // 縫い目の四角の角の丸み(0〜0.5)
            float lineWidthRatio = 0.04f; // 糸の太さ(マスの短い辺に対する割合)
            float stitchLen = 3f;         // 1針の長さ(糸の太さに対する倍率)
            // -----------------------

            var color1 = new SKColor(model.VerticalColorRed1, model.VerticalColorGreen1, model.VerticalColorBlue1, model.Alpha);
            var color2 = new SKColor(model.HorizontalColorRed2, model.HorizontalColorGreen2, model.HorizontalColorBlue2, model.Alpha);
            var threadColor = new SKColor(model.BaseColorRed, model.BaseColorGreen, model.BaseColorBlue, model.BaseAlpha);

            // マスの幅は VerticalSize1、高さは HorizontalSize1
            int cellW = Math.Max(8, model.VerticalSize1);
            int cellH = Math.Max(8, model.HorizontalSize1);
            float minCell = Math.Min(cellW, cellH);

            float inset = minCell * insetRatio;
            float lineWidth = Math.Max(1f, minCell * lineWidthRatio);

            // 縫い目の四角(マスの内側)
            float w = cellW - inset * 2f;
            float h = cellH - inset * 2f;
            float r = Math.Min(w, h) * Math.Clamp(cornerRatio, 0f, 0.5f);

            // 一周の長さに合わせて点線の間隔を調整し、1周でちょうど繋がるようにする
            float perimeter = 2f * (w + h) - 8f * r + 2f * MathF.PI * r;
            float targetPeriod = lineWidth * stitchLen * 1.8f;
            int stitchCount = Math.Max(4, (int)MathF.Round(perimeter / targetPeriod));
            float period = perimeter / stitchCount;

            // 丸い線端の分だけ線を短くして、見た目の針の長さを period の約55%にする
            float on = Math.Max(0.01f, period * 0.55f - lineWidth);
            float off = period - on;

            // 市松は2×2マスで1周期なので、これをタイルにして敷き詰める
            using SKBitmap tile = BussinessLogic.NewCreateImage(cellW * 2, cellH * 2);

            using (var canvas = new SKCanvas(tile))
            {
                canvas.Clear(color2);

                // 下地に合成せず、そのまま置き換えて描く(Alphaが低くても色が濁らない)
                using var fill = new SKPaint
                {
                    IsAntialias = true,
                    Style = SKPaintStyle.Fill,
                    BlendMode = SKBlendMode.Src,
                    Color = color1
                };

                // 1色目のマス(左上と右下)
                canvas.DrawRect(0, 0, cellW, cellH, fill);
                canvas.DrawRect(cellW, cellH, cellW, cellH, fill);

                // 糸(点線)
                using var dash = SKPathEffect.CreateDash(new[] { on, off }, 0f);
                using var thread = new SKPaint
                {
                    IsAntialias = true,
                    Style = SKPaintStyle.Stroke,
                    StrokeWidth = lineWidth,
                    StrokeCap = SKStrokeCap.Round,
                    BlendMode = SKBlendMode.Src,
                    Color = threadColor,
                    PathEffect = dash
                };

                // すべてのマスの内側に縫い目を入れる
                for (int j = 0; j < 2; j++)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        canvas.DrawRoundRect(i * cellW + inset, j * cellH + inset, w, h, r, r, thread);
                    }
                }
            }

            return BussinessLogic.RepeatImage(width, height, tile);
        }
    }
}