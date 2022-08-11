using Andreal.Core.Common;
using Andreal.Core.Model.Arcaea;

namespace Andreal.Core.Utils;

internal static class ArcaeaHelper
{
    internal static (double, double) ConvertToArcaeaRange(this string rawdata) =>
        rawdata switch
        {
            "11"  => (11.0, 11.6),
            "10+" => (10.7, 10.9),
            "10"  => (10.0, 10.6),
            "9+"  => (9.7, 9.9),
            "9"   => (9.0, 9.6),
            "8"   => (8.0, 8.9),
            "7"   => (7.0, 7.9),
            "6"   => (6.0, 6.9),
            "5"   => (5.0, 5.9),
            "4"   => (4.0, 4.9),
            "3"   => (3.0, 3.9),
            "2"   => (2.0, 2.9),
            "1"   => (1.0, 1.9),
            _ => double.TryParse(rawdata, out var value)
                ? (value, value)
                : (-1, -1)
        };

    internal static bool SongInfoParser(IEnumerable<string> command, out ArcaeaSong song, out int dif,
                                        out string errMessage)
    {
        song = null!;
        dif = -1;
        errMessage = "";

        var enumerable = command.ToArray();

        if (enumerable.Length == 0) return false;

        (var songstr, dif) = DifficultyInfo.DifficultyConverter(enumerable[^1]);

        songstr = string.Join("", enumerable, 0, enumerable.Length - 1) + songstr;

        var result = ArcaeaCharts.Query(songstr);

        if (result == null || result.Count == 0)
        {
            errMessage = MessageInfo.RobotReply.NoSongFound!;
            return false;
        }

        if (result.Count > 1)
        {
            errMessage = result.Aggregate(MessageInfo.RobotReply.TooManySongFound,
                                          (cur, i) => cur + "\n" + i[0].NameEn);
            return false;
        }

        song = result[0];

        if (song.SongID == "lasteternity") dif = 3;

        return true;
    }
}
