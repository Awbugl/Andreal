using Andreal.Core.Common;
using Andreal.Core.Data.Api;
using Andreal.Core.Data.Sqlite;
using Andreal.Core.Message;
using Andreal.Core.Utils;
using Konata.Core.Interfaces.Api;
using Konata.Core.Message.Model;

namespace Andreal.Core.Executor;

#pragma warning disable CS8600
#pragma warning disable CS8602
#pragma warning disable CS8604

[Serializable]
internal class OtherExecutor : ExecutorBase
{
    public OtherExecutor(MessageInfo info) : base(info) { }

    [CommandPrefix("/help", "/arc help")]
    private MessageChain HelpMessage() =>
        CommandLength == 0
            ? RobotReply.HelpMessage
            : null;

    [CommandPrefix("/yiyan")]
    private static async Task<MessageChain> Hitokoto() => await OtherApi.HitokotoApi();

    [CommandPrefix("/jrrp")]
    private async Task<MessageChain> Jrrp() =>
        RobotReply.OnJrrpResult(await OtherApi.JrrpApi(Info.FromQQ));

    [CommandPrefix("/dismiss")]
    private async Task<MessageChain?> Dismiss()
    {
        if (Info.Message.Chain.FindChain<AtChain>()?.Any(i => i.AtUin == Info.Bot.Uin) != true) return null;
        if (!IsGroup) return null;
        if (!Info.MasterCheck() && !await Info.PermissionCheck()) return null;

        Info.SendMessage(RobotReply.GroupLeave);
        await Task.Delay(5000);
        await Bot.GroupLeave(Info.FromGroup);

        return null;
    }

    [CommandPrefix("/geterr")]
    private static MessageChain ExceptionReport() => LastExceptionHelper.GetDetails();

    [CommandPrefix("/config")]
    private MessageChain Config()
    {
        if (CommandLength != 1) return RobotReply.ParameterLengthError;
        if (User == null) return RobotReply.NotBind;

        switch (Command[0])
        {
            case "hide":
                User.IsHide ^= 1;
                BotUserInfo.Set(User);
                return $"私密模式已{(User.IsHide == 1 ? "开启" : "关闭")}。";

            case "text":
            case "txt":
                User.IsText = 1;
                BotUserInfo.Set(User);
                return "默认显示方式已更改为文字。";

            case "pic":
            case "img":
                User.IsText = 0;
                BotUserInfo.Set(User);
                return "默认显示方式已更改为图片。";

            default: return RobotReply.ConfigNotFound;
        }
    }

    [CommandPrefix("/ycm")]
    private static async Task<MessageChain> GetBandoriRoom() => await OtherApi.BandoriYcmApi();

    [CommandPrefix("/state")]
    private static MessageChain Statement() => BotStatementHelper.Statement;
}
