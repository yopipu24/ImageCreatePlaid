using SkiaSharp;

namespace ImageCreatePlaid
{
    public class CheckDot1 : PlaidInterface
    {
        public SKBitmap EditImage(SKBitmap bmp, PlaidModel model)
        {
            int width = bmp.Width;
            int height = bmp.Height;

            // --- 調整用パラメータ ---
            float dotRatio = 0.45f;  // ドットの直径(マスの短い辺に対する割合。0.3〜0.7くらい)
            bool dotAll = false;     // true: 両方のマスにドット(色は反転) / false: 2色目のマスだけ
            // -----------------------

            var color1 = new SKColor(model.VerticalColorRed1, model.VerticalColorGreen1, model.VerticalColorBlue1, model.Alpha);
            var color2 = new SKColor(model.HorizontalColorRed2, model.HorizontalColorGreen2, model.HorizontalColorBlue2, model.Alpha);

            // マスの幅は VerticalSize1、高さは HorizontalSize1
            int cellW = Math.Max(2, model.VerticalSize1);
            int cellH = Math.Max(2, model.HorizontalSize1);
            float radius = Math.Min(cellW, cellH) * dotRatio / 2f;

            // 市松は2×2マスで1周期なので、これをタイルにして敷き詰める
            using SKBitmap tile = BussinessLogic.NewCreateImage(cellW * 2, cellH * 2);

            using (var canvas = new SKCanvas(tile))
            {
                canvas.Clear(color2);

                // 下地に合成せず、そのまま置き換えて描く(Alphaが低くても色が濁らない)
                using var paint = new SKPaint
                {
                    IsAntialias = true,
                    Style = SKPaintStyle.Fill,
                    BlendMode = SKBlendMode.Src
                };

                // 1色目のマス(左上と右下)
                paint.Color = color1;
                canvas.DrawRect(0, 0, cellW, cellH, paint);
                canvas.DrawRect(cellW, cellH, cellW, cellH, paint);

                // 2色目のマス(右上と左下)の中心に、1色目のドット
                canvas.DrawCircle(cellW * 1.5f, cellH * 0.5f, radius, paint);
                canvas.DrawCircle(cellW * 0.5f, cellH * 1.5f, radius, paint);

                // 1色目のマスにも、2色目のドット
                if (dotAll)
                {
                    paint.Color = color2;
                    canvas.DrawCircle(cellW * 0.5f, cellH * 0.5f, radius, paint);
                    canvas.DrawCircle(cellW * 1.5f, cellH * 1.5f, radius, paint);
                }
            }

            return BussinessLogic.RepeatImage(width, height, tile);
        }
    }
}