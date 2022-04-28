using Newtonsoft.Json;

#pragma warning disable CS8618

namespace Andreal.Data.Json.Pjsk;

public class PjskCurrentEventItem
{
    [JsonProperty("id")] public int EventID { get; set; }
    [JsonProperty("name")] public string Name { get; set; }
    [JsonProperty("assetbundleName")] public string AssetbundleName { get; set; }
}
