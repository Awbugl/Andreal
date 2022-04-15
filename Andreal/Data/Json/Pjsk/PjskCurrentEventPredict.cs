using Newtonsoft.Json;

#pragma warning disable CS8618

namespace Andreal.Data.Json.Pjsk;

public class PjskCurrentEventPredict
{
    [JsonProperty("status")] public string Status { get; set; }
    [JsonProperty("data")] public Dictionary<string, long> Data { get; set; }
}
