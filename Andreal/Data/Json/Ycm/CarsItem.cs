using Newtonsoft.Json;

#pragma warning disable CS8618

namespace Andreal.Data.Json.Ycm;

[Serializable]
public class CarsItem
{
    [JsonProperty("add_time")] public long AddTime { get; set; }
    [JsonProperty("creator_id")] public string CreatorId { get; set; }
    [JsonProperty("data_from")] public string DataFrom { get; set; }
    [JsonProperty("description")] public string Description { get; set; }
    [JsonProperty("id")] public long Id { get; set; }
    [JsonProperty("more_info")] public string MoreInfo { get; set; }
    [JsonProperty("room_id")] public string RoomId { get; set; }
}
