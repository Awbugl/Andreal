using System.Collections.Concurrent;
using Andreal.Core;
using Andreal.Data.Api;
using Andreal.Data.Json.Pjsk;
using Andreal.Message;
using Andreal.Utils;
using Path = Andreal.Core.Path;

#pragma warning disable CS8618

namespace Andreal.Model.Pjsk;

[Serializable]
internal class SongInfo
{
    [NonSerialized] private static Lazy<ConcurrentDictionary<string, SongInfo>> _songList = new();

    [NonSerialized]
    private static readonly ConcurrentDictionary<string, string> Abbreviations = new();

    static SongInfo()
    {
        var musics = PjskApi.PjskMusics().Result ?? new List<PjskMusics>();
        var musicMetas = PjskApi.PjskMusicMetas().Result ?? new List<PjskMusicMetas>();
        var musicDifficulties = PjskApi.PjskMusicDifficulties().Result ?? new List<PjskMusicDifficulties>();

        lock (_songList.Value)
        {
            foreach (var item in musics)
                if (!CheckFull(item.ID))
                {
                    var metas = musicMetas.Where(i => i.MusicID == item.ID).ToList();
                    var alias = GlobalConfig.PjskAlias.Where(i => i.ID == item.ID).Select(i => i.Alias);
                    if (metas.Any())
                        Insert(item, metas, alias);
                    else
                    {
                        var difficulties = musicDifficulties.Where(i => i.MusicID == item.ID).ToList();
                        if (difficulties.Any()) Insert(item, difficulties, alias);
                    }
                }
        }
    }

    internal IEnumerable<string> Alias { get; set; }
    public string SongID { get; set; }
    public string Songname { get; set; }
    public string Categories { get; set; }
    public string Lyricist { get; set; }
    public string Composer { get; set; }
    public double MusicTime { get; set; }
    public int EventRate { get; set; }
    public string Note { get; set; }
    public string BaseScore { get; set; }
    public string FeverScore { get; set; }
    public string AssetbundleName { get; set; }
    public long PublishedAt { get; set; }

    internal List<int> Levels { get; set; }

    private void Init()
    {
        Abbreviations.TryAdd(SongID, Songname.GetAbbreviation());
        foreach (var alias in Alias) Abbreviations.TryAdd(SongID, alias.GetAbbreviation());
    }

    internal static bool CheckSongID(string songID) => _songList.Value.ContainsKey(songID);

    internal static bool CheckFull(string songID) =>
        _songList.Value.TryGetValue(songID, out var info) && info.FeverScore != "NA";

    internal string GetSongName(byte length) =>
        Songname.Length < length + 3
            ? Songname
            : $"{Songname[..length]}...";

    private ImageMessage SongImage()
    {
        var pth = Path.PjskSong(SongID);
        if (pth.FileExists) return ImageMessage.FromPath(pth);

        var sid = SongID.PadLeft(3, '0');
        WebHelper.DownloadImage($"https://assets.pjsek.ai/file/pjsekai-assets/startapp/music/jacket/jacket_s_{sid}/jacket_s_{sid}.png",
                                pth);

        return ImageMessage.FromPath(pth);
    }

    internal MessageChain FullString() =>
        new(SongImage(),
            (TextMessage)($"\n{Songname}\n作词：{Lyricist}\n作曲：{Composer}\n演出：{Categories}\n"
                          + $"时长：{MusicTime}s   活动点数：{EventRate}x\n发布时间：{DateTime.UnixEpoch.AddMilliseconds(PublishedAt):yyyy/MM/dd hh:mm}\n\n"
                          + $"难度 | Easy{Levels[0]} | Normal{Levels[1]} | Hard{Levels[2]} | Expert{Levels[3]} | Master{Levels[4]}\n"
                          + $"物量 | {Note}\n基础倍率 | {BaseScore}" + $"\nFever倍率 | {FeverScore}"));


    internal string NameWithLevel(sbyte dif) => $"{Songname}  [{DifficultyInfo.GetByIndex(dif).LongStr} {Levels[dif]}]";

    internal static int Count() => _songList.Value.Count;

    internal static SongInfo? GetBySid(string? sid)
    {
        if (sid == null) return null;
        _songList.Value.TryGetValue(sid, out var result);
        return result;
    }

    internal static (int, SongInfo?[]?) GetByAlias(string alias)
    {
        if (alias == "") return (-1, null);

        var data = GetBySid(GlobalConfig.PjskAlias.FirstOrDefault(c => StringHelper.Equals(c.Alias, alias))?.ID)
                   ?? _songList.Value.Values.FirstOrDefault(c => StringHelper.Equals(c.Songname, alias));

        if (data != null) return (0, new[] { data });

        var abbrdata = Abbreviations.Where(c => StringHelper.Equals(c.Value, alias)).Select(i => GetBySid(i.Key))
                                    .Distinct().ToArray();
        if (abbrdata.Length > 0)
            return abbrdata.Length switch
                   {
                       1 => (0, abbrdata),
                       _ => (-2, abbrdata)
                   };

        var dic = new Dictionary<string, byte>();
        foreach (var pjskAlias in GlobalConfig.PjskAlias)
        {
            if (StringHelper.Contains(pjskAlias.Alias, alias)) dic.TryAdd(pjskAlias.ID, 4);
            if (StringHelper.Contains(alias, pjskAlias.Alias)) dic.TryAdd(pjskAlias.ID, 2);
        }

        foreach (var songdata in _songList.Value.Values)
        {
            if (StringHelper.Contains(songdata.Songname, alias)) dic.TryAdd(songdata.SongID, 3);
            if (StringHelper.Contains(alias, songdata.Songname)) dic.TryAdd(songdata.SongID, 1);
        }

        if (dic.Count == 0) return (-1, null);

        var max = dic.Values.Max();
        var ls = dic.Where(i => i.Value == max).Select(j => GetBySid(j.Key)).Distinct().ToArray();

        return ls.Length switch
               {
                   1 => (0, ls),
                   0 => (-1, null),
                   _ => (-2, ls)
               };
    }


    public bool Equals(SongInfo? other)
    {
        if (ReferenceEquals(other, null)) return false;
        if (ReferenceEquals(this, other)) return true;
        return SongID == other.SongID;
    }

    private static IEnumerable<(SongInfo, sbyte)> GetByLevelRange(int lowerlimit, int upperlimit)
    {
        return _songList.Value.Values.SelectMany(c => c.Levels, (c, i) => new { c, i })
                        .Where(t => t.i >= lowerlimit && t.i <= upperlimit)
                        .Select(t => (t.c, (sbyte)t.c.Levels.IndexOf(t.i)));
    }

    private static IEnumerable<(SongInfo, sbyte)> GetByLevel(int limit)
    {
        return _songList.Value.Values.SelectMany(c => c.Levels, (c, i) => new { c, i }).Where(t => t.i == limit)
                        .Select(t => (t.c, (sbyte)t.c.Levels.IndexOf(t.i)));
    }

    internal static (SongInfo?, sbyte) RandomSong(int lowerlimit, int upperlimit)
    {
        var ls = GetByLevelRange(lowerlimit, upperlimit).ToArray();
        return ls.Length > 0
            ? ls.GetRandomItem()
            : (null, -1);
    }

    internal static (SongInfo?, sbyte) RandomSong(int limit)
    {
        var ls = GetByLevel(limit).ToArray();
        return ls.Length > 0
            ? ls.GetRandomItem()
            : (null, -1);
    }

    internal static void Insert(PjskMusics item, List<PjskMusicMetas> musicMetas, IEnumerable<string>? alias)
    {
        var obj = new SongInfo
                  {
                      SongID = item.ID,
                      Songname = item.Title,
                      Categories = string.Join(" | ", item.Categories),
                      Lyricist = item.Lyricist,
                      Composer = item.Composer,
                      MusicTime = musicMetas.First().MusicTime ?? 0,
                      EventRate = musicMetas.First().EventRate ?? 0,
                      Levels = musicMetas.Select(i => i.Level).ToList(),
                      Note = string.Join(" | ", musicMetas.Select(i => i.Combo)),
                      BaseScore = string.Join(" | ", musicMetas.Select(i => (i.BaseScore ?? 0).ToString("0.000"))),
                      FeverScore = string.Join(" | ", musicMetas.Select(i => (i.FeverScore ?? 0).ToString("0.000"))),
                      AssetbundleName = item.AssetbundleName,
                      PublishedAt = item.PublishedAt,
                      Alias = alias ?? Array.Empty<string>()
                  };

        if (!CheckSongID(item.ID))
            _songList.Value.TryAdd(item.ID, obj);
        else
            _songList.Value[item.ID] = obj;
    }

    internal static void Insert(PjskMusics item, List<PjskMusicDifficulties> musicMetas, IEnumerable<string>? alias)
    {
        var obj = new SongInfo
                  {
                      SongID = item.ID,
                      Songname = item.Title,
                      Categories = string.Join(" | ", item.Categories),
                      Lyricist = item.Lyricist,
                      Composer = item.Composer,
                      MusicTime = 0,
                      EventRate = 0,
                      Levels = musicMetas.Select(i => i.PlayLevel).ToList(),
                      Note = string.Join(" | ", musicMetas.Select(i => i.NoteCount)),
                      BaseScore = "NA",
                      FeverScore = "NA",
                      AssetbundleName = item.AssetbundleName,
                      PublishedAt = item.PublishedAt,
                      Alias = alias ?? Array.Empty<string>()
                  };

        if (!CheckSongID(item.ID)) _songList.Value.TryAdd(item.ID, obj);
    }

    internal static SongInfo RandomSong() => _songList.Value.Values.GetRandomItem();
}
