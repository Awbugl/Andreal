using Newtonsoft.Json;

#pragma warning disable CS8618

namespace Andreal.Data.Json.Pjsk;

public class PjskMusicMetas
{
    [JsonProperty("music_id")] public string MusicId { get; set; }

    [JsonProperty("difficulty")] public string Difficulty { get; set; }

    [JsonProperty("level")] public int Level { get; set; }

    [JsonProperty("combo")] public int Combo { get; set; }

    [JsonProperty("music_time")] public double? MusicTime { get; set; }

    [JsonProperty("event_rate")] public int EventRate { get; set; }

    [JsonProperty("base_score")] public double BaseScore { get; set; }

    [JsonProperty("fever_score")] public double FeverScore { get; set; }
}
