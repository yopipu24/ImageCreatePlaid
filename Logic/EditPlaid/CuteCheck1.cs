using SkiaSharp;
using System;

namespace ImageCreatePlaid
{
    public class CuteCheck1 : PlaidInterface
    {
        private static readonly Random _rnd = new Random();

        public SKBitmap EditImage(SKBitmap bmp, PlaidModel model)
        {
            int width = bmp.Width;   // 例: 2000
            int height = bmp.Height; // 例: 2000

            // --- 1. ランダムな構造パラメータの生成 ---
            // 1マスの基本サイズ
            int baseCellSize = model.VerticalSize1 > 0 ? Math.Clamp(model.VerticalSize1, 15, 80) : 40;

            // 角丸の強さ（0.0: 直角 〜 0.4: かなり丸い）
            float roundRatio = (float)(_rnd.NextDouble() * 0.35 + 0.05);

            // ステッチ（縫い目破線）のパターン選定 (0:なし, 1:細かいステッチ, 2:大きめステッチ)
            int stitchType = _rnd.Next(0, 3);

            // 交差点ドットの配置タイプ (0:なし, 1:交差点中心にドット, 2:マスの中心にドット)
            int dotType = _rnd.Next(0, 3);

            // ドットのサイズ（小〜中）
            float dotRadiusRatio = (float)(_rnd.NextDouble() * 0.12 + 0.06);

            // 4x4マスで1リピートのタイルサイズ
            int repeatCells = 4;
            int tileSize = baseCellSize * repeatCells;

            // --- 2. 画面側(model)指定色のみを取得 ---
            SKColor color0Base = new SKColor(model.BaseColorRed, model.BaseColorGreen, model.BaseColorBlue, model.BaseAlpha);
            SKColor color1V = new SKColor(model.VerticalColorRed1, model.VerticalColorGreen1, model.VerticalColorBlue1, model.Alpha);
            SKColor color2H = new SKColor(model.HorizontalColorRed2, model.HorizontalColorGreen2, model.HorizontalColorBlue2, model.Alpha);

            // 混色（画面指定色から計算）
            SKColor colorCross = BussinessLogic.CalcColor2(color1V);

            // --- 3. タイルの描画開始 ---
            using SKBitmap tileBmp = BussinessLogic.NewCreateImage(tileSize, tileSize);
            using SKCanvas tileCanvas = new SKCanvas(tileBmp);

            // 背景色でクリア
            tileCanvas.Clear(color0Base);

            using var fillPaint = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                IsAntialias = true // 滑らかな丸みのためにON
            };

            // --- 4. ランダム角丸格子の描画 ---
            float cornerRadius = baseCellSize * roundRatio;

            for (int y = 0; y < repeatCells; y++)
            {
                for (int x = 0; x < repeatCells; x++)
                {
                    SKRect cellRect = SKRect.Create(x * baseCellSize, y * baseCellSize, baseCellSize, baseCellSize);

                    if (x % 2 == 0 && y % 2 == 0)
                    {
                        // 交差点：ランダムに決定された角丸を適用
                        fillPaint.Color = colorCross;
                        tileCanvas.DrawRoundRect(cellRect, cornerRadius, cornerRadius, fillPaint);
                    }
                    else if (x % 2 == 0)
                    {
                        // 縦線
                        fillPaint.Color = color1V;
                        tileCanvas.DrawRect(cellRect, fillPaint);
                    }
                    else if (y % 2 == 0)
                    {
                        // 横線
                        fillPaint.Color = color2H;
                        tileCanvas.DrawRect(cellRect, fillPaint);
                    }
                }
            }

            // --- 5. ランダムステッチ（破線）の描画 ---
            if (stitchType > 0)
            {
                float[] dashPattern = stitchType == 1
                    ? new float[] { 3, 3 }  // 細かい繊細なステッチ
                    : new float[] { 6, 4 }; // しっかりした縫い目ステッチ

                using var dashPaint = new SKPaint
                {
                    Style = SKPaintStyle.Stroke,
                    Color = colorCross, // 画面指定の交差点色を使用
                    StrokeWidth = Math.Max(1.5f, baseCellSize * 0.04f),
                    IsAntialias = true,
                    PathEffect = SKPathEffect.CreateDash(dashPattern, 0)
                };

                for (int i = 0; i <= repeatCells; i++)
                {
                    int pos = i * baseCellSize;
                    tileCanvas.DrawLine(pos, 0, pos, tileSize, dashPaint); // 縦ステッチ
                    tileCanvas.DrawLine(0, pos, tileSize, pos, dashPaint); // 横ステッチ
                }
            }

            // --- 6. ランダムドット（丸）の描画 ---
            if (dotType > 0)
            {
                // 背景色(color0Base)を使ってドットを描画（色追加なし）
                fillPaint.Color = color0Base;
                float dotRadius = baseCellSize * dotRadiusRatio;

                if (dotType == 1)
                {
                    // パターンA: 格子の交差点の中心にドットを配置
                    for (int y = 0; y <= repeatCells; y++)
                    {
                        for (int x = 0; x <= repeatCells; x++)
                        {
                            tileCanvas.DrawCircle(x * baseCellSize, y * baseCellSize, dotRadius, fillPaint);
                        }
                    }
                }
                else if (dotType == 2)
                {
                    // パターンB: 空きマス（ベース色の領域）の中心にドットを配置
                    for (int y = 0; y < repeatCells; y++)
                    {
                        for (int x = 0; x < repeatCells; x++)
                        {
                            if (x % 2 != 0 && y % 2 != 0)
                            {
                                float cx = (x + 0.5f) * baseCellSize;
                                float cy = (y + 0.5f) * baseCellSize;
                                tileCanvas.DrawCircle(cx, cy, dotRadius, fillPaint);
                            }
                        }
                    }
                }
            }

            // --- 7. 全体キャンバスへタイリング ---
            bmp = BussinessLogic.RepeatImage(width, height, tileBmp);

            return bmp;
        }
    }
}