using System;
using System.Windows;
using System.Windows.Input;

namespace Andreal.Window.Common;

internal class NotifyIconViewModel
{
    public ICommand ShowWindowCommand =>
        new DelegateCommand
        {
            CanExecuteFunc = () => Application.Current.MainWindow?.IsVisible != true,
            CommandAction = (_) => Application.Current.MainWindow?.Show()
        };

    public ICommand HideWindowCommand =>
        new DelegateCommand
        {
            CanExecuteFunc = () => Application.Current.MainWindow?.IsVisible == true,
            CommandAction = (_) => Application.Current.MainWindow?.Hide()
        };

    public ICommand ExitApplicationCommand =>
        new DelegateCommand { CanExecuteFunc = () => true, CommandAction = (_) => Environment.Exit(0) };
}
