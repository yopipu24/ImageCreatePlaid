using SkiaSharp;

namespace ImageCreatePlaid
{
    public class PartialCheck1 : PlaidInterface
    {
        // レイアウトの種類
        private const int ModeWindows = 0;   // 窓・コーナー
        private const int ModeSides = 1;     // 左右の縁
        private const int ModeVignette = 2;  // 周辺フェード
        private const int ModeFrame = 3;     // 額縁(開口部との境目に線)
        private const int ModeArch = 4;      // アーチ枠(帯の両側に線)
        private const int ModeCount = 5;

        // 左右の縁の、内側の縁の飾り方
        private const int EdgeStraight = 0;
        private const int EdgeWave = 1;
        private const int EdgeScallop = 2;
        private const int EdgeZigzag = 3;
        private const int EdgeStyleCount = 4;

        public SKBitmap EditImage(SKBitmap bmp, PlaidModel model)
        {
            int width = bmp.Width;
            int height = bmp.Height;

            // --- 調整用パラメータ ---
            int mode = -1;                 // -1=ランダム、0=窓・コーナー、1=左右の縁、2=周辺フェード、3=額縁、4=アーチ枠
            if (!model.PartialPatternRandom) mode = model.PartialCheckMode;
            float combineRate = 0.35f;     // ランダムのとき、窓・左右の縁・周辺フェードにもう1種類重ねる確率

            // キャラクターを置く場所(安全域)。画面の幅・高さに対する割合
            float safeCx = 0.5f;           // 中心の x
            float safeCy = 0.62f;          // 中心の y(中央よりやや下)
            float safeRx = 0.27f;          // 半径の x
            float safeRy = 0.40f;          // 半径の y
            float safeFeather = 0.25f;     // 安全域の縁のぼかし幅(半径に対する割合)

            // 縁取り線
            float outlineRate = 0.5f;      // 窓・左右の縁に線をつける確率(額縁とアーチ枠は常に線あり)
            int lineStyle = -1;            // -1=ランダム、0=実線、1=二重線、2=点線(ステッチ風)
            float lineWidthRatio = 0.006f; // 線の太さ(画面の短い辺に対する割合)
            SKColor lineColor = SKColors.White; // 線の色(ベース色と近いと見えません)
            // -----------------------

            var rand = new Random();
            var safe = new SafeZone(width * safeCx, height * safeCy, width * safeRx, height * safeRy, safeFeather);

            using var canvas = new SKCanvas(bmp);
            canvas.Clear(new SKColor(model.BaseColorRed, model.BaseColorGreen, model.BaseColorBlue, model.BaseAlpha));

            // 1. チェック柄を画面全体に描く
            string patternName = ChoosePatternName(rand, model);
            using SKBitmap layer = BussinessLogic.NewCreateImage(width, height);
            SKBitmap pattern = PlaidRegistry.Create(patternName).EditImage(layer, model);

            // 2. 残す部分の形(マスク)と、縁取り線の位置を作る
            List<int> modes = ChooseModes(rand, mode, combineRate);
            var outlines = new List<SKPath>();
            using SKBitmap mask = CreateMask(rand, width, height, modes, safe, outlineRate, outlines);

            // 3. 柄をマスクで切り抜いて、背景に重ねる
            using var maskPaint = new SKPaint { BlendMode = SKBlendMode.DstIn };

            canvas.SaveLayer();
            canvas.DrawBitmap(pattern, 0, 0);
            canvas.DrawBitmap(mask, 0, 0, maskPaint);
            canvas.Restore();

            if (!ReferenceEquals(pattern, layer))
            {
                pattern.Dispose();
            }

            // 4. 縁取り線を描く
            float lineWidth = Math.Max(2f, Math.Min(width, height) * lineWidthRatio);
            int style = lineStyle >= 0 ? lineStyle : rand.Next(3);
            DrawOutlines(canvas, outlines, style, lineWidth, lineColor);

            foreach (SKPath path in outlines)
            {
                path.Dispose();
            }

            return bmp;
        }

        // ---- 柄とレイアウトの選択 ----

        private static string ChoosePatternName(Random rand, PlaidModel model)
        {
            if (!model.WindowPatternRandom && PlaidRegistry.Contains(model.WindowPatternName))
            {
                return model.WindowPatternName;
            }

            return PlaidRegistry.RandomPool[rand.Next(PlaidRegistry.RandomPool.Count)];
        }

        private static List<int> ChooseModes(Random rand, int mode, float combineRate)
        {
            var modes = new List<int>();

            if (mode >= 0 && mode < ModeCount)
            {
                modes.Add(mode);
                return modes;
            }

            int first = rand.Next(ModeCount);
            modes.Add(first);

            // 額縁とアーチ枠は単独で使う。窓・左右の縁・周辺フェードは、もう1種類重ねることがある
            if (first <= ModeVignette && rand.NextDouble() < combineRate)
            {
                int second = rand.Next(ModeVignette); // 0〜1のどちらか(firstを除く)
                if (second >= first) second++;
                modes.Add(second);
            }

            return modes;
        }

        // ---- マスク(柄を残す部分が不透明、消す部分が透明) ----

        private static SKBitmap CreateMask(
            Random rand, int width, int height, List<int> modes, SafeZone safe,
            float outlineRate, List<SKPath> outlines)
        {
            var mask = new SKBitmap(width, height);
            using var canvas = new SKCanvas(mask);
            canvas.Clear(SKColors.Transparent);

            using var paint = new SKPaint
            {
                IsAntialias = true,
                Style = SKPaintStyle.Fill,
                Color = SKColors.White
            };

            foreach (int m in modes)
            {
                bool line = rand.NextDouble() < outlineRate;

                switch (m)
                {
                    case ModeWindows:
                        DrawWindows(canvas, paint, rand, width, height, safe, line, outlines);
                        break;
                    case ModeSides:
                        DrawSides(canvas, paint, rand, width, height, safe, line, outlines);
                        break;
                    case ModeVignette:
                        DrawVignette(canvas, rand, safe);
                        break;
                    case ModeFrame:
                        DrawFrame(canvas, paint, rand, width, height, safe, outlines);
                        break;
                    default:
                        DrawArch(canvas, paint, rand, width, height, safe, outlines);
                        break;
                }
            }

            // 念のため、安全域を縁をぼかしながらくり抜く
            CarveSafeZone(canvas, safe);

            return mask;
        }

        // 窓・コーナー: 窓の形を、隅に寄せたり、画面の途中に置いたりする。隅のものは画面の端で切れる
        private static void DrawWindows(
            SKCanvas canvas, SKPaint paint, Random rand, int width, int height,
            SafeZone safe, bool line, List<SKPath> outlines)
        {
            float minSide = Math.Min(width, height);
            int count = rand.Next(3, 7);
            var placed = new List<(float X, float Y, float R)>();
            List<int> shapeOrder = Enumerable.Range(0, WindowCheck1.ShapeTypeCount).OrderBy(_ => rand.Next()).ToList();

            for (int n = 0; n < count; n++)
            {
                float size = minSide * Lerp(0.18f, 0.40f, rand.NextDouble());
                float aspect = Lerp(0.8f, 1.25f, rand.NextDouble());
                bool done = false;

                // 置けなければ、少しずつ小さくして再挑戦する
                for (int shrink = 0; shrink < 4 && !done; shrink++)
                {
                    float w = size * MathF.Sqrt(aspect);
                    float h = size / MathF.Sqrt(aspect);
                    float radius = Math.Max(w, h) * 0.5f * 0.95f;

                    for (int attempt = 0; attempt < 60; attempt++)
                    {
                        float cx;
                        float cy;

                        if (rand.NextDouble() < 0.5)
                        {
                            // 隅に寄せる(画面の端で切れて、コーナーの飾りになる)
                            int corner = rand.Next(4);
                            float k = Lerp(-0.2f, 0.3f, rand.NextDouble()) * radius;
                            cx = (corner & 1) == 0 ? k : width - k;
                            cy = (corner & 2) == 0 ? k : height - k;
                        }
                        else
                        {
                            // 画面の途中に置く
                            cx = width * Lerp(0.06f, 0.94f, rand.NextDouble());
                            cy = height * Lerp(0.06f, 0.94f, rand.NextDouble());
                        }

                        if (safe.Intersects(cx, cy, radius)) continue;

                        bool overlap = false;
                        foreach (var other in placed)
                        {
                            float dx = cx - other.X;
                            float dy = cy - other.Y;
                            float min = (radius + other.R) * 0.95f;
                            if (dx * dx + dy * dy < min * min)
                            {
                                overlap = true;
                                break;
                            }
                        }
                        if (overlap) continue;

                        using SKPath shape = WindowCheck1.CreateShape(shapeOrder[n % shapeOrder.Count], w, h, rand);

                        canvas.Save();
                        canvas.Translate(cx, cy);
                        canvas.RotateDegrees(Lerp(-20f, 20f, rand.NextDouble()));
                        canvas.DrawPath(shape, paint);
                        if (line)
                        {
                            AddOutline(canvas, outlines, shape);
                        }
                        canvas.Restore();

                        placed.Add((cx, cy, radius));
                        done = true;
                        break;
                    }

                    size *= 0.88f;
                }
            }
        }

        // 左右の縁
        private static void DrawSides(
            SKCanvas canvas, SKPaint paint, Random rand, int width, int height,
            SafeZone safe, bool line, List<SKPath> outlines)
        {
            float safeLeft = safe.Cx - safe.Rx * (1f + safe.Feather * 0.5f);
            float limit = Math.Max(width * 0.06f, safeLeft * 0.95f);

            //int pick = rand.Next(3); // 0=左だけ、1=右だけ、2=両側
            int pick = 2;

            for (int side = 0; side < 2; side++)
            {
                if (side == 0 && pick == 1) continue;
                if (side == 1 && pick == 0) continue;

                int style = rand.Next(EdgeStyleCount);
                float desired = width * Lerp(0.10f, 0.20f, rand.NextDouble());

                // 縁の飾り(ホタテ、波、ギザギザ)が内側にはみ出す分を引いておく
                float extra = style == EdgeScallop ? height * 0.09f
                            : (style == EdgeWave || style == EdgeZigzag) ? desired * 0.2f
                            : 0f;
                float thickness = Math.Min(desired, limit - extra);
                if (thickness < width * 0.04f) continue;

                canvas.Save();
                if (side == 0)
                {
                    // 左の縁: 帯を -90度回して、画面の左端に置く
                    canvas.Translate(0, height);
                    canvas.RotateDegrees(-90f);
                }
                else
                {
                    // 右の縁: 帯を 90度回して、画面の右端に置く
                    canvas.Translate(width, 0);
                    canvas.RotateDegrees(90f);
                }
                DrawBand(canvas, paint, rand, height, thickness, style, line, outlines);
                canvas.Restore();
            }
        }

        // 帯を描く。帯が伸びる方向を x(0〜length)、厚みを y(0〜thickness)として、内側の縁は y = thickness 付近
        private static void DrawBand(
            SKCanvas canvas, SKPaint paint, Random rand, float length, float thickness,
            int style, bool line, List<SKPath> outlines)
        {
            List<SKPoint> edge = BuildEdgePoints(rand, style, length, thickness);

            using var fill = new SKPath();
            fill.MoveTo(0, 0);
            fill.LineTo(length, 0);
            for (int i = edge.Count - 1; i >= 0; i--)
            {
                fill.LineTo(edge[i]);
            }
            fill.Close();
            canvas.DrawPath(fill, paint);

            if (line)
            {
                using var edgePath = new SKPath();
                edgePath.MoveTo(edge[0]);
                for (int i = 1; i < edge.Count; i++)
                {
                    edgePath.LineTo(edge[i]);
                }
                AddOutline(canvas, outlines, edgePath);
            }
        }

        // 帯の内側の縁の点列
        private static List<SKPoint> BuildEdgePoints(Random rand, int style, float length, float thickness)
        {
            var pts = new List<SKPoint>();

            switch (style)
            {
                case EdgeWave:
                    {
                        float amp = thickness * 0.18f;
                        float period = length / rand.Next(3, 8);
                        float phase = Lerp(0f, MathF.PI * 2f, rand.NextDouble());

                        for (float x = 0f; x < length; x += 4f)
                        {
                            pts.Add(new SKPoint(x, thickness + amp * MathF.Sin(MathF.PI * 2f * x / period + phase)));
                        }
                        pts.Add(new SKPoint(length, thickness + amp * MathF.Sin(MathF.PI * 2f * length / period + phase)));
                        break;
                    }
                case EdgeScallop:
                    {
                        int n = rand.Next(6, 14);
                        float s = length / n;

                        for (int i = 0; i < n; i++)
                        {
                            float cx = (i + 0.5f) * s;
                            for (int k = 0; k <= 24; k++)
                            {
                                float phi = MathF.PI * k / 24f;
                                pts.Add(new SKPoint(cx - s * 0.5f * MathF.Cos(phi), thickness + s * 0.5f * MathF.Sin(phi)));
                            }
                        }
                        break;
                    }
                case EdgeZigzag:
                    {
                        int n = rand.Next(8, 18);
                        float s = length / n;
                        float amp = thickness * 0.2f;

                        for (int i = 0; i <= n; i++)
                        {
                            pts.Add(new SKPoint(i * s, thickness + ((i & 1) == 0 ? 0f : amp)));
                        }
                        break;
                    }
                default:
                    pts.Add(new SKPoint(0, thickness));
                    pts.Add(new SKPoint(length, thickness));
                    break;
            }

            return pts;
        }

        // 周辺フェード: 安全域の縁から外側へ、柄が少しずつ現れる
        private static void DrawVignette(SKCanvas canvas, Random rand, SafeZone safe)
        {
            // 安全域の何倍の位置で、柄が完全に見えるようになるか
            float outerScale = Lerp(1.6f, 2.2f, rand.NextDouble());

            using SKShader shader = SKShader.CreateRadialGradient(
                new SKPoint(0, 0),
                outerScale,
                new[] { new SKColor(255, 255, 255, 0), new SKColor(255, 255, 255, 0), SKColors.White },
                new[] { 0f, 1f / outerScale, 1f },
                SKShaderTileMode.Clamp);

            using var paint = new SKPaint { Shader = shader, IsAntialias = true };

            // 安全域の楕円を単位円にする座標系で描く
            canvas.Save();
            canvas.Translate(safe.Cx, safe.Cy);
            canvas.Scale(safe.Rx, safe.Ry);
            canvas.DrawRect(-20f, -20f, 40f, 40f, paint);
            canvas.Restore();
        }

        // 額縁: 外周をチェックで埋めて、中央の開口部との境目に線を引く
        private static void DrawFrame(
            SKCanvas canvas, SKPaint paint, Random rand, int width, int height,
            SafeZone safe, List<SKPath> outlines)
        {
            SKRect box = OpeningBox(width, height, safe, 1.1f);
            int type = rand.Next(3);
            float radius = box.Width * Lerp(0.08f, 0.22f, rand.NextDouble());

            using SKPath opening = CreateOpening(type, box, radius);

            // 画面全体から開口部をくり抜いた形(偶奇規則)
            using var ring = new SKPath { FillType = SKPathFillType.EvenOdd };
            ring.AddRect(new SKRect(0, 0, width, height));
            ring.AddPath(opening);
            canvas.DrawPath(ring, paint);

            AddOutline(canvas, outlines, opening);
        }

        // アーチ枠: 開口部のまわりにチェックの帯を置き、帯の内側と外側に線を引く
        private static void DrawArch(
            SKCanvas canvas, SKPaint paint, Random rand, int width, int height,
            SafeZone safe, List<SKPath> outlines)
        {
            SKRect inner = OpeningBox(width, height, safe, 1.05f);
            float thickness = Math.Min(width, height) * Lerp(0.10f, 0.20f, rand.NextDouble());
            var outer = new SKRect(inner.Left - thickness, inner.Top - thickness, inner.Right + thickness, inner.Bottom);

            int type = rand.Next(3);
            float radius = inner.Width * Lerp(0.08f, 0.22f, rand.NextDouble());

            using SKPath innerPath = CreateOpening(type, inner, radius);
            using SKPath outerPath = CreateOpening(type, outer, radius + thickness);

            // 外側の形から内側の形をくり抜いた、帯の形
            using var ring = new SKPath { FillType = SKPathFillType.EvenOdd };
            ring.AddPath(outerPath);
            ring.AddPath(innerPath);
            canvas.DrawPath(ring, paint);

            AddOutline(canvas, outlines, innerPath);
            AddOutline(canvas, outlines, outerPath);
        }

        // 安全域を囲む開口部の箱。下は画面の外まで開ける
        private static SKRect OpeningBox(int width, int height, SafeZone safe, float factor)
        {
            float margin = Math.Min(width, height) * 0.07f; // 最低限の縁の太さ

            float left = Math.Max(margin, safe.Cx - safe.OuterRx * factor);
            float right = Math.Min(width - margin, safe.Cx + safe.OuterRx * factor);
            float top = Math.Max(margin, safe.Cy - safe.OuterRy * factor);
            float bottom = height * 1.3f;

            return new SKRect(left, top, right, bottom);
        }

        // 開口部の形。0=角丸四角、1=アーチ(上が半円)、2=楕円
        private static SKPath CreateOpening(int type, SKRect box, float cornerRadius)
        {
            var path = new SKPath();

            switch (type)
            {
                case 0:
                    path.AddRoundRect(box, cornerRadius, cornerRadius);
                    break;
                case 1:
                    {
                        float rr = Math.Min(box.Width / 2f, box.Height * 0.5f);
                        path.MoveTo(box.Left, box.Bottom);
                        path.LineTo(box.Left, box.Top + rr);
                        path.ArcTo(new SKRect(box.Left, box.Top, box.Right, box.Top + 2f * rr), 180f, 180f, false);
                        path.LineTo(box.Right, box.Bottom);
                        path.Close();
                        break;
                    }
                default:
                    path.AddOval(box);
                    break;
            }

            return path;
        }

        // 安全域をくり抜く(内側は完全に消し、外側へ向かって、ぼかしながら消えなくなる)
        private static void CarveSafeZone(SKCanvas canvas, SafeZone safe)
        {
            float outer = 1f + safe.Feather;

            using SKShader shader = SKShader.CreateRadialGradient(
                new SKPoint(0, 0),
                outer,
                new[] { SKColors.White, SKColors.White, new SKColor(255, 255, 255, 0) },
                new[] { 0f, 1f / outer, 1f },
                SKShaderTileMode.Clamp);

            using var paint = new SKPaint
            {
                Shader = shader,
                IsAntialias = true,
                BlendMode = SKBlendMode.DstOut
            };

            canvas.Save();
            canvas.Translate(safe.Cx, safe.Cy);
            canvas.Scale(safe.Rx, safe.Ry);
            canvas.DrawRect(-20f, -20f, 40f, 40f, paint);
            canvas.Restore();
        }

        // ---- 縁取り線 ----

        // 現在の座標変換を反映した形で、線を引くパスを記録する
        private static void AddOutline(SKCanvas canvas, List<SKPath> outlines, SKPath path)
        {
            var clone = new SKPath(path);
            clone.Transform(canvas.TotalMatrix);
            outlines.Add(clone);
        }

        // 0=実線、1=二重線、2=点線(ステッチ風)
        private static void DrawOutlines(SKCanvas canvas, List<SKPath> outlines, int style, float lineWidth, SKColor color)
        {
            if (outlines.Count == 0) return;

            using var stroke = new SKPaint
            {
                IsAntialias = true,
                Style = SKPaintStyle.Stroke,
                Color = color,
                StrokeJoin = SKStrokeJoin.Round,
                StrokeCap = SKStrokeCap.Round
            };

            switch (style)
            {
                case 1: // 二重線: 太い線を描いて、中央を細い線で抜く
                    {
                        using var clear = new SKPaint
                        {
                            IsAntialias = true,
                            Style = SKPaintStyle.Stroke,
                            StrokeWidth = lineWidth * 1.2f,
                            StrokeJoin = SKStrokeJoin.Round,
                            BlendMode = SKBlendMode.Clear
                        };

                        stroke.StrokeWidth = lineWidth * 3.4f;

                        canvas.SaveLayer();
                        foreach (SKPath path in outlines)
                        {
                            canvas.DrawPath(path, stroke);
                        }
                        foreach (SKPath path in outlines)
                        {
                            canvas.DrawPath(path, clear);
                        }
                        canvas.Restore();
                        break;
                    }
                case 2: // 点線(丸い線端の分だけ、線を短くしてある)
                    {
                        using SKPathEffect dash = SKPathEffect.CreateDash(new[] { lineWidth * 1.2f, lineWidth * 2.6f }, 0f);
                        stroke.PathEffect = dash;
                        stroke.StrokeWidth = lineWidth;

                        foreach (SKPath path in outlines)
                        {
                            canvas.DrawPath(path, stroke);
                        }
                        break;
                    }
                default: // 実線
                    stroke.StrokeWidth = lineWidth * 1.4f;

                    foreach (SKPath path in outlines)
                    {
                        canvas.DrawPath(path, stroke);
                    }
                    break;
            }
        }

        // ---- 安全域 ----

        private sealed class SafeZone
        {
            public readonly float Cx;
            public readonly float Cy;
            public readonly float Rx;
            public readonly float Ry;
            public readonly float Feather;

            public SafeZone(float cx, float cy, float rx, float ry, float feather)
            {
                Cx = cx;
                Cy = cy;
                Rx = rx;
                Ry = ry;
                Feather = feather;
            }

            // ぼかし幅を含めた外側の半径
            public float OuterRx => Rx * (1f + Feather);
            public float OuterRy => Ry * (1f + Feather);

            // 半径 r の円が、安全域(ぼかしを含む)にかかるか
            public bool Intersects(float x, float y, float r)
            {
                float nx = (x - Cx) / (OuterRx + r);
                float ny = (y - Cy) / (OuterRy + r);
                return nx * nx + ny * ny < 1f;
            }
        }

        private static float Lerp(float a, float b, double t)
        {
            return a + (b - a) * (float)t;
        }
    }
}