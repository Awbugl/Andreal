using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using Andreal.Window.Common;

namespace Andreal.Window.UI;

internal partial class Login
{
    internal Login() { InitializeComponent(); }

    private static readonly Regex Regex = new("[^0-9]+", RegexOptions.Compiled);

    private async void OnLoginBtnClick(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(UserTextBox.Text) || string.IsNullOrWhiteSpace(PwdBox.Password))
        {
            MessageBox.Show("请输入QQ和密码！");
            return;
        }

        if (!uint.TryParse(UserTextBox.Text, out _))
        {
            MessageBox.Show("请正确输入QQ号！");
            return;
        }

        var loginresult = await Program.OnLogin(UserTextBox.Text, PwdBox.Password);

        if (!loginresult.Success)
            MessageBox.Show("登陆失败！ 原因：" + loginresult.Type, "登陆失败！");
        else
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

    private void OnCloseBtnClick(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private void OnPreviewTextInput(object sender, TextCompositionEventArgs e) => e.Handled = Regex.IsMatch(e.Text);
}
