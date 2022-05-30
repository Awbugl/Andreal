using Newtonsoft.Json;

#pragma warning disable CS8618

namespace Andreal.Core.Data.Json.Pjsk.PjskProfile;

public class UserGamedata
{
    [JsonProperty("userId")] public long UserID { get; set; }

    [JsonProperty("name")] public string Name { get; set; }

    [JsonProperty("deck")] public int Deck { get; set; }

    [JsonProperty("rank")] public int Rank { get; set; }
}
