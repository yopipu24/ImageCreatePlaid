using static ImageCreatePlaid.Pages.CreateFrame;

namespace ImageCreatePlaid
{
    public class FrameModel
    {
        public FrameModel()
        {
            BaseColorRed = 255;
            BaseColorGreen = 255;
            BaseColorBlue = 255;
            BaseAlpha = 0;
        }

        /// <summary>
        /// 枠種類
        /// </summary>
        public string KindFrame { get; set; }
        
        public byte BaseColorRed { get; set; }
        public byte BaseColorGreen { get; set; }
        public byte BaseColorBlue { get; set; }
        public byte BaseAlpha { get; set; }

        public int FrameSize { get; set; }
        public int RepeatSize { get; set; }
        public FramePatternValue FrameType { get; set; }

        /// <summary>
        /// 色１
        /// </summary>
        public byte ColorGreen1 { get; set; }
        public byte ColorRed1 { get; set; }
        public byte ColorBlue1 { get; set; }

        /// <summary>
        /// 色２
        /// </summary>
        public byte ColorRed2 { get; set; }
        public byte ColorGreen2 { get; set; }
        public byte ColorBlue2 { get; set; }

        /// <summary>
        /// 色３
        /// </summary>
        public byte ColorRed3 { get; set; }
        public byte ColorGreen3 { get; set; }
        public byte ColorBlue3 { get; set; }

    }
}
