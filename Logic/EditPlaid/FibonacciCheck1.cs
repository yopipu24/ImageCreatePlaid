using SkiaSharp;
using System;
using System.Collections.Generic;

namespace ImageCreatePlaid
{
    public class FibonacciCheck1 : PlaidInterface
    {
        private static readonly Random _rnd = new Random();

        // 基本となるフィボナッチ数列のパターン（スケール調整用）
        private static readonly int[] FibonacciSequence = new[] { 1, 1, 2, 3, 5, 8, 13, 21, 13, 8, 5, 3, 2, 1 };

        public SKBitmap EditImage(SKBitmap bmp, PlaidModel model)
        {
            int width = bmp.Width;   // 例: 2000
            int height = bmp.Height; // 例: 2000

            // 1ユニット（最小単位）のpxサイズ（例: 4px〜12px程度）
            int unitSize = model.VerticalSize1 > 0 ? Math.Max(2, model.VerticalSize1 / 10) : 6;

            // --- 1. フィボナッチ数列に基づくラインパターンの生成 ---
            // X軸（縦線）とY軸（横線）で開始位置や反転（ミラーリング）を変えて不規則性をプラス
            bool[] lineX = CreateFibonacciPattern(out int totalUnitsX, reverse: false);
            bool[] lineY = CreateFibonacciPattern(out int totalUnitsY, reverse: true);

            int tileWidth = totalUnitsX * unitSize;
            int tileHeight = totalUnitsY * unitSize;

            // --- 2. カラー設定 ---
            SKColor color0Base = new SKColor(model.BaseColorRed, model.BaseColorGreen, model.BaseColorBlue, model.BaseAlpha);
            SKColor color1V = new SKColor(model.VerticalColorRed1, model.VerticalColorGreen1, model.VerticalColorBlue1, model.Alpha);
            SKColor color2H = new SKColor(model.HorizontalColorRed2, model.HorizontalColorGreen2, model.HorizontalColorBlue2, model.Alpha);

            // 交差点の混色（手動計算）
            SKColor colorCross = BussinessLogic.CalcColor2(color1V);

            // --- 3. タイルの作成と描画 ---
            using SKBitmap tileBmp = BussinessLogic.NewCreateImage(tileWidth, tileHeight);
            using SKCanvas tileCanvas = new SKCanvas(tileBmp);

            // 背景クリア
            tileCanvas.Clear(color0Base);

            using var paint = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                IsAntialias = false
            };

            // 幾何学的な格子を描画
            for (int y = 0; y < totalUnitsY; y++)
            {
                for (int x = 0; x < totalUnitsX; x++)
                {
                    bool isV = lineX[x]; // 縦ストライプの有無
                    bool isH = lineY[y]; // 横ストライプの有無

                    if (isV && isH)
                    {
                        paint.Color = colorCross; // フィボナッチ交差領域
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
                        continue; // ベース背景色
                    }

                    SKRect rect = SKRect.Create(x * unitSize, y * unitSize, unitSize, unitSize);
                    tileCanvas.DrawRect(rect, paint);
                }
            }

            // --- 5. タイルを敷き詰める ---
            bmp = BussinessLogic.RepeatImage(width, height, tileBmp);

            return bmp;
        }

        /// <summary>
        /// フィボナッチ数列の比率に従ってライン(ON)とギャップ(OFF)を交互に配置するパターン作成
        /// </summary>
        private static bool[] CreateFibonacciPattern(out int totalUnits, bool reverse)
        {
            List<bool> patternList = new List<bool>();

            int[] seq = (int[])FibonacciSequence.Clone();
            if (reverse)
            {
                Array.Reverse(seq);
            }

            // フィボナッチ数を「線の太さ(ON)」と「隙間の幅(OFF)」に交互に割り当てる
            bool isLine = true;
            foreach (int fibValue in seq)
            {
                for (int i = 0; i < fibValue; i++)
                {
                    patternList.Add(isLine);
                }
                // 次の要素は反転（線 ↔ 隙間）
                isLine = !isLine;
            }

            totalUnits = patternList.Count;
            return patternList.ToArray();
        }
    }
}