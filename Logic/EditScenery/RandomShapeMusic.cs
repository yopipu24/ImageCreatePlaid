using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageCreatePlaid
{

    public class RandomShapeMusic : SceneryInterface
    {
        private static readonly string[] Symbols =
        {
        "♪",
        "♫",
        "♬",
        "♩",
        "♭",
        "♯"
        };

        public Bitmap EditImage(Bitmap bmp, SceneryModel model)
        {
            using Graphics g = Graphics.FromImage(bmp);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;

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

            int symbolCount = model.ShapeCount; ;
            int minFontSize = model.ShapeMinSize;
            int maxFontSize = model.HorizontalSize1;
            int margin = 8;
            int maxRetry = 100;

            List<RectangleF> usedAreas = new List<RectangleF>();

            for (int i = 0; i < symbolCount; i++)
            {
                bool found = false;

                for (int retry = 0; retry < maxRetry; retry++)
                {
                    string symbol = Symbols[random.Next(Symbols.Length)];
                    int fontSize = random.Next(minFontSize, maxFontSize + 1);

                    using Font font = new Font("Segoe UI Symbol", fontSize, FontStyle.Bold, GraphicsUnit.Pixel);

                    SizeF textSize = g.MeasureString(symbol, font);

                    if (textSize.Width >= bmp.Width || textSize.Height >= bmp.Height)
                    {
                        continue;
                    }

                    float x = random.Next(0, Math.Max(1, bmp.Width - (int)textSize.Width));
                    float y = random.Next(0, Math.Max(1, bmp.Height - (int)textSize.Height));

                    RectangleF rect = new RectangleF(x, y, textSize.Width, textSize.Height);
                    RectangleF hitArea = Inflate(rect, margin);

                    if (IsOverlapped(hitArea, usedAreas))
                    {
                        continue;
                    }

                    Color symbolColor = random.Next(2) == 0 ? color1 : color2;
                    using Brush brush = new SolidBrush(symbolColor);

                    g.DrawString(symbol, font, brush, x, y);

                    usedAreas.Add(hitArea);
                    found = true;
                    break;
                }

                if (!found)
                {
                    continue;
                }
            }

            return bmp;
        }

        private static RectangleF Inflate(RectangleF rect, float margin)
        {
            return new RectangleF(
                rect.X - margin,
                rect.Y - margin,
                rect.Width + margin * 2,
                rect.Height + margin * 2
            );
        }

        private static bool IsOverlapped(RectangleF target, List<RectangleF> usedAreas)
        {
            foreach (RectangleF area in usedAreas)
            {
                if (target.IntersectsWith(area))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
