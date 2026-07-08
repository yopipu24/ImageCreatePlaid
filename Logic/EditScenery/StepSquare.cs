using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MudBlazor.CategoryTypes;

namespace ImageCreatePlaid
{
    public class StepSquare : SceneryInterface
    {
        public Bitmap EditImage(Bitmap bmp, SceneryModel model)
        {
            int width = bmp.Width;
            int height = bmp.Height;
            Color basecolor = Color.FromArgb(model.BaseAlpha, model.BaseColorRed, model.BaseColorGreen, model.BaseColorBlue);
            int wStepCount = width / model.HorizontalSize1;
            int hStepCount = height / model.VerticalSize1;
            Color h1color = Color.FromArgb(model.Alpha, model.HorizontalColorRed1, model.HorizontalColorGreen1, model.HorizontalColorBlue1);
            Color v1color = Color.FromArgb(model.Alpha, model.VerticalColorRed1, model.VerticalColorGreen1, model.VerticalColorBlue1); ;
            Color h2color = Color.FromArgb(model.Alpha, model.HorizontalColorRed2, model.HorizontalColorGreen2, model.HorizontalColorBlue2); ;
            Color v2color = Color.FromArgb(model.Alpha, model.VerticalColorRed2, model.VerticalColorGreen2, model.VerticalColorBlue2); ;
            string pattern = model.StepPattern;
            Color aColor = h1color;

            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.Clear(basecolor);
                for (int x = 0; x < wStepCount; x++)
                {
                    for (int y = 0; y < hStepCount; y++)
                    {
                        int squareSize = 0;
                        if (pattern == "right") {
                            squareSize = model.HorizontalSize1 * x / wStepCount;
                            if (model.StepColorPattern == "two") aColor = x % 2 == 0 ? h1color : h2color;
                        }
                        else if (pattern == "bottom")
                        {
                            squareSize = model.VerticalSize1 * y / hStepCount;
                            if (model.StepColorPattern == "two") aColor = y % 2 == 0 ? v1color : v2color;
                        }
                        else
                        {
                            squareSize = model.HorizontalSize1 * x / wStepCount * y / hStepCount;
                            if (model.StepColorPattern == "two") aColor = aColor == h1color ? h2color : h1color;
                        }

                        if (model.StepColorPattern == "alt")
                        {
                            if (x % 2 == 0 && y % 2 == 0 || x % 2 == 1 && y % 2 == 1)
                            {
                                aColor = h1color;
                            }
                            else
                            {
                                aColor = h2color;
                            }
                        }
                        int offsetX = x * model.HorizontalSize1 + (model.HorizontalSize1 - squareSize) / 2;
                        int offsetY = y * model.VerticalSize1 + (model.VerticalSize1 - squareSize) / 2;

                        Rectangle rect = new Rectangle(offsetX, offsetY, squareSize, squareSize);
                        using (GraphicsPath path = CreateRoundedRectanglePath(rect, 15))
                        {
                            using (Brush brush = new SolidBrush(aColor))
                            {
                                g.FillPath(brush, path);
                            }

                        }
                    }
                }
            }

            return bmp;
        }

        /// <summary>
        /// 角丸矩形のパスを作成する
        /// </summary>
        private static GraphicsPath CreateRoundedRectanglePath(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();

            int diameter = radius * 2;
            Size size = new Size(diameter, diameter);

            // 左上
            path.AddArc(new Rectangle(rect.Location, size), 180, 90);
            // 右上
            path.AddArc(new Rectangle(new Point(rect.Right - diameter, rect.Top), size), 270, 90);
            // 右下
            path.AddArc(new Rectangle(new Point(rect.Right - diameter, rect.Bottom - diameter), size), 0, 90);
            // 左下
            path.AddArc(new Rectangle(new Point(rect.Left, rect.Bottom - diameter), size), 90, 90);

            path.CloseFigure();
            return path;
        }
    }
}
