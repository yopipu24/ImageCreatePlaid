using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageCreatePlaid
{
    public class TartanCheck5 : PlaidInterface
    {
        public Bitmap EditImage(Bitmap bmp, PlaidModel model)
        {
            int width = bmp.Width;
            int height = bmp.Height;
            int stripeSize1 = model.HorizontalSize1 / 2;
            int stripeSize2 = BussinessLogic.GetRandomInt(model.HorizontalSize1 / 3) + 10;
            int stripeSize3 = BussinessLogic.GetRandomInt(model.HorizontalSize1 / 4) + 15;
            var hColor1 = Color.FromArgb(180, model.HorizontalColorRed1, model.HorizontalColorGreen1, model.HorizontalColorBlue1);
            var hColor2 = Color.FromArgb(180, model.HorizontalColorRed2, model.HorizontalColorGreen2, model.HorizontalColorBlue2);
            var randomColor2 = Color.FromArgb(120, BussinessLogic.GetRandomInt(255), BussinessLogic.GetRandomInt(255), BussinessLogic.GetRandomInt(255));
            var randomColor3 = Color.FromArgb(120, BussinessLogic.GetRandomInt(255), BussinessLogic.GetRandomInt(255), BussinessLogic.GetRandomInt(255));
            var offset = BussinessLogic.GetRandomInt(model.HorizontalSize1);
            var offset2 = BussinessLogic.GetRandomInt(model.HorizontalSize1);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.FromArgb(model.BaseAlpha, model.BaseColorRed, model.BaseColorGreen, model.BaseColorBlue));

                DrawTartan(g, width, height, stripeSize1, stripeSize2, hColor1, hColor2);
                DrawDiagonalMesh(g, width, height, stripeSize3);
            }

            return bmp;
        }

        // 🔹 タータンチェックの基本ストライプを描く
        static void DrawTartan(Graphics g, int width, int height, int s1, int s2, Color c1, Color c2)
        {
            var baseSize = BussinessLogic.GetRandomInt(s1);
            using (Brush redBrush = new SolidBrush(c1))
            using (Brush blueBrush = new SolidBrush(c2))
            {
                // 縦のストライプ
                for (int x = 0; x < width; x += s1 + s2 + baseSize)
                {
                    g.FillRectangle(redBrush, x, 0, s1, height);
                    g.FillRectangle(blueBrush, x + s1, 0, s2, height);
                }

                // 横のストライプ
                for (int y = 0; y < height; y += s1 + s2 + baseSize)
                {
                    g.FillRectangle(redBrush, 0, y, width, s1);
                    g.FillRectangle(blueBrush, 0, y + s1, width, s2);
                }
            }
        }

        // 🔹 斜めの網目（メッシュ）を描画
        static void DrawDiagonalMesh(Graphics g, int width, int height, int spacing)
        {
            var randomColor = Color.FromArgb(120, BussinessLogic.GetRandomInt(255), BussinessLogic.GetRandomInt(255), BussinessLogic.GetRandomInt(255));
            using (Pen meshPen = new Pen(randomColor, BussinessLogic.GetRandomInt(3)))
            {
                meshPen.DashStyle = DashStyle.Dot; // 🔹 点線の網目

                // 🔹 斜めに格子線を描く
                for (int i = -1 * width; i < width; i += spacing)
                {
                    g.DrawLine(meshPen, i, 0, i + width, height);
                }

                for (int i = 0; i < width * 2; i += spacing)
                {
                    g.DrawLine(meshPen, i, 0, i - 1*width, height);
                }

            }
        }
    }
}
