using Newtonsoft.Json;

namespace Andreal.Data.Json.Pjsk.PjskProfile;

public class UserDecksItem
{
    [JsonProperty("leader")] public int Leader { get; set; }

    [JsonProperty("member1")] public int Member1 { get; set; }

    [JsonProperty("member2")] public int Member2 { get; set; }

    [JsonProperty("member3")] public int Member3 { get; set; }

    [JsonProperty("member4")] public int Member4 { get; set; }

    [JsonProperty("member5")] public int Member5 { get; set; }
}
