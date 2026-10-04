using SkiaSharp;

namespace ImageCreatePlaid
{
    public class StitchCheck2 : PlaidInterface
    {
        public SKBitmap EditImage(SKBitmap bmp, PlaidModel model)
        {
            int width = bmp.Width;
            int height = bmp.Height;

            // --- 調整用パラメータ ---
            float randomness = 1f;        // ばらつき全体の強さ(0=なし、1=標準、2=強め)
            float insetRatio = 0.14f;     // 縫い目の位置(マスの端からの距離。短い辺に対する割合)
            float cornerRatio = 0.12f;    // 縫い目の四角の角の丸み(0〜0.5)
            float lineWidthRatio = 0.04f; // 糸の太さ(マスの短い辺に対する割合)
            float stitchLen = 3f;         // 1針の長さ(糸の太さに対する倍率)

            // 以下は randomness=1 のときのばらつき幅
            float lengthJitter = 0.25f;   // 針の長さのばらつき(±割合)
            float spaceJitter = 0.12f;    // 針の間隔のばらつき(±割合)
            float wobble = 0.35f;         // 針の横ぶれ(糸の太さに対する割合)
            float frameRotate = 1.5f;     // 縫い目の四角ごとの傾き(±度)
            float framePosJitter = 0.02f; // 縫い目の四角の位置のずれ(マスの短い辺に対する割合)
            float toneJitter = 0.05f;     // マスの色と糸の色の濃淡のばらつき(±割合)
            float swapRate = 0.05f;       // マスの色が、市松と逆になる確率
            float noStitchRate = 0.08f;   // 縫い目が入らないマスの確率
            // -----------------------

            var rand = new Random();

            var color1 = new SKColor(model.VerticalColorRed1, model.VerticalColorGreen1, model.VerticalColorBlue1, model.Alpha);
            var color2 = new SKColor(model.HorizontalColorRed2, model.HorizontalColorGreen2, model.HorizontalColorBlue2, model.Alpha);
            var threadColor = new SKColor(model.BaseColorRed, model.BaseColorGreen, model.BaseColorBlue, model.BaseAlpha);

            // マスの幅は VerticalSize1、高さは HorizontalSize1
            int cellW = Math.Max(8, model.VerticalSize1);
            int cellH = Math.Max(8, model.HorizontalSize1);
            float minCell = Math.Min(cellW, cellH);

            float inset = minCell * insetRatio;
            float lineWidth = Math.Max(1f, minCell * lineWidthRatio);

            // 縫い目の四角(中心を原点にして作っておき、マスごとに移動・回転して使う)
            float w = cellW - inset * 2f;
            float h = cellH - inset * 2f;
            float r = Math.Min(w, h) * Math.Clamp(cornerRatio, 0f, 0.5f);

            using var framePath = new SKPath();
            framePath.AddRoundRect(new SKRect(-w / 2f, -h / 2f, w / 2f, h / 2f), r, r);
            using var measure = new SKPathMeasure(framePath, true);
            float perimeter = measure.Length;

            // 一周の長さに合わせて針の間隔を調整し、針の数を決める
            float targetPeriod = lineWidth * stitchLen * 1.8f;
            int stitchCount = Math.Max(4, (int)MathF.Round(perimeter / targetPeriod));
            float period = perimeter / stitchCount;

            // 丸い線端の分だけ短くして、見た目の針の長さを period の約55%にする
            float on = Math.Max(0.5f, period * 0.55f - lineWidth);

            using var canvas = new SKCanvas(bmp);
            canvas.Clear(color2);

            // 下地に合成せず、そのまま置き換えて描く(Alphaが低くても色が濁らない)
            using var fill = new SKPaint
            {
                IsAntialias = true,
                Style = SKPaintStyle.Fill,
                BlendMode = SKBlendMode.Src
            };

            using var thread = new SKPaint
            {
                IsAntialias = true,
                Style = SKPaintStyle.Stroke,
                StrokeWidth = lineWidth,
                StrokeCap = SKStrokeCap.Round,
                BlendMode = SKBlendMode.Src
            };

            using var segment = new SKPath();

            int cols = (int)Math.Ceiling(width / (float)cellW);
            int rows = (int)Math.Ceiling(height / (float)cellH);

            for (int j = 0; j < rows; j++)
            {
                for (int i = 0; i < cols; i++)
                {
                    // マスの色(市松。たまに逆の色になる)
                    bool useColor1 = ((i + j) & 1) == 0;
                    if (rand.NextDouble() < swapRate * randomness)
                    {
                        useColor1 = !useColor1;
                    }

                    fill.Color = Tone(useColor1 ? color1 : color2, Random11(rand) * toneJitter * randomness);
                    canvas.DrawRect(i * cellW, j * cellH, cellW, cellH, fill);

                    // 縫い目が入らないマス
                    if (rand.NextDouble() < noStitchRate) continue;

                    thread.Color = Tone(threadColor, Random11(rand) * toneJitter * 0.5f * randomness);

                    // 縫い目の四角を、マスの中心に置いて少し傾ける
                    float cx = (i + 0.5f) * cellW + Random11(rand) * framePosJitter * minCell * randomness;
                    float cy = (j + 0.5f) * cellH + Random11(rand) * framePosJitter * minCell * randomness;

                    canvas.Save();
                    canvas.Translate(cx, cy);
                    canvas.RotateDegrees(Random11(rand) * frameRotate * randomness);

                    for (int k = 0; k < stitchCount; k++)
                    {
                        // 針の中心位置と長さを、少しずつばらつかせる
                        float center = (k + 0.5f) * period + Random11(rand) * period * spaceJitter * randomness;
                        float len = on * (1f + Random11(rand) * lengthJitter * randomness);
                        float start = Math.Max(0f, center - len / 2f);
                        float end = Math.Min(perimeter, center + len / 2f);
                        if (end - start < 0.2f) continue;

                        // 針の向きに対して横方向にずらす(手縫いのぶれ)
                        measure.GetPositionAndTangent(Math.Clamp(center, 0f, perimeter), out _, out SKPoint tangent);
                        float offset = Random11(rand) * lineWidth * wobble * randomness;

                        segment.Reset();
                        measure.GetSegment(start, end, segment, true);

                        canvas.Save();
                        canvas.Translate(-tangent.Y * offset, tangent.X * offset);
                        canvas.DrawPath(segment, thread);
                        canvas.Restore();
                    }

                    canvas.Restore();
                }
            }

            return bmp;
        }

        // 色の濃淡を変える。t が正なら白に近づけ、負なら黒に近づける(-1〜1)
        private static SKColor Tone(SKColor c, float t)
        {
            t = Math.Clamp(t, -1f, 1f);

            if (t >= 0f)
            {
                return new SKColor(
                    (byte)(c.Red + (255 - c.Red) * t),
                    (byte)(c.Green + (255 - c.Green) * t),
                    (byte)(c.Blue + (255 - c.Blue) * t),
                    c.Alpha);
            }

            float f = 1f + t;
            return new SKColor((byte)(c.Red * f), (byte)(c.Green * f), (byte)(c.Blue * f), c.Alpha);
        }

        // -1〜1の乱数
        private static float Random11(Random rand)
        {
            return (float)(rand.NextDouble() * 2 - 1);
        }
    }
}