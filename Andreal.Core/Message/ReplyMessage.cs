using Konata.Core.Message;

namespace Andreal.Core.Message;

[Serializable]
public class ReplyMessage : IMessage
{
    public readonly MessageStruct Message;

    internal ReplyMessage(MessageStruct message)
    {
        Message = message;
    }
}
