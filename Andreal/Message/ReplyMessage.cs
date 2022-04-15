using Konata.Core.Message;

namespace Andreal.Message;

[Serializable]
public class ReplyMessage : IMessage
{
    public readonly MessageStruct Message;

    internal ReplyMessage(MessageStruct message) { Message = message; }
}
