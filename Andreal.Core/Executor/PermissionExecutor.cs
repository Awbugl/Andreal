using Andreal.Core.Common;
using Andreal.Core.Message;
using Andreal.Core.Model.Arcaea;
using Konata.Core.Interfaces.Api;

namespace Andreal.Core.Executor;

[Serializable]
internal class PermissionExecutor : ExecutorBase
{
    public PermissionExecutor(MessageInfo info) : base(info) { }

    [CommandPrefix("/echo")]
    private MessageChain? Echo()
    {
        if (Info.MasterCheck()) Info.SendMessageOnly(string.Join(" ", Command));
        return null;
    }

    [CommandPrefix("/remove")]
    private async Task<MessageChain?> RemoveGroup()
    {
        if (!Info.MasterCheck()) return null;
        if (CommandLength != 1) return RobotReply.ParameterLengthError;
        await Info.Bot.GroupLeave(uint.Parse(Command[0]));
        return "Removed.";
    }

    [CommandPrefix("/update")]
    private MessageChain? Update()
    {
        if (!Info.MasterCheck()) return null;
        ArcaeaCharts.Init();
        return "Arcaea songlist updated";
    }
}
