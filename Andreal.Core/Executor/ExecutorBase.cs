using Andreal.Core.Common;
using Andreal.Core.Data.Sqlite;
using Konata.Core;

namespace Andreal.Core.Executor;

[Serializable]
internal abstract class ExecutorBase
{
    protected readonly MessageInfo Info;

    protected ExecutorBase(MessageInfo info) { Info = info; }

    protected Bot Bot => Info.Bot;
    protected bool IsGroup => Info.MessageType == MessageInfoType.Group;
    protected string[] Command => Info.CommandWithoutPrefix;
    protected int CommandLength => Info.CommandWithoutPrefix.Length;
    protected BotUserInfo? User => Info.UserInfo.Value;
    protected static RobotReply RobotReply => GlobalConfig.RobotReply;
}
