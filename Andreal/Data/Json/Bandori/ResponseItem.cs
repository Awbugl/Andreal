using Newtonsoft.Json;

#pragma warning disable CS8618

namespace Andreal.Data.Json.Bandori;

[Serializable]
public class ResponseItem
{
    [JsonProperty("raw_message")] public string RawMessage { get; set; }

    [JsonProperty("source_info")] public SourceInfo SourceInfo { get; set; }

    [JsonProperty("time")] public long Time { get; set; }
}
