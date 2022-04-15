using System.Drawing;

#pragma warning disable CS8618

namespace Andreal.Model.Arcaea;

[Serializable]
internal class DifficultyInfo
{
    private static readonly List<DifficultyInfo> List = new()
                                                        {
                                                            new()
                                                            {
                                                                Index = 3,
                                                                LongStr = "Beyond",
                                                                ShortStr = "BYD",
                                                                Alias = new[] { "byn", "byd", "beyond" },
                                                                Color = Color.FromArgb(165, 20, 49)
                                                            },
                                                            new()
                                                            {
                                                                Index = 2,
                                                                LongStr = "Future",
                                                                ShortStr = "FTR",
                                                                Alias = new[] { "ftr", "future" },
                                                                Color = Color.FromArgb(115, 35, 100)
                                                            },
                                                            new()
                                                            {
                                                                Index = 1,
                                                                LongStr = "Present",
                                                                ShortStr = "PRS",
                                                                Alias = new[] { "prs", "present" },
                                                                Color = Color.FromArgb(120, 155, 80)
                                                            },
                                                            new()
                                                            {
                                                                Index = 0,
                                                                LongStr = "Past",
                                                                ShortStr = "PST",
                                                                Alias = new[] { "pst", "past" },
                                                                Color = Color.FromArgb(20, 165, 215)
                                                            }
                                                        };

    private sbyte Index { get; set; }
    private string[] Alias { get; set; }
    internal string LongStr { get; private set; }
    internal string ShortStr { get; private set; }
    internal Color Color { get; private set; }

    internal static DifficultyInfo GetByIndex(int index) { return List.FirstOrDefault(i => i.Index == index)!; }

    internal static (string, sbyte?) DifficultyConverter(string dif)
    {
        foreach (var info in List)
            foreach (var alias in info.Alias.Where(dif.EndsWith))
                return (dif[..^alias.Length], info);

        return (dif, null);
    }

    public static implicit operator string(DifficultyInfo info) => info.ShortStr;

    public static implicit operator sbyte(DifficultyInfo info) => info.Index;
}
