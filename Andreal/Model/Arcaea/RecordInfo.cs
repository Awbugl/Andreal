using System.Drawing;
using System.Text;
using Andreal.Data.Json.Arcaea.ArcaeaLimited;
using Andreal.Data.Json.Arcaea.BotArcApi;
using Image = Andreal.UI.Image;

namespace Andreal.Model.Arcaea;

[Serializable]
internal class RecordInfo
{
    private readonly string _score;

    internal RecordInfo(RecordDataItem recentdata, sbyte difficulty = -128)
    {
        Difficulty = difficulty == -128
            ? recentdata.Difficulty
            : difficulty;
        SongInfo = new(recentdata.SongId, Difficulty);
        Cleartype = 0;
        SongId = recentdata.SongId;
        Rating = recentdata.PotentialValue.ToString("0.0000");
        Pure = recentdata.PureCount;
        MaxPure = recentdata.ShinyPureCount;
        Far = recentdata.FarCount;
        Lost = recentdata.LostCount;
        Time = DateTime.UnixEpoch.AddMilliseconds(recentdata.TimePlayed);

        _score = recentdata.Score;
    }

    public RecordInfo(ArcSongdata recentdata)
    {
        Difficulty = recentdata.Difficulty;
        SongInfo = new(recentdata.SongId, Difficulty);
        Cleartype = recentdata.ClearType;
        SongId = recentdata.SongId;
        Rating = recentdata.Rating.ToString("0.0000");
        Pure = recentdata.Pure;
        MaxPure = recentdata.MaxPure;
        Far = recentdata.Far;
        Lost = recentdata.Lost;
        Time = new DateTime(1970, 1, 1, 8, 0, 0, DateTimeKind.Utc).AddMilliseconds(recentdata.TimePlayed);
        _score = recentdata.Score;
    }

    internal SongInfo SongInfo { get; }

    internal sbyte Cleartype { get; }

    internal sbyte Difficulty { get; }

    internal bool IsRecent => Difficulty == -128;

    internal string SongId { get; }

    internal string Rating { get; }

    internal string Pure { get; }

    internal string MaxPure { get; }

    internal string Far { get; }

    internal string Lost { get; }

    internal DateTime Time { get; }

    internal string TimeStr => Time.ToString("yyyy/MM/dd HH:mm:ss");

    internal DifficultyInfo DifficultyInfo => SongInfo.DifficultyInfo;

    internal double Const => SongInfo.Const;

    internal Color MainColor => SongInfo.MainColor;

    internal string Score
    {
        get
        {
            var result = new StringBuilder();
            var len = _score.Length;
            for (var i = 0; i < 8; i++)
            {
                var j = len - 8 + i;
                result.Append(j < 0
                                  ? '0'
                                  : _score[j]);
                if (i is 1 or 4) result.Append('\'');
            }

            return result.ToString();
        }
    }

    internal string Rate
    {
        get
        {
            return Convert.ToInt32(_score) switch
                   {
                       >= 9900000 => "[EX+]",
                       >= 9800000 => "[EX]",
                       >= 9500000 => "[AA]",
                       >= 9200000 => "[A]",
                       >= 8900000 => "[B]",
                       >= 8600000 => "[C]",
                       _          => "[D]"
                   };
        }
    }

    internal async Task<Image> GetSongImg() => await SongInfo.GetSongImg();

    internal string SongName(byte length) => SongInfo.GetSongName(length);

    internal bool Compare(RecordInfo? info)
    {
        if (info == null) return true;
        return Convert.ToInt32(_score) > Convert.ToInt32(info._score);
    }
}
