using SkiaSharp;
using System;

namespace ImageCreatePlaid
{
    public class GlitchMosaicCheck1 : PlaidInterface
    {
        private static readonly Random _rnd = new Random();

        public SKBitmap EditImage(SKBitmap bmp, PlaidModel model)
        {
            int width = bmp.Width;   // 例: 2000
            int height = bmp.Height; // 例: 2000

            // --- 1. 1マスのサイズ（px）を調整 ---
            int cellSizeV = model.VerticalSize1 > 0 ? Math.Min(model.VerticalSize1, 40) : 20;
            int cellSizeH = model.HorizontalSize1 > 0 ? Math.Min(model.HorizontalSize1, 40) : 20;

            // --- 2. 1タイルのマス数（グリッド数）を設定 ---
            int repeatCellsX = 10;
            int repeatCellsY = 10;

            int tileWidth = cellSizeV * repeatCellsX;   // 例: 200px
            int tileHeight = cellSizeH * repeatCellsY;  // 例: 200px

            // --- 3. ベース・格子カラーの設定 ---
            SKColor color0Base = new SKColor(model.BaseColorRed, model.BaseColorGreen, model.BaseColorBlue, model.BaseAlpha);
            SKColor color1V = new SKColor(model.VerticalColorRed1, model.VerticalColorGreen1, model.VerticalColorBlue1, model.Alpha);
            SKColor color2H = new SKColor(model.HorizontalColorRed2, model.HorizontalColorGreen2, model.HorizontalColorBlue2, model.Alpha);

            SKColor colorCross = BussinessLogic.CalcColor2(color1V);

            // --- 4. タイルの描画 ---
            using SKBitmap tileBmp = BussinessLogic.NewCreateImage(tileWidth, tileHeight);
            using SKCanvas tileCanvas = new SKCanvas(tileBmp);

            tileCanvas.Clear(color0Base);

            using var paint = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                IsAntialias = false
            };

            for (int y = 0; y < repeatCellsY; y++)
            {
                for (int x = 0; x < repeatCellsX; x++)
                {
                    SKRect cellRect = SKRect.Create(x * cellSizeV, y * cellSizeH, cellSizeV, cellSizeH);

                    // ベース格子の塗り分け
                    if (x % 2 == 0 && y % 2 == 0)
                    {
                        paint.Color = colorCross;
                    }
                    else if (x % 2 == 0)
                    {
                        paint.Color = color1V;
                    }
                    else if (y % 2 == 0)
                    {
                        paint.Color = color2H;
                    }
                    else
                    {
                        continue; // ベース色
                    }

                    tileCanvas.DrawRect(cellRect, paint);

                    // --- 5. 【修正】グリッチ色のランダム化 & 薄め（アルファ下げ）描画 ---
                    if (_rnd.Next(100) < 12) // 確率 12%
                    {
                        // HSLカラー空間を使って「明るく彩度の高い色」をランダム生成
                        // 色相(Hue): 0~360度（完全ランダム）
                        // 彩度(S): 70%~100%, 輝度(L): 60%~80%（濁らず綺麗な色合いに）
                        float hue = _rnd.Next(0, 360);
                        float saturation = 0.8f;
                        float lightness = 0.7f;

                        SKColor randomGlitchColor = SKColor.FromHsl(hue, saturation * 100, lightness * 100);

                        // 【ポイント】アルファ値を低め（例: 80〜120 / 255）にして薄く透けさせる
                        // model.Alpha などを基準にして比率調整してもOKです
                        byte thinAlpha = (byte)_rnd.Next(70, 120);
                        paint.Color = randomGlitchColor.WithAlpha(thinAlpha);

                        // 1マス、または横・縦に伸びるグリッチサイズ
                        int gWidthMultiplier = _rnd.Next(10) > 7 ? 2 : 1;
                        int gHeightMultiplier = _rnd.Next(10) > 8 ? 2 : 1;

                        SKRect glitchRect = SKRect.Create(
                            x * cellSizeV,
                            y * cellSizeH,
                            cellSizeV * gWidthMultiplier,
                            cellSizeH * gHeightMultiplier);

                        tileCanvas.DrawRect(glitchRect, paint);
                    }
                }
            }

            // --- 6. タイルを敷き詰める ---
            bmp = BussinessLogic.RepeatImage(width, height, tileBmp);

            return bmp;
        }
    }
}