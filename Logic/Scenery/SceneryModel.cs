using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageCreatePlaid
{
    public class SceneryModel
    {
        public SceneryModel()
        {
            BaseColorRed = 255;
            BaseColorGreen = 255;
            BaseColorBlue = 255;
        }

        /// <summary>
        /// 背景種類
        /// </summary>
        public string KindScenery { get; set; }
        public int Alpha { get; set; }
        public int BaseBalance { get; set; }

        public int BaseColorRed { get; set; }
        public int BaseColorGreen { get; set; }
        public int BaseColorBlue { get; set; }
        public int BaseAlpha { get; set; }

        public string StepPattern { get; set; }
        public string StepColorPattern { get; set; }

        public float HueValue { get; set; }
        public float SaturationValue { get; set; }

        /// <summary>
        /// 縦線１
        /// </summary>
        public int VerticalColorRed1 { get; set; }
        public int VerticalColorGreen1 { get; set; }
        public int VerticalColorBlue1 { get; set; }
        public int VerticalColorAlpha1 { get; set; }
        public int VerticalSize1 { get; set; }
        public int VerticalCount1 { get; set; }
        public int VerticalDottedWeight { get; set; }
        public string VerticalDottedPattern { get; set; }

        /// <summary>
        /// 縦線２
        /// </summary>
        public int VerticalColorRed2 { get; set; }
        public int VerticalColorGreen2 { get; set; }
        public int VerticalColorBlue2 { get; set; }
        public int VerticalColorAlpha2 { get; set; }
        public int VerticalSize2 { get; set; }
        public int VerticalCount2 { get; set; }

        /// <summary>
        /// 横線１
        /// </summary>
        public int HorizontalColorRed1 { get; set; }
        public int HorizontalColorGreen1 { get; set; }
        public int HorizontalColorBlue1 { get; set; }
        public int HorizontalColorAlpha1 { get; set; }
        public int HorizontalSize1 { get; set; }
        public int HorizontalCount1 { get; set; }
        public int HorizontalDottedWeight { get; set; }
        public string HorizontalDottedPattern { get; set; }

        /// <summary>
        /// 横線２
        /// </summary>
        public int HorizontalColorRed2 { get; set; }
        public int HorizontalColorGreen2 { get; set; }
        public int HorizontalColorBlue2 { get; set; }
        public int HorizontalColorAlpha2 { get; set; }
        public int HorizontalSize2 { get; set; }
        public int HorizontalCount2 { get; set; }

        /// <summary>
        /// ランダム図形用
        /// </summary>
        public int ShapeCount { get; set; }
        public int ShapeMinSize { get; set; }
    }
}
