using SkiaSharp;
using System;
using System.Collections.Generic;

namespace ImageCreatePlaid
{
    public class ExtremeRandomCheck2 : PlaidInterface
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

            // --- 1. キャンバス全体に対するタータンストライプ位置の生成 ---
            // 縦線（X座標）と横線（Y座標）の帯データリストを作成
            var vStripes = GenerateTartanStripes(width, model.VerticalSize1, _rnd);
            var hStripes = GenerateTartanStripes(height, model.HorizontalSize1, _rnd);

            // --- 2. キャンバスを不規則なランダムグリッドブロックに分割 ---
            int cols = _rnd.Next(4, 9); // 4〜8列
            int rows = _rnd.Next(4, 9); // 4〜8行

            float blockWidth = (float)width / cols;
            float blockHeight = (float)height / rows;

            using var fillPaint = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                IsAntialias = true
            };

            // --- 3. ブロック単位での完全不規則コラージュ描画 ---
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    SKRect blockRect = SKRect.Create(c * blockWidth, r * blockHeight, blockWidth, blockHeight);

                    canvas.Save();
                    // ブロック領域でクリッピング（切り抜き）
                    canvas.ClipRect(blockRect);

                    // A. 各ブロックにランダムな位置シフト（ズレ）を適用
                    float offsetX = _rnd.Next(-100, 101);
                    float offsetY = _rnd.Next(-100, 101);

                    // B. 低確率(15%)でブロック自体を微小回転（1度〜3度斜めにする）
                    if (_rnd.Next(100) < 15)
                    {
                        float cx = blockRect.MidX;
                        float cy = blockRect.MidY;
                        canvas.RotateDegrees(_rnd.Next(-5, 6), cx, cy);
                    }

                    // --- 4. ブロック内のタータン柄の直接描画 ---
                    // 縦ラインの描画
                    foreach (var v in vStripes)
                    {
                        float drawX = v.Position + offsetX;
                        SKRect rect = SKRect.Create(drawX, blockRect.Top - 100, v.Width, blockHeight + 200);

                        fillPaint.Color = color1V;
                        canvas.DrawRect(rect, fillPaint);
                    }

                    // 横ラインの描画と交差部の判定
                    foreach (var h in hStripes)
                    {
                        float drawY = h.Position + offsetY;
                        SKRect rect = SKRect.Create(blockRect.Left - 100, drawY, blockWidth + 200, h.Width);

                        fillPaint.Color = color2H;
                        canvas.DrawRect(rect, fillPaint);

                        // 縦ストライプとの重なり領域（交差点）を混色で塗る
                        foreach (var v in vStripes)
                        {
                            float drawX = v.Position + offsetX;

                            // 交差判定のAABB矩形
                            SKRect crossRect = SKRect.Create(drawX, drawY, v.Width, h.Width);

                            if (blockRect.IntersectsWith(crossRect))
                            {
                                fillPaint.Color = colorCross;

                                // 20%の確率で交差領域を四角ではなく「斜めトライアングル」にして幾何学カット
                                if (_rnd.Next(100) < 20)
                                {
                                    using var path = new SKPath();
                                    path.MoveTo(crossRect.Left, crossRect.Top);
                                    path.LineTo(crossRect.Right, crossRect.Top);
                                    path.LineTo(crossRect.Left, crossRect.Bottom);
                                    path.Close();
                                    canvas.DrawPath(path, fillPaint);
                                }
                                else
                                {
                                    canvas.DrawRect(crossRect, fillPaint);
                                }
                            }
                        }
                    }

                    canvas.Restore(); // クリッピング解除
                }
            }

            // --- 5. 全体を貫く幾何学スリット・グラフィック線の挿入 ---
            using var linePaint = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                Color = color0Base, // 背景色でダイナミックに切り抜く
                StrokeWidth = _rnd.Next(2, 6),
                IsAntialias = true
            };

            int slitCount = _rnd.Next(3, 7);
            for (int i = 0; i < slitCount; i++)
            {
                // 全体を斜めや垂直に切断するライン
                if (_rnd.Next(2) == 0)
                {
                    float x = _rnd.Next(0, width);
                    canvas.DrawLine(x, 0, x + _rnd.Next(-200, 200), height, linePaint);
                }
                else
                {
                    float y = _rnd.Next(0, height);
                    canvas.DrawLine(0, y, width, y + _rnd.Next(-200, 200), linePaint);
                }
            }

            return bmp;
        }

        // ストライプの構造体
        private record StripeInfo(float Position, float Width);

        /// <summary>
        /// キャンバス全体長さに渡るタータン不規則ストライプ位置の生成
        /// </summary>
        private static List<StripeInfo> GenerateTartanStripes(int totalLength, int baseSize, Random rnd)
        {
            var stripes = new List<StripeInfo>();
            float pos = rnd.Next(10, 50);

            int unit = baseSize > 0 ? Math.Clamp(baseSize / 2, 4, 30) : 10;

            while (pos < totalLength)
            {
                int patternType = rnd.Next(0, 4);

                switch (patternType)
                {
                    case 0: // 太い帯
                        float w1 = rnd.Next(3, 8) * unit;
                        stripes.Add(new StripeInfo(pos, w1));
                        pos += w1 + rnd.Next(2, 6) * unit;
                        break;

                    case 1: // ピンストライプ（細い2本並走線）
                        float w2 = Math.Max(2, unit * 0.4f);
                        stripes.Add(new StripeInfo(pos, w2));
                        pos += w2 + unit;
                        stripes.Add(new StripeInfo(pos, w2));
                        pos += w2 + rnd.Next(3, 7) * unit;
                        break;

                    case 2: // 中幅帯
                        float w3 = rnd.Next(2, 4) * unit;
                        stripes.Add(new StripeInfo(pos, w3));
                        pos += w3 + rnd.Next(2, 5) * unit;
                        break;

                    default: // 余白
                        pos += rnd.Next(3, 8) * unit;
                        break;
                }
            }

            return stripes;
        }
    }
}