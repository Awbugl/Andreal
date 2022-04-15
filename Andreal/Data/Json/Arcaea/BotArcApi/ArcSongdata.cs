using Newtonsoft.Json;

#pragma warning disable CS8618

namespace Andreal.Data.Json.Arcaea.BotArcApi;

public class ArcSongdata
{
    [JsonProperty("song_id")] public string SongId { get; set; }

    [JsonProperty("difficulty")] public sbyte Difficulty { get; set; }

    [JsonProperty("score")] public string Score { get; set; }

    [JsonProperty("shiny_perfect_count")] public string MaxPure { get; set; }

    [JsonProperty("perfect_count")] public string Pure { get; set; }

    [JsonProperty("near_count")] public string Far { get; set; }

    [JsonProperty("miss_count")] public string Lost { get; set; }

    [JsonProperty("time_played")] public long TimePlayed { get; set; }

    [JsonProperty("clear_type")] public sbyte ClearType { get; set; }

    [JsonProperty("rating")] public double Rating { get; set; }
}
