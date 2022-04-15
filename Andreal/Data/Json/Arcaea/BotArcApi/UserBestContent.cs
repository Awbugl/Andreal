using Newtonsoft.Json;

#pragma warning disable CS8618

namespace Andreal.Data.Json.Arcaea.BotArcApi;

public class UserBestContent
{
    [JsonProperty("account_info")] public AccountInfo AccountInfo { get; set; }
    [JsonProperty("record")] public ArcSongdata Record { get; set; }
}
