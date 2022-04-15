using Andreal.Core;
using Andreal.Message;
using Andreal.Model.Arcaea;

namespace Andreal.Utils;

internal static class AliasErrorHelper
{
    internal static TextMessage GetSongAliasErrorMessage(RobotReply info, int status, IEnumerable<SongInfo> ls) =>
        status switch
        {
            -1 => info.NoSongFound!,
            -2 => ls.Aggregate(info.TooManySongFound, (cur, i) => cur + "\n" + i.SongName),
            _  => ""
        };
}
