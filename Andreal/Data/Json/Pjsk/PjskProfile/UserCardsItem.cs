using Newtonsoft.Json;

#pragma warning disable CS8618

namespace Andreal.Data.Json.Pjsk.PjskProfile;

public class UserCardsItem
{
    [JsonProperty("cardId")] public int CardID { get; set; }

    [JsonProperty("level")] public int Level { get; set; }

    [JsonProperty("masterRank")] public int MasterRank { get; set; }

    [JsonProperty("specialTrainingStatus")]
    public string SpecialTrainingStatus { get; set; }

    [JsonProperty("defaultImage")] public string DefaultImage { get; set; }

    [JsonProperty("episodes")] public List<EpisodesItem> Episodes { get; set; }
}
