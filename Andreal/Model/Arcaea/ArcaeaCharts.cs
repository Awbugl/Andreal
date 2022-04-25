using System.Collections.Concurrent;
using System.Text;
using Andreal.Data.Api;
using Andreal.Data.Json.Arcaea.ArcaeaUnlimited;
using Andreal.UI;
using Andreal.Utils;
using Path = Andreal.Core.Path;

namespace Andreal.Model.Arcaea;

internal static partial class ArcaeaCharts
{
    private static readonly ConcurrentDictionary<string, ArcaeaSong> Songs = new();
    private static readonly ConcurrentDictionary<string, List<string>> Aliases = new();
    private static readonly Dictionary<string, Stream> SongImage = new();

    internal static ArcaeaSong? QueryById(string? songid) => GetById(songid);

    internal static List<ArcaeaSong>? Query(string alias)
    {
        if (string.IsNullOrWhiteSpace(alias)) return default;

        if (AliasCache.ContainsKey(alias)) return AliasCache[alias];

        var data = GetById(alias) ?? GetByName(Songs, alias) ?? GetByAlias(alias);

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

    internal static async Task<Image> GetSongImg(string sid, int difficulty)
    {
        var path = await Path.ArcaeaSong(sid, difficulty);

        try
        {
            if (!SongImage.TryGetValue(path, out var stream))
            {
                await using var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
                var bytes = new byte[fileStream.Length];
                fileStream.Read(bytes, 0, bytes.Length);
                fileStream.Close();
                stream = new MemoryStream(bytes);
                SongImage.Add(path, stream);
            }

            var img = new Image(stream);
            if (img.Width == 512) return img;
            var newimg = new Image(img, 512, 512);
            newimg.SaveAsPng(path);
            img.Dispose();
            return newimg;
        }
        catch
        {
            File.Delete(path);
            throw new ArgumentException($"InvalidSongImage: {sid}, deleted.");
        }
    }

    internal static List<string> GetSongAlias(string sid) => Aliases[sid];

    internal static ArcaeaSong RandomSong() => Songs.Values.GetRandomItem();

    internal static ArcaeaChart? RandomSong(double start, double end)
    {
        var ls = GetByConstRange(start, end).ToArray();
        if (ls.Length == 0) return null;
        return ls.GetRandomItem();
    }
}

internal static partial class ArcaeaCharts
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
            for (var i = 0; i < songitem.Difficulties.Count; ++i) songitem.Difficulties[i].RatingClass = i;

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
                    abbrs.Add(GetAbbreviation(value[index].NameEn));
                    names.Add(value[index].NameEn);

                    if (!string.IsNullOrWhiteSpace(value[index].NameJp))
                    {
                        abbrs.Add(GetAbbreviation(value[index].NameJp));
                        names.Add(value[index].NameJp);
                    }
                }

            Abbreviations.TryAdd(value, abbrs);
            Names.TryAdd(value, names);
        }
    }

    private static string GetAbbreviation(string str)
    {
        var sb = new StringBuilder();
        sb.Append(str[0]);

        for (var index = 0; index < str.Length - 1; ++index)
            if (str[index] == ' ')
                sb.Append(str[index + 1]);

        return sb.ToString();
    }
}

internal static partial class ArcaeaCharts
{
    private static ArcaeaSong? GetById(string? songid) =>
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
        return GetById(result);
    }

    private static List<ArcaeaSong>? GetByPriorityQueue(string alias)
    {
        var dic = new PriorityQueue<ArcaeaSong, byte>();

        Aliases.ForAllItems<string, string, List<string>>((song, sid) =>
                                                              Enqueue(dic, alias, sid, GetById(song)!, 1, 4));

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

    internal static IEnumerable<(string SongID, ArcaeaChart chart)> GetByConst(double @const)
    {
        const double lerance = 0.001;

        // ReSharper disable once LoopCanBePartlyConvertedToQuery
        foreach (var song in Songs.Values)
            // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
            foreach (var chart in song)
                if (Math.Abs(chart.Rating - @const) < lerance)
                    yield return (song.SongID, chart);
    }

    private static IEnumerable<ArcaeaChart> GetByConstRange(double lowerlimit, double upperlimit)
    {
        // ReSharper disable once LoopCanBeConvertedToQuery
        foreach (var song in Songs.Values)
            // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
            foreach (var chart in song)
                if (chart.Rating >= lowerlimit && chart.Rating <= upperlimit)
                    yield return chart;
    }
}
