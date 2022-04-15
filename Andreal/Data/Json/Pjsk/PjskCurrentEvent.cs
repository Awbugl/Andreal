using Newtonsoft.Json;

namespace Andreal.Data.Json.Pjsk;

public class PjskCurrentEvent
{
    [JsonProperty("eventJson")] public PjskCurrentEventItem? EventJson { get; set; }
}
