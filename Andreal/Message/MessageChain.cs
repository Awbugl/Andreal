namespace Andreal.Message;

public class MessageChain
{
    private IEnumerable<IMessage> _messages;
    
    internal MessageChain() { _messages = Array.Empty<IMessage>(); }

    internal MessageChain(params IMessage[] messages) { _messages = messages.ToList(); }

    internal void Append(IMessage? message)
    {
        if (message is not null) _messages = _messages.Append(message);
    }

    internal void Append(string? message)
    {
        if (message is not null) _messages = _messages.Append((TextMessage)message!);
    }

    internal MessageChain Prepend(IMessage message)
    {
        _messages = _messages.Prepend(message);
        return this;
    }

    internal IEnumerable<IMessage> ToArray() => _messages;

    public static implicit operator MessageChain(string value) => new((TextMessage)value!);

    public static implicit operator MessageChain(TextMessage value) => new(value);

    public static implicit operator MessageChain(ImageMessage value) => new(value);
}
