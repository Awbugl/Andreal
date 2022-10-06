using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Andreal.Window.Common;

namespace Andreal.Window.UI;

internal partial class Login
{
    private static readonly Regex Regex = new("[^0-9]+", RegexOptions.Compiled);

    internal Login()
    {
        InitializeComponent();
    }

    private async void OnLoginBtnClick(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(UserTextBox.Text) || string.IsNullOrWhiteSpace(PwdBox.Password))
        {
            MessageBox.Show("请输入QQ和密码！");
            return;
        }

        if (!uint.TryParse(UserTextBox.Text, out var uin) || uin <= 10000)
        {
            MessageBox.Show("请输入正确的QQ号！");
            return;
        }

        if (Program.Accounts.Any(i => i.Robot == uin))
        {
            MessageBox.Show("此QQ号已登录或正在登录中！");
            Close();
            return;
        }

        await Program.OnPreLogin(uin, PwdBox.Password);

        Close();
    }

    private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        try
        {
            DragMove();
        }
        catch
        {
            //ignored
        }
    }

    private void OnCloseBtnClick(object sender, RoutedEventArgs e) => Close();

    private void OnPreviewTextInput(object sender, TextCompositionEventArgs e) => e.Handled = Regex.IsMatch(e.Text);

    private void OnKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter) LoginButton.RaiseEvent(new(ButtonBase.ClickEvent));
    }
}
