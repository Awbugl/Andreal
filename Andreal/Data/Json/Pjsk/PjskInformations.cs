using Newtonsoft.Json;

#pragma warning disable CS8618

namespace Andreal.Data.Json.Pjsk;

public class PjskInformations
{
    [JsonProperty("id")] public string Id { get; set; }
    [JsonProperty("informationTag")] public string InformationTag { get; set; }
    [JsonProperty("title")] public string Title { get; set; }
}
