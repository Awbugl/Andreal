using Newtonsoft.Json;

namespace Andreal.Core.Data.Json.Pjsk.PjskProfile;

public class UserChallengeLiveSoloResultsItem
{
    [JsonProperty("characterId")] public int CharacterID { get; set; }

    [JsonProperty("highScore")] public int HighScore { get; set; }
}
