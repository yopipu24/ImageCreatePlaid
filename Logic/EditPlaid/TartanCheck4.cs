using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageCreatePlaid
{
    public class TartanCheck4 : PlaidInterface
    {
        public Bitmap EditImage(Bitmap bmp, PlaidModel model)
        {
            int width = bmp.Width;
            int height = bmp.Height;
            int stripeSize1 = model.HorizontalSize1 / 5;
            int stripeSize2 = BussinessLogic.GetRandomInt(10) + 1;
            var randomColor = Color.FromArgb(120, BussinessLogic.GetRandomInt(255), BussinessLogic.GetRandomInt(255), BussinessLogic.GetRandomInt(255));
            var randomColor2 = Color.FromArgb(120, BussinessLogic.GetRandomInt(255), BussinessLogic.GetRandomInt(255), BussinessLogic.GetRandomInt(255));
            var randomColor3 = Color.FromArgb(120, BussinessLogic.GetRandomInt(255), BussinessLogic.GetRandomInt(255), BussinessLogic.GetRandomInt(255));
            var offset = BussinessLogic.GetRandomInt(model.HorizontalSize1);
            var offset2 = BussinessLogic.GetRandomInt(model.HorizontalSize1);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.FromArgb(model.BaseAlpha, model.BaseColorRed, model.BaseColorGreen, model.BaseColorBlue));

                for (int x = 0; x < width; x += stripeSize1 + stripeSize2 * 3)
                {
                    using (Brush brush = new SolidBrush(Color.FromArgb(180, model.HorizontalColorRed1, model.HorizontalColorGreen1, model.HorizontalColorBlue1)))
                    {
                        g.FillRectangle(brush, x, 0, stripeSize1, height);
                    }

                    using (Brush brush = new SolidBrush(Color.FromArgb(150, model.HorizontalColorRed2, model.HorizontalColorGreen2, model.HorizontalColorBlue2)))
                    {
                        g.FillRectangle(brush, x + stripeSize1, 0, stripeSize2, height);
                    }

                }

                for (int y = 0; y < height; y += stripeSize1 + stripeSize2 * 3)
                {
                    using (Brush brush = new SolidBrush(Color.FromArgb(180, model.HorizontalColorRed1, model.VerticalColorGreen1, model.VerticalColorBlue1)))
                    {
                        g.FillRectangle(brush, 0, y, width, stripeSize1);
                    }

                    using (Brush brush = new SolidBrush(Color.FromArgb(150, model.HorizontalColorRed2, model.HorizontalColorGreen2, model.HorizontalColorBlue2)))
                    {
                        g.FillRectangle(brush, 0, y + stripeSize1, width, stripeSize2);
                    }

                }

                // 点線
                Pen dashedPen = new Pen(randomColor3, 3);
                dashedPen.DashStyle = DashStyle.DashDot;
                // 縦
                for (int x = 0 + offset2; x < width + offset2; x += 50)
                {
                    g.DrawLine(dashedPen, x, 0, x, height);
                }

                // 横
                for (int y = 0 + offset2; y < height + offset2; y += 50)
                {
                    g.DrawLine(dashedPen, 0, y, width, y);
                }

                // 網目
                Pen gridPen = new Pen(randomColor, BussinessLogic.GetRandomInt(3));
                for (int x = 0 + offset; x < width + offset; x += model.HorizontalSize1)
                {
                    g.DrawLine(gridPen, x, 0, x, height);
                }

                for (int y = 0 + offset; y < height + offset; y += model.HorizontalSize1)
                {
                    g.DrawLine(gridPen, 0, y, width, y);
                }

                // 網目の交差点
                Brush dotBrush = new SolidBrush(randomColor2);
                for (int y = 0 + offset; y <= height + offset; y += model.HorizontalSize1)
                {
                    for (int x = 0 + offset; x <= width + offset; x += model.HorizontalSize1)
                    {
                        g.FillEllipse(dotBrush, x - stripeSize1/4, y - stripeSize1/4, stripeSize1/2, stripeSize1/2); // 中心に小さな点
                    }
                }
            }

            return bmp;
        }
    }
}
