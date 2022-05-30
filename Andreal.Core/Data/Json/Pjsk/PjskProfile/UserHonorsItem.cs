using Newtonsoft.Json;

namespace Andreal.Core.Data.Json.Pjsk.PjskProfile;

public class UserHonorsItem
{
    [JsonProperty("userId")] public long UserID { get; set; }

    [JsonProperty("honorId")] public int HonorID { get; set; }

    [JsonProperty("level")] public int Level { get; set; }

    [JsonProperty("obtainedAt")] public long ObtainedAt { get; set; }
}
