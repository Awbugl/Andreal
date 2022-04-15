using Newtonsoft.Json;

#pragma warning disable CS8618

namespace Andreal.Data.Json.Pjsk.PjskProfile;

public class PjskProfiles
{
    [JsonProperty("user")] public User? User { get; set; }

    [JsonProperty("userProfile")] public UserProfile UserProfile { get; set; }

    [JsonProperty("userDecks")] public List<UserDecksItem> UserDecks { get; set; }

    [JsonProperty("userCards")] public List<UserCardsItem> UserCards { get; set; }

    [JsonProperty("userMusics")] public List<UserMusicsItem> UserMusics { get; set; }

    [JsonProperty("userCharacters")] public List<UserCharactersItem> UserCharacters { get; set; }

    [JsonProperty("userChallengeLiveSoloResults")]
    public List<UserChallengeLiveSoloResultsItem> UserChallengeLiveSoloResults { get; set; }

    [JsonProperty("userChallengeLiveSoloStages")]
    public List<UserChallengeLiveSoloStagesItem> UserChallengeLiveSoloStages { get; set; }

    [JsonProperty("userAreaItems")] public List<UserAreaItemsItem> UserAreaItems { get; set; }

    [JsonProperty("userHonors")] public List<UserHonorsItem> UserHonors { get; set; }

    [JsonProperty("userMusicResults")] public List<UserMusicResultsItem> UserMusicResults { get; set; }
}
