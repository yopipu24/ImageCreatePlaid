namespace ImageCreatePlaid.Models
{
    public class EnumModel
    {
        public enum DottedValue {
            one,
            two,
            three
        }

        public enum StepColorPatternValue 
        {
            one,
            two,
            alt
        }

        public enum FramePatternType
        {
            Checker,
            Triangle,
            Diamond,
            Dot
        }

        public enum RandomFramePattern
        {
            PolkaDot,      // ランダムなドット
            Stitch,        // 点線・縫い目風
            Confetti,      // 紙吹雪風
            ZigZag,        // 山形模様
            DiagonalLine   // 斜線
        }

        public enum FramePatternValue
        {
            topbottom,
            all
        }
    }
}
