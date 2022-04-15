using Newtonsoft.Json;

namespace Andreal.Data.Json.Pjsk.PjskProfile;

public class UserCharactersItem
{
    [JsonProperty("characterId")] public int CharacterId { get; set; }

    [JsonProperty("characterRank")] public int CharacterRank { get; set; }
}
