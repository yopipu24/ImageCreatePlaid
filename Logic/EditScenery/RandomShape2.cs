using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageCreatePlaid
{
    public class RandomShape2 : SceneryInterface
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

            Color color1 = Color.FromArgb(
                model.Alpha,
                model.VerticalColorRed1,
                model.VerticalColorGreen1,
                model.VerticalColorBlue1
            );

            Color color2 = Color.FromArgb(
                model.Alpha,
                model.VerticalColorRed2,
                model.VerticalColorGreen2,
                model.VerticalColorBlue2
            );

            g.Clear(baseColor);

            Random random = new Random();

            int shapeCount = model.ShapeCount;
            int minSize = model.ShapeMinSize;
            int maxSize = model.HorizontalSize1;
            int margin = 5;
            int maxRetry = 200;

            List<Rectangle> usedAreas = new List<Rectangle>();

            for (int i = 0; i < shapeCount; i++)
            {
                Rectangle rect;
                bool found = false;

                for (int retry = 0; retry < maxRetry; retry++)
                {
                    int size = random.Next(minSize, maxSize + 1);

                    int x = random.Next(0, bmp.Width - size);
                    int y = random.Next(0, bmp.Height - size);

                    rect = new Rectangle(x, y, size, size);

                    Rectangle hitArea = Inflate(rect, margin);

                    if (!IsOverlapped(hitArea, usedAreas))
                    {
                        Color color = random.Next(2) == 0 ? color1 : color2;
                        DrawRandomShape(g, random, rect, color);

                        usedAreas.Add(hitArea);
                        found = true;
                        break;
                    }
                }

                // 空き場所が見つからなかった場合は、その図形は描かない
                if (!found)
                {
                    continue;
                }
            }

            return bmp;
        }

        private static Rectangle Inflate(Rectangle rect, int margin)
        {
            return new Rectangle(
                rect.X - margin,
                rect.Y - margin,
                rect.Width + margin * 2,
                rect.Height + margin * 2
            );
        }

        private static bool IsOverlapped(Rectangle target, List<Rectangle> usedAreas)
        {
            foreach (Rectangle area in usedAreas)
            {
                if (target.IntersectsWith(area))
                {
                    return true;
                }
            }

            return false;
        }

        private static void DrawRandomShape(
            Graphics g,
            Random random,
            Rectangle rect,
            Color color)
        {
            PatternShape shape = (PatternShape)random.Next(0, 4);

            using Brush brush = new SolidBrush(color);
            using Pen pen = new Pen(color, Math.Max(2, rect.Width / 8));

            switch (shape)
            {
                case PatternShape.Circle:
                    g.FillEllipse(brush, rect);
                    break;

                case PatternShape.Square:
                    g.FillRectangle(brush, rect);
                    break;

                case PatternShape.Triangle:
                    Point[] points =
                    {
                    new Point(rect.Left + rect.Width / 2, rect.Top),
                    new Point(rect.Left, rect.Bottom),
                    new Point(rect.Right, rect.Bottom)
                };

                    g.FillPolygon(brush, points);
                    break;

                case PatternShape.Cross:
                    g.DrawLine(pen, rect.Left, rect.Top, rect.Right, rect.Bottom);
                    g.DrawLine(pen, rect.Right, rect.Top, rect.Left, rect.Bottom);
                    break;
            }
        }
    }
}
