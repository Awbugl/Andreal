using Newtonsoft.Json;

#pragma warning disable CS8618

namespace Andreal.Data.Json.Pjsk.PjskProfile;

public class UserProfile
{
    [JsonProperty("userId")] public long UserId { get; set; }

    [JsonProperty("word")] public string Word { get; set; }

    [JsonProperty("honorId1")] public int HonorId1 { get; set; }

    [JsonProperty("honorLevel1")] public int HonorLevel1 { get; set; }

    [JsonProperty("honorId2")] public int HonorId2 { get; set; }

    [JsonProperty("honorLevel2")] public int HonorLevel2 { get; set; }

    [JsonProperty("honorId3")] public int HonorId3 { get; set; }

    [JsonProperty("honorLevel3")] public int HonorLevel3 { get; set; }

    [JsonProperty("twitterId")] public string TwitterId { get; set; }

    [JsonProperty("profileImageType")] public string ProfileImageType { get; set; }
}
