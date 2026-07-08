using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageCreatePlaid
{
    public class StepCircle : SceneryInterface
    {
        public Bitmap EditImage(Bitmap bmp, SceneryModel model)
        {
            int width = bmp.Width;
            int height = bmp.Height;
            Color basecolor = Color.FromArgb(model.BaseAlpha, model.BaseColorRed, model.BaseColorGreen, model.BaseColorBlue);
            int wStepCount = width / model.HorizontalSize1;
            int hStepCount = height / model.VerticalSize1;
            Color h1color = Color.FromArgb(model.Alpha, model.HorizontalColorRed1, model.HorizontalColorGreen1, model.HorizontalColorBlue1);
            Color v1color = Color.FromArgb(model.Alpha, model.VerticalColorRed1, model.VerticalColorGreen1, model.VerticalColorBlue1);
            Color h2color = Color.FromArgb(model.Alpha, model.HorizontalColorRed2, model.HorizontalColorGreen2, model.HorizontalColorBlue2);
            Color v2color = Color.FromArgb(model.Alpha, model.VerticalColorRed2, model.VerticalColorGreen2, model.VerticalColorBlue2);
            string pattern = model.StepPattern;
            Color aColor = h1color;

            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.Clear(basecolor);
                for (int x = 0; x < wStepCount; x++)
                {
                    for (int y = 0; y < hStepCount; y++)
                    {
                        int circleSize = 0;
                        if (pattern == "right") {
                            circleSize = model.HorizontalSize1 * x / wStepCount;
                            if (model.StepColorPattern == "two") aColor = x % 2 == 0 ? h1color : h2color;
                        }
                        else if (pattern == "bottom")
                        {
                            circleSize = model.VerticalSize1 * y / hStepCount;
                            if (model.StepColorPattern == "two") aColor = y % 2 == 0 ? v1color : v2color;
                        }
                        else
                        {
                            circleSize = model.HorizontalSize1 * x / wStepCount * y / hStepCount;
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
                        int offsetX = x * model.HorizontalSize1 + (model.HorizontalSize1 - circleSize) / 2;
                        int offsetY = y * model.VerticalSize1 + (model.VerticalSize1 - circleSize) / 2;


                        using (Brush brush = new SolidBrush(aColor))
                        {
                            g.FillEllipse(brush, offsetX, offsetY, circleSize, circleSize);
                        }
                    }
                }
            }

            return bmp;
        }
    }
}
