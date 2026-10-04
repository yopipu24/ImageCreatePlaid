using SkiaSharp;

namespace ImageCreatePlaid
{
    public class RoundPatch1 : PlaidInterface
    {
        public SKBitmap EditImage(SKBitmap bmp, PlaidModel model)
        {
            int width = bmp.Width;
            int height = bmp.Height;

            // --- 調整用パラメータ ---
            float gapRatio = 0.12f;    // マス同士の隙間(マスの短い辺に対する割合。0〜0.3くらい)
            float cornerRatio = 0.28f; // 角の丸み(パッチの短い辺に対する割合。0=角ばる、0.5=円)
            // -----------------------

            var color1 = new SKColor(model.VerticalColorRed1, model.VerticalColorGreen1, model.VerticalColorBlue1, model.Alpha);
            var color2 = new SKColor(model.HorizontalColorRed2, model.HorizontalColorGreen2, model.HorizontalColorBlue2, model.Alpha);
            var gapColor = new SKColor(model.BaseColorRed, model.BaseColorGreen, model.BaseColorBlue, model.BaseAlpha);

            // マスの幅は VerticalSize1、高さは HorizontalSize1
            int cellW = Math.Max(4, model.VerticalSize1);
            int cellH = Math.Max(4, model.HorizontalSize1);

            float gap = Math.Min(cellW, cellH) * Math.Clamp(gapRatio, 0f, 0.4f);
            float patchW = cellW - gap;
            float patchH = cellH - gap;
            float radius = Math.Min(patchW, patchH) * Math.Clamp(cornerRatio, 0f, 0.5f);

            // 市松は2×2マスで1周期なので、これをタイルにして敷き詰める
            using SKBitmap tile = BussinessLogic.NewCreateImage(cellW * 2, cellH * 2);

            using (var canvas = new SKCanvas(tile))
            {
                canvas.Clear(gapColor);

                // 下地に合成せず、そのまま置き換えて描く(Alphaが低くても色が濁らない)
                using var paint = new SKPaint
                {
                    IsAntialias = true,
                    Style = SKPaintStyle.Fill,
                    BlendMode = SKBlendMode.Src
                };

                for (int j = 0; j < 2; j++)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        paint.Color = ((i + j) & 1) == 0 ? color1 : color2;

                        // マスの内側に、隙間の半分ずつ余白を取って角丸の四角を描く
                        canvas.DrawRoundRect(
                            i * cellW + gap / 2f,
                            j * cellH + gap / 2f,
                            patchW,
                            patchH,
                            radius,
                            radius,
                            paint);
                    }
                }
            }

            return BussinessLogic.RepeatImage(width, height, tile);
        }
    }
}