using SkiaSharp;

namespace ImageCreatePlaid
{
    public class StainedGlass2 : SceneryInterface
    {
        private static readonly Random Rand = new Random();

        public SKBitmap EditImage(SKBitmap bmp, SceneryModel model)
        {
            int width = bmp.Width;
            int height = bmp.Height;
            float saturation = model.SaturationValue;

            using var canvas = new SKCanvas(bmp);
            canvas.Clear(SKColors.White);

            int cellCount = Math.Max(1, (width * height) / 5000);
            int GridSize = Math.Max(64, (int)Math.Sqrt(width * height / cellCount));

            int gridWidth = (width + GridSize - 1) / GridSize;
            int gridHeight = (height + GridSize - 1) / GridSize;

            List<StainedCellModel>[,] grid = new List<StainedCellModel>[gridWidth, gridHeight];

            for (int gx = 0; gx < gridWidth; gx++)
            {
                for (int gy = 0; gy < gridHeight; gy++)
                {
                    grid[gx, gy] = new List<StainedCellModel>();
                }
            }

            List<StainedCellModel> cells = new();

            for (int i = 0; i < cellCount; i++)
            {
                double hue = Rand.NextDouble() * 360;
                StainedCellModel cell = new()
                {
                    X = Rand.Next(width),
                    Y = Rand.Next(height),
                    Hue = hue,
                    BaseValue = 0.65 + Rand.NextDouble() * 0.25,
                    BaseColor = FromHSV(hue, saturation, 1.0)
                };

                cells.Add(cell);

                int gx = cell.X / GridSize;
                int gy = cell.Y / GridSize;

                grid[gx, gy].Add(cell);
            }

            ParallelOptions options = new()
            {
                MaxDegreeOfParallelism =
                Math.Max(1, Environment.ProcessorCount - 1)
            };

            unsafe
            {
                uint* pixels = (uint*)bmp.GetPixels().ToPointer();
                Parallel.For(0, height, options, y =>
                {
                    int row = y * width;

                    for (int x = 0; x < width; x++)
                    {
                        StainedCellModel cell = FindNearestCell(x, y, grid, GridSize);

                        if (cell == null)
                        {
                            // 念のため白
                            pixels[row + x] = 0xffffffff;
                            continue;
                        }

                        // 光計算
                        double value = cell.BaseValue;

                        // セル中心Glow
                        double dx = x - cell.X;
                        double dy = y - cell.Y;

                        double glow = Math.Max(0.0, 1.0 - (dx * dx + dy * dy) / 2500.0);

                        value *= 0.8 + glow * 0.3;

                        // 左上からの光
                        double directional = 1.0 - ((double)x / width + (double)y / height) * 0.5;
                        value *= 0.75 + directional * 0.45;

                        value = Math.Clamp(value, 0, 1);

                        byte r = (byte)(cell.BaseColor.Red * value);
                        byte g = (byte)(cell.BaseColor.Green * value);
                        byte b = (byte)(cell.BaseColor.Blue * value);

                        pixels[row + x] =
                            0xFF000000u |
                            ((uint)b << 16) |
                            ((uint)g << 8) |
                            r;
                    }
                });
            }

            return bmp;
        }

        private static SKColor FromHSV(double hue, double saturation, double value)
        {
            int hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
            double f = hue / 60 - Math.Floor(hue / 60);

            value *= 255;

            byte v = ClampToByte(value);
            byte p = ClampToByte(value * (1 - saturation));
            byte q = ClampToByte(value * (1 - f * saturation));
            byte t = ClampToByte(value * (1 - (1 - f) * saturation));

            return hi switch
            {
                0 => new SKColor(v, t, p, 255),
                1 => new SKColor(q, v, p, 255),
                2 => new SKColor(p, v, t, 255),
                3 => new SKColor(p, q, v, 255),
                4 => new SKColor(t, p, v, 255),
                _ => new SKColor(v, p, q, 255),
            };
        }

        private static StainedCellModel FindNearestCell(
            int x,
            int y,
            List<StainedCellModel>[,] grid,
            int gridSize)
        {
            int gridWidth = grid.GetLength(0);
            int gridHeight = grid.GetLength(1);

            int gx = x / gridSize;
            int gy = y / gridSize;

            StainedCellModel nearest = null;
            int bestDist = int.MaxValue;


            // 最大5マスまで探索
            for (int radius = 1; radius <= 5; radius++)
            {
                int minX = Math.Max(0, gx - radius);
                int maxX = Math.Min(gridWidth - 1, gx + radius);

                int minY = Math.Max(0, gy - radius);
                int maxY = Math.Min(gridHeight - 1, gy + radius);


                for (int yy = minY; yy <= maxY; yy++)
                {
                    for (int xx = minX; xx <= maxX; xx++)
                    {
                        List<StainedCellModel> list = grid[xx, yy];

                        if (list == null)
                            continue;


                        foreach (StainedCellModel cell in list)
                        {
                            int dx = x - cell.X;
                            int dy = y - cell.Y;

                            int dist = dx * dx + dy * dy;


                            if (dist < bestDist)
                            {
                                bestDist = dist;
                                nearest = cell;
                            }
                        }
                    }
                }


                // 見つかったら終了
                if (nearest != null)
                    break;
            }


            return nearest;
        }

        private static byte ClampToByte(double value)
        {
            return (byte)Math.Max(0, Math.Min(255, value));
        }

        private static double Beam(int x, int y, double centerX, double strength)
        {
            // 左上→右下方向へ伸びる光
            double dx = x - centerX + y * 0.35;

            return strength *
                   Math.Exp(-(dx * dx) / 14000.0);
        }
    }
}