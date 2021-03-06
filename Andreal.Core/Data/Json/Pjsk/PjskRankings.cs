using Newtonsoft.Json;

#pragma warning disable CS8618

namespace Andreal.Core.Data.Json.Pjsk;

public class PjskRankings
{
    [JsonProperty("userId")] public long UserID { get; set; }

    [JsonProperty("name")] public string Name { get; set; }

    [JsonProperty("score")] public int Score { get; set; }

    [JsonProperty("rank")] public int Rank { get; set; }
}
