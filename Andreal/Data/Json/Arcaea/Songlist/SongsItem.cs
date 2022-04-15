using Newtonsoft.Json;

namespace Andreal.Data.Json.Arcaea.Songlist;

#pragma warning disable CS8618

[Serializable]
public class SongsItem
{
    [JsonProperty("id")] public string Id { get; set; }

    [JsonProperty("title_localized")] public TitleLocalized TitleLocalized { get; set; }

    [JsonProperty("artist")] public string Artist { get; set; }

    [JsonProperty("bpm")] public string Bpm { get; set; }

    [JsonProperty("bpm_base")] public double BpmBase { get; set; }

    [JsonProperty("set")] public string Set { get; set; }
    [JsonProperty("set_friendly")] public string? Package { get; set; }

    [JsonProperty("world_unlock")] public bool WorldUnlock { get; set; }

    [JsonProperty("remote_dl")] public bool NeedDownload { get; set; }

    [JsonProperty("side")] public int Side { get; set; }

    [JsonProperty("time")] public int Time { get; set; }

    [JsonProperty("date")] public long Date { get; set; }

    [JsonProperty("version")] public string Version { get; set; }

    [JsonProperty("difficulties")] public List<DifficultiesItem> Difficulties { get; set; }
}
