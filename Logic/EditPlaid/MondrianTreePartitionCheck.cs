using SkiaSharp;
using System;
using System.Collections.Generic;

namespace ImageCreatePlaid
{
    public class MondrianTreePartitionCheck : PlaidInterface
    {
        private static readonly Random _rnd = new Random();

        public SKBitmap EditImage(SKBitmap bmp, PlaidModel model)
        {
            int width = bmp.Width;
            int height = bmp.Height;

            SKColor color0Base = new SKColor(model.BaseColorRed, model.BaseColorGreen, model.BaseColorBlue, model.BaseAlpha);
            SKColor color1V = new SKColor(model.VerticalColorRed1, model.VerticalColorGreen1, model.VerticalColorBlue1, model.Alpha);
            SKColor color2H = new SKColor(model.HorizontalColorRed2, model.HorizontalColorGreen2, model.HorizontalColorBlue2, model.Alpha);
            SKColor colorCross = BussinessLogic.CalcColor2(color1V);

            using SKCanvas canvas = new SKCanvas(bmp);
            canvas.Clear(color0Base);

            // 1. 再帰的空間分割でブロック群（矩形リスト）を作る
            List<SKRect> blocks = new List<SKRect>();
            SKRect initialBounds = SKRect.Create(0, 0, width, height);

            int maxDepth = _rnd.Next(5, 8); // 分割の深さ
            SubdivideRect(initialBounds, 0, maxDepth, blocks, _rnd);

            using var fillPaint = new SKPaint { Style = SKPaintStyle.Fill, IsAntialias = true };
            using var linePaint = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                Color = color0Base, // ブロック同士の境界線（目地）
                StrokeWidth = 4,
                IsAntialias = true
            };

            // 2. 分割ブロックへ属性（縦・横・交点・背景）を色付け
            foreach (var rect in blocks)
            {
                int colorType = _rnd.Next(0, 4);
                switch (colorType)
                {
                    case 0: fillPaint.Color = color1V; break;     // 縦属性
                    case 1: fillPaint.Color = color2H; break;     // 横属性
                    case 2: fillPaint.Color = colorCross; break;  // 交点属性
                    default: continue; // 背景色のまま
                }

                canvas.DrawRect(rect, fillPaint);
                canvas.DrawRect(rect, linePaint); // モンドリアン風の黒/背景目地
            }

            return bmp;
        }

        // 矩形を再帰的に分割するメソッド
        private static void SubdivideRect(SKRect rect, int currentDepth, int maxDepth, List<SKRect> result, Random rnd)
        {
            // 最小サイズに達したか、最大深さに達したら終了
            if (currentDepth >= maxDepth || rect.Width < 60 || rect.Height < 60)
            {
                result.Add(rect);
                return;
            }

            bool splitHorizontally = rnd.Next(2) == 0;

            if (splitHorizontally && rect.Height > 100)
            {
                // 横に割る
                float splitY = rect.Top + (float)(rnd.NextDouble() * 0.6 + 0.2) * rect.Height;
                SubdivideRect(new SKRect(rect.Left, rect.Top, rect.Right, splitY), currentDepth + 1, maxDepth, result, rnd);
                SubdivideRect(new SKRect(rect.Left, splitY, rect.Right, rect.Bottom), currentDepth + 1, maxDepth, result, rnd);
            }
            else if (rect.Width > 100)
            {
                // 縦に割る
                float splitX = rect.Left + (float)(rnd.NextDouble() * 0.6 + 0.2) * rect.Width;
                SubdivideRect(new SKRect(rect.Left, rect.Top, splitX, rect.Bottom), currentDepth + 1, maxDepth, result, rnd);
                SubdivideRect(new SKRect(splitX, rect.Top, rect.Right, rect.Bottom), currentDepth + 1, maxDepth, result, rnd);
            }
            else
            {
                result.Add(rect);
            }
        }
    }
}