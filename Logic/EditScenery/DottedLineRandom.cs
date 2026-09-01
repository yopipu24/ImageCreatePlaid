using SkiaSharp;

namespace ImageCreatePlaid;

public class DottedLineRandom : SceneryInterface
{
    public SKBitmap EditImage(SKBitmap bmp, SceneryModel model)
    {
        int width = bmp.Width;
        int height = bmp.Height;

        int horizontalSize = model.HorizontalSize1;
        int verticalSize = model.VerticalSize1;

        int px = BussinessLogic.GetRandomInt(horizontalSize);
        int py = BussinessLogic.GetRandomInt(verticalSize);
        int px2 = BussinessLogic.GetRandomInt(horizontalSize);
        int py2 = BussinessLogic.GetRandomInt(verticalSize);

        var baseColor = new SKColor(
            model.BaseColorRed, model.BaseColorGreen, model.BaseColorBlue, model.BaseAlpha);
        var h1Color = new SKColor(
            model.HorizontalColorRed1, model.HorizontalColorGreen1, model.HorizontalColorBlue1, model.Alpha);
        var v1Color = new SKColor(
            model.VerticalColorRed1, model.VerticalColorGreen1, model.VerticalColorBlue1, model.Alpha);
        var h2Color = new SKColor(
            model.HorizontalColorRed2, model.HorizontalColorGreen2, model.HorizontalColorBlue2, model.Alpha);
        var v2Color = new SKColor(
            model.VerticalColorRed2, model.VerticalColorGreen2, model.VerticalColorBlue2, model.Alpha);

        int[] hlines = BussinessLogic.GetDotPattern(
            model.VerticalDottedPattern, horizontalSize);
        int[] vlines = BussinessLogic.GetDotPattern(
            model.HorizontalDottedPattern, verticalSize);

        int horizontalRange = horizontalSize / 100 + model.HorizontalDottedWeight;
        int verticalRange = verticalSize / 100 + model.VerticalDottedWeight;

        // X/Y ごとの判定を前計算する
        var isHLine = new bool[width];
        var isH1Area = new bool[width];
        var isH2Area = new bool[width];

        for (int x = 0; x < width; x++)
        {
            int localX = x % horizontalSize;

            isHLine[x] = hlines[localX] == 1;
            isH1Area[x] = localX > px - horizontalRange &&
                          localX < px + horizontalRange;
            isH2Area[x] = localX > px2 - horizontalRange &&
                          localX < px2 + horizontalRange;
        }

        var isVLine = new bool[height];
        var isV1Area = new bool[height];
        var isV2Area = new bool[height];

        for (int y = 0; y < height; y++)
        {
            int localY = y % verticalSize;

            isVLine[y] = vlines[localY] == 1;
            isV1Area[y] = localY > py - verticalRange &&
                          localY < py + verticalRange;
            isV2Area[y] = localY > py2 - verticalRange &&
                          localY < py2 + verticalRange;
        }

        // 配列をまとめて設定する
        var pixels = new SKColor[width * height];

        for (int y = 0; y < height; y++)
        {
            int rowOffset = y * width;

            for (int x = 0; x < width; x++)
            {
                SKColor color;

                if (isH2Area[x] && isVLine[y])
                    color = h2Color;
                else if (isV2Area[y] && isHLine[x])
                    color = v2Color;
                else if (isH1Area[x] && isVLine[y])
                    color = h1Color;
                else if (isV1Area[y] && isHLine[x])
                    color = v1Color;
                else
                    color = baseColor;

                pixels[rowOffset + x] = color;
            }
        }

        bmp.Pixels = pixels;
        return bmp;
    }
}