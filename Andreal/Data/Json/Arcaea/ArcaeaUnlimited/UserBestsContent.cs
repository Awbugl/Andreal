using Newtonsoft.Json;

#pragma warning disable CS8618

namespace Andreal.Data.Json.Arcaea.ArcaeaUnlimited;

public class UserBestsContent
{
    [JsonProperty("best30_avg")] public double Best30Avg { get; set; }
    [JsonProperty("recent10_avg")] public double Recent10Avg { get; set; }
    [JsonProperty("account_info")] public AccountInfo AccountInfo { get; set; }
    [JsonProperty("best30_list")] public List<ArcSongdata> Best30List { get; set; }
    [JsonProperty("best30_overflow")] public List<ArcSongdata>? OverflowList { get; set; }
}
