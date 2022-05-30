using Newtonsoft.Json;

#pragma warning disable CS8618

namespace Andreal.Core.Data.Json.Pjsk.PjskProfile;

public class UserProfile
{
    [JsonProperty("userId")] public long UserID { get; set; }

    [JsonProperty("word")] public string Word { get; set; }

    [JsonProperty("honorId1")] public int HonorID1 { get; set; }

    [JsonProperty("honorLevel1")] public int HonorLevel1 { get; set; }

    [JsonProperty("honorId2")] public int HonorID2 { get; set; }

    [JsonProperty("honorLevel2")] public int HonorLevel2 { get; set; }

    [JsonProperty("honorId3")] public int HonorID3 { get; set; }

    [JsonProperty("honorLevel3")] public int HonorLevel3 { get; set; }

    [JsonProperty("twitterId")] public string TwitterID { get; set; }

    [JsonProperty("profileImageType")] public string ProfileImageType { get; set; }
}
