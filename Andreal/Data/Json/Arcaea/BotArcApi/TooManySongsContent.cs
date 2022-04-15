using Newtonsoft.Json;

#pragma warning disable CS8618

namespace Andreal.Data.Json.Arcaea.BotArcApi;

public class TooManySongsContent
{
    [JsonProperty("songs")] public List<string> Songs { get; set; }
}
