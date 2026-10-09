using SkiaSharp;
using System;

namespace ImageCreatePlaid
{
    public class KaleidoscopicGridCheck : PlaidInterface
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

            // 1. 1/8 領域（扇型/三角形）用の小さな一次ビットマップを作成
            int sectorSize = Math.Max(width, height) / 2;
            using var sectorBmp = new SKBitmap(sectorSize, sectorSize);
            using var sectorCanvas = new SKCanvas(sectorBmp);

            sectorCanvas.Clear(color0Base);

            // 2. 1/8 領域にランダムでカオスなチェック柄を描く
            using var paint = new SKPaint { Style = SKPaintStyle.Fill, IsAntialias = true };
            int step = model.VerticalSize1 > 0 ? Math.Clamp(model.VerticalSize1, 10, 40) : 20;

            for (int y = 0; y < sectorSize; y += step)
            {
                for (int x = 0; x < sectorSize; x += step)
                {
                    if (_rnd.Next(100) < 60)
                    {
                        int type = _rnd.Next(3);
                        paint.Color = type == 0 ? color1V : (type == 1 ? color2H : colorCross);

                        // ランダムな大きさの格子・多角形を打つ
                        SKRect rect = SKRect.Create(x, y, step * _rnd.Next(1, 3), step * _rnd.Next(1, 3));
                        sectorCanvas.DrawRect(rect, paint);
                    }
                }
            }

            // 3. メインキャンバスへ万華鏡（8回対称コピー）合成
            using SKCanvas mainCanvas = new SKCanvas(bmp);
            mainCanvas.Clear(color0Base);

            float cx = width / 2f;
            float cy = height / 2f;

            for (int i = 0; i < 8; i++)
            {
                mainCanvas.Save();
                mainCanvas.Translate(cx, cy);
                mainCanvas.RotateDegrees(i * 45); // 45度ずつ回転

                // 偶数回は鏡像反転（Mirror）
                if (i % 2 != 0)
                {
                    mainCanvas.Scale(-1, 1);
                }

                // 1/8の三角形で切り抜き（クリッピング）
                using var clipPath = new SKPath();
                clipPath.MoveTo(0, 0);
                clipPath.LineTo(sectorSize * 1.5f, 0);
                clipPath.LineTo(sectorSize * 1.5f, sectorSize * 1.5f * (float)Math.Tan(Math.PI / 8));
                clipPath.Close();

                mainCanvas.ClipPath(clipPath);

                // セクター画像を貼る
                mainCanvas.DrawBitmap(sectorBmp, 0, 0);

                mainCanvas.Restore();
            }

            return bmp;
        }
    }
}