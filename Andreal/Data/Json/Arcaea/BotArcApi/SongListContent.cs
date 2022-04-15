using Andreal.Data.Json.Arcaea.Songlist;
using Newtonsoft.Json;

#pragma warning disable CS8618

namespace Andreal.Data.Json.Arcaea.BotArcApi;

public class SongListContent
{
    [JsonProperty("songs")] public List<SongsItem> Songs { get; set; }
}
