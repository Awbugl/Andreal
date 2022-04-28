using System.Collections.Concurrent;
using Andreal.Core;

namespace Andreal.Data.Json.Arcaea.PartnerPosInfoBase;

internal static class PartnerPosInfoBase
{
    private static readonly Lazy<ConcurrentDictionary<string, Dictionary<string, PosInfoItem>>> Dict
        = new(() => new(Init()));

    private static readonly PosInfoItem ImgV1 = new() { PositionX = 770, PositionY = 58, Size = 950 };
    private static readonly PosInfoItem ImgV2 = new() { PositionX = 850, PositionY = 0, Size = 1400 };
    private static readonly PosInfoItem ImgV4 = new() { PositionX = 550, PositionY = 50, Size = 1500 };

    private static Dictionary<string, Dictionary<string, PosInfoItem>> Init()
    {
        var ls = new Dictionary<string, Dictionary<string, PosInfoItem>>();
        foreach (var (key, value) in GlobalConfig.Locations) ls.Add(key, value.ToDictionary(i => i.Partner));
        return ls;
    }

    internal static PosInfoItem? Get(string partner, ImgVersion imgVersion)
    {
        return imgVersion switch
               {
                   ImgVersion.ImgV1 => Dict.Value["1"].TryGetValue(partner, out var result)
                       ? result
                       : ImgV1,
                   ImgVersion.ImgV2 => Dict.Value["2"].TryGetValue(partner, out var result)
                       ? result
                       : ImgV2,
                   ImgVersion.ImgV4 => Dict.Value["4"].TryGetValue(partner, out var result)
                       ? result
                       : ImgV4,
                   _ => null
               };
    }
}
