using Newtonsoft.Json;

namespace Andreal.Core.Data.Json.Arcaea.ArcaeaUnlimited;

#pragma warning disable CS8618
public class SessionQueryingContent
{
    [JsonProperty("queried_charts")]
    public string QueriedCharts { get; set; } = "0";

    [JsonProperty("current_account")]
    public string CurrentAccount { get; set; } = "0";
}
