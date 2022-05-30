using Newtonsoft.Json;

namespace Andreal.Core.Data.Json.Pjsk.PjskProfile;

public class UserCharactersItem
{
    [JsonProperty("characterId")] public int CharacterID { get; set; }

    [JsonProperty("characterRank")] public int CharacterRank { get; set; }
}
