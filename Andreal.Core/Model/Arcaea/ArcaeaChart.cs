using System.Collections.Concurrent;
using Andreal.Core.UI;
using Newtonsoft.Json;
using Path = Andreal.Core.Common.Path;

#pragma warning disable CS8618

namespace Andreal.Core.Model.Arcaea;

public class ArcaeaChart
{
    private static readonly ConcurrentDictionary<string, Stream> SongImage = new();

    [JsonProperty("name_en")]
    public string NameEn { get; set; }

    [JsonProperty("name_jp")]
    public string NameJp { get; set; }

    [JsonProperty("artist")]
    public string Artist { get; set; }

    [JsonProperty("bpm")]
    public string Bpm { get; set; }

    [JsonProperty("bpm_base")]
    public double BpmBase { get; set; }

    [JsonProperty("set")]
    public string Set { get; set; }

    [JsonProperty("set_friendly")]
    public string SetFriendly { get; set; }

    [JsonProperty("time")]
    public int Time { get; set; }

    [JsonProperty("side")]
    public int Side { get; set; }

    [JsonProperty("world_unlock")]
    public bool WorldUnlock { get; set; }

    [JsonProperty("remote_download")]
    public bool RemoteDownload { get; set; }

    [JsonProperty("bg")]
    public string Bg { get; set; }

    [JsonProperty("date")]
    public int Date { get; set; }

    [JsonProperty("version")]
    public string Version { get; set; }

    [JsonProperty("difficulty")]
    public int Difficulty { get; set; }

    [JsonProperty("rating")]
    public int Rating { get; set; }

    [JsonProperty("note")]
    public int Note { get; set; }

    [JsonProperty("chart_designer")]
    public string ChartDesigner { get; set; }

    [JsonProperty("jacket_designer")]
    public string JacketDesigner { get; set; }

    [JsonProperty("jacket_override")]
    public bool JacketOverride { get; set; }

    [JsonProperty("audio_override")]
    public bool AudioOverride { get; set; }

    internal string SongID { get; set; }

    internal double Const => (double)Rating / 10;

    internal int RatingClass { get; set; }

    internal DifficultyInfo DifficultyInfo => DifficultyInfo.GetByIndex(RatingClass);

    internal string ConstString => $"[{DifficultyInfo.ShortStr} {Const:0.0}]";

    internal string NameWithPackageAndConst => $"{NameEn}\n(Package: {SetFriendly})\n{ConstString}";

    internal string GetSongName(byte length) => NameEn.Length < length + 3 ? NameEn : $"{NameEn[..length]}...";

    internal async Task<Image> GetSongImage()
    {
        var path = await Path.ArcaeaSong(this);

        try
        {
            if (!SongImage.TryGetValue(path, out var stream))
            {
                await using var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

                using var reader = new BinaryReader(fileStream);
                var bytes = reader.ReadBytes((int)fileStream.Length);
                stream = new MemoryStream(bytes);
                reader.Close();
                fileStream.Close();

                SongImage.TryAdd(path, stream);
            }

            var img = new Image(stream);
            if (img.Width == 512) return img;
            var newimg = new Image(img, 512, 512);
            newimg.SaveAsPng(path);
            img.Dispose();
            return newimg;
        }
        catch (Exception e)
        {
            SongImage.TryRemove(path, out var s);
            s?.DisposeAsync();
            throw new ArgumentException("GetSongImage Failed.", NameEn, e);
        }
    }
}
