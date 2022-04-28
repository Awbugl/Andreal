using Newtonsoft.Json;

#pragma warning disable CS8618

namespace Andreal.Data.Json.Pjsk;

public class PjskMusicDifficulties
{
    [JsonProperty("musicId")] public string MusicID { get; set; }
    [JsonProperty("musicDifficulty")] public string MusicDifficulty { get; set; }
    [JsonProperty("playLevel")] public int PlayLevel { get; set; }
    [JsonProperty("noteCount")] public int NoteCount { get; set; }
}
