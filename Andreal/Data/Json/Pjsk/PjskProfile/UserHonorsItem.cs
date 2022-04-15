using Newtonsoft.Json;

namespace Andreal.Data.Json.Pjsk.PjskProfile;

public class UserHonorsItem
{
    [JsonProperty("userId")] public long UserId { get; set; }

    [JsonProperty("honorId")] public int HonorId { get; set; }

    [JsonProperty("level")] public int Level { get; set; }

    [JsonProperty("obtainedAt")] public long ObtainedAt { get; set; }
}
