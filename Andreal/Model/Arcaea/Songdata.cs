using System.Collections.Concurrent;
using Andreal.Data.Api;
using Andreal.Data.Json.Arcaea.BotArcApi;
using Andreal.Data.Json.Arcaea.Songlist;
using Andreal.Utils;

namespace Andreal.Model.Arcaea;

[Serializable]
internal class Songdata : IEquatable<Songdata>
{
    [NonSerialized] private static Lazy<ConcurrentDictionary<string, Songdata>> _songList
        = new(() => new(ArcaeaUnlimitedApi.SongList().Result!.DeserializeContent<SongListContent>().Songs
                                          .Select(i => new Songdata(i)).ToDictionary(i => i.SongId, i => i)));

    private Songdata(SongsItem data)
    {
        Data = data;
        Consts = new()
                 {
                     (double)data.Difficulties[0].RealRating / 10,
                     (double)data.Difficulties[1].RealRating / 10,
                     (double)data.Difficulties[2].RealRating / 10,
                     data.Difficulties.Count == 4
                         ? (double)data.Difficulties[3].RealRating / 10
                         : -0.1
                 };

        Notes = new()
                {
                    data.Difficulties[0].TotalNotes,
                    data.Difficulties[1].TotalNotes,
                    data.Difficulties[2].TotalNotes,
                    data.Difficulties.Count == 4
                        ? data.Difficulties[3].TotalNotes
                        : -1
                };
    }

    public SongsItem Data { get; }

    internal string SongId => Data.Id;

    internal string Songname => Data.TitleLocalized.En;

    internal Side PartnerSide => (Side)Data.Side;

    internal string PackageInfo => Data.Package!;

    internal List<double> Consts { get; }

    internal List<long> Notes { get; }

    public bool Equals(Songdata? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return SongId.Equals(other.SongId);
    }


    internal static void Init()
    {
        _songList = new(() => new(ArcaeaUnlimitedApi.SongList().Result!.DeserializeContent<SongListContent>().Songs
                                                    .Select(i => new Songdata(i)).ToDictionary(i => i.SongId, i => i)));
    }

    internal string ConstString(sbyte difficulty) =>
        $"[{DifficultyInfo.GetByIndex(difficulty).ShortStr} {Consts[difficulty]:0.0}]";

    internal string GetSongName(byte length) =>
        Songname.Length < length + 3
            ? Songname
            : $"{Songname[..length]}...";

    internal static Songdata? GetBySid(string? sid)
    {
        if (sid == null) return null;
        return _songList.Value.TryGetValue(sid, out var result)
            ? result
            : null;
    }

    internal static async Task<(int, Songdata?[]?)> GetByAlias(string alias)
    {
        var result = await ArcaeaUnlimitedApi.SongByAlias(alias);

        return result!.Status switch
               {
                   0 => (0, new[] { new Songdata(result.DeserializeContent<SongsItem>()) }),
                   -8 => (
                       -2, result.DeserializeContent<TooManySongsContent>().Songs.Select(s => GetBySid(s)!).ToArray()),
                   _ => (-1, null)
               };
    }

    internal static IEnumerable<(Songdata, sbyte)> GetByConst(double theconst)
    {
        const double lerance = 0.001;
        return _songList.Value.Values.SelectMany(c => c.Consts, (c, i) => new { c, i })
                        .Where(t => Math.Abs(t.i - theconst) < lerance)
                        .Select(t => (t.c, (sbyte)t.c.Consts.IndexOf(t.i))).OrderBy(r => r.c.Data.TitleLocalized.En);
    }

    private static IEnumerable<(Songdata, sbyte)> GetByConstRange(double lowerlimit, double upperlimit)
    {
        return _songList.Value.Values.SelectMany(c => c.Consts, (c, i) => new { c, i })
                        .Where(t => t.i >= lowerlimit && t.i <= upperlimit)
                        .Select(t => (t.c, (sbyte)t.c.Consts.IndexOf(t.i))).OrderBy(r => r.c.Data.TitleLocalized.En);
    }

    internal static (Songdata?, sbyte) RandomSong(double lowerlimit, double upperlimit)
    {
        var ls = GetByConstRange(lowerlimit, upperlimit).ToArray();
        return ls.Length > 0
            ? ls.GetRandomItem()
            : (null, -1);
    }

    internal static Songdata RandomSong() => _songList.Value.Values.GetRandomItem();

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((Songdata)obj);
    }

    public override int GetHashCode() => SongId.GetHashCode();
}
