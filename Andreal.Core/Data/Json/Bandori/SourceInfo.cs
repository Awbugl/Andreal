using Newtonsoft.Json;

#pragma warning disable CS8618

namespace Andreal.Core.Data.Json.Bandori;

[Serializable]
public class SourceInfo
{
    [JsonProperty("name")]
    public string Name { get; set; }
}
