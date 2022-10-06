using Newtonsoft.Json;

#pragma warning disable CS8618

namespace Andreal.Core.Data.Json.Arcaea.ArcaeaUnlimited;

public class AccountInfo
{
    [JsonProperty("code")]
    public int Code { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("user_id")]
    public int UserID { get; set; }

    [JsonProperty("is_mutual")]
    public bool IsMutual { get; set; }

    [JsonProperty("is_char_uncapped_override")]
    public bool IsCharUncappedOverride { get; set; }

    [JsonProperty("is_char_uncapped")]
    public bool IsCharUncapped { get; set; }

    [JsonProperty("is_skill_sealed")]
    public bool IsSkillSealed { get; set; }

    [JsonProperty("rating")]
    public short Rating { get; set; }

    [JsonProperty("character")]
    public int Character { get; set; }
}
