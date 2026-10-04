using SkiaSharp;

namespace ImageCreatePlaid
{
    public class TartanCheck6 : PlaidInterface
    {
        // 糸の並び(セット)。(色番号, 糸の本数)
        // 色番号: 0=1色目, 1=2色目, 2=混色
        // 先頭と末尾の縞を軸に、左右対称に折り返して繰り返す。
        private static readonly (int Color, int Threads)[][] Setts =
        {
            // 0: 大きな2色の帯を、混色の細い線でつなぐ落ち着いた柄
            new (int Color, int Threads)[] { (0, 16), (2, 4), (1, 8), (2, 2), (1, 16) },
            // 1: 細い線が多い、きめ細かい柄
            new (int Color, int Threads)[] { (0, 12), (2, 2), (1, 2), (2, 2), (0, 2), (2, 2), (1, 12) },
        };

        public SKBitmap EditImage(SKBitmap bmp, PlaidModel model)
        {
            int width = bmp.Width;
            int height = bmp.Height;

            // --- 調整用パラメータ ---
            int settType = 0;        // 糸の並び(0 or 1)
            float mixRatio = 0.5f;   // 混色の割合(0=1色目寄り、1=2色目寄り)
            float twillShade = 0.1f; // 綾目の陰影(0で陰影なし。0.05〜0.15くらい)
            // -----------------------

            // 指定された2色(PolkaDot1 と同じ組み合わせ)
            var color1 = new SKColor(model.VerticalColorRed1, model.VerticalColorGreen1, model.VerticalColorBlue1);
            var color2 = new SKColor(model.HorizontalColorRed2, model.HorizontalColorGreen2, model.HorizontalColorBlue2);
            SKColor mixed = Mix(color1, color2, mixRatio);

            // 縦糸が上に出るときの色 / 横糸が上に出るときの色(少し暗くして綾目を出す)
            SKColor[] warpColors = { color1, color2, mixed };
            SKColor[] weftColors =
            {
                Darken(color1, twillShade),
                Darken(color2, twillShade),
                Darken(mixed, twillShade)
            };

            // 糸1本の太さ(px)。VerticalSize1 を 20 で割った値(例: 40 → 2px、100 → 5px)
            int threadPx = Math.Clamp(model.VerticalSize1 / 20, 1, 12);

            int[] threads = BuildThreads(Setts[settType % Setts.Length]);
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

                    // 2/2綾織り: 縦糸2本分が上、次の2本分が下、をずらしながら繰り返す
                    bool warpOnTop = ((tx + ty) & 3) < 2;

                    pixels[row + x] = warpOnTop
                        ? warpColors[threads[tx]]   // 縦糸(列 tx の糸の色)
                        : weftColors[threads[ty]];  // 横糸(行 ty の糸の色)
                }
            }

            tile.Pixels = pixels;

            return BussinessLogic.RepeatImage(width, height, tile);
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

            // 綾目(4本周期)が継ぎ目でずれないよう、本数を4の倍数にそろえる
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