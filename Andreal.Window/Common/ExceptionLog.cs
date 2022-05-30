using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Konata.Core;

namespace Andreal.Window.Common;

internal class ExceptionLog
{
    public DateTime Time { get; set; }
    public Exception? Exception { get; set; }
}

internal class MessageLog
{
    public DateTime Time { get; set; }
    public string Robot { get; set; } = "";
    public string FromQQ { get; set; } = "";
    public string FromGroup { get; set; } = "";
    public string Message { get; set; } = "";
}

internal sealed class AccountLog : INotifyPropertyChanged
{
    public AccountLog(Bot bot, uint uin, string state, string message)
    {
        Bot = bot;
        Robot = uin;
        State = state;
        Message = message;
    }

    internal readonly Bot Bot;
    private string _nick = "";
    private string _state = "";
    private string _message = "";

    public uint Robot { get; }

    public string Nick
    {
        get => _nick;
        set
        {
            _nick = value;
            OnPropertyChanged(nameof(Nick));
        }
    }

    public string State
    {
        get => _state;
        set
        {
            _state = value;
            OnPropertyChanged(nameof(State));
        }
    }

    public string Message
    {
        get => _message;
        set
        {
            _message = value;
            OnPropertyChanged(nameof(Message));
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new(propertyName));
    }
}
