using Newtonsoft.Json;

namespace Andreal.Core.Data.Json.Pjsk;

public class PjskCurrentEvent
{
    [JsonProperty("eventJson")] public PjskCurrentEventItem? EventJson { get; set; }
}
