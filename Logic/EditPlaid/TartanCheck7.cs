using SkiaSharp;

namespace ImageCreatePlaid
{
    public class TartanCheck7 : PlaidInterface
    {
        public SKBitmap EditImage(SKBitmap bmp, PlaidModel model)
        {
            int width = bmp.Width;
            int height = bmp.Height;

            // --- 調整用パラメータ ---
            float mixRatio = 0.5f;   // 混色の割合(0=1色目寄り、1=2色目寄り)
            float twillShade = 0.1f; // 綾目の陰影(0で陰影なし。0.05〜0.15くらい)
            // -----------------------

            var rand = new Random();

            // 指定された2色(PolkaDot1 と同じ組み合わせ)
            var color1 = new SKColor(model.VerticalColorRed1, model.VerticalColorGreen1, model.VerticalColorBlue1);
            var color2 = new SKColor(model.HorizontalColorRed2, model.HorizontalColorGreen2, model.HorizontalColorBlue2);
            SKColor mixed = Mix(color1, color2, mixRatio);

            SKColor[] warpColors = { color1, color2, mixed };
            SKColor[] weftColors =
            {
                Darken(color1, twillShade),
                Darken(color2, twillShade),
                Darken(mixed, twillShade)
            };

            // 糸1本の太さ(px)。VerticalSize1 を 20 で割った値。
            // ランダムなセットは長くなることがあるので、タイルが大きくなりすぎないよう上限は8px
            int threadPx = Math.Clamp(model.VerticalSize1 / 20, 1, 8);

            int[] threads = BuildThreads(CreateRandomSett(rand));
            int tileSize = threads.Length * threadPx;

            using SKBitmap tile = BussinessLogic.NewCreateImage(tileSize, tileSize);
            SKColor[] pixels = new SKColor[tileSize * tileSize];

            for (int y = 0; y < tileSize; y++)
            {
                int ty = y / threadPx;
                int row = y * tileSize;

                for (int x = 0; x < tileSize; x++)
                {
                    int tx = x / threadPx;

                    // 2/2綾織り
                    bool warpOnTop = ((tx + ty) & 3) < 2;

                    pixels[row + x] = warpOnTop
                        ? warpColors[threads[tx]]
                        : weftColors[threads[ty]];
                }
            }

            tile.Pixels = pixels;

            return BussinessLogic.RepeatImage(width, height, tile);
        }

        /// <summary>
        /// ランダムな糸の並び(セット)を作る。(色番号, 糸の本数) の配列。
        /// 色番号: 0=1色目, 1=2色目, 2=混色
        /// </summary>
        private static (int Color, int Threads)[] CreateRandomSett(Random rand)
        {
            // 糸の本数の候補(好みで変えてください)
            int[] wideWidths = { 12, 16, 20 };   // 両端の太い帯
            int[] bandWidths = { 4, 6, 8, 12 };  // 中間の帯
            int[] accentWidths = { 2, 4 };       // 細い線

            var sett = new List<(int Color, int Threads)>();

            // 先頭(軸): 太い帯。1色目か2色目
            int firstColor = rand.Next(2);
            sett.Add((firstColor, Pick(rand, wideWidths)));

            // 中間: 帯と細い線をランダムに2〜5本
            int prevColor = firstColor;
            int middleCount = rand.Next(2, 6);
            for (int n = 0; n < middleCount; n++)
            {
                bool accent = rand.NextDouble() < 0.5;
                int color = accent ? PickAccentColor(rand, prevColor) : PickBandColor(rand, prevColor);
                int threads = accent ? Pick(rand, accentWidths) : Pick(rand, bandWidths);

                sett.Add((color, threads));
                prevColor = color;
            }

            // 末尾(軸): 太い帯。先頭とは別の主色にして、2色とも主役にする
            int lastColor = 1 - firstColor;
            if (prevColor == lastColor)
            {
                // 同じ色が隣り合わないよう、混色の細い線をはさむ
                sett.Add((2, 2));
            }
            int lastThreads = Pick(rand, wideWidths);
            sett.Add((lastColor, lastThreads));

            // 折り返したあとの糸の総数を4の倍数にそろえる。
            // 軸の縞は折り返しで増えないので、末尾の太さで調整する
            int total = 0;
            for (int i = 0; i < sett.Count; i++)
            {
                total += sett[i].Threads;
            }
            for (int i = 1; i <= sett.Count - 2; i++)
            {
                total += sett[i].Threads;
            }
            int rem = total % 4;
            if (rem != 0)
            {
                sett[sett.Count - 1] = (lastColor, lastThreads + (4 - rem));
            }

            return sett.ToArray();
        }

        // 太めの帯の色: 1色目か2色目(直前と同じ色は避ける)
        private static int PickBandColor(Random rand, int prevColor)
        {
            int color = rand.Next(2);
            if (color == prevColor)
            {
                color = 1 - color;
            }
            return color;
        }

        // 細い線の色: 混色が多め(直前が混色でなければ60%)
        private static int PickAccentColor(Random rand, int prevColor)
        {
            if (prevColor != 2 && rand.NextDouble() < 0.6)
            {
                return 2;
            }
            return PickBandColor(rand, prevColor);
        }

        private static int Pick(Random rand, int[] values)
        {
            return values[rand.Next(values.Length)];
        }

        // セットを左右対称に折り返し、1本ずつの色番号の並びにする
        private static int[] BuildThreads((int Color, int Threads)[] sett)
        {
            var order = new List<(int Color, int Threads)>(sett);
            for (int i = sett.Length - 2; i >= 1; i--)
            {
                order.Add(sett[i]);
            }

            var threads = new List<int>();
            foreach (var (color, count) in order)
            {
                for (int n = 0; n < count; n++)
                {
                    threads.Add(color);
                }
            }

            // 念のため、4の倍数でなければ繰り返してそろえる
            int baseCount = threads.Count;
            int repeat = 4 / Gcd(baseCount, 4);
            for (int r = 1; r < repeat; r++)
            {
                for (int n = 0; n < baseCount; n++)
                {
                    threads.Add(threads[n]);
                }
            }

            return threads.ToArray();
        }

        private static int Gcd(int a, int b)
        {
            while (b != 0)
            {
                (a, b) = (b, a % b);
            }
            return a;
        }

        private static SKColor Mix(SKColor a, SKColor b, float t)
        {
            return new SKColor(
                (byte)Math.Round(a.Red + (b.Red - a.Red) * t),
                (byte)Math.Round(a.Green + (b.Green - a.Green) * t),
                (byte)Math.Round(a.Blue + (b.Blue - a.Blue) * t));
        }

        private static SKColor Darken(SKColor c, float amount)
        {
            float f = 1f - Math.Clamp(amount, 0f, 1f);
            return new SKColor((byte)(c.Red * f), (byte)(c.Green * f), (byte)(c.Blue * f));
        }
    }
}