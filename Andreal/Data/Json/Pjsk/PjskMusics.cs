using Newtonsoft.Json;

#pragma warning disable CS8618

namespace Andreal.Data.Json.Pjsk;

public class PjskMusics
{
    [JsonProperty("id")] public string Id { get; set; }

    [JsonProperty("categories")] public List<string> Categories { get; set; }

    [JsonProperty("title")] public string Title { get; set; }

    [JsonProperty("lyricist")] public string Lyricist { get; set; }

    [JsonProperty("composer")] public string Composer { get; set; }

    [JsonProperty("arranger")] public string Arranger { get; set; }

    [JsonProperty("assetbundleName")] public string AssetbundleName { get; set; }

    [JsonProperty("publishedAt")] public long PublishedAt { get; set; }
}
