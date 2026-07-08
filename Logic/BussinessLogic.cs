using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace ImageCreatePlaid
{
    public static class BussinessLogic
    {
        /// <summary>
        /// Bitmapデータ作成
        /// </summary>
        /// <param name="width">幅</param>
        /// <param name="height">高さ</param>
        /// <returns></returns>
        public static Bitmap NewCreateImage(int width, int height)
        {
            return new Bitmap(width, height);
        }

        /// <summary>
        /// 重複色計算（乗算）
        /// </summary>
        /// <param name="color1">１つ目のRGB値</param>
        /// <param name="color2">２つ目のRGB値</param>
        /// <returns></returns>
        public static int CalcColor(int color1, int color2)
        {
            int calcColor = 0;
            calcColor = (color1 + color2) / 2 * color1 / 255 * color2 / 255;
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
        public static int CalcColor2(int color1, int color2)
        {
            int calcColor = 0;
            calcColor = (color1 + color2) / 2 - ((255 - color1) / 2 + (255 - color2) / 2) / 2;
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
        public static Color CalcColor(Color color)
        {
            int rCalcColor = CalcColor(color.R, color.R);
            int gCalcColor = CalcColor(color.G, color.G);
            int bCalcColor = CalcColor(color.B, color.B);

            return Color.FromArgb(rCalcColor, gCalcColor, bCalcColor);
        }

        /// <summary>
        /// 同重複色計算2（50%）
        /// </summary>
        /// <param name="color">１つ目の色</param>
        /// <returns></returns>
        public static Color CalcColor2(Color color)
        {
            int rCalcColor = CalcColor2(color.R, color.R);
            int gCalcColor = CalcColor2(color.G, color.G);
            int bCalcColor = CalcColor2(color.B, color.B);

            return Color.FromArgb(rCalcColor, gCalcColor, bCalcColor);
        }

        /// <summary>
        /// 重複色取得
        /// </summary>
        /// <param name="color1">１つ目の色</param>
        /// <param name="color2">２つ目の色</param>
        /// <returns></returns>
        public static Color GetMixColor(Color color1, Color color2)
        {
            int ucolor1r = color1.R;
            int ucolor1g = color1.G;
            int ucolor1b = color1.B;
            int ucolor2r = color2.R;
            int ucolor2g = color2.G;
            int ucolor2b = color2.B;

            return Color.FromArgb((ucolor1r + ucolor2r) / 2, (ucolor1g + ucolor2g) / 2, (ucolor1b + ucolor2b) / 2);
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
            string color = "FFFFFF";

            //乱数生成
            byte[] bs = new byte[4];
            System.Security.Cryptography.RNGCryptoServiceProvider rng = new System.Security.Cryptography.RNGCryptoServiceProvider();
            rng.GetBytes(bs);
            rng.Dispose();
            int i = System.BitConverter.ToInt32(bs, 0);

            //背景として使うので薄めの色
            System.Random r = new System.Random(i);
            int randomNumber1 = r.Next(196, 256);
            int randomNumber2 = r.Next(196, 256);
            int randomNumber3 = r.Next(196, 256);
            color = string.Format("{0:X2}{1:X2}{2:X2}",randomNumber1, randomNumber2, randomNumber3);
            return color;
        }

        /// <summary>
        /// 乱数色取得2（薄め）
        /// </summary>
        /// <returns></returns>
        public static string GetColor2()
        {
            string color = "FFFFFF";

            //乱数生成
            byte[] bs = new byte[4];
            System.Security.Cryptography.RNGCryptoServiceProvider rng = new System.Security.Cryptography.RNGCryptoServiceProvider();
            rng.GetBytes(bs);
            rng.Dispose();
            int i = System.BitConverter.ToInt32(bs, 0);

            //背景として使うのでさらに薄めの色
            System.Random r = new System.Random(i);
            int randomNumber1 = r.Next(224, 256);
            int randomNumber2 = r.Next(224, 256);
            int randomNumber3 = r.Next(224, 256);
            color = string.Format("{0:X2}{1:X2}{2:X2}", randomNumber1, randomNumber2, randomNumber3);
            return color;
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

            //乱数生成
            byte[] bs = new byte[4];
            System.Security.Cryptography.RNGCryptoServiceProvider rng = new System.Security.Cryptography.RNGCryptoServiceProvider();
            rng.GetBytes(bs);
            rng.Dispose();
            int i = System.BitConverter.ToInt32(bs, 0);

            System.Random r = new System.Random(i);
            return r.Next(1, maxInt);
        }

        /// <summary>
        /// イメージ編集（チェック柄）
        /// </summary>
        /// <param name="bmp">Bitmap</param>
        /// <param name="model">チェック柄モデル</param>
        /// <returns></returns>
        public static Bitmap EditImage(Bitmap bmp, PlaidModel model)
        {
            //チェック柄のパターン毎に処理呼び出し
            Type t = Type.GetType("ImageCreatePlaid." + model.KindPlaid);
            object o = t.InvokeMember(null, BindingFlags.CreateInstance, null, null, null);
            object result = t.InvokeMember("EditImage", BindingFlags.InvokeMethod, null, o, new object[]{ bmp, model });
            bmp = (Bitmap)result;

            return bmp;
        }

        /// <summary>
        /// イメージ編集（背景）
        /// </summary>
        /// <param name="bmp">Bitmap</param>
        /// <param name="model">チェック柄モデル</param>
        /// <returns></returns>
        public static Bitmap EditImage(Bitmap bmp, SceneryModel model)
        {
            //背景のパターン毎に処理呼び出し
            Type t = Type.GetType("ImageCreatePlaid." + model.KindScenery);
            object o = t.InvokeMember(null, BindingFlags.CreateInstance, null, null, null);
            object result = t.InvokeMember("EditImage", BindingFlags.InvokeMethod, null, o, new object[] { bmp, model });
            bmp = (Bitmap)result;

            return bmp;
        }

        /// <summary>
        /// 繰り返しイメージ作成
        /// </summary>
        /// <param name="filePath">繰り返しの元画像パス</param>
        /// <param name="width">幅</param>
        /// <param name="height">高さ</param>
        /// <returns></returns>
        public static Bitmap RepeatImage(string filePath, int width, int height)
        {
            Bitmap srcBitmap = new Bitmap(filePath);
            int srcWidth = srcBitmap.Width;
            int srcHeight = srcBitmap.Height;
            int wCount = (int)Math.Floor((decimal)width / srcWidth);
            int hCount = (int)Math.Floor((decimal)height / srcHeight);
            Bitmap bitmap = NewCreateImage(width, height);
            for(int i = 0; i < wCount; i++)
            {
                for(int j = 0; j < hCount; j++)
                {
                    for(int w = 0; w < srcWidth; w++)
                    {
                        for(int h = 0; h < srcHeight; h++)
                        {
                            Color c = srcBitmap.GetPixel(w, h);
                            bitmap.SetPixel(i * srcWidth + w, j * srcHeight + h, c);
                        }
                    }
                }
            }

            return bitmap;
        }
    }
}
