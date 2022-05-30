using Newtonsoft.Json;

#pragma warning disable CS8618

namespace Andreal.Core.Data.Json.Pjsk.PjskProfile;

public class UserMusicsItem
{
    [JsonProperty("musicId")] public int MusicId { get; set; }

    [JsonProperty("userMusicDifficultyStatuses")]
    public List<UserMusicDifficultyStatusesItem> UserMusicDifficultyStatuses { get; set; }
}
