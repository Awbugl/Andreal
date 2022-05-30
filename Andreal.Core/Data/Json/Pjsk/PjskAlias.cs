using Newtonsoft.Json;

#pragma warning disable CS8618

namespace Andreal.Core.Data.Json.Pjsk;

public class PjskAlias
{
    [JsonProperty("id")] public string ID { get; set; }

    [JsonProperty("alias")] public string Alias { get; set; }
}
