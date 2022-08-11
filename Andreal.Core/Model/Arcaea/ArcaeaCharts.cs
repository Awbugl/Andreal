using System.Collections.Concurrent;
using Andreal.Core.Data.Api;
using Andreal.Core.Data.Json.Arcaea.ArcaeaUnlimited;
using Andreal.Core.Utils;

namespace Andreal.Core.Model.Arcaea;

public static partial class ArcaeaCharts
{
    private static readonly ConcurrentDictionary<string, ArcaeaSong> Songs = new();
    private static readonly ConcurrentDictionary<string, List<string>> Aliases = new();

    internal static ArcaeaSong? QueryByID(string? songid) => GetByID(songid);

    internal static List<ArcaeaSong>? Query(string alias)
    {
        if (string.IsNullOrWhiteSpace(alias)) return default;

        if (AliasCache.ContainsKey(alias)) return AliasCache[alias];

        var data = GetByID(alias) ?? GetByName(Songs, alias) ?? GetByAlias(alias);

        if (data != null) return new() { data };

        var abbrdata = new List<ArcaeaSong>();

        Abbreviations.ForAllItems<ArcaeaSong, string, List<string>>((song, value) =>
                                                                    {
                                                                        if (StringHelper.Equals(value, alias)
                                                                            && !abbrdata.Contains(song))
                                                                            abbrdata.Add(song);
                                                                    });

        if (abbrdata.Count > 0) return abbrdata;

        return GetByPriorityQueue(alias);
    }

    internal static List<string> GetSongAlias(string songid) => Aliases[songid];

    internal static ArcaeaSong RandomSong() => Songs.Values.GetRandomItem();

    internal static ArcaeaChart? RandomSong(double start, double end)
    {
        var ls = GetByConstRange(start, end).ToArray();
        if (ls.Length == 0) return null;
        return ls.GetRandomItem();
    }
}

public static partial class ArcaeaCharts
{
    [NonSerialized] private static readonly ConcurrentDictionary<ArcaeaSong, List<string>> Abbreviations = new();
    [NonSerialized] private static readonly ConcurrentDictionary<ArcaeaSong, List<string>> Names = new();
    [NonSerialized] private static readonly ConcurrentDictionary<string, List<ArcaeaSong>> AliasCache = new();

    static ArcaeaCharts() { Init(); }

    public static void Init()
    {
        Songs.Clear();
        Aliases.Clear();
        Abbreviations.Clear();
        Names.Clear();

        var slst = ArcaeaUnlimitedApi.SongList().Result!.DeserializeContent<SongListContent>().Songs;

        foreach (var songitem in slst)
        {
            songitem.Difficulties.SongID = songitem.SongID;

            for (var i = 0; i < songitem.Difficulties.Count; ++i)
            {
                songitem.Difficulties[i].RatingClass = i;
                songitem.Difficulties[i].SongID = songitem.SongID;
            }

            Aliases.TryAdd(songitem.SongID, songitem.Alias);
            Songs.TryAdd(songitem.SongID, songitem.Difficulties);
        }

        foreach (var (_, value) in Songs)
        {
            var abbrs = new List<string>();
            var names = new List<string>();

            for (var index = 0; index < value.Count; index++)
                if (index == 0 || value[index].AudioOverride)
                {
                    abbrs.Add(value[index].NameEn.GetAbbreviation());
                    names.Add(value[index].NameEn);

                    if (string.IsNullOrWhiteSpace(value[index].NameJp)) continue;

                    abbrs.Add(value[index].NameJp.GetAbbreviation());
                    names.Add(value[index].NameJp);
                }

            Abbreviations.TryAdd(value, abbrs);
            Names.TryAdd(value, names);
        }
    }
}

public static partial class ArcaeaCharts
{
    private static ArcaeaSong? GetByID(string? songid) =>
        songid is not null && Songs.TryGetValue(songid, out var value)
            ? value
            : null;

    private static ArcaeaSong? GetByName(ConcurrentDictionary<string, ArcaeaSong> values, string alias)
    {
        values.TryTakeValues((ArcaeaChart item) => StringHelper.Equals(item.NameEn, alias) || StringHelper.Equals(item.NameJp, alias),
                             out var result);

        return result;
    }

    private static ArcaeaSong? GetByAlias(string alias)
    {
        Aliases.TryTakeKey<string, string, List<string>>(value => StringHelper.Equals(value, alias), out var result);
        return GetByID(result);
    }

    private static List<ArcaeaSong>? GetByPriorityQueue(string alias)
    {
        var dic = new PriorityQueue<ArcaeaSong, byte>();

        Aliases.ForAllItems<string, string, List<string>>((song, sid) =>
                                                              Enqueue(dic, alias, sid, GetByID(song)!, 1, 4));

        dic.TryPeek(out _, out var firstpriority);

        if (firstpriority != 1)
            foreach (var (sid, song) in Songs)
                Enqueue(dic, alias, sid, song, 2, 5);

        dic.TryPeek(out _, out firstpriority);

        if (firstpriority != 2)
            Names.ForAllItems<ArcaeaSong, string, List<string>>((song, name) => Enqueue(dic, alias, name, song, 3, 6));

        if (dic.Count == 0) return default;

        dic.TryDequeue(out var firstobj, out var lowestpriority);

        var ls = new List<ArcaeaSong> { firstobj! };

        while (dic.TryDequeue(out var obj, out var priority) && priority == lowestpriority)
            if (!ls.Contains(obj))
                ls.Add(obj);

        AliasCache.TryAdd(alias, ls);
        return ls;
    }

    private static void Enqueue(PriorityQueue<ArcaeaSong, byte> dic, string alias, string key, ArcaeaSong song,
                                byte upperpriority, byte lowerpriority)
    {
        if (StringHelper.Contains(key, alias)) dic.Enqueue(song, upperpriority);
        if (StringHelper.Contains(alias, key)) dic.Enqueue(song, lowerpriority);
    }

    internal static IEnumerable<ArcaeaChart> GetByConst(double @const)
    {
        const double lerance = 0.001;

        // ReSharper disable once LoopCanBePartlyConvertedToQuery
        foreach (var song in Songs.Values)
            // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
            foreach (var chart in song)
                if (Math.Abs(chart.Const - @const) < lerance)
                    yield return chart;
    }

    private static IEnumerable<ArcaeaChart> GetByConstRange(double lowerlimit, double upperlimit)
    {
        // ReSharper disable once LoopCanBeConvertedToQuery
        foreach (var song in Songs.Values)
            // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
            foreach (var chart in song)
                if (chart.Const >= lowerlimit && chart.Const <= upperlimit)
                    yield return chart;
    }
}
