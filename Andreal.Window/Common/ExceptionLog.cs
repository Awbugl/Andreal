using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Konata.Core;

namespace Andreal.Window.Common;

internal class ExceptionLog : IComparable<ExceptionLog>, IComparable
{
    public DateTime Time { get; set; }
    public Exception? Exception { get; set; }

    public int CompareTo(ExceptionLog? other)
    {
        if (ReferenceEquals(this, other)) return 0;
        if (ReferenceEquals(null, other)) return 1;
        return Time.CompareTo(other.Time);
    }

    public int CompareTo(object? obj)
    {
        if (ReferenceEquals(null, obj)) return 1;
        if (ReferenceEquals(this, obj)) return 0;
        return obj is ExceptionLog other
            ? CompareTo(other)
            : throw new ArgumentException($"Object must be of type {nameof(ExceptionLog)}");
    }

    public static bool operator <(ExceptionLog? left, ExceptionLog? right) =>
        Comparer<ExceptionLog>.Default.Compare(left, right) < 0;

    public static bool operator >(ExceptionLog? left, ExceptionLog? right) =>
        Comparer<ExceptionLog>.Default.Compare(left, right) > 0;

    public static bool operator <=(ExceptionLog? left, ExceptionLog? right) =>
        Comparer<ExceptionLog>.Default.Compare(left, right) <= 0;

    public static bool operator >=(ExceptionLog? left, ExceptionLog? right) =>
        Comparer<ExceptionLog>.Default.Compare(left, right) >= 0;
}

internal class MessageLog : IComparable<MessageLog>, IComparable
{
    public DateTime Time { get; set; }
    public string Robot { get; set; } = "";
    public string FromQQ { get; set; } = "";
    public string FromGroup { get; set; } = "";
    public string Message { get; set; } = "";

    public int CompareTo(MessageLog? other)
    {
        if (ReferenceEquals(this, other)) return 0;
        if (ReferenceEquals(null, other)) return 1;
        return Time.CompareTo(other.Time);
    }

    public int CompareTo(object? obj)
    {
        if (ReferenceEquals(null, obj)) return 1;
        if (ReferenceEquals(this, obj)) return 0;
        return obj is MessageLog other
            ? CompareTo(other)
            : throw new ArgumentException($"Object must be of type {nameof(MessageLog)}");
    }

    public static bool operator <(MessageLog? left, MessageLog? right) =>
        Comparer<MessageLog>.Default.Compare(left, right) < 0;

    public static bool operator >(MessageLog? left, MessageLog? right) =>
        Comparer<MessageLog>.Default.Compare(left, right) > 0;

    public static bool operator <=(MessageLog? left, MessageLog? right) =>
        Comparer<MessageLog>.Default.Compare(left, right) <= 0;

    public static bool operator >=(MessageLog? left, MessageLog? right) =>
        Comparer<MessageLog>.Default.Compare(left, right) >= 0;
}

internal sealed class AccountLog : INotifyPropertyChanged, IComparable<AccountLog>, IComparable
{
    public AccountLog(Bot bot, uint uin, string state, string message)
    {
        Bot = bot;
        Robot = uin;
        State = state;
        Message = message;
    }

    internal readonly Bot? Bot;
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

    public int CompareTo(AccountLog? other)
    {
        if (ReferenceEquals(this, other)) return 0;
        if (ReferenceEquals(null, other)) return 1;
        return Robot.CompareTo(other.Robot);
    }

    public int CompareTo(object? obj)
    {
        if (ReferenceEquals(null, obj)) return 1;
        if (ReferenceEquals(this, obj)) return 0;
        return obj is AccountLog other
            ? CompareTo(other)
            : throw new ArgumentException($"Object must be of type {nameof(AccountLog)}");
    }

    public static bool operator <(AccountLog? left, AccountLog? right) =>
        Comparer<AccountLog>.Default.Compare(left, right) < 0;

    public static bool operator >(AccountLog? left, AccountLog? right) =>
        Comparer<AccountLog>.Default.Compare(left, right) > 0;

    public static bool operator <=(AccountLog? left, AccountLog? right) =>
        Comparer<AccountLog>.Default.Compare(left, right) <= 0;

    public static bool operator >=(AccountLog? left, AccountLog? right) =>
        Comparer<AccountLog>.Default.Compare(left, right) >= 0;
}
