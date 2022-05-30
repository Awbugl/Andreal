using Newtonsoft.Json;

#pragma warning disable CS8618

namespace Andreal.Core.Data.Json.Arcaea.ArcaeaLimited;

[Serializable]
public class ScoreinfoData
{
    [JsonProperty("data")] public RecordDataItem Data { get; set; }
}
