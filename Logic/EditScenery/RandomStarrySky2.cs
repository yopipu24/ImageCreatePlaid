using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageCreatePlaid
{
    public class RandomStarrySky2 : SceneryInterface
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

            Color starColor2 = Color.FromArgb(
                model.Alpha,
                model.VerticalColorRed2,
                model.VerticalColorGreen2,
                model.VerticalColorBlue2
            );

            g.Clear(baseColor);

            Random random = new Random();

            int bigStarCount = model.LargeStarCount;
            int smallStarCount = model.SmallStarCount;
            int sparkleCount = model.SparkleCount;

            DrawAnimeStars(g, bmp.Width, bmp.Height, random, bigStarCount, 24, 46, starColor1, starColor2);
            DrawAnimeStars(g, bmp.Width, bmp.Height, random, smallStarCount, 10, 22, starColor1, starColor2);
            DrawSparkles(g, bmp.Width, bmp.Height, random, sparkleCount, starColor1, starColor2);

            return bmp;
        }

        private static void DrawAnimeStars(
            Graphics g,
            int width,
            int height,
            Random random,
            int count,
            int minSize,
            int maxSize,
            Color color1,
            Color color2)
        {
            for (int i = 0; i < count; i++)
            {
                int size = random.Next(minSize, maxSize + 1);
                int radius = size / 2;

                int x = random.Next(radius, Math.Max(radius + 1, width - radius));
                int y = random.Next(radius, Math.Max(radius + 1, height - radius));

                Color color = random.Next(2) == 0 ? color1 : color2;
                float rotation = random.Next(0, 360);

                PointF[] star = CreateStarPoints(
                    x,
                    y,
                    radius,
                    radius * 0.45f,
                    5,
                    rotation
                );

                using Brush brush = new SolidBrush(color);
                g.FillPolygon(brush, star);

                // アニメっぽい白いハイライト
                if (size >= 24)
                {
                    DrawHighlight(g, x, y, radius);
                }
            }
        }

        private static PointF[] CreateStarPoints(
            float centerX,
            float centerY,
            float outerRadius,
            float innerRadius,
            int points,
            float rotationDegrees)
        {
            PointF[] result = new PointF[points * 2];

            double rotation = Math.PI / 180 * rotationDegrees;
            double step = Math.PI / points;

            for (int i = 0; i < result.Length; i++)
            {
                double angle = rotation - Math.PI / 2 + step * i;
                float radius = i % 2 == 0 ? outerRadius : innerRadius;

                result[i] = new PointF(
                    centerX + (float)Math.Cos(angle) * radius,
                    centerY + (float)Math.Sin(angle) * radius
                );
            }

            return result;
        }

        private static void DrawHighlight(Graphics g, int x, int y, int radius)
        {
            int highlightSize = Math.Max(3, radius / 4);

            using Brush brush = new SolidBrush(Color.FromArgb(180, 255, 255, 255));

            g.FillEllipse(
                brush,
                x - radius / 4,
                y - radius / 3,
                highlightSize,
                highlightSize
            );
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

                using Pen pen = new Pen(color, 2);

                g.DrawLine(pen, x - half, y, x + half, y);
                g.DrawLine(pen, x, y - half, x, y + half);
            }
        }
    }
}
