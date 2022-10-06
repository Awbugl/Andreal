using Newtonsoft.Json;

#pragma warning disable CS8618

namespace Andreal.Core.Data.Json.Arcaea.ArcaeaLimited;

[Serializable]
public class UserinfoData
{
    [JsonProperty("data")]
    public UserinfoDataItem Data { get; set; }
}
