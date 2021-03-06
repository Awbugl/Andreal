using Newtonsoft.Json;

#pragma warning disable CS8618

namespace Andreal.Core.Data.Json.Pjsk.PjskProfile;

public class EpisodesItem
{
    [JsonProperty("cardEpisodeId")] public int CardEpisodeID { get; set; }

    [JsonProperty("scenarioStatus")] public string ScenarioStatus { get; set; }

    [JsonProperty("scenarioStatusReasons")]
    public List<string> ScenarioStatusReasons { get; set; }

    [JsonProperty("isNotSkipped")] public string IsNotSkipped { get; set; }
}
