using SkiaSharp;
using System;

namespace ImageCreatePlaid
{
    public class VoronoiShatteredCheck : PlaidInterface
    {
        private static readonly Random _rnd = new Random();

        public SKBitmap EditImage(SKBitmap bmp, PlaidModel model)
        {
            int width = bmp.Width;   // 例: 2000
            int height = bmp.Height; // 例: 2000

            // 画面指定色のみを取得
            SKColor color0Base = new SKColor(model.BaseColorRed, model.BaseColorGreen, model.BaseColorBlue, model.BaseAlpha);
            SKColor color1V = new SKColor(model.VerticalColorRed1, model.VerticalColorGreen1, model.VerticalColorBlue1, model.Alpha);
            SKColor color2H = new SKColor(model.HorizontalColorRed2, model.HorizontalColorGreen2, model.HorizontalColorBlue2, model.Alpha);

            // 交差点の混色
            SKColor colorCross = BussinessLogic.CalcColor2(color1V);

            using SKCanvas canvas = new SKCanvas(bmp);
            canvas.Clear(color0Base);

            // 1マスのサイズ
            int cellSize = model.VerticalSize1 > 0 ? Math.Clamp(model.VerticalSize1, 20, 80) : 40;

            int cols = width / cellSize + 2;
            int rows = height / cellSize + 2;

            // --- 1. 格子点の座標配列を作成し、各交点にランダムなジッター（ゆらぎ）を加える ---
            SKPoint[,] points = new SKPoint[cols + 1, rows + 1];
            float maxJitter = cellSize * 0.45f; // 変形の強さ（最大 45% 歪ませる）

            for (int r = 0; r <= rows; r++)
            {
                for (int c = 0; c <= cols; c++)
                {
                    // 端の点は固定、内部の点だけを不規則にゆらす
                    float jitterX = (c == 0 || c == cols) ? 0 : (float)((_rnd.NextDouble() * 2 - 1) * maxJitter);
                    float jitterY = (r == 0 || r == rows) ? 0 : (float)((_rnd.NextDouble() * 2 - 1) * maxJitter);

                    points[c, r] = new SKPoint(c * cellSize + jitterX, r * cellSize + jitterY);
                }
            }

            using var fillPaint = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                IsAntialias = true
            };

            using var linePaint = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                Color = color0Base, // クラッシュした隙間の目地（割れ目）
                StrokeWidth = 2f,
                IsAntialias = true
            };

            // --- 2. ゆがんだ格子ポリゴン（四角形）を描画 ---
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    // 4つの変形頂点を取得
                    SKPoint p0 = points[c, r];         // 左上
                    SKPoint p1 = points[c + 1, r];     // 右上
                    SKPoint p2 = points[c + 1, r + 1]; // 右下
                    SKPoint p3 = points[c, r + 1];     // 左下

                    // チェックの役割判定
                    bool isV = c % 2 == 0;
                    bool isH = r % 2 == 0;

                    if (isV && isH) fillPaint.Color = colorCross;
                    else if (isV) fillPaint.Color = color1V;
                    else if (isH) fillPaint.Color = color2H;
                    else continue; // 背景色

                    // ゆがんだ四角形（ポリゴン）パスを作成
                    using var path = new SKPath();
                    path.MoveTo(p0);
                    path.LineTo(p1);
                    path.LineTo(p2);
                    path.LineTo(p3);
                    path.Close();

                    // 面の塗りと、クラッシュ感を出す割れ目線の描画
                    canvas.DrawPath(path, fillPaint);
                    canvas.DrawPath(path, linePaint);
                }
            }

            return bmp;
        }
    }
}