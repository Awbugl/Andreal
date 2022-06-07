using Andreal.Core.Common;
using Andreal.Core.Data.Json.Arcaea.ArcaeaUnlimited;
using Andreal.Core.Message;
using Newtonsoft.Json;
using Path = Andreal.Core.Common.Path;

namespace Andreal.Core.Data.Api;

public static class ArcaeaUnlimitedApi
{
    private static HttpClient? _client;

    public static void Init(AndrealConfig config)
    {
        _client = new();
        _client.BaseAddress = new(config.Api["unlimited"].Url);
        _client.DefaultRequestHeaders.Add("User-Agent", config.Api["unlimited"].Token);
    }

    private static async Task<ResponseRoot?> GetString(string url) =>
        JsonConvert.DeserializeObject<ResponseRoot>(await (await _client!.SendAsync(new(HttpMethod.Get, url)))
                                                          .EnsureSuccessStatusCode().Content.ReadAsStringAsync());

    private static async Task GetStream(string url, Path filename)
    {
        FileStream? fileStream = null;

        try
        {
            fileStream = new(filename, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
            await (await _client!.GetAsync(url)).EnsureSuccessStatusCode().Content.CopyToAsync(fileStream);
        }
        catch
        {
            try
            {
                File.Delete(filename);
            }
            catch
            {
                // ignore
            }
            
            throw;
        }
        finally
        {
            if (fileStream is not null)
            {
                fileStream.Close();
                await fileStream.DisposeAsync();
            }
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

    internal static async Task<ResponseRoot?> SongList() => await GetString("song/list");

    internal static async Task SongAssets(string sid, int difficulty, Path pth) =>
        await GetStream($"assets/song?songid={sid}&difficulty={difficulty}", pth);

    internal static async Task CharAssets(int partner, bool awakened, Path pth) =>
        await GetStream($"assets/char?partner={partner}&awakened={(awakened ? "true" : "false")}", pth);

    internal static async Task IconAssets(int partner, bool awakened, Path pth) =>
        await GetStream($"assets/icon?partner={partner}&awakened={(awakened ? "true" : "false")}", pth);

    internal static async Task PreviewAssets(string sid, int difficulty, Path pth) =>
        await GetStream($"assets/preview?songid={sid}&difficulty={difficulty}", pth);

    internal static TextMessage GetErrorMessage(RobotReply info, int status, string message)
    {
        return status switch
               {
                   -1 or -2 or -3 or -13 => info.ArcUidNotFound,
                   -4                    => info.TooManyArcUid,
                   -14                   => info.NoBydChart,
                   -15                   => info.NotPlayedTheChart,
                   -16                   => info.GotShadowBanned,
                   -23                   => info.BelowTheThreshold,
                   -24                   => info.NeedUpdateAUA,
                   _                     => info.OnAPIQueryFailed(status, message)
               };
    }
}
