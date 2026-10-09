using SkiaSharp;
using System;

namespace ImageCreatePlaid
{
    public class GlitchMosaicCheck2 : PlaidInterface
    {
        private static readonly Random _rnd = new Random();

        public SKBitmap EditImage(SKBitmap bmp, PlaidModel model)
        {
            int width = bmp.Width;   // 例: 2000
            int height = bmp.Height; // 例: 2000

            // --- 1. 1マスの最小Unit（px）---
            // グリッドの解像度となる基準ユニットサイズ（例: 8px〜20px）
            int unitSize = model.VerticalSize1 > 0 ? Math.Min(model.VerticalSize1, 20) : 12;

            // 1タイルのマス数（例: 16x16 ユニットで1リピートのタイルを作成）
            int tileUnitsX = 16;
            int tileUnitsY = 16;

            int tileWidth = unitSize * tileUnitsX;
            int tileHeight = unitSize * tileUnitsY;

            // --- 2. X軸・Y軸のベースチェック柄パターンをランダム生成 ---
            // 規則的な交互描画ではなく、不規則な太さと間隔を持つライン配列(bool[])を作成
            bool[] lineX = CreateRandomStripePattern(tileUnitsX, _rnd);
            bool[] lineY = CreateRandomStripePattern(tileUnitsY, _rnd);

            // --- 3. カラー設定 ---
            SKColor color0Base = new SKColor(model.BaseColorRed, model.BaseColorGreen, model.BaseColorBlue, model.BaseAlpha);
            SKColor color1V = new SKColor(model.VerticalColorRed1, model.VerticalColorGreen1, model.VerticalColorBlue1, model.Alpha);
            SKColor color2H = new SKColor(model.HorizontalColorRed2, model.HorizontalColorGreen2, model.HorizontalColorBlue2, model.Alpha);

            // 混色（手動計算）
            SKColor colorCross = BussinessLogic.CalcColor2(color1V);

            // --- 4. タイルの描画 ---
            using SKBitmap tileBmp = BussinessLogic.NewCreateImage(tileWidth, tileHeight);
            using SKCanvas tileCanvas = new SKCanvas(tileBmp);

            // ベース背景クリア
            tileCanvas.Clear(color0Base);

            using var paint = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                IsAntialias = false
            };

            // --- 5. ランダムベースチェックの描画 ---
            for (int y = 0; y < tileUnitsY; y++)
            {
                for (int x = 0; x < tileUnitsX; x++)
                {
                    bool isV = lineX[x]; // 縦線の有無
                    bool isH = lineY[y]; // 横線の有無

                    // 線の交差判定による色塗り
                    if (isV && isH)
                    {
                        paint.Color = colorCross; // 交点
                    }
                    else if (isV)
                    {
                        paint.Color = color1V;     // 縦ストライプ
                    }
                    else if (isH)
                    {
                        paint.Color = color2H;     // 横ストライプ
                    }
                    else
                    {
                        continue; // 背景色（Clearで塗られているためスキップ）
                    }

                    SKRect unitRect = SKRect.Create(x * unitSize, y * unitSize, unitSize, unitSize);
                    tileCanvas.DrawRect(unitRect, paint);

                    // --- 6. グリッチ・ノイズの割り込み（確率 10%〜15%） ---
                    if (_rnd.Next(100) < 12)
                    {
                        // HSLで鮮やかな色をランダム生成
                        float hue = _rnd.Next(0, 360);
                        SKColor randomGlitchColor = SKColor.FromHsl(hue, 85, 70);

                        // アルファ値を低く（50〜110）して薄く透けさせる
                        byte thinAlpha = (byte)_rnd.Next(50, 110);
                        paint.Color = randomGlitchColor.WithAlpha(thinAlpha);

                        // グリッチのサイズ（1〜3ユニット幅でランダム）
                        int gWidthUnits = _rnd.Next(10) > 6 ? _rnd.Next(2, 4) : 1;
                        int gHeightUnits = _rnd.Next(10) > 8 ? _rnd.Next(2, 4) : 1;

                        SKRect glitchRect = SKRect.Create(
                            x * unitSize,
                            y * unitSize,
                            unitSize * gWidthUnits,
                            unitSize * gHeightUnits);

                        tileCanvas.DrawRect(glitchRect, paint);
                    }
                }
            }

            // --- 7. 全体キャンバスへタイリング ---
            bmp = BussinessLogic.RepeatImage(width, height, tileBmp);

            return bmp;
        }

        /// <summary>
        /// 太さや間隔がランダムな不規則ストライプ配列(bool[])を生成するメソッド
        /// </summary>
        private static bool[] CreateRandomStripePattern(int length, Random rnd)
        {
            bool[] pattern = new bool[length];
            int pos = rnd.Next(0, 3); // 開始オフセット

            while (pos < length)
            {
                // 線の太さ（1〜3ユニット）をランダム決定
                int stripeWidth = rnd.Next(1, 4);

                for (int i = 0; i < stripeWidth && (pos + i) < length; i++)
                {
                    pattern[pos + i] = true;
                }
                pos += stripeWidth;

                // 線と線の間隔・余白（1〜4ユニット）をランダム決定
                int gap = rnd.Next(1, 5);
                pos += gap;
            }

            return pattern;
        }
    }
}