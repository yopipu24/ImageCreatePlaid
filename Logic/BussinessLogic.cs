using System.Reflection;
using SkiaSharp;
using System.Security.Cryptography;

namespace ImageCreatePlaid
{
    public static class BussinessLogic
    {
        public static SKTypeface? MusicTypeface { get; private set; }

        public static void SetMusicTypeface(SKTypeface typeface)
        {
            MusicTypeface = typeface;
        }

        /// <summary>
        /// Bitmapデータ作成
        /// </summary>
        /// <param name="width">幅</param>
        /// <param name="height">高さ</param>
        /// <returns></returns>
        public static SKBitmap NewCreateImage(int width, int height)
        {
            var imageInfo = new SKImageInfo(
                width,
                height,
                SKColorType.Rgba8888,
                SKAlphaType.Premul);

            return new SKBitmap(imageInfo);
        }

        /// <summary>
        /// 重複色計算（乗算）
        /// </summary>
        /// <param name="color1">１つ目のRGB値</param>
        /// <param name="color2">２つ目のRGB値</param>
        /// <returns></returns>
        public static byte CalcColor(byte color1, byte color2)
        {
            byte calcColor = 0;
            calcColor = (byte)((color1 + color2) / 2 * color1 / 255 * color2 / 255);
            if (calcColor < 0)
            {
                calcColor = 0;
            }

            return calcColor;
        }

        /// <summary>
        /// 重複色計算2（50%）
        /// </summary>
        /// <param name="color1">１つ目のRGB値</param>
        /// <param name="color2">２つ目のRGB値</param>
        /// <returns></returns>
        public static byte CalcColor2(byte color1, byte color2)
        {
            byte calcColor = 0;
            calcColor = (byte)((color1 + color2) / 2 - ((255 - color1) / 2 + (255 - color2) / 2) / 2);
            if (calcColor < 0)
            {
                calcColor = 0;
            }

            return calcColor;
        }

        /// <summary>
        /// 同重複色計算
        /// </summary>
        /// <param name="color">１つ目の色</param>
        /// <returns></returns>
        public static SKColor CalcColor(SKColor color)
        {
            byte rCalcColor = CalcColor(color.Red, color.Red);
            byte gCalcColor = CalcColor(color.Green, color.Green);
            byte bCalcColor = CalcColor(color.Blue, color.Blue);

            return new SKColor(rCalcColor, gCalcColor, bCalcColor);
        }

        /// <summary>
        /// 同重複色計算2（50%）
        /// </summary>
        /// <param name="color">１つ目の色</param>
        /// <returns></returns>
        public static SKColor CalcColor2(SKColor color)
        {
            byte rCalcColor = CalcColor2(color.Red, color.Red);
            byte gCalcColor = CalcColor2(color.Green, color.Green);
            byte bCalcColor = CalcColor2(color.Blue, color.Blue);

            return new SKColor(rCalcColor, gCalcColor, bCalcColor);
        }

        /// <summary>
        /// 重複色取得
        /// </summary>
        /// <param name="color1">１つ目の色</param>
        /// <param name="color2">２つ目の色</param>
        /// <returns></returns>
        public static SKColor GetMixColor(SKColor color1, SKColor color2)
        {
            byte ucolor1r = color1.Red;
            byte ucolor1g = color1.Green;
            byte ucolor1b = color1.Blue;
            byte ucolor2r = color2.Red;
            byte ucolor2g = color2.Green;
            byte ucolor2b = color2.Blue;

            return new SKColor((byte)((ucolor1r + ucolor2r) / 2), (byte)((ucolor1g + ucolor2g) / 2), (byte)((ucolor1b + ucolor2b) / 2));
        }

        /// <summary>
        /// 白色文字列取得
        /// </summary>
        /// <returns></returns>
        public static string GetWhiteColor()
        {
            return "FFFFFF";
        }

        /// <summary>
        /// 乱数色取得
        /// </summary>
        /// <returns></returns>
        public static string GetColor()
        {
            int r = RandomNumberGenerator.GetInt32(196, 256);
            int g = RandomNumberGenerator.GetInt32(196, 256);
            int b = RandomNumberGenerator.GetInt32(196, 256);

            return string.Format("{0:X2}{1:X2}{2:X2}", r, g, b);
        }

        /// <summary>
        /// 乱数色取得2（薄め）
        /// </summary>
        /// <returns></returns>
        public static string GetColor2()
        {
            int r = RandomNumberGenerator.GetInt32(224, 256);
            int g = RandomNumberGenerator.GetInt32(224, 256);
            int b = RandomNumberGenerator.GetInt32(224, 256);

            return string.Format("{0:X2}{1:X2}{2:X2}", r, g, b);
        }

        /// <summary>
        /// 点線パターン
        /// </summary>
        /// <param name="pattern">1:15分の10 2:半分</param>
        /// <param name="scale">ブロックサイズ</param>
        /// <returns></returns>
        public static int[] GetDotPattern(string pattern, int scale)
        {
            int[] result = new int[scale];
            if (pattern == "one")
            {
                for (int i = 0; i < scale; i++)
                {
                    if (i % 15 < 10) result[i] = 1;
                }
            }
            else if (pattern == "two")
            {
                for (int i = 0; i < scale; i++)
                {
                    if (i % (scale / 10) < (scale / 10 / 2)) result[i] = 1;
                }
            }
            else if (pattern == "three")
            {
                for (int i = 0; i < scale; i++)
                {
                    if (i % 20 < 10) result[i] = 1;
                    if (i % 20 > 12 && i % 15 < 15) result[i] = 1;
                }
            }

            return result;
        }

        /// <summary>
        /// 乱数整数取得
        /// </summary>
        /// <returns></returns>
        public static int GetRandomInt(int maxInt)
        {
            if (maxInt <= 1)
            {
                return 1;
            }

            return RandomNumberGenerator.GetInt32(1, maxInt);
        }

        /// <summary>
        /// イメージ編集（チェック柄）
        /// </summary>
        /// <param name="bmp">Bitmap</param>
        /// <param name="model">チェック柄モデル</param>
        /// <returns></returns>
        public static SKBitmap EditImage(SKBitmap bmp, PlaidModel model)
        {
            //チェック柄のパターン毎に処理呼び出し
            Type t = Type.GetType("ImageCreatePlaid." + model.KindPlaid);
            object o = t.InvokeMember(null, BindingFlags.CreateInstance, null, null, null);
            object result = t.InvokeMember("EditImage", BindingFlags.InvokeMethod, null, o, new object[]{ bmp, model });
            bmp = (SKBitmap)result;

            return bmp;
        }

        /// <summary>
        /// イメージ編集（背景）
        /// </summary>
        /// <param name="bmp">Bitmap</param>
        /// <param name="model">背景モデル</param>
        /// <returns></returns>
        public static SKBitmap EditImage(SKBitmap bmp, SceneryModel model)
        {
            //背景のパターン毎に処理呼び出し
            Type t = Type.GetType("ImageCreatePlaid." + model.KindScenery);
            object o = t.InvokeMember(null, BindingFlags.CreateInstance, null, null, null);
            object result = t.InvokeMember("EditImage", BindingFlags.InvokeMethod, null, o, new object[] { bmp, model });
            bmp = (SKBitmap)result;

            return bmp;
        }

        /// <summary>
        /// イメージ編集（枠）
        /// </summary>
        /// <param name="bmp">Bitmap</param>
        /// <param name="model">枠モデル</param>
        /// <returns></returns>
        public static SKBitmap EditImage(SKBitmap bmp, FrameModel model)
        {
            //背景のパターン毎に処理呼び出し
            Type t = Type.GetType("ImageCreatePlaid." + model.KindFrame);
            object o = t.InvokeMember(null, BindingFlags.CreateInstance, null, null, null);
            object result = t.InvokeMember("EditImage", BindingFlags.InvokeMethod, null, o, new object[] { bmp, model });
            bmp = (SKBitmap)result;

            return bmp;
        }

        /// <summary>
        /// 画像繰り返し
        /// </summary>
        /// <param name="width">幅</param>
        /// <param name="height">高さ</param>
        /// <param name="srcImage">元画像</param>
        public static SKBitmap RepeatImage(int width, int height, SKBitmap srcImage)
        {
            SKBitmap bmp = NewCreateImage(width, height);
            using SKCanvas canvas = new SKCanvas(bmp);
            using SKImage tileImage = SKImage.FromBitmap(srcImage);
            int tileWidth = (int)tileImage.Width;
            int tileHeight = (int)tileImage.Height;

            for (int x = 0; x < width; x += tileWidth)
            {
                for (int y = 0; y < height; y += tileHeight)
                {
                    var dest = new SKRect(
                        x,
                        y,
                        Math.Min(x + tileWidth, width),
                        Math.Min(y + tileHeight, height));

                    var src = new SKRect(
                        0,
                        0,
                        dest.Width,
                        dest.Height);

                    canvas.DrawImage(tileImage, src, dest, SKSamplingOptions.Default);
                }
            }

            return bmp;
        }
    }
}
