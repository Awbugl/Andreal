using Newtonsoft.Json;

#pragma warning disable CS8618

namespace Andreal.Core.Data.Json.Hitokoto;

[Serializable]
public class Hitokoto
{
    [JsonProperty("hitokoto")] public string Content { get; set; }

    [JsonProperty("from")] public string From { get; set; }
}
