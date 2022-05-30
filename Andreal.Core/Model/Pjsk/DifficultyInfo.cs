namespace Andreal.Core.Model.Pjsk;

#pragma warning disable CS8618

[Serializable]
internal class DifficultyInfo
{
    private static readonly List<DifficultyInfo> List = new()
                                                        {
                                                            new()
                                                            {
                                                                Index = 4,
                                                                LongStr = "Master",
                                                                ShortStr = "MA",
                                                                Alias = new[] { "ma", "紫", "master" }
                                                            },
                                                            new()
                                                            {
                                                                Index = 3,
                                                                LongStr = "Expert",
                                                                ShortStr = "EX",
                                                                Alias = new[] { "ex", "红", "expert" }
                                                            },
                                                            new()
                                                            {
                                                                Index = 2,
                                                                LongStr = "Hard",
                                                                ShortStr = "HD",
                                                                Alias = new[] { "hd", "hard" }
                                                            },
                                                            new()
                                                            {
                                                                Index = 1,
                                                                LongStr = "Normal",
                                                                ShortStr = "NM",
                                                                Alias = new[] { "nm", "normal" }
                                                            },
                                                            new()
                                                            {
                                                                Index = 0,
                                                                LongStr = "Easy",
                                                                ShortStr = "EA",
                                                                Alias = new[] { "ey", "easy" }
                                                            }
                                                        };

    private sbyte Index { get; set; }
    private string[] Alias { get; set; }
    internal string LongStr { get; private set; }
    internal string ShortStr { get; private set; }


    internal static DifficultyInfo GetByIndex(int index) { return List.FirstOrDefault(i => i.Index == index)!; }

    internal static DifficultyInfo? DifficultyConverter(string dif) =>
        (from info in List from alias in info.Alias.Where(dif.EndsWith) select info).FirstOrDefault();

    internal static sbyte GetByLongStr(string dif) =>
        dif switch
        {
            "easy"   => 0,
            "normal" => 1,
            "hard"   => 2,
            "expert" => 3,
            "master" => 4,
            _        => -1
        };

    public static implicit operator string(DifficultyInfo info) => info.ShortStr;

    public static implicit operator sbyte(DifficultyInfo info) => info.Index;
}
