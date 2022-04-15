using Newtonsoft.Json;

#pragma warning disable CS8618

namespace Andreal.Data.Json.Arcaea.BotArcApi;

public class UserInfoContent
{
    [JsonProperty("account_info")] public AccountInfo AccountInfo { get; set; }
    [JsonProperty("recent_score")] public List<ArcSongdata> RecentScore { get; set; }
}
