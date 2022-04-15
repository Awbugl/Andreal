using System.Text;
using Andreal.Data.Json.Bandori;
using Andreal.Data.Json.Hitokoto;
using Andreal.Data.Json.Ycm;
using Newtonsoft.Json;

namespace Andreal.Data.Api;

internal static class OtherApi
{
    [NonSerialized] private static readonly HttpClient Client;

    static OtherApi() { Client = new(); }

    private static async Task<string> GetString(string url) =>
        await (await Client.SendAsync(new(HttpMethod.Get, url))).EnsureSuccessStatusCode().Content.ReadAsStringAsync();

    internal static async Task<string> HitokotoApi()
    {
        var data = JsonConvert.DeserializeObject<Hitokoto>(await GetString("https://v1.hitokoto.cn"));
        return $"{data?.Content}\n  ----「{data?.From}」";
    }

    internal static async Task<string> JrrpApi(long qqid) =>
        await (await Client.PostAsync("http://api.kokona.tech:5555/jrrp",
                                      new StringContent($"QQ=2967373629&v=20190114&QueryQQ={qqid}", Encoding.UTF8,
                                                        new("application/x-www-form-urlencoded")))).Content
                                                                                                   .ReadAsStringAsync();

    internal static async Task<YcmResponse?> YcmApi(string carType) =>
        JsonConvert.DeserializeObject<YcmResponse>(await
                                                       GetString($"https://ycm.chinosk6.cn/get_car?car_type={carType}&time_limit=900&token=L4ETRgHBPUt8KSkvcO"));

    internal static async Task<YcmResponse?>
        AddCarApi(string carType, string roomid, string description, long userid) =>
        JsonConvert.DeserializeObject<YcmResponse>(await
                                                       GetString($"https://ycm.chinosk6.cn/add_car?car_type={carType}&room_id={roomid}&description={description}&creator_id={userid}&data_from=AndreaBot&token=L4ETRgHBPUt8KSkvcO"));

    internal static async Task<string?> BandoriYcmApi()
    {
        var jsonData
            = JsonConvert
                .DeserializeObject<
                    BandoriRoomInfo>(await GetString("https://api.bandoristation.com/?function=query_room_number"));

        return jsonData?.Response.Count == 0
            ? "myc"
            : jsonData?.Response.Aggregate("",
                                           (current, i) =>
                                               current
                                               + $"\n\n{((DateTime.Now.ToUniversalTime() - new DateTime(1970, 1, 1)).TotalMilliseconds - i.Time) / 1000:##.000}秒前\n来自 {i.SourceInfo.Name} 的车牌：\n{i.RawMessage}");
    }
}
