using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageCreatePlaid
{
    public enum PatternShape
    {
        Circle,
        Cross,
        Square,
        Triangle
    }

    public class RandomShape : SceneryInterface
    {
        public Bitmap EditImage(Bitmap bmp, SceneryModel model)
        {
            using Graphics g = Graphics.FromImage(bmp);

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

            // 2色目を使う場合。SceneryModel 側に存在する名前に合わせて変更してください。
            Color color2 = Color.FromArgb(
                model.Alpha,
                model.VerticalColorRed2,
                model.VerticalColorGreen2,
                model.VerticalColorBlue2
            );

            g.Clear(baseColor);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            int cellWidth = model.HorizontalSize1;
            int cellHeight = model.VerticalSize1;

            Random random = new Random();

            for (int x = 0; x < bmp.Width; x += cellWidth)
            {
                for (int y = 0; y < bmp.Height; y += cellHeight)
                {
                    PatternShape shape = GetRandomShape(random);

                    Color color = random.Next(2) == 0 ? color1 : color2;

                    using Brush brush = new SolidBrush(color);
                    using Pen pen = new Pen(color, Math.Max(2, Math.Min(cellWidth, cellHeight) / 10));

                    Rectangle rect = CreateShapeRect(x, y, cellWidth, cellHeight);

                    DrawShape(g, shape, rect, brush, pen);
                }
            }

            return bmp;
        }
        private static PatternShape GetRandomShape(Random random)
        {
            PatternShape[] shapes =
            {
                PatternShape.Circle,
                PatternShape.Cross,
                PatternShape.Square,
                PatternShape.Triangle
            };

            return shapes[random.Next(shapes.Length)];
        }

        private static Rectangle CreateShapeRect(int x, int y, int cellWidth, int cellHeight)
        {
            int size = Math.Min(cellWidth, cellHeight) / 2;
            int left = x + (cellWidth - size) / 2;
            int top = y + (cellHeight - size) / 2;

            return new Rectangle(left, top, size, size);
        }

        private static void DrawShape(
            Graphics g,
            PatternShape shape,
            Rectangle rect,
            Brush brush,
            Pen pen)
        {
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
