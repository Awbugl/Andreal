using Newtonsoft.Json;

#pragma warning disable CS8618

namespace Andreal.Core.Data.Json.Pjsk;

public class PjskEventUserRanking
{
    [JsonProperty("rankings")] public List<PjskRankings?> Rankings { get; set; }
}
