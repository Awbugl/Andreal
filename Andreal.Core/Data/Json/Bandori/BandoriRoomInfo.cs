using Newtonsoft.Json;

#pragma warning disable CS8618

namespace Andreal.Core.Data.Json.Bandori;

[Serializable]
public class BandoriRoomInfo
{
    [JsonProperty("response")] public List<ResponseItem> Response { get; set; }
}
