using Newtonsoft.Json;

#pragma warning disable CS8618

namespace Andreal.Core.Data.Json.Pjsk.PjskProfile;

public class UserChallengeLiveSoloStagesItem
{
    [JsonProperty("challengeLiveStageType")]
    public string ChallengeLiveStageType { get; set; }

    [JsonProperty("characterId")] public int CharacterID { get; set; }

    [JsonProperty("challengeLiveStageId")] public int ChallengeLiveStageID { get; set; }

    [JsonProperty("rank")] public int Rank { get; set; }
}
