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
        public byte Alpha { get; set; }
        public int BaseBalance { get; set; }

        public byte BaseColorRed { get; set; }
        public byte BaseColorGreen { get; set; }
        public byte BaseColorBlue { get; set; }
        public byte BaseAlpha { get; set; }

        public string StepPattern { get; set; }
        public string StepColorPattern { get; set; }

        public float HueValue { get; set; }
        public float SaturationValue { get; set; }

        /// <summary>
        /// 縦線１
        /// </summary>
        public byte VerticalColorRed1 { get; set; }
        public byte VerticalColorGreen1 { get; set; }
        public byte VerticalColorBlue1 { get; set; }
        public byte VerticalColorAlpha1 { get; set; }
        public int VerticalSize1 { get; set; }
        public int VerticalCount1 { get; set; }
        public int VerticalDottedWeight { get; set; }
        public string VerticalDottedPattern { get; set; }

        /// <summary>
        /// 縦線２
        /// </summary>
        public byte VerticalColorRed2 { get; set; }
        public byte VerticalColorGreen2 { get; set; }
        public byte VerticalColorBlue2 { get; set; }
        public byte VerticalColorAlpha2 { get; set; }
        public int VerticalSize2 { get; set; }
        public int VerticalCount2 { get; set; }

        /// <summary>
        /// 横線１
        /// </summary>
        public byte HorizontalColorRed1 { get; set; }
        public byte HorizontalColorGreen1 { get; set; }
        public byte HorizontalColorBlue1 { get; set; }
        public byte HorizontalColorAlpha1 { get; set; }
        public int HorizontalSize1 { get; set; }
        public int HorizontalCount1 { get; set; }
        public int HorizontalDottedWeight { get; set; }
        public string HorizontalDottedPattern { get; set; }

        /// <summary>
        /// 横線２
        /// </summary>
        public byte HorizontalColorRed2 { get; set; }
        public byte HorizontalColorGreen2 { get; set; }
        public byte HorizontalColorBlue2 { get; set; }
        public byte HorizontalColorAlpha2 { get; set; }
        public int HorizontalSize2 { get; set; }
        public int HorizontalCount2 { get; set; }

        /// <summary>
        /// ランダム図形用
        /// </summary>
        public int ShapeCount { get; set; }
        public int ShapeMinSize { get; set; }

        /// <summary>
        /// 星空用
        /// </summary>
        public int LargeStarCount { get; set; }
        public int SmallStarCount { get; set; }
        public int SparkleCount { get; set; }

        /// <summary>
        /// 雪用
        /// </summary>
        public int CrystalCount { get; set; }
        public int SnowMinSize { get; set; }
        public int SnowMaxSize { get; set; }
        public float SnowMargin { get; set; }
    }
}
