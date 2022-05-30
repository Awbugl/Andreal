using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Andreal.Window.Common;
using Konata.Core.Interfaces.Api;

namespace Andreal.Window.UI.UserControl;

internal partial class Accounts
{
    private AccountLog? _selectedobject;

    public Accounts()
    {
        InitializeComponent();

        List.ItemsSource = Program.Accounts;
        Program.Accounts.CollectionChanged += OnAccountsChanged;
    }

    private void OnAccountsChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        var b = List.GetBindingExpression(ItemsControl.ItemsSourceProperty);
        b?.UpdateTarget();
    }

    private void OnMouseRightDown(object? sender, MouseButtonEventArgs e)
    {
        if (((DataGridRow)sender!).Item is AccountLog item)
        {
            _selectedobject = item;

            foreach (var items in ((DataGridRow)sender).ContextMenu.Items)
            {
                var i = (MenuItem)items;
                i.Command = i.Header switch
                            {
                                "上线" => new DelegateCommand
                                        {
                                            CanExecuteFunc = () => _selectedobject?.Bot.IsOnline() == false,
                                            CommandAction = OnLoginAccountCommandExecute
                                        },
                                "离线" => new DelegateCommand
                                        {
                                            CanExecuteFunc = () => _selectedobject?.Bot.IsOnline() == true,
                                            CommandAction = OnLogoutAccountCommandExecute
                                        },
                                "删除" => new DelegateCommand
                                        {
                                            CanExecuteFunc = () => _selectedobject is not null,
                                            CommandAction = OnDeleteAccountCommandExecute
                                        },
                                _ => i.Command
                            };
              
            }
        }
    }

    private void OnAddAccountCommandExecute(object parameter, RoutedEventArgs routedEventArgs) { new Login().Show(); }

    private async void OnLoginAccountCommandExecute(object parameter)
    {
        var loginresult = await _selectedobject?.Bot.Login()!;
        await Program.OnLogin(_selectedobject.Bot, loginresult);
    }

    private async void OnLogoutAccountCommandExecute(object parameter) { await _selectedobject!.Bot.Logout(); }

    private async void OnDeleteAccountCommandExecute(object parameter) { await Program.OnRemove(_selectedobject!); }
}
