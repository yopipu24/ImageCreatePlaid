using SkiaSharp;
using System;

namespace ImageCreatePlaid
{
    public class CuteCheck2 : PlaidInterface
    {
        private static readonly Random _rnd = new Random();

        public SKBitmap EditImage(SKBitmap bmp, PlaidModel model)
        {
            int width = bmp.Width;   // 例: 2000
            int height = bmp.Height; // 例: 2000

            // 1マスのサイズ（ビスケット・レース柄は大きめのマス目が見映え◎）
            int cellSize = model.VerticalSize1 > 0 ? Math.Clamp(model.VerticalSize1, 20, 80) : 50;

            // 4x4マスで1リピートのタイルを作成
            int repeatCells = 4;
            int tileSize = cellSize * repeatCells;

            // --- 1. ランダムなバリエーション選択 ---
            // レースのフチ形状 (0: なみなみスカラップ, 1: クラシックステッチ, 2: ギザギザビスケット)
            int edgeStyle = _rnd.Next(0, 3);

            // ビスケットのくぼみドット配置 (0: 四隅に配置, 1: マス中心に配置, 2: 交差点リング)
            int pitStyle = _rnd.Next(0, 3);

            // --- 2. 画面指定色のみを取得 ---
            SKColor color0Base = new SKColor(model.BaseColorRed, model.BaseColorGreen, model.BaseColorBlue, model.BaseAlpha);
            SKColor color1V = new SKColor(model.VerticalColorRed1, model.VerticalColorGreen1, model.VerticalColorBlue1, model.Alpha);
            SKColor color2H = new SKColor(model.HorizontalColorRed2, model.HorizontalColorGreen2, model.HorizontalColorBlue2, model.Alpha);

            // 混色（手動計算）
            SKColor colorCross = BussinessLogic.CalcColor2(color1V);

            // --- 3. タイルの描画開始 ---
            using SKBitmap tileBmp = BussinessLogic.NewCreateImage(tileSize, tileSize);
            using SKCanvas tileCanvas = new SKCanvas(tileBmp);

            tileCanvas.Clear(color0Base);

            using var fillPaint = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                IsAntialias = true
            };

            // --- 4. ベース格子の描画 ---
            for (int y = 0; y < repeatCells; y++)
            {
                for (int x = 0; x < repeatCells; x++)
                {
                    SKRect cellRect = SKRect.Create(x * cellSize, y * cellSize, cellSize, cellSize);

                    if (x % 2 == 0 && y % 2 == 0)
                    {
                        fillPaint.Color = colorCross;
                        // 角を丸くしたビスケットクッキー風ベース
                        tileCanvas.DrawRoundRect(cellRect, cellSize * 0.2f, cellSize * 0.2f, fillPaint);
                    }
                    else if (x % 2 == 0)
                    {
                        fillPaint.Color = color1V;
                        tileCanvas.DrawRect(cellRect, fillPaint);
                    }
                    else if (y % 2 == 0)
                    {
                        fillPaint.Color = color2H;
                        tileCanvas.DrawRect(cellRect, fillPaint);
                    }
                }
            }

            // --- 5. レース・フチ（なみなみ/スカラップ）模様の描画 ---
            using var strokePaint = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                Color = colorCross,
                StrokeWidth = Math.Max(1.5f, cellSize * 0.04f),
                IsAntialias = true
            };

            if (edgeStyle == 0)
            {
                // 【なみなみスカラップ】円弧を連続して配置してレースのフチを表現
                float waveRadius = cellSize * 0.1f;
                int waveCount = (int)(cellSize / (waveRadius * 2));

                for (int i = 0; i < repeatCells; i++)
                {
                    for (int w = 0; w < waveCount * repeatCells; w++)
                    {
                        float cx = (w + 0.5f) * (waveRadius * 2);
                        float cy = i * cellSize;

                        // 縦横の格子ライン上に小さな半円（スカラップ）を描画
                        tileCanvas.DrawArc(
                            new SKRect(cx - waveRadius, cy - waveRadius, cx + waveRadius, cy + waveRadius),
                            0, 180, false, strokePaint);

                        tileCanvas.DrawArc(
                            new SKRect(cy - waveRadius, cx - waveRadius, cy + waveRadius, cx + waveRadius),
                            90, 180, false, strokePaint);
                    }
                }
            }
            else if (edgeStyle == 1)
            {
                // 【点々ステッチ】[3px描く, 3pxあける] 破線で細やかなレース縫い目
                strokePaint.PathEffect = SKPathEffect.CreateDash(new float[] { 3, 3 }, 0);

                for (int i = 0; i <= repeatCells; i++)
                {
                    int pos = i * cellSize;
                    tileCanvas.DrawLine(pos, 0, pos, tileSize, strokePaint);
                    tileCanvas.DrawLine(0, pos, tileSize, pos, strokePaint);
                }
            }

            // --- 6. ビスケットのくぼみドット（背景色抜き） ---
            fillPaint.Color = color0Base; // 背景色で繰り抜くように描画
            float pitRadius = cellSize * 0.08f;

            if (pitStyle == 0)
            {
                // 交差点の「四隅」に小さなクッキーのくぼみを配置
                float offset = cellSize * 0.25f;
                for (int y = 0; y < repeatCells; y++)
                {
                    for (int x = 0; x < repeatCells; x++)
                    {
                        if (x % 2 == 0 && y % 2 == 0)
                        {
                            float cx = (x + 0.5f) * cellSize;
                            float cy = (y + 0.5f) * cellSize;

                            tileCanvas.DrawCircle(cx - offset, cy - offset, pitRadius, fillPaint);
                            tileCanvas.DrawCircle(cx + offset, cy - offset, pitRadius, fillPaint);
                            tileCanvas.DrawCircle(cx - offset, cy + offset, pitRadius, fillPaint);
                            tileCanvas.DrawCircle(cx + offset, cy + offset, pitRadius, fillPaint);
                        }
                    }
                }
            }
            else if (pitStyle == 1)
            {
                // 空きマスの中心にビスケットホールを1つ配置
                for (int y = 0; y < repeatCells; y++)
                {
                    for (int x = 0; x < repeatCells; x++)
                    {
                        if (x % 2 != 0 && y % 2 != 0)
                        {
                            float cx = (x + 0.5f) * cellSize;
                            float cy = (y + 0.5f) * cellSize;
                            tileCanvas.DrawCircle(cx, cy, pitRadius * 1.5f, fillPaint);
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