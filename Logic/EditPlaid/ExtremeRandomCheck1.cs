using SkiaSharp;
using System;

namespace ImageCreatePlaid
{
    public class ExtremeRandomCheck1 : PlaidInterface
    {
        private static readonly Random _rnd = new Random();

        public SKBitmap EditImage(SKBitmap bmp, PlaidModel model)
        {
            int width = bmp.Width;   // 例: 2000
            int height = bmp.Height; // 例: 2000

            // 1マスの基準サイズ
            int cellSize = model.VerticalSize1 > 0 ? Math.Clamp(model.VerticalSize1, 15, 60) : 30;

            // 8x8 マスの大きめリピート単位（ランダム性を際立たせるため）
            int gridCount = 8;
            int tileSize = cellSize * gridCount;

            // --- 1. 画面指定色のみを取得 ---
            SKColor color0Base = new SKColor(model.BaseColorRed, model.BaseColorGreen, model.BaseColorBlue, model.BaseAlpha);
            SKColor color1V = new SKColor(model.VerticalColorRed1, model.VerticalColorGreen1, model.VerticalColorBlue1, model.Alpha);
            SKColor color2H = new SKColor(model.HorizontalColorRed2, model.HorizontalColorGreen2, model.HorizontalColorBlue2, model.Alpha);

            // 混色（画面指定色から計算）
            SKColor colorCross = BussinessLogic.CalcColor2(color1V);

            // --- 2. 行・列ごとのランダムズレ（Shift）量の生成 ---
            int[] xOffsets = new int[gridCount];
            int[] yOffsets = new int[gridCount];

            for (int i = 0; i < gridCount; i++)
            {
                // 各行・各列を 0〜cellSize*1.5 程度ランダムにズラす
                xOffsets[i] = _rnd.Next(-cellSize, cellSize);
                yOffsets[i] = _rnd.Next(-cellSize, cellSize);
            }

            // --- 3. タイルの描画開始 ---
            using SKBitmap tileBmp = BussinessLogic.NewCreateImage(tileSize, tileSize);
            using SKCanvas tileCanvas = new SKCanvas(tileBmp);

            tileCanvas.Clear(color0Base);

            using var fillPaint = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                IsAntialias = true
            };

            // --- 4. ズレを伴う壊れた格子の描画 ---
            for (int y = 0; y < gridCount; y++)
            {
                for (int x = 0; x < gridCount; x++)
                {
                    // ランダムズレ（Shift）を足した座標
                    float posX = x * cellSize + xOffsets[y];
                    float posY = y * cellSize + yOffsets[x];

                    SKRect cellRect = SKRect.Create(posX, posY, cellSize, cellSize);

                    // 基本の交差判定
                    bool isV = x % 2 == 0;
                    bool isH = y % 2 == 0;

                    if (isV && isH) fillPaint.Color = colorCross;
                    else if (isV) fillPaint.Color = color1V;
                    else if (isH) fillPaint.Color = color2H;
                    else continue;

                    // --- ランダム変形パターン ---
                    int transformType = _rnd.Next(100);

                    if (transformType < 40)
                    {
                        // 普通の矩形（ズレているため四角が断裂して見えます）
                        tileCanvas.DrawRect(cellRect, fillPaint);
                    }
                    else if (transformType < 70)
                    {
                        // 三角形に斜め分割（対角線でカットした現代アート風格子）
                        using var path = new SKPath();
                        if (_rnd.Next(2) == 0)
                        {
                            path.MoveTo(cellRect.Left, cellRect.Top);
                            path.LineTo(cellRect.Right, cellRect.Top);
                            path.LineTo(cellRect.Left, cellRect.Bottom);
                        }
                        else
                        {
                            path.MoveTo(cellRect.Right, cellRect.Top);
                            path.LineTo(cellRect.Right, cellRect.Bottom);
                            path.LineTo(cellRect.Left, cellRect.Bottom);
                        }
                        path.Close();
                        tileCanvas.DrawPath(path, fillPaint);
                    }
                    else if (transformType < 90)
                    {
                        // 円形・ドット状にくり抜かれた格子ブロック
                        tileCanvas.DrawRoundRect(cellRect, cellSize * 0.4f, cellSize * 0.4f, fillPaint);
                    }
                    else
                    {
                        // 隣のマスへ大きく伸びるノイズブロック（幾何学ストライプ風）
                        SKRect extendedRect = SKRect.Create(posX, posY, cellSize * _rnd.Next(2, 4), cellSize);
                        tileCanvas.DrawRect(extendedRect, fillPaint);
                    }
                }
            }

            // --- 5. 仕上げ: 全体にランダムな極細のスリット（断裂線）を差し込む ---
            using var linePaint = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                Color = color0Base, // 背景色でカット
                StrokeWidth = 2,
                IsAntialias = true
            };

            int slitCount = _rnd.Next(3, 8);
            for (int i = 0; i < slitCount; i++)
            {
                float slitX = _rnd.Next(0, tileSize);
                float slitY = _rnd.Next(0, tileSize);

                // 垂直・水平に切れ目を入れる
                if (_rnd.Next(2) == 0)
                    tileCanvas.DrawLine(slitX, 0, slitX, tileSize, linePaint);
                else
                    tileCanvas.DrawLine(0, slitY, tileSize, slitY, linePaint);
            }

            // --- 6. 全体キャンバスへタイリング ---
            bmp = BussinessLogic.RepeatImage(width, height, tileBmp);

            return bmp;
        }
    }
}