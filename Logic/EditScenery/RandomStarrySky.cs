using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageCreatePlaid
{

    public class RandomStarrySky : SceneryInterface
    {
        public Bitmap EditImage(Bitmap bmp, SceneryModel model)
        {
            using Graphics g = Graphics.FromImage(bmp);

            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            Color baseColor = Color.FromArgb(
                model.BaseAlpha,
                model.BaseColorRed,
                model.BaseColorGreen,
                model.BaseColorBlue
            );

            Color starColor1 = Color.FromArgb(
                model.Alpha,
                model.VerticalColorRed1,
                model.VerticalColorGreen1,
                model.VerticalColorBlue1
            );

            // 2色目を使わない場合は、この色を starColor1 と同じにしてください。
            Color starColor2 = Color.FromArgb(
                model.Alpha,
                model.VerticalColorRed2,
                model.VerticalColorGreen2,
                model.VerticalColorBlue2
            );

            g.Clear(baseColor);

            Random random = new Random();

            int smallStarCount = model.SmallStarCount;
            int largeStarCount = model.LargeStarCount;
            int sparkleCount = model.SparkleCount;

            DrawSmallStars(g, bmp.Width, bmp.Height, random, smallStarCount, starColor1, starColor2);
            DrawLargeStars(g, bmp.Width, bmp.Height, random, largeStarCount, starColor1, starColor2);
            DrawSparkles(g, bmp.Width, bmp.Height, random, sparkleCount, starColor1, starColor2);

            return bmp;
        }

        private static void DrawSmallStars(
            Graphics g,
            int width,
            int height,
            Random random,
            int count,
            Color color1,
            Color color2)
        {
            for (int i = 0; i < count; i++)
            {
                int size = random.Next(1, 4);
                int x = random.Next(0, width);
                int y = random.Next(0, height);

                Color color = random.Next(2) == 0 ? color1 : color2;

                int alpha = random.Next(90, color.A + 1);
                Color starColor = Color.FromArgb(alpha, color.R, color.G, color.B);

                using Brush brush = new SolidBrush(starColor);
                g.FillEllipse(brush, x, y, size, size);
            }
        }

        private static void DrawLargeStars(
            Graphics g,
            int width,
            int height,
            Random random,
            int count,
            Color color1,
            Color color2)
        {
            for (int i = 0; i < count; i++)
            {
                int size = random.Next(4, 9);
                int x = random.Next(0, Math.Max(1, width - size));
                int y = random.Next(0, Math.Max(1, height - size));

                Color color = random.Next(2) == 0 ? color1 : color2;

                int alpha = random.Next(140, color.A + 1);
                Color starColor = Color.FromArgb(alpha, color.R, color.G, color.B);

                using Brush brush = new SolidBrush(starColor);
                g.FillEllipse(brush, x, y, size, size);
            }
        }

        private static void DrawSparkles(
            Graphics g,
            int width,
            int height,
            Random random,
            int count,
            Color color1,
            Color color2)
        {
            for (int i = 0; i < count; i++)
            {
                int size = random.Next(8, 18);
                int half = size / 2;

                int x = random.Next(half, Math.Max(half + 1, width - half));
                int y = random.Next(half, Math.Max(half + 1, height - half));

                Color color = random.Next(2) == 0 ? color1 : color2;

                int alpha = random.Next(120, color.A + 1);
                Color sparkleColor = Color.FromArgb(alpha, color.R, color.G, color.B);

                using Pen pen = new Pen(sparkleColor, 1);

                g.DrawLine(pen, x - half, y, x + half, y);
                g.DrawLine(pen, x, y - half, x, y + half);

                if (random.Next(2) == 0)
                {
                    int diagonal = half / 2;
                    g.DrawLine(pen, x - diagonal, y - diagonal, x + diagonal, y + diagonal);
                    g.DrawLine(pen, x + diagonal, y - diagonal, x - diagonal, y + diagonal);
                }
            }
        }
    }
}
