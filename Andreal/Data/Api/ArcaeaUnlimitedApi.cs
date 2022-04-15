using Andreal.Core;
using Andreal.Data.Json.Arcaea.BotArcApi;
using Andreal.Message;
using Newtonsoft.Json;
using Path = Andreal.Core.Path;

namespace Andreal.Data.Api;

internal static class ArcaeaUnlimitedApi
{
    private static readonly HttpClient Client = new();

    internal static void Init(AndrealConfig config)
    {
        Client.BaseAddress = new(config.Api["unlimited"].Url);
        Client.DefaultRequestHeaders.Add("User-Agent", config.Api["unlimited"].Token);
    }

    private static async Task<ResponseRoot?> GetString(string url) =>
        JsonConvert.DeserializeObject<ResponseRoot>(await (await Client.SendAsync(new(HttpMethod.Get, url)))
                                                          .EnsureSuccessStatusCode().Content.ReadAsStringAsync());

    private static async Task GetStream(string url, Path filename)
    {
        await using var fileStream
            = new FileStream(filename, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);

        try
        {
            await (await Client.GetAsync(url)).EnsureSuccessStatusCode().Content.CopyToAsync(fileStream);
            fileStream.Close();
        }
        catch
        {
            fileStream.Close();
            File.Delete(filename);
        }
    }

    internal static async Task<ResponseRoot?> UserInfo(long ucode) => await GetString($"user/info?usercode={ucode:D9}");

    internal static async Task<ResponseRoot?> UserInfo(string uname) => await GetString($"user/info?user={uname}");

    internal static async Task<ResponseRoot?> UserBest(long ucode, string song, object dif) =>
        await GetString($"user/best?usercode={ucode:D9}&songid={song}&difficulty={dif}");

    internal static async Task<ResponseRoot?> UserBest30(long ucode) =>
        await GetString($"user/best30?usercode={ucode:D9}");

    internal static async Task<ResponseRoot?> UserBest40(long ucode) =>
        await GetString($"user/best30?usercode={ucode:D9}&overflow=9");

    internal static async Task<ResponseRoot?> SongByAlias(string alias) =>
        await GetString($"song/info?songname={alias}");

    internal static async Task<ResponseRoot?> SongAlias(string alias) =>
        await GetString($"song/alias?songname={alias}");

    internal static async Task<ResponseRoot?> SongList() => await GetString("song/list");

    internal static async Task SongAssets(string sid, sbyte difficulty, Path pth) =>
        await GetStream($"assets/song?songid={sid}&difficulty={difficulty}", pth);

    internal static async Task CharAssets(int partner, bool awakened, Path pth) =>
        await GetStream($"assets/char?partner={partner}&awakened={(awakened ? "true" : "false")}", pth);

    internal static async Task IconAssets(int partner, bool awakened, Path pth) =>
        await GetStream($"assets/icon?partner={partner}&awakened={(awakened ? "true" : "false")}", pth);

    internal static TextMessage GetErrorMessage(RobotReply info, int status, string message)
    {
        return status switch
               {
                   -1 or -2 or -3 => info.ArcUidNotFound,
                   -4             => info.TooManyArcUid,
                   -14            => info.NoBydChart,
                   -15            => info.NotPlayedTheChart,
                   -16            => info.GotShadowBanned,
                   _              => $"{info.APIQueryFailed}({status}: {message})"
               };
    }
}
