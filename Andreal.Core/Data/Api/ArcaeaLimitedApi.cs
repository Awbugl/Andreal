using Andreal.Core.Common;
using Andreal.Core.Data.Json.Arcaea.ArcaeaLimited;
using Newtonsoft.Json;

namespace Andreal.Core.Data.Api;

public static class ArcaeaLimitedApi
{
    private static HttpClient? _client;

    internal static bool Available;

    public static void Init(AndrealConfig config)
    {
        Available = config.Api.ContainsKey("limited") && !string.IsNullOrWhiteSpace(config.Api["limited"].Url)
                                                      && !string.IsNullOrWhiteSpace(config.Api["limited"].Token);

        if (!Available) return;

        _client = new();
        _client.BaseAddress = new(config.Api["limited"].Url);
        _client.DefaultRequestHeaders.Authorization = new("Bearer", config.Api["limited"].Token);
    }

    private static async Task<string> GetString(string url) =>
        await (await _client!.SendAsync(new(HttpMethod.Get, url))).EnsureSuccessStatusCode().Content
                                                                  .ReadAsStringAsync();

    internal static async Task<UserinfoDataItem?> Userinfo(long uid) =>
        JsonConvert.DeserializeObject<UserinfoData>(await GetString($"user/{uid:D9}"))?.Data;

    internal static async Task<Best30?> Userbest30(long uid) =>
        JsonConvert.DeserializeObject<Best30>(await GetString($"user/{uid:D9}/best"));
}
