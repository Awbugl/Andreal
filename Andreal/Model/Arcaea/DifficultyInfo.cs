using System.Collections.Concurrent;
using System.Drawing;

#pragma warning disable CS8618

namespace Andreal.Model.Arcaea;

[Serializable]
internal class DifficultyInfo
{
    private static readonly ConcurrentDictionary<int, DifficultyInfo> List = new();

    static DifficultyInfo()
    {
        List.TryAdd(3,
                    new()
                    {
                        LongStr = "Beyond",
                        ShortStr = "BYD",
                        Alias = new[] { "byn", "byd", "beyond" },
                        Color = Color.FromArgb(165, 20, 49)
                    });

        List.TryAdd(2,
                    new()
                    {
                        LongStr = "Future",
                        ShortStr = "FTR",
                        Alias = new[] { "ftr", "future" },
                        Color = Color.FromArgb(115, 35, 100)
                    });

        List.TryAdd(1,
                    new()
                    {
                        LongStr = "Present",
                        ShortStr = "PRS",
                        Alias = new[] { "prs", "present" },
                        Color = Color.FromArgb(120, 155, 80)
                    });

        List.TryAdd(0,
                    new()
                    {
                        LongStr = "Past",
                        ShortStr = "PST",
                        Alias = new[] { "pst", "past" },
                        Color = Color.FromArgb(20, 165, 215)
                    });
    }

    private string[] Alias { get; set; }
    internal string LongStr { get; private set; }
    internal string ShortStr { get; private set; }
    internal Color Color { get; private set; }

    internal static DifficultyInfo GetByIndex(int index) => List[index];

    internal static (string, int) DifficultyConverter(string dif)
    {
        foreach (var (key, value) in List)
            foreach (var alias in value.Alias.Where(dif.EndsWith))
                return (dif[..^alias.Length], key);

        return (dif, 2);
    }

    public static implicit operator string(DifficultyInfo info) => info.ShortStr;
}
