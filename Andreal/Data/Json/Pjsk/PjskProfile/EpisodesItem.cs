using Newtonsoft.Json;

#pragma warning disable CS8618

namespace Andreal.Data.Json.Pjsk.PjskProfile;

public class EpisodesItem
{
    [JsonProperty("cardEpisodeId")] public int CardEpisodeId { get; set; }

    [JsonProperty("scenarioStatus")] public string ScenarioStatus { get; set; }

    [JsonProperty("scenarioStatusReasons")]
    public List<string> ScenarioStatusReasons { get; set; }

    [JsonProperty("isNotSkipped")] public string IsNotSkipped { get; set; }
}
