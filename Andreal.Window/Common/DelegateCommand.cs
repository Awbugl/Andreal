using System;
using System.Windows.Input;

namespace Andreal.Window.Common;

public class DelegateCommand : ICommand
{
    public Action<object>? CommandAction { set; get; }
    public Func<object?, bool>? CanExecuteFunc { get; set; }

    public void Execute(object? parameter) { CommandAction?.Invoke(parameter!); }

    public bool CanExecute(object? parameter) => CanExecuteFunc?.Invoke(parameter) ?? false;

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}
