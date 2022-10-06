using Newtonsoft.Json;

#pragma warning disable CS8618

namespace Andreal.Core.Data.Json.Arcaea.ArcaeaLimited;

[Serializable]
public class Best30
{
    [JsonProperty("data")]
    public List<RecordDataItem> Data { get; set; }
}
