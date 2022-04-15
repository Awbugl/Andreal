using Andreal.Core;
using Andreal.Message;
using Andreal.Model.Arcaea;

namespace Andreal.Executor;

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

    [CommandPrefix("/update")]
    private MessageChain? Update()
    {
        if (!Info.MasterCheck()) return null;
        Songdata.Init();
        return "Arcaea songlist updated";
    }
}
