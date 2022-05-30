using Newtonsoft.Json;

#pragma warning disable CS8618

namespace Andreal.Core.Data.Json.Pjsk.PjskProfile;

public class User
{
    [JsonProperty("userGamedata")] public UserGamedata UserGamedata { get; set; }
}
