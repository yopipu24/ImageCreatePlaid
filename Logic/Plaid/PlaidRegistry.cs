namespace ImageCreatePlaid
{
    internal static class PlaidRegistry
    {
        // 窓の中に入れられる柄。WindowCheck1 自身は、再帰になるので入れません
        private static readonly Dictionary<string, Func<PlaidInterface>> Factories = new()
        {
            ["RandomCheck"] = () => new RandomCheck(),
            ["RandomCheck1"] = () => new RandomCheck1(),
            ["RandomCheck2"] = () => new RandomCheck2(),
            ["RandomCheck3"] = () => new RandomCheck3(),
            ["RandomCheck4"] = () => new RandomCheck4(),
            ["RandomCheck5"] = () => new RandomCheck5(),
            ["Check2Line"] = () => new Check2Line(),
            ["GinghamCheck1"] = () => new GinghamCheck1(),
            ["GinghamCheck2"] = () => new GinghamCheck2(),
            ["Check1"] = () => new Check1(),
            ["Check1_2"] = () => new Check1_2(),
            ["Check2"] = () => new Check2(),
            ["Check3"] = () => new Check3(),
            ["Check4"] = () => new Check4(),
            ["Checkered1"] = () => new Checkered1(),
            ["Checkered2"] = () => new Checkered2(),
            ["TartanCheck1"] = () => new TartanCheck1(),
            ["TartanCheck2"] = () => new TartanCheck2(),
            ["TartanCheck3"] = () => new TartanCheck3(),
            ["TartanCheck4"] = () => new TartanCheck4(),
            ["TartanCheck5"] = () => new TartanCheck5(),
            ["TartanCheck6"] = () => new TartanCheck6(),
            ["TartanCheck7"] = () => new TartanCheck7(),
            ["VerticalLine1"] = () => new VerticalLine1(),
            ["HorizontalLine1"] = () => new HorizontalLine1(),
            ["HandDrawn1"] = () => new HandDrawn1(),
            ["HandDrawn2"] = () => new HandDrawn2(),
            ["Weave1"] = () => new Weave1(),
            ["Weave2"] = () => new Weave2(),
            ["GradientCheck1"] = () => new GradientCheck1(),
            ["RoundPatch1"] = () => new RoundPatch1(),
            ["RoundPatch2"] = () => new RoundPatch2(),
            ["StitchCheck1"] = () => new StitchCheck1(),
            ["StitchCheck2"] = () => new StitchCheck2(),
            ["GlitchMosaicCheck1"] = () => new GlitchMosaicCheck1(),
            ["GlitchMosaicCheck2"] = () => new GlitchMosaicCheck2(),
            ["FibonacciCheck1"] = () => new FibonacciCheck1(),
            ["FibonacciCheck2"] = () => new FibonacciCheck2(),
            ["CuteCheck1"] = () => new CuteCheck1(),
            ["CuteCheck2"] = () => new CuteCheck2(),
            ["ExtremeRandomCheck1"] = () => new ExtremeRandomCheck1(),
            ["ExtremeRandomCheck2"] = () => new ExtremeRandomCheck2(),
            ["VoronoiShatteredCheck"] = () => new VoronoiShatteredCheck(),
            ["PhaseShiftWaveCheck"] = () => new PhaseShiftWaveCheck(),
            ["MondrianTreePartitionCheck"] = () => new MondrianTreePartitionCheck(),
            ["KaleidoscopicGridCheck"] = () => new KaleidoscopicGridCheck(),
        };

        // 画面の選択肢に使う、窓の中に入れられる柄の名前の一覧
        public static IReadOnlyList<string> InnerNames { get; } = Factories.Keys.ToList();

        // ランダムのときに選ばれる柄。外したい柄(重いもの、好みでないもの)があれば、ここから消してください
        public static IReadOnlyList<string> RandomPool { get; } = Factories.Keys.ToList();

        public static bool Contains(string? name)
        {
            return name != null && Factories.ContainsKey(name);
        }

        public static PlaidInterface Create(string name)
        {
            return Factories[name]();
        }
    }
}