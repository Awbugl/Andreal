using System.Collections.Concurrent;
using Andreal.Core.Data.Json.Arcaea.PartnerPosInfoBase;
using Newtonsoft.Json;

namespace Andreal.Core.Common;

public static class GlobalConfig
{
    internal static ConcurrentDictionary<string, List<PosInfoItem>> Locations
        = new(JsonConvert.DeserializeObject<Dictionary<string, List<PosInfoItem>>>(File.ReadAllText(Path.PartnerConfig))
              !);

    public static RobotReply RobotReply = JsonConvert.DeserializeObject<RobotReply>(File.ReadAllText(Path.RobotReply))!;

    internal static void Init(string file)
    {
        switch (file)
        {
            case "positioninfo.json":
                Locations
                    = new(JsonConvert
                              .DeserializeObject<
                                  Dictionary<string, List<PosInfoItem>>>(File.ReadAllText(Path.PartnerConfig))!);
                return;

            case "replytemplate.json":
                RobotReply = JsonConvert.DeserializeObject<RobotReply>(File.ReadAllText(Path.RobotReply))!;
                return;
        }
    }
}
