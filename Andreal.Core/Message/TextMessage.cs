namespace Andreal.Core.Message;

[Serializable]
public class TextMessage : IMessage
{
    private readonly string _message;

    private TextMessage(string message) { _message = message; }

    public override string ToString() => _message;

    public static implicit operator string(TextMessage value) => value._message;

    public static implicit operator TextMessage(string value) => new(value);
}
