using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Andreal.Window.Common;
using Konata.Core.Interfaces.Api;

namespace Andreal.Window.UI.UserControl;

internal partial class Accounts
{
    public Accounts()
    {
        InitializeComponent();

        List.ItemsSource = Program.Accounts;
        Program.Accounts.CollectionChanged += OnAccountsChanged;
    }

    public ICommand LoginAccountCommand
        => new DelegateCommand
           {
               CanExecuteFunc = obj =>
               {
                   try
                   {
                       return (obj as AccountLog)?.Bot?.IsOnline() == false;
                   }
                   catch
                   {
                       return false;
                   }
               },
               CommandAction = OnLoginAccountCommandExecute
           };

    public ICommand LogoutAccountCommand
        => new DelegateCommand
           {
               CanExecuteFunc = obj =>
               {
                   try
                   {
                       return (obj as AccountLog)?.Bot?.IsOnline() == true;
                   }
                   catch
                   {
                       return false;
                   }
               },
               CommandAction = OnLogoutAccountCommandExecute
           };

    public ICommand DeleteAccountCommand
        => new DelegateCommand
           {
               CanExecuteFunc = obj =>
               {
                   try
                   {
                       return obj != null;
                   }
                   catch
                   {
                       return false;
                   }
               },
               CommandAction = OnDeleteAccountCommandExecute
           };

    private void OnAccountsChanged(object? sender, NotifyCollectionChangedEventArgs e)
        => List.GetBindingExpression(ItemsControl.ItemsSourceProperty)?.UpdateTarget();

    private void OnMouseRightDown(object? sender, MouseButtonEventArgs e)
    {
        if (sender is not DataGridRow row) return;

        if (row.Item is AccountLog item)
            foreach (var items in row.ContextMenu!.Items)
            {
                var i = (MenuItem)items;
                i.CommandParameter = item;
                i.Command = i.Header switch
                            {
                                "上线" => LoginAccountCommand,
                                "离线" => LogoutAccountCommand,
                                "删除" => DeleteAccountCommand,
                                _    => i.Command
                            };
            }
    }

    private void OnAddAccountCommandExecute(object parameter, RoutedEventArgs routedEventArgs) => new Login().ShowDialog();

    private async void OnLoginAccountCommandExecute(object bot)
    {
        var log = bot as AccountLog;
        var loginresult = await log!.Bot.Login();
        await Program.OnLogin(log.Bot!, loginresult);
    }

    private async void OnLogoutAccountCommandExecute(object bot) => await (bot as AccountLog)?.Bot?.Logout()!;

    private async void OnDeleteAccountCommandExecute(object bot) => await Program.OnRemove((bot as AccountLog)!);
}
