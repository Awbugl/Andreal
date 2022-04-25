using Newtonsoft.Json;

#pragma warning disable CS8618

namespace Andreal.Data.Json.Arcaea.ArcaeaUnlimited;

public class ResponseRoot
{
    [JsonProperty("status")] public int Status { get; set; }

    [JsonProperty("message")] public string Message { get; set; }

    [JsonProperty("content")] public dynamic Content { get; set; }

    internal T DeserializeContent<T>() => JsonConvert.DeserializeObject<T>(Content.ToString());
}
