using Newtonsoft.Json;

#pragma warning disable CS8618

namespace Andreal.Data.Json.Ycm;

[Serializable]
public class YcmResponse
{
    [JsonProperty("car_type")] public string CarType { get; set; }
    [JsonProperty("code")] public int Code { get; set; }
    [JsonProperty("message")] public string Message { get; set; }
    [JsonProperty("cars")] public List<CarsItem> Cars { get; set; }
}
