using Newtonsoft.Json;

#pragma warning disable CS8618

namespace Andreal.Data.Json.Pjsk.PjskProfile;

public class UserMusicDifficultyStatusesItem
{
    [JsonProperty("musicId")] public int MusicId { get; set; }
    [JsonProperty("musicDifficulty")] public string MusicDifficulty { get; set; }

    [JsonProperty("userMusicResults")] public List<UserMusicResultsItem> UserMusicResults { get; set; }
}
